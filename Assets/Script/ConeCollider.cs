using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeCollider : MonoBehaviour
{
    public Transform perfectPoint;
    public List<GrapplingPoint> pointList;

    private void OnTriggerEnter(Collider other)
    {
        GrapplingPoint _point = other.GetComponent<GrapplingPoint>();
        if (_point != null)
        {
            pointList.Add(_point);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GrapplingPoint _point = other.GetComponent<GrapplingPoint>();
        if (_point != null)
        {
            if (pointList.Contains(_point))
            {
                pointList.Remove(_point);
            }
        }
    }

    public GrapplingPoint GetBestPoint()
    {
        if (pointList.Count == 0) return null;
        float _distance = 1000;
        GrapplingPoint _point = pointList[0];
        for (int i = 0; i < pointList.Count; i++)
        {
            float _currentDistance = Vector3.Distance(perfectPoint.position, pointList[i].transform.position);
            if (_currentDistance < _distance)
            {
                _point = pointList[i];
                _distance = _currentDistance;
            }
        }
        return _point;
    }
}
