using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingAreaSelection : MonoBehaviour {
	public GameObject leftHand;
	public GameObject rightHand;

	void OnGUI()
	{
		//top left point of rectangle
		Vector3 boxPosHiLeftWorld = leftHand.transform.position;
		//bottom right point of rectangle
		Vector3 boxPosLowRightWorld = rightHand.transform.position;

		Vector3 boxPosHiLeftCamera = Camera.main.WorldToScreenPoint(boxPosHiLeftWorld);
		Vector3 boxPosLowRightCamera = Camera.main.WorldToScreenPoint(boxPosLowRightWorld);

		float width = Math.Abs(boxPosHiLeftCamera.x - boxPosLowRightCamera.x);
		float height = Math.Abs(boxPosHiLeftCamera.y - boxPosLowRightCamera.y);
		float x = Math.Min(boxPosHiLeftCamera.x, boxPosLowRightCamera.x);
		float y = Screen.height - Math.Max(boxPosHiLeftCamera.y, boxPosLowRightCamera.y);
		Rect rect = new Rect(x, y, width, height);

		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, new Color(0, 0, 0.5f, 0.2f));
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(rect, GUIContent.none);
	}
}
