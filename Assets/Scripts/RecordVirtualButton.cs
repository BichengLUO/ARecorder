using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public enum RecordingState {Idle, Prepare, Recording};
public class RecordVirtualButton : MonoBehaviour, IVirtualButtonEventHandler {
	public GameObject virtualButton;
	public GameObject recordingAreaSelection;
	public TextMesh buttonTextMesh;
	public GameObject buttonCube;
	public RecordingState currentRecordingState = RecordingState.Idle;

	void Start() {
		virtualButton.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
	}
	public void OnButtonPressed(VirtualButtonBehaviour vb) {
		if (currentRecordingState == RecordingState.Idle) {
			recordingAreaSelection.SetActive(true);
			switchStatus(RecordingState.Prepare);
		} else if (currentRecordingState == RecordingState.Recording) {
			recordingAreaSelection.SetActive(false);
			switchStatus(RecordingState.Idle);
		}
	}
	public void OnButtonReleased(VirtualButtonBehaviour vb) {

	}

	public void switchStatus(RecordingState toState) {
		currentRecordingState = toState;
		if (toState == RecordingState.Recording) {
			buttonTextMesh.text = "Stop";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 195);
		} else if (toState == RecordingState.Idle) {
			buttonTextMesh.text = "Record";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(8, 103, 16, 195);
		} else if (toState == RecordingState.Prepare) {
			buttonTextMesh.text = "Prepare";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(191, 173, 27, 195);
		}
	}

}
