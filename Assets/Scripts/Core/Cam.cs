﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
	[Range(-1f,1f)]
	public float upDown;

	public int currentLevel;
	private const float levelHeigth = 5.2f;

	private void LateUpdate()
	{
		var h = levelHeigth*currentLevel + levelHeigth*upDown + 2.65f;
		var newPos = transform.parent.InverseTransformPoint (Vector3.up*h);
		newPos.x = transform.localPosition.x;
		newPos.z = transform.localPosition.z;
		transform.localPosition = newPos;
	}

	public void MoveLevel ( int dir ) 
	{ currentLevel += dir; }
}