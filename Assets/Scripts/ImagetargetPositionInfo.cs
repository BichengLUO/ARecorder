using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImagetargetPositionInfo {
	public ImagetargetPositionInfo() {
		
	}
	public ImagetargetPositionInfo(string i, float x_, float y_, float z_) {
		imageTargetId = i;
		x = x_;
		y = y_;
		z = z_;
	}
	public string imageTargetId;
	public float x;
	public float y;
	public float z;
}