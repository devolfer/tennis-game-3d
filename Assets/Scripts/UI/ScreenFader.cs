using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private GameObject screenFaderPanel;
    [SerializeField] private Image faderMaskImage;
    [SerializeField] private Image inverseMaskImage;
    [SerializeField] private RectTransform rootCanvasRectTransform;

    private void OnEnable() {
        if (applicationEventRelay) applicationEventRelay.OnScreenFade += Show;
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnScreenFade -= Show;
    }

    private void Show(float duration, bool show) {
        inverseMaskImage.rectTransform.sizeDelta = rootCanvasRectTransform.rect.size;
        
        if (show) {
            AnimationSequence sequence = new AnimationSequence(e => {
                if (applicationEventRelay) applicationEventRelay.RequestStartingCoroutine(e);
            });

            AnimationOperation activateOperation = new AnimationOperation(screenFaderPanel, UIAnimationType.Activate, EaseType.None, 0, 0) {
                activate = true
            };

            AnimationOperation showOperation = new AnimationOperation(faderMaskImage.gameObject, UIAnimationType.AnchoredPosition, EaseType.QuarticIn, 0, duration) {
                anchoredPositionSettings = new AnimationOperation.AnchoredPositionSettings {
                    startMin = new Vector2(-1, -1),
                    startMax = new Vector2(2, 2),
                    targetMin = new Vector2(0.5f, 0.5f),
                    targetMax = new Vector2(0.5f, 0.5f)
                }
            };

            sequence.AddOperation(activateOperation);
            sequence.AddOperation(showOperation);

            sequence.Play();
        } else {
            AnimationSequence sequence = new AnimationSequence(e => {
                if (applicationEventRelay) applicationEventRelay.RequestStartingCoroutine(e);
            });

            AnimationOperation showOperation = new AnimationOperation(faderMaskImage.gameObject, UIAnimationType.AnchoredPosition, EaseType.QuarticIn, 0, duration) {
                anchoredPositionSettings = new AnimationOperation.AnchoredPositionSettings {
                    targetMin = new Vector2(-1, -1),
                    targetMax = new Vector2(2, 2),
                    startMin = new Vector2(0.5f, 0.5f),
                    startMax = new Vector2(0.5f, 0.5f)
                }
            };

            AnimationOperation activateOperation = new AnimationOperation(screenFaderPanel, UIAnimationType.Activate, EaseType.None, duration, 0) {
                activate = false
            };

            sequence.AddOperation(showOperation);
            sequence.AddOperation(activateOperation);


            sequence.Play();
        }
    }
}