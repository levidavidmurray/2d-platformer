using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
                var enterPoint = new TrackEnterPoint(child.position, enterPointIndexes.Count, child.name);
                enterPointIndexes.Add(i);
                points.Add(enterPoint);

                continue;
            }

            var point = new TrackPoint(child.position, child.name);
            points.Add(point);
        }

        for (int i = 0; i < points.Count; i++)
        {
            TrackPoint point = points[i];
            var child = transform.GetChild(i);

            if (i == 0 && point is TrackEnterPoint)
            {
                point.AddTarget(points[i+1], 0);
                continue;
            }

            if (i == points.Count - 1)
            {
                if (point is TrackEnterPoint)
                {
                    point.AddTarget(points[i-1], 1);
                }

                continue;
            }

            point.AddTarget(points[i+1], 0);

            if (enterPointIndexes.Count > 1)
            {
                point.AddTarget(points[i-1], 1);
            }
        }
    }

}
