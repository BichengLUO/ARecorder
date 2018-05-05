﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlaceMultipleVideos : MonoBehaviour {
	public List<Row> list;
	public List<ImagetargetPositionInfo> allImagetargetInfo;
	private List<GameObject> arrows = new List<GameObject>();
	private List<float> arrowsToOriginDistance = new List<float> ();
	public GameObject videoPlayerPrefab;
	public GameObject arrowPrefab;
	public ImagetargetPositionInfo currentImagetargetInfo;
	private Vector3 initPos = Vector3.zero;
	// Use this for initialization
	IEnumerator Start () {
		PersistentStorage.init();
		ReadImagetargetPosition.init ();
		list = PersistentStorage.findForImageTargetId(gameObject.name);
		currentImagetargetInfo = ReadImagetargetPosition.findForImageTargetPosition (gameObject.name);
		allImagetargetInfo = ReadImagetargetPosition.getAllImagetargerPositionInfo ();

		foreach (Row row in list) {
			Vector3 shrinkedPos = new Vector3(row.localPosition.x / 3.0f,
											  row.localPosition.y / 3.0f,
											  row.localPosition.z / 3.0f);
			Vector3 position = transform.TransformPoint(shrinkedPos);
			Quaternion rotation = transform.rotation * row.localRotation;
			GameObject videoPlayer = Instantiate(videoPlayerPrefab, transform);
			videoPlayer.transform.position = position;
			videoPlayer.transform.rotation = rotation;
			GameObject videoPlane = videoPlayer.transform.Find("VideoPlane").gameObject;
			TextMesh videoName = videoPlayer.transform.Find("VideoName").GetComponent<TextMesh>();
			videoName.text = row.videoPath;
			VideoPlayer player = videoPlane.GetComponent<VideoPlayer>();
			player.url = Application.persistentDataPath + "/" + row.videoPath;
			Debug.LogFormat("Row: {0} {1}", row.imageTargetId, row.videoPath);
			player.Play();
			yield return new WaitForSeconds(1f); //wait the first frame to show up
			player.Pause();
		}

		foreach (ImagetargetPositionInfo info in allImagetargetInfo) {
			if (info.imageTargetId == this.gameObject.name) {
				initPos = new Vector3 (info.x, info.y, info.z);
				arrows.Add (null);
				arrowsToOriginDistance.Add (0.0f);
			} else {
				GameObject newArrow = GameObject.Instantiate (arrowPrefab, transform);
				TextMesh pointedImage = newArrow.transform.Find ("pointedImage").GetComponent<TextMesh>();
				pointedImage.text = info.imageTargetId;
				float distance = Vector3.Distance (transform.position, newArrow.transform.position);
				arrows.Add (newArrow);
				arrowsToOriginDistance.Add (distance);
			}
		}
	}

	void Update() {
		Vector3 curPos = transform.position;
		for (int i = 0; i < allImagetargetInfo.Count; i++) {
			if (arrows [i] != null) {
				float temp = Mathf.Atan2 (curPos.z - allImagetargetInfo[i].z, curPos.x - allImagetargetInfo[i].x) * 180 / Mathf.PI;
				arrows[i].transform.eulerAngles = new Vector3(0, temp, 0);
				arrows[i].transform.position = new Vector3 (arrowsToOriginDistance[i] * Mathf.Cos(temp), 0, arrowsToOriginDistance[i] * Mathf.Sin(temp));
			}
		}
	}
}
