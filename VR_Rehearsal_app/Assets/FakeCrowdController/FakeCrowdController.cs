﻿using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;

public class FakeCrowdController : MonoBehaviour
{	
	public GameObject crowdSimObj;
	private CrowdSimulator _crowdSim;

	void Start(){

		_crowdSim = crowdSimObj.GetComponent<CrowdSimulator> ();
		StartCoroutine (StartWebSocket_CR ());
	}

    // Use this for initialization
    IEnumerator StartWebSocket_CR()
    {
        WebSocket w = new WebSocket(new Uri("ws://128.2.236.66:3000"));
        yield return StartCoroutine(w.Connect());

		w.SendString("Ctrl Connected");
        while (true)
        {
            string res = w.RecvString();
			if (res != null)
            {
				Debug.Log("Received: " + res);
                //w.SendString("Hi there" + i++);

				switch(res){
					case "m+":{
						float currMean = _crowdSim.globalAttentionMean;
						_crowdSim.globalAttentionMean = currMean + 0.01f;
						break;
					}
					case "m-":{
						float currMean = _crowdSim.globalAttentionMean;
						_crowdSim.globalAttentionMean = currMean - 0.01f;
						break;
					}
					case "d+":{
						float currDV = _crowdSim.globalAttentionStDev ;
						_crowdSim.globalAttentionStDev  = currDV + 0.01f;
						break;
					}
					case "d-":{
						float currDV = _crowdSim.globalAttentionStDev ;
						_crowdSim.globalAttentionStDev  = currDV - 0.01f;
						break;
					}
					case "u+":{
						float posUp = _crowdSim.seatPosAttentionUpper;
						_crowdSim.seatPosAttentionUpper = posUp + 0.01f;
						break;
					}
					case "u-":{
						float posUp = _crowdSim.seatPosAttentionUpper ;
						_crowdSim.seatPosAttentionUpper  = posUp - 0.01f;
						break;
					}
					case "l+":{
						float posLow = _crowdSim.seatPosAttentionLower ;
						_crowdSim.seatPosAttentionLower  = posLow + 0.01f;
						break;
					}
					case "l-":{
						float posLow = _crowdSim.seatPosAttentionLower ;
						_crowdSim.seatPosAttentionLower  = posLow - 0.01f;
						break;
					}

				}
			
				w.SendString("|Unity> currStat: m=" + _crowdSim.globalAttentionMean + ", dev=" + _crowdSim.globalAttentionStDev 
				             + ", A_up=" + _crowdSim.seatPosAttentionUpper + ", A_Low=" +  _crowdSim.seatPosAttentionLower + "." );

				
				//w.SendString("|Unity> blar blar" + res);

            }
            if (w.Error != null)
            {
                Debug.LogError("Error: " + w.Error);
                break;
            }
            yield return 0;
        }
        w.Close();
    }
}