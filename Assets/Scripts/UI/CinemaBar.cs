using UnityEngine;

public class CinemaBar : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject blurBackground;
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;

    private UIAnimation panelAnimation;
    private UIAnimation blurBackgroundAnimation;
    private UIAnimation topAnimation;
    private UIAnimation bottomAnimation;

    private void Awake() {
        panel.TryGetComponent(out panelAnimation);
        blurBackground.TryGetComponent(out blurBackgroundAnimation);
        top.TryGetComponent(out topAnimation);
        bottom.TryGetComponent(out bottomAnimation);
    }

    private void OnEnable() {
        if (applicationEventRelay) applicationEventRelay.OnCinemaBar += Show;
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnCinemaBar -= Show;
    }

    private void Show(bool show, bool useBlurBackground) {
        if (panelAnimation) panelAnimation.Show(show);
        if (blurBackgroundAnimation && useBlurBackground) blurBackgroundAnimation.Show(show);
        if (topAnimation) topAnimation.Show(show);
        if (bottomAnimation) bottomAnimation.Show(show);
    }
}