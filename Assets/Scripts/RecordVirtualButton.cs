using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Vuforia;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.CognitoIdentity;

public enum RecordingState {Idle, Prepare, Recording};
public class RecordVirtualButton : MonoBehaviour, IVirtualButtonEventHandler {
	public GameObject virtualButton;
	public GameObject recordingAreaSelection;
	public TextMesh buttonTextMesh;
	public GameObject buttonCube;
	public RecordingState currentRecordingState = RecordingState.Idle;
	public CamController camController;
	public AmazonS3Client S3Client;

	void Start() {
		virtualButton.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
		PersistentStorage.init();
		initHelper();

		UnityInitializer.AttachToGameObject(this.gameObject);
		AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
		AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;
		AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
		AWSConfigs.LoggingConfig.LogMetrics = true;
		AWSConfigs.CorrectForClockSkew = true;
		
		// Initialize the Amazon Cognito credentials provider
		CognitoAWSCredentials credentials = new CognitoAWSCredentials (
			"us-east-1:4616ae63-91e5-49e2-b3b1-4844f7af97f2", // Identity pool ID
			RegionEndpoint.USEast1 // Region
		);
		S3Client = new AmazonS3Client(credentials);
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
			startRecording(camController.cameraWidth, camController.cameraHeight, videoPath);
			
			Row row = new Row();
			row.imageTargetId = gameObject.name;
			row.localPosition = transform.InverseTransformPoint(Camera.main.transform.position);
			row.localRotation = Camera.main.transform.rotation * Quaternion.Inverse(transform.rotation);
			row.videoPath = Encoding.ASCII.GetString(videoPath);
			postVideo(row);
		} else if (toState == RecordingState.Idle) {
			buttonTextMesh.text = "Record";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(8, 103, 16, 195);
			stopRecording();
		} else if (toState == RecordingState.Prepare) {
			buttonTextMesh.text = "Prepare";
			buttonCube.GetComponent<Renderer>().material.color = new Color32(191, 173, 27, 195);
		}
	}

	public void postVideo(Row row) {
		var stream = new FileStream(row.videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		string key = Path.GetFileName(row.videoPath);
		var request = new PostObjectRequest()
		{
			Bucket = "arecorder-video-files",
			Key = key,
			InputStream = stream,
			CannedACL = S3CannedACL.Private
		};
		S3Client.PostObjectAsync(request, (responseObj) =>
		{
			if (responseObj.Exception == null)
			{
				row.videoPath = "https://s3.amazonaws.com/arecorder-video-files/" + key;
				PersistentStorage.appendNewRow(row);
				Debug.LogFormat("object {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket);
			}
			else
			{
				Debug.LogFormat("Exception while posting the result object\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString());
			}
		});
	}

	[DllImport ("__Internal")]
	private static extern void initHelper();
	[DllImport ("__Internal")]
	private static extern void startRecording(int width, int height, byte[] videoPath);
	[DllImport ("__Internal")]
	private static extern void stopRecording();
}
