using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineController : MonoBehaviour {
	public List<RowVideoPair> rowVideoList = new List<RowVideoPair>();
	public Slider timelineSlider;
	private bool sorted = false;

	// Use this for initialization
	void Start() {
		PersistentStorage.init();
		timelineSlider.maxValue = PersistentStorage.list.Count;
		timelineSlider.value = timelineSlider.maxValue;
		Debug.LogFormat("Found {0} video players", timelineSlider.maxValue);
	}

	public void onTimelineSliderValueChanged() {
		Debug.LogFormat("Show video before {0}", timelineSlider.value);
		if (!sorted) {
			rowVideoList.Sort();
			sorted = true;
		}
		for (int i = 0; i < rowVideoList.Count; i++) {
			if (i < timelineSlider.value) {
				changeVisibility(rowVideoList[i].videoPlayer, true);
			} else {
				changeVisibility(rowVideoList[i].videoPlayer, false);
			}
		}
	}

	public void changeVisibility(GameObject videoPlayer, bool showing) {
		// toggles the visibility of this gameobject and all it's children
		var renderers = videoPlayer.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers) {
			r.enabled = showing;
		}
	}
 }
