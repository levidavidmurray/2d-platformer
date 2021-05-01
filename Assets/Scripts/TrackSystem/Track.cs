using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Track : MonoBehaviour
{
    public TrackRider rider;
    public List<int> enterPointIndexes = new List<int>();
    public List<TrackPoint> points = new List<TrackPoint>();

    private LineRenderer lineRenderer;
    private Vector3 positionSumLastFrame;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        CreateTrackPoints();
    }

    void Update()
    {
        if (points.Count != transform.childCount || lineRenderer.positionCount != transform.childCount)
        {
            CreateTrackPoints();
        }

        Vector3[] positions = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            positions[i] = transform.GetChild(i).position;
            points[i].Position = positions[i];
        }

        lineRenderer.SetPositions(positions);
    }

    public TrackEnterPoint GetTrackEnterPointForTransform(string name)
    {
        for (int i = 0; i < enterPointIndexes.Count; i++)
        {
            var index = enterPointIndexes[i];
            if (transform.GetChild(index).name == name) {
                return (TrackEnterPoint) points[index];
            }
        }

        return null;
    }

    void CreateTrackPoints()
    {
        lineRenderer.positionCount = transform.childCount;
        points = new List<TrackPoint>(transform.childCount);
        enterPointIndexes = new List<int>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            lineRenderer.SetPosition(i, child.position);
            
            if (child.name.Contains("TrackEnter") && enterPointIndexes.Count < 2)
            {
                print($"Enter point found: {child.name}");
                var enterPoint = new TrackEnterPoint(child.position, enterPointIndexes.Count, child.name);
                enterPointIndexes.Add(i);
                points.Add(enterPoint);

                continue;
            }

            print($"Track point found: {child.name}");

            var point = new TrackPoint(child.position, child.name);
            points.Add(point);
        }

        print($"Points created: {points.Count}");

        for (int i = 0; i < points.Count; i++)
        {
            TrackPoint point = points[i];
            var child = transform.GetChild(i);

            if (i == 0 && point is TrackEnterPoint)
            {
                print($"Enter point found: {child?.name}, adding target: {transform.GetChild(i+1)?.name}, i: {i}, points[{i+1}]: {points[i+1]}");
                point.AddTarget(points[i+1], 0);
                print($"Enter point target added {child?.name}");
                continue;
            }

            print($"{child?.name} is not first enter point");

            if (i == points.Count - 1)
            {
                print($"Last point found: {child?.name}");
                if (point is TrackEnterPoint)
                {
                    print($"Last point is enter point: {child?.name}, target: {transform.GetChild(i-1)?.name}");
                    point.AddTarget(points[i-1], 1);
                }

                print($"Last point is track point: {child?.name}, no target added");

                continue;
            }

            print($"track point: {child?.name}, track count: {enterPointIndexes.Count}");

            print($"track point: {child?.name}, track 0 target: {transform.GetChild(i+1)?.name}");

            point.AddTarget(points[i+1], 0);

            if (enterPointIndexes.Count > 1)
            {
                print($"track point: {child?.name}, track 1 target: {transform.GetChild(i-1)?.name}");
                point.AddTarget(points[i-1], 1);
            }
        }
    }
}
