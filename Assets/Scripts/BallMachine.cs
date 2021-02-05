using CustomUtilities;
using UnityEngine;

public class BallMachine : MonoBehaviour {
    [SerializeField] private Transform machineTransform;
    [SerializeField] private Transform aimPointTransform;
    [SerializeField] private Vector3 shootOffset;

    public void MoveTo(Vector3 position, bool animated = false, float duration = 0) {
        if (animated) {
            Vector3 startPosition = machineTransform.position;
            Vector3 targetPosition = new Vector3(position.x + shootOffset.x, startPosition.y, position.z + shootOffset.z);

            this.DoRoutine(duration,
                null,
                t => {
                    machineTransform.position = Lerp.Value(startPosition, targetPosition, t, Easing.SmoothStep.Smoother);
                },
                () => {
                    machineTransform.position = targetPosition;
                });
        } else {
            machineTransform.position = new Vector3(position.x + shootOffset.x, machineTransform.position.y, position.z + shootOffset.z);
        }
    }

    public void AimAt(Vector3 position, bool animated = false, float duration = 0) {
        if (animated) {
            Vector3 startPosition = aimPointTransform.position;
            Vector3 targetPosition = position;

            this.DoRoutine(duration,
                null,
                t => {
                    aimPointTransform.position = Lerp.Value(startPosition, targetPosition, t, Easing.SmoothStep.Smoother);
                },
                () => {
                    aimPointTransform.position = targetPosition;
                });
        } else {
            aimPointTransform.position = position;
        }
    }

    public void ShootBall(TennisBall ball, FixedShot shot) {
        ball.transform.position = shot.StartPosition;
        ball.ResetProperties();
        ball.gameObject.SetActive(true);

        ball.Hit(shot);
    }
}