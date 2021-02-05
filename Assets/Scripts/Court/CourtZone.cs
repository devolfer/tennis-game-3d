using System;
using UnityEngine;

[Serializable]
public class CourtZone {
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;

    public Vector3 MinBounds => minBounds;
    public Vector3 MaxBounds => maxBounds;

    public bool WithinXBounds(Vector3 position, float tolerance = 0f) {
        return position.x >= minBounds.x - tolerance && position.x <= maxBounds.x + tolerance;
    }

    public bool WithinYBounds(Vector3 position, float tolerance = 0f) {
        return position.y >= minBounds.y - tolerance && position.y <= maxBounds.y + tolerance;
    }

    public bool WithinZBounds(Vector3 position, float tolerance = 0f) {
        return position.z >= minBounds.z - tolerance && position.z <= maxBounds.z + tolerance;
    }

    public bool WithinXYBounds(Vector3 position, float tolerance = 0f) {
        return WithinXBounds(position, tolerance) && WithinYBounds(position, tolerance);
    }

    public bool WithinXZBounds(Vector3 position, float tolerance = 0f) {
        return WithinXBounds(position, tolerance) && WithinZBounds(position, tolerance);
    }

    public bool WithinYZBounds(Vector3 position, float tolerance = 0f) {
        return WithinYBounds(position, tolerance) && WithinZBounds(position, tolerance);
    }

    public bool WithinBounds(Vector3 position, float tolerance = 0f) {
        return WithinXBounds(position, tolerance) && WithinYBounds(position, tolerance) && WithinZBounds(position, tolerance);
    }
}