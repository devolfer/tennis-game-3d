using UnityEngine;

public class UIAnimation : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;

    [SerializeField] private bool useUnscaledTime;
    [SerializeField] private bool showOnEnable;
    [SerializeField] private bool hideOnDisable;
    [SerializeField] private AnimationSequence showAnimation;
    [SerializeField] private AnimationSequence hideAnimation;

    private AnimationSequence internalShowAnimation;
    private AnimationSequence internalHideAnimation;

    private void Awake() {
        internalShowAnimation ??= new AnimationSequence(showAnimation, applicationEventRelay.RequestStartingCoroutine);
        internalHideAnimation ??= new AnimationSequence(hideAnimation, applicationEventRelay.RequestStartingCoroutine);
    }

    private void OnEnable() {
        if (showOnEnable) Show(true);
    }

    private void OnDisable() {
        if (hideOnDisable) Show(false);
    }

    public void Show(bool show) {
        if (show) {
            internalShowAnimation ??= new AnimationSequence(showAnimation, applicationEventRelay.RequestStartingCoroutine);
            internalShowAnimation.Play(useUnscaledTime);
        } else {
            internalHideAnimation ??= new AnimationSequence(hideAnimation, applicationEventRelay.RequestStartingCoroutine);
            internalHideAnimation.Play(useUnscaledTime);
        }
    }
}