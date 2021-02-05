using System.Linq;
using UnityEngine;

public class TennisBall : MonoBehaviour {
    [SerializeField] private Rigidbody ballBody;

    [Header("Court Bounce Settings")]
    [SerializeField] private LayerMask courtLayer;
    [SerializeField] private float courtBounceFactor;
    [SerializeField] private float courtFrictionFactor;

    [Header("Reducing Bounce Settings")]
    [SerializeField] private LayerMask reduceLayer;
    [SerializeField] private float reduceBounceFactor;
    [SerializeField] private float reduceFrictionFactor;

    [Header("Absorbing Bounce Settings")]
    [SerializeField] private LayerMask absorbLayer;
    [SerializeField] private float absorbBounceFactor;
    [SerializeField] private float absorbFrictionFactor;

    private Vector3 velocityInLastFrame;
    private Vector3 impulseForce;
    private Vector3 bounceForce;
    private float gravityMagnitude;
    private float spinFactor = 1f;

    private Shot currentShot;
    public Shot CurrentShot => currentShot;

    private int numberCourtBounces; // the number of consecutive bounces without being hit by player
    public int NumberCourtBounces => numberCourtBounces;

    private bool canBeHit;
    public bool CanBeHit {
        get => canBeHit;
        set => canBeHit = value;
    }

    private void Start() {
        ballBody.useGravity = false;
    }

    private void FixedUpdate() {
        ApplyGravity(ballBody);

        velocityInLastFrame = ballBody.velocity;
    }
    
    private void OnCollisionEnter(Collision other) {
        // with court surface
        if ((1 << other.gameObject.layer & courtLayer) != 0) {
            // apply friction
            ballBody.velocity *= courtFrictionFactor;
            // reduce spin
            spinFactor *= courtFrictionFactor;
            
            Vector3 collisionNormalAverage = other.contacts.Aggregate(Vector3.zero, (current, t) => current + t.normal) / other.contacts.Length;

            // calculate and add bounce & spin force
            Vector3 projectedNormalVelocity = Vector3.Project(velocityInLastFrame, collisionNormalAverage);
            Vector3 tangentVelocity = velocityInLastFrame - projectedNormalVelocity;
            Vector3 bouncingForce = tangentVelocity - projectedNormalVelocity * courtBounceFactor;
            Vector3 bounceAndSpinForce = bouncingForce + bounceForce * spinFactor;

            ballBody.AddForce(bounceAndSpinForce, ForceMode.Impulse);

            // increase ground bounces counter
            ++numberCourtBounces;

            if (numberCourtBounces > 1) {
                canBeHit = false;
            }

            // Debug.Log($"Bounce Force: {bounceForce}");
            // Debug.Log($"Spin Force: {spinForce}");
            // Debug.Log($"Bounce & Spin Force: {bounceAndSpinForce}");
        }
        // with surfaces that reduce velocity and spin
        else if ((1 << other.gameObject.layer & reduceLayer) != 0) {
            // apply friction
            ballBody.velocity *= reduceFrictionFactor;
            // reduce spin
            spinFactor *= reduceBounceFactor;
            
            Vector3 collisionNormalAverage = other.contacts.Aggregate(Vector3.zero, (current, t) => current + t.normal) / other.contacts.Length;

            // calculate and add bounce & spin force
            Vector3 projectedNormalVelocity = Vector3.Project(velocityInLastFrame, collisionNormalAverage);
            Vector3 tangentVelocity = velocityInLastFrame - projectedNormalVelocity;
            Vector3 bouncingForce = tangentVelocity - projectedNormalVelocity * reduceBounceFactor;
            Vector3 bounceAndSpinForce = bouncingForce + bounceForce * spinFactor;

            ballBody.AddForce(bounceAndSpinForce, ForceMode.Impulse);
            
            canBeHit = false;
        }
        // with surfaces that absorb most of velocity & spin
        else if ((1 << other.gameObject.layer & absorbLayer) != 0) {
            // apply friction
            ballBody.velocity *= absorbFrictionFactor;
            // reduce spin
            spinFactor *= absorbBounceFactor;
            
            Vector3 collisionNormalAverage = other.contacts.Aggregate(Vector3.zero, (current, t) => current + t.normal) / other.contacts.Length;

            // calculate and add bounce & spin force
            Vector3 projectedNormalVelocity = Vector3.Project(velocityInLastFrame, collisionNormalAverage);
            Vector3 tangentVelocity = velocityInLastFrame - projectedNormalVelocity;
            Vector3 bouncingForce = (tangentVelocity - projectedNormalVelocity) * absorbBounceFactor;
            Vector3 bounceAndSpinForce = bouncingForce + bounceForce * spinFactor;

            ballBody.AddForce(bounceAndSpinForce, ForceMode.Impulse);
            
            canBeHit = false;
        }
    }
    
    public void ResetProperties() {
        impulseForce = Vector3.zero;
        gravityMagnitude = 0f;
        bounceForce = Vector3.zero;

        spinFactor = 1f;
        velocityInLastFrame = Vector3.zero;
        ballBody.velocity = Vector3.zero;

        currentShot = null;
        numberCourtBounces = 0;
        canBeHit = true;
    }

    public void Hit(Shot shot, Vector3 startPosition, Vector3 targetPosition) {
        if (!canBeHit) return;
        if (shot == null) return;
        
        ResetProperties();
        
        impulseForce = shot.GetShotImpulse(startPosition, targetPosition);
        gravityMagnitude = shot.GravityMagnitude;
        bounceForce = shot.BounceForce;

        ballBody.AddForce(impulseForce, ForceMode.Impulse);

        currentShot = shot;
    }

    public void Hit(FixedShot fixedShot) {
        Hit(fixedShot, fixedShot.StartPosition, fixedShot.TargetPosition);
    }

    private void ApplyGravity(Rigidbody body) {
        body.AddForce(gravityMagnitude * Vector3.down, ForceMode.Force);
    }
}