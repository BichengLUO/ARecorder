using System;
using System.Collections.Generic;
using UnityEngine;

public class RowVideoPair: IComparable<RowVideoPair>{
    public Row row;
    public GameObject videoPlayer;

    public RowVideoPair(Row r, GameObject v) {
        row = r;
        videoPlayer = v;
    }
    public int CompareTo(RowVideoPair otherRowVideoPair) {
        return row.videoPath.CompareTo(otherRowVideoPair.row.videoPath);
    }
}