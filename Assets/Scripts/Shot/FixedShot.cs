using System;
using UnityEngine;

[Serializable]
public class FixedShot : Shot {
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 targetPosition;

    public Vector3 StartPosition => startPosition;
    public Vector3 TargetPosition => targetPosition;
}