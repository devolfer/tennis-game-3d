using System;
using UnityEngine;

public class Hitting : MonoBehaviour {
    [SerializeField] private InputEventRelay playerInputEventRelay;
    [SerializeField] private Transform aimPointTransform;
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private bool autoHit;
    [SerializeField] private bool useVolleys;

    // all possible shots that can be made
    [SerializeField] private Shot flatShotDefault;
    [SerializeField] private Shot topSpinShotDefault;
    [SerializeField] private Shot sliceShotDefault;
    [SerializeField] private Shot dropShotDefault;
    [SerializeField] private Shot lobShotDefault;
    [SerializeField] private Shot volleyShotDefault;

    private TennisBall tennisBallInHitZone;
    private ShotType toggledShotType = ShotType.Flat;

    public Action<TennisBall> onHit;

    private void OnEnable() {
        if (playerInputEventRelay) playerInputEventRelay.OnShotFlatEvent += FlatShot;
        if (playerInputEventRelay) playerInputEventRelay.OnShotTopSpinEvent += TopSpinShot;
        if (playerInputEventRelay) playerInputEventRelay.OnShotSliceEvent += SliceShot;
        if (playerInputEventRelay) playerInputEventRelay.OnShotDropOrLobEvent += DropOrLobShot;
    }

    private void OnDisable() {
        if (playerInputEventRelay) playerInputEventRelay.OnShotFlatEvent -= FlatShot;
        if (playerInputEventRelay) playerInputEventRelay.OnShotTopSpinEvent -= TopSpinShot;
        if (playerInputEventRelay) playerInputEventRelay.OnShotSliceEvent -= SliceShot;
        if (playerInputEventRelay) playerInputEventRelay.OnShotDropOrLobEvent -= DropOrLobShot;
    }

    private void FlatShot() {
        toggledShotType = ShotType.Flat;

        if (autoHit) return;
        if (!tennisBallInHitZone) return;

        Hit(tennisBallInHitZone, toggledShotType);
    }
    
    private void TopSpinShot() {
        toggledShotType = ShotType.TopSpin;

        if (autoHit) return;
        if (!tennisBallInHitZone) return;

        Hit(tennisBallInHitZone, toggledShotType);
    }
    
    private void SliceShot() {
        toggledShotType = ShotType.Slice;

        if (autoHit) return;
        if (!tennisBallInHitZone) return;

        Hit(tennisBallInHitZone, toggledShotType);
    }
    
    private void DropOrLobShot() {
        toggledShotType = ShotType.DropOrLob;

        if (autoHit) return;
        if (!tennisBallInHitZone) return;

        Hit(tennisBallInHitZone, toggledShotType);
    }

    private void OnTriggerEnter(Collider other) {
        if ((1 << other.gameObject.layer & ballLayerMask) == 0) return;
        if (!other.TryGetComponent(out tennisBallInHitZone)) return;

        if (!autoHit) return;

        Hit(tennisBallInHitZone, toggledShotType);
    }

    private void OnTriggerExit(Collider other) {
        if ((1 << other.gameObject.layer & ballLayerMask) == 0) return;

        tennisBallInHitZone = null;
    }

    private void Hit(TennisBall tennisBall, ShotType shotType) {
        if (!tennisBall) return;

        Shot currentShot = null;
        Vector3 startPosition = tennisBall.transform.position;
        Vector3 targetPosition = aimPointTransform.position;
        bool validVolleySituation = ValidVolleySituation(tennisBall, startPosition, 2f);

        switch (shotType) {
            case ShotType.Flat:
                currentShot = validVolleySituation ? volleyShotDefault : flatShotDefault;
                break;
            case ShotType.TopSpin:
                currentShot = validVolleySituation ? volleyShotDefault : topSpinShotDefault;
                break;
            case ShotType.Slice:
                currentShot = validVolleySituation ? volleyShotDefault : sliceShotDefault;
                break;
            case ShotType.DropOrLob:
                bool validDropSituation = ValidDropSituation(targetPosition, 23.78f);
                if (validDropSituation) {
                    currentShot = validVolleySituation ? volleyShotDefault : dropShotDefault;
                } else {
                    currentShot = validVolleySituation ? volleyShotDefault : lobShotDefault;
                }

                // currentShot = validVolleySituation ? volleyShotDefault : targetPosition.z < 23.78f ? dropShotDefault : lobShotDefault;
                break;
        }

        tennisBall.Hit(currentShot, startPosition, targetPosition);

        onHit?.Invoke(tennisBall);
    }

    private bool ValidDropSituation(Vector3 targetPosition, float maximumTargetZPosition) {
        return targetPosition.z < maximumTargetZPosition;
    }

    private bool ValidVolleySituation(TennisBall tennisBall, Vector3 startPosition, float minimumBallHeight) {
        return tennisBall.NumberCourtBounces == 0 && useVolleys && startPosition.y > minimumBallHeight;
    }
}