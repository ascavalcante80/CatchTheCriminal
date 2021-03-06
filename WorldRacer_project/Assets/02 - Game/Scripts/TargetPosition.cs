﻿using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    private AbstractMap _map;
    [System.NonSerialized]
    public Coordinate targetCoordinate;

    void Awake()
    {
        _map = FindObjectOfType<AbstractMap>();
    }

    public void UpdateInformation(Coordinate coordinate)
    {
        targetCoordinate = coordinate;
        Vector2d vector2d = new Vector2d(coordinate.latitude, coordinate.longitude);
        transform.position = _map.GeoToWorldPosition(vector2d);
    }
}
