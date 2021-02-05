using UnityEngine;
using UnityEngine.InputSystem;

public class Aiming : MonoBehaviour {
    [SerializeField] private InputEventRelay playerInputEventRelay;
    [SerializeField] private TrainingEventRelay trainingEventRelay;
    [SerializeField] private Transform aimPointTransform;
    
    [SerializeField] private float aimMoveSpeedKeyboardMouse;
    [SerializeField] private float aimMoveSpeedGamepad;
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private bool useBounds;
    [SerializeField] private Vector2 xBounds;
    [SerializeField] private Vector2 zBounds;

    private Vector3 aimDirection;
    private float currentAimMoveSpeed;
    private Vector3 startingPosition;
    private Vector3 targetPosition;

    private bool aimingEnabled;

    public Transform AimPointTransform {
        get => aimPointTransform;
        set => aimPointTransform = value;
    }

    private void OnEnable() {
        if (playerInputEventRelay) playerInputEventRelay.OnAimEvent += SetAimDirection;
        
        if (trainingEventRelay) trainingEventRelay.OnStartPlay += OnStartPlay;
        if (trainingEventRelay) trainingEventRelay.OnEndPlay += OnEndPlay;
        
        // GameEvents.Current.OnStartPlay += OnStartPlay;
        // GameEvents.Current.OnEndPlay += OnEndPlay;
    }
    
    private void OnDisable() {
        if (playerInputEventRelay) playerInputEventRelay.OnAimEvent -= SetAimDirection;
        
        if (trainingEventRelay) trainingEventRelay.OnStartPlay -= OnStartPlay;
        if (trainingEventRelay) trainingEventRelay.OnEndPlay -= OnEndPlay;
        
        // if (GameEvents.Current) GameEvents.Current.OnStartPlay -= OnStartPlay;
        // if (GameEvents.Current) GameEvents.Current.OnEndPlay -= OnEndPlay;
    }

    private void Start() {
        aimPointTransform.position = defaultPosition;
        currentAimMoveSpeed = aimMoveSpeedKeyboardMouse;
        startingPosition = defaultPosition;
        targetPosition = defaultPosition;
    }

    private void Update() {
        if (!aimingEnabled) return;

        Aim();
    }

    public void SetAimDirection(Vector2 direction) {
        aimDirection.x = direction.x;
        aimDirection.z = direction.y;
    }
    
    public void SetAimDirection(Vector2 direction, bool isMouse) {
        aimDirection.x = direction.x;
        aimDirection.z = direction.y;
        
        currentAimMoveSpeed = isMouse ? aimMoveSpeedKeyboardMouse : aimMoveSpeedGamepad;
    }

    private void Aim() {
        startingPosition = aimPointTransform.position;
        targetPosition = startingPosition + aimDirection * currentAimMoveSpeed;
        // targetPosition += aimDirection * (GetCurrentAimMoveSpeed() * Time.deltaTime);

        if (useBounds) {
            targetPosition.x = Mathf.Clamp(targetPosition.x, xBounds.x, xBounds.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, zBounds.x, zBounds.y);
        }

        aimPointTransform.position = targetPosition;
    }

    private void OnStartPlay() {
        aimingEnabled = true;
    }

    private void OnEndPlay() {
        aimingEnabled = false;
    }
}