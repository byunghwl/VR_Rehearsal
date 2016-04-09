﻿using UnityEngine;
using System.Collections;

public class ClockTimer : MonoBehaviour
{
	public TextMesh textMesh;
    public Transform infoTransform;
    public Transform timerTransform;
    public string startInfo;
    public string endInfo;
	public float _maxSecond = 0;
	private float _currSecond = 0f; 

	public void SetMaxTime(int time)
    {
		_maxSecond = time;
		_currSecond = _maxSecond;
	}

    public void ResetTime()
    {
        _currSecond = _maxSecond;
    }

    private void Awake()
    {
        textMesh.text = startInfo;
        enabled = false;
        textMesh.transform.parent = infoTransform;
        textMesh.transform.localPosition = Vector3.zero;
        textMesh.transform.localRotation = Quaternion.identity;
    }

    public void StartCounting()
    {
        enabled = true;
        textMesh.transform.parent = timerTransform;
        textMesh.transform.localPosition = Vector3.zero;
        textMesh.transform.localRotation = Quaternion.identity;
    }

    public void StopCounting()
    {
        enabled = false;
    }

    private void Update()
    {
        if (_currSecond >= 0)
        {
            _currSecond -= Time.deltaTime;

            int minutes = (int)_currSecond / 60;
            int seconds = (int)_currSecond % 60;
            textMesh.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (_currSecond < 0.25f * _maxSecond)
                textMesh.color = Color.red;
        }
        else
            textMesh.text = endInfo;
    }
}
