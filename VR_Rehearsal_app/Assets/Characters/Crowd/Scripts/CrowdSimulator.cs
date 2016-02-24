﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MangoBehaviorTree;
using CrowdConfigInfo = spaceInfoParser.parsedData_spaceInfo;

public class CrowdSimulator : MonoBehaviour
{
    private static CrowdSimulator _currSim = null;
    public static CrowdSimulator currSim
    {
        get
        {
            if (_currSim == null)
                _currSim = FindObjectOfType<CrowdSimulator>();
            return _currSim;
        }
    }

    public Audience[] prefabsL0;
    public Audience[] prefabsL1;
    public Audience[] prefabsL2;
    public Audience[] prefabsL3;

    public Transform crowdParent;
    public string crowdConfigFileName;
    public bool dummy = true;
    public float stepInterval;

    public float globalAttentionMean { get; set; }
    public float globalAttentionStDev { get; set; }
    public AnimationCurve seatPosAttentionFactor;
    public float seatPosAttentionUpper
    {
        get
        {
            return seatPosAttentionFactor.keys[0].value;
        }
        set
        {
            var keys = seatPosAttentionFactor.keys;
            keys[0].value = value;
            seatPosAttentionFactor = new AnimationCurve(keys);

        }
    }
    public float seatPosAttentionLower
    {
        get
        {
            return seatPosAttentionFactor.keys[1].value;
        }
        set
        {
            var keys = seatPosAttentionFactor.keys;
            keys[1].value = value;
            seatPosAttentionFactor = new AnimationCurve(keys);
        }
    }

    public SimpleGazeCollision gazeCollision;

    [Range(0f,1f)]
    public float gazeCumulativeIntensity;

    private List<Audience> audiences;
    public int audienceNum { get { return audiences.Count; } }
    //private BehaviorTree<Audience> _audienceBt = null;
    private BehaviorTree<Audience> _dummyAudienceBt;

    private void CreateDummyTree()
    {
        _dummyAudienceBt = new BehaviorTree<Audience>(
            new SequenceNode<Audience>(
                new AudienceSimStepNode(),
                new AudienceStateSelectorNode(1,
                    new List<BaseNode<Audience>>
                    {
                        //new AudienceStateNode(Audience.States.Focused),
                        //new AudienceStateNode(Audience.States.Bored),
                        //new AudienceStateNode(Audience.States.Chatting)
                        new AudienceFocusStateNode(),
                        new AudienceBoredStateNode(),
                        new AudienceChatStateNode()
                    })
                ));
    }

    private void CreateCrowd()
    {
        CrowdConfigInfo tx = spaceInfoParser.Parse(crowdConfigFileName);
        audiences = new List<Audience>();
        for (int i = 0; i < tx.seat_RowNum * tx.seat_ColNum; i++)
        {
            int rand = Random.Range(0, (prefabsL1.Length - 1));
            Audience ad;
            if (i % tx.seat_ColNum < 2)
            {
                ad = Instantiate(prefabsL1[rand], Vector3.zero, Quaternion.identity) as Audience;
                ad.detailLevel = Audience.DetailLevel.FullSize_VL_FullAnim;
            }
            else
            {
                ad = Instantiate(prefabsL2[rand], Vector3.zero, Quaternion.identity) as Audience;
                ad.detailLevel = Audience.DetailLevel.HalfSize_VL_FullAnim;
            }
            ad.followingTransform = RoomCenter.currRoom.presenterHead;
            

            //to Phan: fix the layout here
            ad.normalizedPos = (float)(i % tx.seat_ColNum) / (float)tx.seat_ColNum;
            ad.transform.parent = crowdParent;
            ad.transform.localPosition = tx.seat_posVecs[i];
            audiences.Add(ad);
        }
    }

    private void Awake()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        globalAttentionMean = 0.6f;
        globalAttentionStDev = 0.05f;

        if (_dummyAudienceBt == null)
            CreateDummyTree();

        CreateCrowd();

    }

    private void Start()
    {
        if (dummy && _dummyAudienceBt != null)
        {
            StartCoroutine(Simulate_CR());
        }
    }

    private IEnumerator Simulate_CR()
    {
        float stepPerAudience = stepInterval / audienceNum;
        while (true)
        {
            gazeCollision.UpdateGazeContact();
            Shuffle(audiences);
            for (int i = 0; i < audiences.Count; ++i)
            {
                _dummyAudienceBt.NextTick(audiences[i]);
                yield return new WaitForSeconds(stepPerAudience);
            }
        }
    }

    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
