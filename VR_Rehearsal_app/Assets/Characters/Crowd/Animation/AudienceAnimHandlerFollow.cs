﻿using UnityEngine;
using System.Collections;

public class AudienceAnimHandlerFollow : AudienceAnimHandlerBasic
{
    //Layer lerping speed during following transition
    public float LerpAnimLayerSpeed = 1.0f;

    //Neck rotatin angular speed during following transition
    public float SwitchFollowDegSpeed = 60f;

    protected Transform _currTarget;
    //private bool _isFollowing = false;
    protected Quaternion _defaultHeadLocalRotation;

    protected Coroutine _currStartFollowCR = null;
    protected Coroutine _currStopFollowCR = null;
    protected Coroutine _currLookAtCR = null;

    //Animator layer index, must be consistent with the actual animator
    protected const int neckConstraintLayerIdx = 0;
    protected const int defaultLayerIdx = 1;

    //Use me to start following target!
    //will do nothing if the audience has already been in following state
    //will do nothing during transition
    public void StartToFollow(Transform target)
    {

        //if (_isFollowing /*|| _isTransiting*/)
        //    return;
        _defaultHeadLocalRotation = _audience.headTransform.localRotation;
        if (_currStartFollowCR != null)
            StopCoroutine(_currStartFollowCR);
        if (_currStopFollowCR != null)
            StopCoroutine(_currStopFollowCR);
        if (_currLookAtCR != null)
            StopCoroutine(_currLookAtCR);
        _currStartFollowCR = StartCoroutine(StartToFollow_CR(target));
    }

    //Use me to stop following!
    //will do nothing if the audience has already been out of following state
    //will do nothing during transisiton
    public void StopToFollow()
    {
        if (_currStartFollowCR != null)
            StopCoroutine(_currStartFollowCR);
        if (_currStopFollowCR != null)
            StopCoroutine(_currStopFollowCR);
        if (_currLookAtCR != null)
            StopCoroutine(_currLookAtCR);
        _currStopFollowCR = StartCoroutine(StopToFollow_CR());
    }

    protected IEnumerator StartToFollow_CR(Transform target)
    {
        //1. gradually switch animator layer
        float initWeight = controller.GetLayerWeight(defaultLayerIdx);
        float timeElapsed = 0f;
        float totalTime = Mathf.Abs(0f - initWeight) / LerpAnimLayerSpeed;
        while (timeElapsed < totalTime)
        {
            controller.SetLayerWeight(defaultLayerIdx,
                Mathf.Lerp(initWeight, 0f, Mathf.Clamp01(timeElapsed / totalTime)));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        controller.SetLayerWeight(defaultLayerIdx, 0f);

        //2. lerp neck rotation to follow target
        Quaternion lookRot;
        do
        {
            lookRot = Quaternion.LookRotation
                ((target.position - _audience.headTransform.position).normalized, Vector3.up);
            _audience.headTransform.rotation = Quaternion.RotateTowards
                (_audience.headTransform.rotation, lookRot, SwitchFollowDegSpeed * Time.deltaTime);
            yield return null;
        }
        while (Quaternion.Angle(_audience.headTransform.rotation, lookRot) > 0.5f);
        _audience.headTransform.rotation = lookRot;

        _currTarget = target;
        if (_currTarget != null)
            _currLookAtCR = StartCoroutine(LookAt_CR());

        _currStartFollowCR = null;
    }

    protected IEnumerator StopToFollow_CR()
    {
        if (_currLookAtCR != null)
        {
            StopCoroutine(_currLookAtCR);
            _currLookAtCR = null;
        }

        _currTarget = null;
        //1. lerp neck rotation to default
        while (Quaternion.Angle(_defaultHeadLocalRotation,
            _audience.headTransform.localRotation) > 0.5f)
        {
            _audience.headTransform.localRotation = Quaternion.RotateTowards(_audience.headTransform.localRotation,
                                                                _defaultHeadLocalRotation,
                                                                SwitchFollowDegSpeed * Time.deltaTime);
            yield return null;
        }
        _audience.headTransform.localRotation = _defaultHeadLocalRotation;

        //2. gradually switch animator layer
        float initWeight = controller.GetLayerWeight(defaultLayerIdx);
        float timeElapsed = 0f;
        float totalTime = Mathf.Abs(1f - initWeight) / LerpAnimLayerSpeed;
        while (timeElapsed < totalTime)
        {
            controller.SetLayerWeight(defaultLayerIdx,
                Mathf.Lerp(initWeight, 1f, Mathf.Clamp01(timeElapsed / totalTime)));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        controller.SetLayerWeight(defaultLayerIdx, 1f);

        _currStopFollowCR = null;
    }

    protected IEnumerator LookAt_CR()
    {
        while (true)
        {
            _audience.headTransform.LookAt(_currTarget);
            yield return null;
        }
    }

    protected override void Awake()
    {
        _audience = GetComponent<Audience>();
        controller = GetComponentInChildren<Animator>();
        controller.runtimeAnimatorController = AudienceAnimClipHolder.curr.followController;
        controller.SetLayerWeight(defaultLayerIdx, 1f);
        RandomizeBasicClips();
    }
}
