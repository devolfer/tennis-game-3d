using System;
using TMPro;
using UnityEngine;

public class ModalityWindow : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private MenuButton confirmButton;
    [SerializeField] private MenuButton cancelButton;

    private AnimationSequence showAnimation;
    private AnimationSequence hideAnimation;

    private Action confirmingAction;
    private Action cancellingAction;

    private void OnEnable() {
        applicationEventRelay.OnModalityWindow += Show;
    }

    private void OnDisable() {
        applicationEventRelay.OnModalityWindow -= Show;
    }

    private void Start() {
        InitAnimations();
        
        confirmButton.onPress.AddListener(OnConfirm);
        cancelButton.onPress.AddListener(OnCancel);
    }
    
    private void Setup(ModalityWindowSettings settings) {
        if (settings.confirmAction != null) confirmingAction = settings.confirmAction;
        if (settings.cancelAction != null) cancellingAction = settings.cancelAction;

        if (settings.displayText != null) headerText.text = settings.displayText;
        if (settings.infoText != null) descriptionText.text = settings.infoText;
        if (settings.confirmText != null) confirmButton.DescriptionText.text = settings.confirmText;
        if (settings.cancelText != null) cancelButton.DescriptionText.text = settings.cancelText;
        if (settings.confirmIcon != null) confirmButton.ButtonImage.sprite = settings.confirmIcon;
        if (settings.cancelIcon != null) cancelButton.ButtonImage.sprite = settings.cancelIcon;
    }

    private void OnDestroy() {
        confirmButton.onPress.RemoveListener(OnConfirm);
        cancelButton.onPress.RemoveListener(OnCancel);

        showAnimation?.Cleanup();
        hideAnimation?.Cleanup();
    }
    
    private void InitAnimations() {
        AnimationOperation scaleContentOperation = new AnimationOperation(content, UIAnimationType.Scale, EaseType.SmoothStepSmoother, 0, 0.5f) {
            scaleSettings = new AnimationOperation.ScaleSettings {
                startScale = Vector3.zero,
                targetScale = Vector3.one
            }
        };
        AnimationOperation fadeContentOperation = new AnimationOperation(content, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
            fadeSettings = new AnimationOperation.FadeSettings {
                startAlpha = 0,
                targetAlpha = 1
            }
        };
        AnimationOperation fadeBackgroundOperation = new AnimationOperation(fadeContentOperation) {targetObject = background};
            
        showAnimation = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        showAnimation.AddOperation(scaleContentOperation, fadeContentOperation, fadeBackgroundOperation, new AnimationOperation {targetObject = panel, type = UIAnimationType.Activate, activate = true});
        showAnimation.OnFinished(() => confirmButton.Select());
        
        hideAnimation = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        hideAnimation.AddOperation(scaleContentOperation.Reversed(), fadeContentOperation.Reversed(), fadeBackgroundOperation.Reversed(), new AnimationOperation{targetObject = panel, type = UIAnimationType.Activate, activate = false, delay = 0.5f});
    }

    private void Show(bool show, bool animated, ModalityWindowSettings settings = null) {
        if (settings != null) Setup(settings);
        
        if (animated) {
            if (show) {
                showAnimation.Play(true);
            } else {
                hideAnimation.Play(true);
            }
        } else {
            panel.SetActive(show);
        }
    }

    private void OnConfirm(MenuButton button) {
        confirmingAction?.Invoke();
    }

    private void OnCancel(MenuButton button) {
        cancellingAction?.Invoke();
    }
    
    public class ModalityWindowSettings {
        public Action confirmAction;
        public Action cancelAction;

        public string displayText;
        public string infoText;
        public string confirmText;
        public string cancelText;
        public Sprite confirmIcon;
        public Sprite cancelIcon;
    }
}