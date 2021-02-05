using System.Collections;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;

    private void OnEnable() {
        if (applicationEventRelay) applicationEventRelay.OnRequestedStartingCoroutine += DoRoutine;
        if (applicationEventRelay) applicationEventRelay.OnRequestedStoppingCoroutine += StopRoutine;
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnRequestedStartingCoroutine -= DoRoutine;
        if (applicationEventRelay) applicationEventRelay.OnRequestedStoppingCoroutine -= StopRoutine;
    }

    private void DoRoutine(IEnumerator routine) {
        StartCoroutine(routine);
    }
    
    private void StopRoutine(IEnumerator routine) {
        StopCoroutine(routine);
    }
}