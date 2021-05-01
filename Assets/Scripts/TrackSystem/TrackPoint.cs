using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPoint
{
    public List<TrackPoint> Targets;
    public Vector2 Position;
    public string name { get; private set; }

    public TrackPoint(Vector2 position, string name, int numOfTracks = 1)
    {
        Position = position;
        Targets = new List<TrackPoint> ( new TrackPoint[numOfTracks] );
        this.name = name;

        Debug.Log($"TrackPoint INIT: name: {name}, numOfTracks: {numOfTracks}, targets[{Targets.Count}]=[{TargetNames()}]");
    }

    public void AddTarget(TrackPoint target)
    {
        Targets.Add(target);
    }

    public void AddTarget(TrackPoint target, int track)
    {
        Debug.Log($"ADDING TARGET: {this}");
        Targets.Insert(track, target);
    }

    public bool HasTrackTarget(int track)
    {
        var hasTrack = Targets.Count > track && Targets[track] != null;
        return hasTrack;
    }

    public TrackPoint GetTarget(int track)
    {
        if (!HasTrackTarget(track)) return null;

        return Targets[track];
    }

    public string TargetNames()
    {
        string names = "";
        for (var i = 0; i < Targets.Count; i++)
        {
            bool isNull = Targets[i] == null;
            names = !isNull ? $"{Targets[i].name}, " : $"{null}, ";
        }

        return names;
    }

    public override string ToString()
    {
        return $"<{this.GetType()}>{{name={name} targets[{Targets.Count}]=[{TargetNames()}] position={Position} }}";
    }

}