using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Vuforia;

public enum RecordingState {Idle, Prepare, Recording};
public class RecordVirtualButton : MonoBehaviour, IVirtualButtonEventHandler {
	public GameObject virtualButton;
	public GameObject recordingAreaSelection;
	public TextMesh buttonTextMesh;
	public GameObject buttonCube;
	public RecordingState currentRecordingState = RecordingState.Idle;
	public CamController camController;
	public GameObject loading;
	public Row currentRow;

	void Start() {
		virtualButton.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
		PersistentStorage.init();
#if !UNITY_EDITOR
		initHelper();
#endif
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
			byte[] videoPath = new byte[128];
			Array.Clear(videoPath, 0, videoPath.Length);
#if !UNITY_EDITOR
			startRecording(camController.cameraWidth, camController.cameraHeight, videoPath);
#endif		
			currentRow = new Row();
			currentRow.imageTargetId = gameObject.name;
			currentRow.localPosition = transform.InverseTransformPoint(Camera.main.transform.position);
			currentRow.localRotation = Camera.main.transform.rotation * Quaternion.Inverse(transform.rotation);
			currentRow.videoPath = Encoding.ASCII.GetString(videoPath);
			PersistentStorage.appendNewRow(currentRow);
		} else if (toState == RecordingState.Idle) {
			buttonTextMesh.text = "Record";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(8, 103, 16, 195);
#if !UNITY_EDITOR
			stopRecording();
#endif
		} else if (toState == RecordingState.Prepare) {
			buttonTextMesh.text = "Prepare";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(191, 173, 27, 195);
		}
	}

	[DllImport ("__Internal")]
	private static extern void initHelper();
	[DllImport ("__Internal")]
	private static extern void startRecording(int width, int height, byte[] videoPath);
	[DllImport ("__Internal")]
	private static extern void stopRecording();
}
