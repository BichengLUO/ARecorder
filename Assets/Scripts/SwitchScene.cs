﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickRecordButton() {
		Debug.Log("Scene2 loading: ");
        SceneManager.LoadScene(1);
	}

	public void OnClickReplayButton() {
		Debug.Log("Scene2 loading: ");
		SceneManager.LoadScene(2);
	}

	public void OnClickBackButton() {
		Debug.Log("Scene2 loading: ");
		SceneManager.LoadScene(0);
	}	
}
