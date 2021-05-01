using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEnterPoint : TrackPoint
{
    public int track;

    public TrackEnterPoint(Vector2 location, int track, string name) : base(location, name, track+1)
    {
        this.track = track;
        Debug.Log($"{name}, {track}, {Targets.Count}");
    }
}