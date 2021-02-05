using System;
using System.Collections;
using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : Button {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private Canvas buttonCanvas;
    [SerializeField] private Image selectImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private AnimationSequence showAnimation;
    [SerializeField] private AnimationSequence hideAnimation;
    [SerializeField] private AnimationSequence selectAnimation;
    [SerializeField] private AnimationSequence deselectAnimation;
    [SerializeField] private AnimationSequence pressAnimation;
    [SerializeField] private AnimationSequence enableAnimation;
    [SerializeField] private AnimationSequence disableAnimation;

    [SerializeField] private Image[] colourTintTargetGraphics;
    [SerializeField] private bool selectOnShow;
    [SerializeField] private float selectDelay;

    [Serializable] public class MenuButtonEvent : UnityEvent<MenuButton>{}
    public MenuButtonEvent onPress = new MenuButtonEvent();
    public MenuButtonEvent onSelect = new MenuButtonEvent();
    public MenuButtonEvent onDeselect = new MenuButtonEvent();
    public MenuButtonEvent onEnable = new MenuButtonEvent();
    public MenuButtonEvent onDisable = new MenuButtonEvent();

    public Image SelectImage {
        get => selectImage;
        set => selectImage = value;
    }

    public Image BackgroundImage {
        get => backgroundImage;
        set => backgroundImage = value;
    }

    public Image ButtonImage {
        get => buttonImage;
        set => buttonImage = value;
    }

    public TextMeshProUGUI HeaderText {
        get => headerText;
        set => headerText = value;
    }

    public TextMeshProUGUI DescriptionText {
        get => descriptionText;
        set => descriptionText = value;
    }

    public bool SelectOnShow {
        get => selectOnShow;
        set {
            selectOnShow = value;
            
            if (value) {
                showAnim?.OnFinished(SelectWithDelay);
            } else {
                showAnim?.RemoveOnFinishedAction(SelectWithDelay);
            }
        }
    }

    public float SelectDelay {
        get => selectDelay;
        set => selectDelay = value;
    }

    private AnimationSequence showAnim;
    private AnimationSequence hideAnim;
    private AnimationSequence selectAnim;
    private AnimationSequence deselectAnim;
    private AnimationSequence pressAnim;
    private AnimationSequence enableAnim;
    private AnimationSequence disableAnim;

    private SelectionState previousState = SelectionState.Normal;
    private SelectionState currentState = SelectionState.Normal;

    private bool canBePressed = true;
    
    public bool CanBePressed {
        get => canBePressed;
        set => canBePressed = value;
    }
    
    private int originalSortingOrder;

    protected override void Awake() {
        base.Awake();

        if (showAnim == null) {
            showAnim = new AnimationSequence(showAnimation, RelayRoutine);
            if (selectOnShow) showAnim.OnFinished(SelectWithDelay);
        }
        hideAnim ??= new AnimationSequence(hideAnimation, RelayRoutine);
        selectAnim ??= new AnimationSequence(selectAnimation, RelayRoutine);
        deselectAnim ??= new AnimationSequence(deselectAnimation, RelayRoutine);
        if (pressAnim == null) {
            pressAnim = new AnimationSequence(pressAnimation, RelayRoutine);
            pressAnim.OnFinished(EnablePress);
        }
        enableAnim ??= new AnimationSequence(enableAnimation, RelayRoutine);
        disableAnim ??= new AnimationSequence(disableAnimation, RelayRoutine);

        originalSortingOrder = buttonCanvas.sortingOrder;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        
        showAnim.Cleanup();
        hideAnim.Cleanup();
        selectAnim.Cleanup();
        deselectAnim.Cleanup();
        pressAnim.Cleanup();
        enableAnim.Cleanup();
        disableAnim.Cleanup();
    }

    private void SelectWithDelay() {
        IEnumerator e = Utility.SequenceRoutine(new Func<float>[] {
            () => selectDelay,
            () => {
                Select();
                return 0f;
            }
        }, true);
        
        RelayRoutine(e);
    }

    private void EnablePress() {
        const float safetyDelay = 0.2f;
        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => safetyDelay,
            () => {
                canBePressed = true;

                return 0f;
            }
        }, true));
    }
    
    private void RelayRoutine(IEnumerator e) {
        applicationEventRelay.RequestStartingCoroutine(e);
    }

    public void Show(bool show, bool animated) {
        if (animated) {
            if (show) {
                if (showAnim == null) {
                    showAnim = new AnimationSequence(showAnimation, RelayRoutine);
                    if (selectOnShow) showAnim.OnFinished(SelectWithDelay);
                }
                // Debug.Log($"{gameObject.name}: {showAnim.Duration}");
                showAnim.Play(true);
            } else {
                hideAnim ??= new AnimationSequence(hideAnimation, RelayRoutine);
                hideAnim.Play(true);
            }
        } else {
            gameObject.SetActive(show);
            if (show & selectOnShow) {
                Select();
            }
        }
        
        canBePressed = show;
    }

    protected override void DoStateTransition(SelectionState state, bool instant) {
        base.DoStateTransition(state, instant);

        previousState = currentState;
        currentState = state;

        Color tintColour;

        switch (state) {
            case SelectionState.Normal:
                tintColour = colors.normalColor;
                
                buttonCanvas.sortingOrder = originalSortingOrder;

                if (previousState != SelectionState.Normal && previousState != SelectionState.Disabled) {
                    deselectAnim.Play(true);
                    onDeselect?.Invoke(this);
                }

                if (previousState == SelectionState.Disabled) {
                    enableAnim.Play(true);
                    onEnable?.Invoke(this);
                }
                
                // Debug.Log($"{gameObject.name} Normal");
                break;
            case SelectionState.Highlighted:
                tintColour = colors.highlightedColor;
                Select();
                
                // Debug.Log($"{gameObject.name} Highlighted");
                break;
            case SelectionState.Pressed:
                if (!canBePressed) return;
                
                canBePressed = false;
                tintColour = colors.pressedColor;

                pressAnim.Play(true);

                onPress?.Invoke(this);

                // Debug.Log($"{gameObject.name} Pressed");
                break;
            case SelectionState.Selected:
                tintColour = colors.selectedColor;

                buttonCanvas.sortingOrder = originalSortingOrder + 10;
                
                if (previousState != SelectionState.Pressed && previousState != SelectionState.Selected) selectAnim.Play(true);
                
                onSelect?.Invoke(this);
                
                // Debug.Log($"{gameObject.name} Selected");
                break;
            case SelectionState.Disabled:
                tintColour = colors.disabledColor;
                
                disableAnim.Play(true);
                
                onDisable?.Invoke(this);
                
                // Debug.Log($"{gameObject.name} Disabled");
                break;
            default:
                tintColour = Color.black;
                break;
        }

        switch (transition) {
            case Transition.None:
                break;
            case Transition.ColorTint:
                if (colourTintTargetGraphics != null && colourTintTargetGraphics.Length > 0) {
                    foreach (Image graphic in colourTintTargetGraphics) {
                        StartColorTween(graphic, tintColour, instant);
                    }
                }
                break;
            case Transition.SpriteSwap:
                break;
            case Transition.Animation:
                break;
        }
    }

    public void Deselect() {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject != gameObject) return;

        DoStateTransition(SelectionState.Normal, false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public override void OnMove(AxisEventData eventData) {
        base.OnMove(eventData);
        
        if (currentState == SelectionState.Highlighted) DoStateTransition(SelectionState.Normal, false);
    }

    private void StartColorTween(Graphic graphic, Color targetColor, bool instant) {
        if (graphic == null) return;

        graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
    }
}