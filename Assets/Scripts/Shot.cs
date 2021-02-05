using UnityEngine;

[System.Serializable]
public class Shot {
    [SerializeField] private ShotType shotType;
    [SerializeField] private float peakHeight;
    [SerializeField] private float gravityMagnitude;
    [SerializeField] private Vector3 bounceForce;

    public ShotType TypeOfShot => shotType;
    public float PeakHeight => peakHeight;
    public float GravityMagnitude => gravityMagnitude;
    public Vector3 BounceForce => bounceForce;

    public Vector3 GetShotImpulse(Vector3 startPosition, Vector3 targetPosition) {
        float unsignedGravityMagnitude = Mathf.Abs(gravityMagnitude);
        float targetToStartPositionDistanceY = targetPosition.y - startPosition.y;
        float worldPeakHeight = peakHeight - startPosition.y;
        Vector3 targetToStartPositionDistanceXZ = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z);

        if (targetPosition.y > worldPeakHeight) {
            worldPeakHeight = targetPosition.y;
        }

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * unsignedGravityMagnitude * worldPeakHeight);
        Vector3 velocityXZ = targetToStartPositionDistanceXZ / (Mathf.Sqrt(2 * worldPeakHeight / unsignedGravityMagnitude) + Mathf.Sqrt(2 * (worldPeakHeight - targetToStartPositionDistanceY) / unsignedGravityMagnitude));

        return velocityXZ + velocityY * Mathf.Sign(gravityMagnitude);
    }
}