using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayHeadMover : MonoBehaviour {

	public GameObject startPoint;
	public GameObject endPoint;
	private Vector3 startPosition, endPosition;
	public VideoPlayer videoPlayer;
	public bool stopHead;
	public GameObject processingBar;

	void Start () {
		startPosition = startPoint.transform.position;
		endPosition = endPoint.transform.position;
		stopHead = false;
	}

	public void MovePlayhead(double playedFraction)
	{
		transform.position = Vector3.Lerp (startPosition, endPosition, (float)playedFraction);

	}
		
	// Update is called once per frame
	void Update () {
		RaycastHit hitInfo3 = new RaycastHit ();
		if (Input.GetMouseButton (0) && Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo3) && hitInfo3.transform.tag == "processingBar") {
			stopHead = true;
			videoPlayer.Pause ();
			Transform prevHeadTransform = this.transform;
//			this.transform.position = new Vector3 (hitInfo3.transform.position.x, prevHeadTransform.transform.position.y, prevHeadTransform.transform.position.z);
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float distance; 
			float planeZ = 0;
			Plane plane = new Plane (Vector3.back, Vector3.back * planeZ); 
			if (plane.Raycast (ray, out distance)) {
				Vector3 temp_position = ray.GetPoint (distance);
				temp_position = new Vector3 (temp_position.x, prevHeadTransform.position.y, prevHeadTransform.position.z);
				if (temp_position.x < startPosition.x) {
					temp_position.x = startPosition.x;
				}
				if (temp_position.x > endPosition.x) {
					temp_position.x = endPosition.x;
				}
				prevHeadTransform.position = temp_position;
			}

			float newXPosition = prevHeadTransform.position.x;
			double fraction = (double)(newXPosition - startPosition.x) / (double)(endPosition.x - startPosition.x);
			double frame = (double)videoPlayer.clip.frameCount * fraction;

			videoPlayer.frame = (long)frame;
			videoPlayer.Play ();
			stopHead = false;
		}
	}
}
