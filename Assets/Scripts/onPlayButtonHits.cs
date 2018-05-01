using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class onPlayButtonHits : MonoBehaviour {

	public VideoPlayer videoPlayer;

	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo2 = new RaycastHit ();
		if (Input.GetMouseButton (0) && Physics.Raycast (Camera.allCameras[0].ScreenPointToRay (Input.mousePosition), out hitInfo2)
			&& hitInfo2.rigidbody.gameObject == gameObject) {
			Debug.Log("Play button hit!");
			VideoPlayerController vpc = videoPlayer.GetComponent<VideoPlayerController> ();
			vpc.PlayOrPause ();
		}
	}
}
