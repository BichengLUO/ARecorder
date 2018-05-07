using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadImagetargetPosition : MonoBehaviour {
	public static string filePath;
	public static List<ImagetargetPositionInfo> imagetargetPositionInfo = new List<ImagetargetPositionInfo>();
	public static void init() {
		filePath = Application.persistentDataPath + "/position.csv";
		Debug.Log("Position path: " + filePath);
		if (!File.Exists(filePath)) {
			StreamWriter sw = File.CreateText(filePath);
			sw.Close();
		}
		imagetargetPositionInfo.Clear();
		
		using (StreamReader sr = File.OpenText(filePath)) {
			string line;
			while((line = sr.ReadLine()) != null) {
				if (line.Equals("")) {
					continue;
				}
				ImagetargetPositionInfo entry = new ImagetargetPositionInfo();
				string[] parts = line.Split(',');
				entry.imageTargetId = parts[0];
				entry.x = float.Parse(parts [1]);
				entry.y = float.Parse(parts [2]);
				entry.z = float.Parse(parts [3]);
				imagetargetPositionInfo.Add(entry);
			}
		}
		if (imagetargetPositionInfo.Count == 0) {
			imagetargetPositionInfo.Add(new ImagetargetPositionInfo("ExperimentDesk", 0, 0, 0));
			imagetargetPositionInfo.Add(new ImagetargetPositionInfo("ExperimentDesk2", 1, 0, 0));
		}
	}

	public static ImagetargetPositionInfo findForImageTargetPosition(string imageTargetId) {
		ImagetargetPositionInfo result = new ImagetargetPositionInfo();
		foreach (ImagetargetPositionInfo imagetargetPositionInfo in imagetargetPositionInfo) {
			if (imagetargetPositionInfo.imageTargetId == imageTargetId) {
				return imagetargetPositionInfo;
			}
		}
		return null;
	}

	public static List<ImagetargetPositionInfo> getAllImagetargerPositionInfo() {
		return imagetargetPositionInfo;
	}
}
