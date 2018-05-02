using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingAreaSelection : MonoBehaviour {
	public RecordVirtualButton recordVirtualButton;
	public GameObject leftHand;
	public GameObject rightHand;
	public int waitCount = 100;
	public float waitEps = 10.0f;
	private Rect lastRect;
	private int equalCnt = 0;

	void OnGUI()
	{
		Rect rect;
		Texture2D texture = new Texture2D(1, 1);
		if (recordVirtualButton.currentRecordingState == RecordingState.Recording) {
			rect = lastRect;
			texture.SetPixel(0, 0, new Color(1f, 0, 0, 0.2f));
		} else {
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
			rect = new Rect(x, y, width, height);

			if (almostRectEqual(rect, lastRect, waitEps)) {
				equalCnt++;
				if (equalCnt == waitCount) {
					recordVirtualButton.switchStatus(RecordingState.Recording);
					equalCnt = 0;
				}
			} else {
				equalCnt = 0;
			}
			lastRect = rect;
			texture.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.5f));
		}

		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(rect, GUIContent.none);
	}

	public Rect getVideoRect() {
		return lastRect;
	}

	private bool almostRectEqual(Rect r1, Rect r2, float eps) {
		return Math.Abs(r1.x - r2.x) < eps &&
			   Math.Abs(r1.y - r2.y) < eps &&
			   Math.Abs(r1.width - r2.width) < eps &&
			   Math.Abs(r1.height - r2.height) < eps;
	}
}
