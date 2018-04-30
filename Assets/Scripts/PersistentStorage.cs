using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentStorage {
	public static string filePath;
	public static List<Row> list = new List<Row>();
	public static void init() {
		filePath = Application.persistentDataPath + "/storage.csv";
		Debug.Log("Storage path: " + filePath);
		if (!File.Exists(filePath)) {
			File.CreateText(filePath);
		}
		list.Clear();
		using (StreamReader sr = File.OpenText(filePath)) {
			string line;
			while((line = sr.ReadLine()) != null) {
				if (line.Equals("")) {
					continue;
				}
				Row entry = new Row();
				string[] parts = line.Split(',');
				entry.imageTargetId = parts[0];
				string[] positionParts = parts[1].Split(null);
				entry.localPosition = new Vector3(float.Parse(positionParts[0]),
												float.Parse(positionParts[1]),
												float.Parse(positionParts[2]));
				string[] rotationParts = parts[2].Split(null);
				entry.localRotation = new Quaternion(float.Parse(rotationParts[0]),
													float.Parse(rotationParts[1]),
													float.Parse(rotationParts[2]),
													float.Parse(rotationParts[3]));
				entry.videoPath = parts[3];
				list.Add(entry);
			}
		}
	}

	public static List<Row> findForImageTargetId(string imageTargetId) {
		List<Row> results = new List<Row>();
		foreach (Row row in list) {
			if (row.imageTargetId == imageTargetId) {
				results.Add(row);
			}
		}
		return results;
	}

	public static void appendNewRow(Row row) {
		list.Add(row);
		using (StreamWriter sw = File.AppendText(filePath)) {
			sw.Write(row.imageTargetId + ",");
			sw.Write(row.localPosition.x.ToString() + " ");
			sw.Write(row.localPosition.y.ToString() + " ");
			sw.Write(row.localPosition.z.ToString() + ",");
			sw.Write(row.localRotation.x.ToString() + " ");
			sw.Write(row.localRotation.y.ToString() + " ");
			sw.Write(row.localRotation.z.ToString() + " ");
			sw.Write(row.localRotation.w.ToString() + ",");
			sw.WriteLine(row.videoPath);
		}
	}
}
