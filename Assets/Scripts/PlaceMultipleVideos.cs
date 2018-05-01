using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class PlaceMultipleVideos : MonoBehaviour {
	public List<Row> list;
	public GameObject videoPlayerPrefab;
	// Use this for initialization
	void Start () {
		PersistentStorage.init();
		list = PersistentStorage.findForImageTargetId(gameObject.name);
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
			VideoPlayer player = videoPlane.GetComponent<VideoPlayer>();
			player.url = Application.persistentDataPath + "/" + row.videoPath;
			Debug.LogFormat("Row: {0} {1}", row.imageTargetId, row.videoPath);
		}
	}
}
