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

	public Rect mapScreenToCamera(Rect screenRect, int screenWidth, int screenHeight, int videoWidth, int videoHeight) {
		Rect videoRect = new Rect();
		videoRect.x = (int)Math.Round(screenRect.x / screenWidth * videoWidth);
		videoRect.y = (int)Math.Round(screenRect.y / screenHeight * videoHeight);
		videoRect.width = (int)Math.Round(screenRect.width / screenWidth * videoWidth);
		videoRect.height = (int)Math.Round(screenRect.height / screenHeight * videoHeight);
		return videoRect;
	}

	public void switchStatus(RecordingState toState) {
		currentRecordingState = toState;
		if (toState == RecordingState.Recording) {
			buttonTextMesh.text = "Stop";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 195);
			byte[] videoPath = new byte[128];
			Array.Clear(videoPath, 0, videoPath.Length);
			Rect screenRect = recordingAreaSelection.GetComponent<RecordingAreaSelection>().getVideoRect();
			Rect videoRect = mapScreenToCamera(screenRect, Screen.width, Screen.height, camController.cameraWidth, camController.cameraHeight);

			Debug.LogFormat("Screen dimension: {0}, {1}", Screen.width, Screen.height);
			Debug.LogFormat("Screen rect: {0}", screenRect);
			Debug.LogFormat("Video dimension: {0}, {1}", camController.cameraWidth, camController.cameraHeight);
			Debug.LogFormat("Video rect: {0}", videoRect);
#if !UNITY_EDITOR
			int pathLength = startRecording(camController.cameraWidth, camController.cameraHeight, (int)videoRect.x, (int)videoRect.y, (int)videoRect.width, (int)videoRect.height, videoPath);	
			currentRow = new Row();
			currentRow.imageTargetId = gameObject.name;
			currentRow.localPosition = transform.InverseTransformPoint(Camera.main.transform.position);
			currentRow.localRotation = Camera.main.transform.rotation * Quaternion.Inverse(transform.rotation);
			byte[] videoPathTrim = new byte[pathLength];
			Array.Copy(videoPath, videoPathTrim, pathLength);
			Debug.LogFormat("Video filename: {0} length: {1}", videoPathTrim, pathLength);
			currentRow.videoPath = Encoding.UTF8.GetString(videoPathTrim);
			PersistentStorage.appendNewRow(currentRow);
#endif
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
	private static extern int startRecording(int width, int height, int x, int y, int w, int h, byte[] videoPath);
	[DllImport ("__Internal")]
	private static extern void stopRecording();
}
