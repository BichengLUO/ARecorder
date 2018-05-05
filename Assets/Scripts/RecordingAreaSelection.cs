using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LShapeType {TopLeft, TopRight, BottomLeft, BottomRight};
public class RecordingAreaSelection : MonoBehaviour {
	public RecordVirtualButton recordVirtualButton;
	public GameObject leftHand;
	public GameObject rightHand;
	public int waitCount = 100;
	public float waitEps = 10.0f;
	private Rect lastRect;
	private int equalCnt = 0;
	public Color lShapeColor = new Color(1f, 1f, 1f);
	public int longEdge = 50;
	public int shortEdge = 20;
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
			drawLShapeForTwoCorner(boxPosHiLeftCamera, boxPosLowRightCamera);

			float width = Math.Abs(boxPosHiLeftCamera.x - boxPosLowRightCamera.x) - 2 * shortEdge;
			float height = Math.Abs(boxPosHiLeftCamera.y - boxPosLowRightCamera.y) - 2 * shortEdge;
			float x = Math.Min(boxPosHiLeftCamera.x, boxPosLowRightCamera.x) + shortEdge;
			float y = Screen.height - Math.Max(boxPosHiLeftCamera.y, boxPosLowRightCamera.y) + shortEdge;
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

	public void drawLShapeForTwoCorner(Vector3 boxPosHiLeftCamera, Vector3 boxPosLowRightCamera) {
		if (boxPosHiLeftCamera.x < boxPosLowRightCamera.x && boxPosHiLeftCamera.y > boxPosLowRightCamera.y) {
			drawLShape(boxPosHiLeftCamera, LShapeType.TopLeft);
			drawLShape(boxPosLowRightCamera, LShapeType.BottomRight);
		} else if (boxPosHiLeftCamera.x >= boxPosLowRightCamera.x && boxPosHiLeftCamera.y > boxPosLowRightCamera.y) {
			drawLShape(boxPosHiLeftCamera, LShapeType.TopRight);
			drawLShape(boxPosLowRightCamera, LShapeType.BottomLeft);
		} else if (boxPosHiLeftCamera.x < boxPosLowRightCamera.x && boxPosHiLeftCamera.y <= boxPosLowRightCamera.y) {
			drawLShape(boxPosHiLeftCamera, LShapeType.BottomLeft);
			drawLShape(boxPosLowRightCamera, LShapeType.TopRight);
		} else {
			drawLShape(boxPosHiLeftCamera, LShapeType.BottomRight);
			drawLShape(boxPosLowRightCamera, LShapeType.TopLeft);
		}
	}

	public void drawLShape(Vector3 pos, LShapeType shapeType) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, lShapeColor);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		Rect rect1 = new Rect(0, 0, longEdge, shortEdge);
		Rect rect2 = new Rect(0, 0, shortEdge, longEdge);
		if (shapeType == LShapeType.TopLeft) {
			rect1.x = rect2.x = pos.x;
			rect1.y = rect2.y = Screen.height - pos.y;
		} else if (shapeType == LShapeType.TopRight) {
			rect1.x = pos.x - longEdge;
			rect2.x = pos.x - shortEdge;
			rect1.y = rect2.y = Screen.height - pos.y;
		} else if (shapeType == LShapeType.BottomLeft) {
			rect1.y = Screen.height - pos.y - shortEdge;
			rect2.y = Screen.height - pos.y - longEdge;
			rect1.x = rect2.x = pos.x;
		} else if (shapeType == LShapeType.BottomRight){
			rect1.x = pos.x - longEdge;
			rect1.y = Screen.height - pos.y - shortEdge;
			rect2.x = pos.x - shortEdge;
			rect2.y = Screen.height - pos.y - longEdge;
		}
		GUI.Box(rect1, GUIContent.none);
		GUI.Box(rect2, GUIContent.none);
	}

	private bool almostRectEqual(Rect r1, Rect r2, float eps) {
		return Math.Abs(r1.x - r2.x) < eps &&
			   Math.Abs(r1.y - r2.y) < eps &&
			   Math.Abs(r1.width - r2.width) < eps &&
			   Math.Abs(r1.height - r2.height) < eps;
	}
}
