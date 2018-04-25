using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour {

	public GameObject playButton;
	private Renderer rend;
	public Material playButtonMaterial;
	public Material pauseButtonMaterial;
	private VideoPlayer videoPlayer;
	public GameObject playHead;

	void Awake () {
		videoPlayer = this.transform.GetComponent<VideoPlayer> ();
	}

	// Use this for initialization
	void Start () {
		rend = playButton.transform.GetComponent<Renderer> ();
		rend.enabled = enabled;
	}
	
	// Update is called once per frame
	void Update () {
		bool stopHead = playHead.transform.GetComponent<PlayHeadMover> ().stopHead;
		if (videoPlayer.isPlaying && !stopHead) {
			double fraction = (double)videoPlayer.frame / (double)videoPlayer.clip.frameCount;
			playHead.transform.GetComponent<PlayHeadMover> ().MovePlayhead (fraction);
		}
	}

	public void PlayOrPause() {
		if (videoPlayer.isPlaying) {
			videoPlayer.Pause ();
			rend.sharedMaterial = playButtonMaterial;
		} else {
			videoPlayer.Play ();
			rend.sharedMaterial = pauseButtonMaterial;
		}
	}

	public void MoveForward() {
		if (videoPlayer.isPlaying) {
			double fraction = (double)videoPlayer.frame / (double)videoPlayer.clip.frameCount + (double)0.05;
			if (fraction > (double)1.0) {
				fraction = (double)0.0;
			} 
			double frame = (double)videoPlayer.clip.frameCount * fraction;
			videoPlayer.frame = (long)frame;
		}
	}
		

}
