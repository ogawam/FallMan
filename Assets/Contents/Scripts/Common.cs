﻿using UnityEngine;
using System.Collections;

public class Common {

	static public float viewWidth = 1136f;
	static public float viewHeight = 640f;
	static public float scrn2View { get { return viewHeight / Screen.height; } }

	static public float r2d = (180f / Mathf.PI);
	static public float Rad2Deg(float radian) { return radian * r2d; }
	static public float ClipDeg(float degree) { 
		while(degree >= 360) degree -= 360;
		while(degree < 0) degree += 360;
		return degree;
	}
	static public float ClipDegMin(float degree) {
		degree = ClipDeg(degree);
		if(degree > 180)
			degree -= 360;
		return degree;
	} 
}