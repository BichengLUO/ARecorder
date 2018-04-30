using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using Vuforia;

public class CamController : MonoBehaviour
{

	public int cameraWidth;
	public int cameraHeight;
	#region PRIVATE_MEMBERS

	private Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;

	private bool mAccessCameraImage = true;
	private bool mFormatRegistered = false;

	#endregion // PRIVATE_MEMBERS

	#region MONOBEHAVIOUR_METHODS

	void Start()
	{
#if !UNITY_EDITOR
		mPixelFormat = Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
#else
		mPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;
#endif

		// Register Vuforia life-cycle callbacks:
		VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
		VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);

	}

	#endregion // MONOBEHAVIOUR_METHODS

	#region PRIVATE_METHODS

	void OnVuforiaStarted()
	{

		// Try register camera image format
		if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
		{
			Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());

			mFormatRegistered = true;
		}
		else
		{
			Debug.LogError(
				"\nFailed to register pixel format: " + mPixelFormat.ToString() +
				"\nThe format may be unsupported by your device." +
				"\nConsider using a different pixel format.\n");

			mFormatRegistered = false;
		}

	}

	/// <summary>
	/// Called each time the Vuforia state is updated
	/// </summary>
	void OnTrackablesUpdated()
	{
		if (mFormatRegistered)
		{
			if (mAccessCameraImage)
			{
				Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);

				if (image != null)
				{
//					Debug.Log(
//						"\nImage Format: " + image.PixelFormat +
//						"\nImage Size:   " + image.Width + "x" + image.Height +
//						"\nBuffer Size:  " + image.BufferWidth + "x" + image.BufferHeight +
//						"\nImage Stride: " + image.Stride + "\n"
//					);

					byte[] pixels = image.Pixels;
					cameraWidth = image.Width;
					cameraHeight = image.Height;
//					if (pixels != null && pixels.Length > 0)
//					{
//						Debug.Log(
//							"\nImage pixels: " +
//							pixels[0] + ", " +
//							pixels[1] + ", " +
//							pixels[2] + ", ...\n"
//						);
//						Debug.Log (pixels.Length);
//					}
#if !UNITY_EDITOR
					unsafe {
						fixed (byte *pixelData = image.Pixels) {
							processBuffer ((int)(Time.time * 600), image.Width, image.Height, image.Stride, (IntPtr)pixelData);
						}
					}
#endif
				}
			}
		}
	}

	/// <summary>
	/// Called when app is paused / resumed
	/// </summary>
	void OnPause(bool paused)
	{
		if (paused)
		{
			Debug.Log("App was paused");
			UnregisterFormat();
		}
		else
		{
			Debug.Log("App was resumed");
			RegisterFormat();
		}
	}

	/// <summary>
	/// Register the camera pixel format
	/// </summary>
	void RegisterFormat()
	{
		if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
		{
			Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
			mFormatRegistered = true;
		}
		else
		{
			Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
			mFormatRegistered = false;
		}
	}

	/// <summary>
	/// Unregister the camera pixel format (e.g. call this when app is paused)
	/// </summary>
	void UnregisterFormat()
	{
		Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
		CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
		mFormatRegistered = false;
	}

	#endregion //PRIVATE_METHODS

	[DllImport ("__Internal")]
	private static extern void processBuffer(int ts, int width, int height, int stride, IntPtr pixelData);
}
