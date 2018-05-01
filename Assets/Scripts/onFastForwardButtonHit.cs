using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class onFastForwardButtonHit : MonoBehaviour {
	
	public VideoPlayer videoPlayer;

	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo = new RaycastHit ();
		if (Input.GetMouseButton (0) && Physics.Raycast (Camera.allCameras[0].ScreenPointToRay (Input.mousePosition), out hitInfo)
			&& hitInfo.rigidbody.gameObject == gameObject) {
			VideoPlayerController vpc = videoPlayer.GetComponent<VideoPlayerController> ();
			vpc.MoveForward ();
		}

	}
}
