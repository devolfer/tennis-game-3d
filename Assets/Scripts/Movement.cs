using CustomUtilities;
using UnityEngine;

public class Movement : MonoBehaviour {
    [SerializeField] private InputEventRelay playerInputEventRelay;
    [SerializeField] private TrainingEventRelay trainingEventRelay;
    [SerializeField] private Rigidbody body;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedSmoothTime;

    private Vector3 lastMoveDirection;
    private Vector3 moveDirection;
    private float increaseLerpTimer;
    private float decreaseLerpTimer;

    private float targetSpeed;
    private float currentSpeed;
    private float speedSmoothVelocity;
    private Vector3 velocityChange;
    private Vector3 currentVelocity;

    private bool movingEnabled;

    private void OnEnable() {
        if (playerInputEventRelay) playerInputEventRelay.OnMoveEvent += SetMoveDirection;
        
        if (trainingEventRelay) trainingEventRelay.OnStartPlay += OnStartPlay;
        if (trainingEventRelay) trainingEventRelay.OnEndPlay += OnEndPlay;
        
        // GameEvents.Current.OnStartPlay += OnStartPlay;
        // GameEvents.Current.OnEndPlay += OnEndPlay;
    }
    
    private void OnDisable() {
        if (playerInputEventRelay) playerInputEventRelay.OnMoveEvent -= SetMoveDirection;
        
        if (trainingEventRelay) trainingEventRelay.OnStartPlay -= OnStartPlay;
        if (trainingEventRelay) trainingEventRelay.OnEndPlay -= OnEndPlay;
        
        // if (GameEvents.Current) GameEvents.Current.OnStartPlay -= OnStartPlay;
        // if (GameEvents.Current) GameEvents.Current.OnEndPlay -= OnEndPlay;
    }

    private void FixedUpdate() {
        if (!movingEnabled) return;

        Move();
    }

    public void SetMoveDirection(Vector2 direction) {
        moveDirection.x = direction.x;
        moveDirection.z = direction.y;
    }

    private void Move() {
        targetSpeed = moveSpeed * moveDirection.magnitude;

        // TODO detect moving exactly opposite to current move direction
        if (moveDirection.sqrMagnitude > 0 && moveDirection == -lastMoveDirection) {
            // Debug.Log("Change");
        }

        if (moveDirection.sqrMagnitude != 0) {
            if (increaseLerpTimer < speedSmoothTime) increaseLerpTimer += Time.fixedDeltaTime;
            decreaseLerpTimer = 0f;

            currentSpeed = Lerp.Value(currentSpeed, targetSpeed, increaseLerpTimer / speedSmoothTime, Easing.SmoothStep.Smooth);

            currentVelocity = body.velocity;
            velocityChange = currentSpeed * moveDirection - currentVelocity;
        } else {
            increaseLerpTimer = 0f;
            if (decreaseLerpTimer < speedSmoothTime) decreaseLerpTimer += Time.fixedDeltaTime;

            currentSpeed = Lerp.Value(currentSpeed, targetSpeed, decreaseLerpTimer / speedSmoothTime, Easing.SmoothStep.Smooth);

            currentVelocity = body.velocity;
            velocityChange = currentSpeed * currentVelocity.normalized - currentVelocity;
        }

        lastMoveDirection = moveDirection;

        // targetSpeed = moveSpeed * moveDirection.magnitude;
        //
        // currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        // currentVelocity = body.velocity;
        // velocityChange = (moveDirection.sqrMagnitude > 0 ? currentSpeed * moveDirection : currentSpeed * currentVelocity.normalized) - currentVelocity;
        // velocityChange.y = 0f;

        body.AddForce(velocityChange, ForceMode.Impulse);
    }
    
    private void OnStartPlay() {
        movingEnabled = true;
    }

    private void OnEndPlay() {
        body.velocity = Vector3.zero;
        movingEnabled = false;
    }
}