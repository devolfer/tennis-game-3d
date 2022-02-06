using System;
using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectedShotType : MonoBehaviour {
    [SerializeField] private ShotSessionEventRelay shotSessionEventRelay;
    [SerializeField] private InputEventRelay inputEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private Image shotSelectionBackgroundImage;
    [SerializeField] private TextMeshProUGUI shotSelectionText;
    
    [SerializeField] private Color flatShotColour;
    [SerializeField] private Color topSpinShotColour;
    [SerializeField] private Color sliceShotColour;
    [SerializeField] private Color dropOrLobShotColour;

    private ShotType selectedShotType;
    
    private Action flatShotSelectedAction;
    private Action topSpinShotSelectedAction;
    private Action sliceShotSelectedAction;
    private Action dropOrLobShotSelectedAction;

    private void Awake() {
        flatShotSelectedAction = () => UpdateShotSelection(ShotType.Flat, true);
        topSpinShotSelectedAction = () => UpdateShotSelection(ShotType.TopSpin, true);
        sliceShotSelectedAction = () => UpdateShotSelection(ShotType.Slice, true);
        dropOrLobShotSelectedAction = () => UpdateShotSelection(ShotType.DropOrLob, true);
    }

    private void Start() {
        SetSelectedShotType(ShotType.Flat, false);
        ShowHudElement(false, false);
    }

    private void OnEnable()
    {
        inputEventRelay.OnShotFlatEvent += flatShotSelectedAction;
        inputEventRelay.OnShotTopSpinEvent += topSpinShotSelectedAction;
        inputEventRelay.OnShotSliceEvent += sliceShotSelectedAction;
        inputEventRelay.OnShotDropOrLobEvent += dropOrLobShotSelectedAction;
        
        shotSessionEventRelay.OnBegin += Show;
        shotSessionEventRelay.OnEnd += Hide;
    }

    private void OnDisable() {
        inputEventRelay.OnShotFlatEvent -= flatShotSelectedAction;
        inputEventRelay.OnShotTopSpinEvent -= topSpinShotSelectedAction;
        inputEventRelay.OnShotSliceEvent -= sliceShotSelectedAction;
        inputEventRelay.OnShotDropOrLobEvent -= dropOrLobShotSelectedAction;

        shotSessionEventRelay.OnBegin -= Show;
        shotSessionEventRelay.OnEnd -= Hide;
    }

    private void Show() {
        ShowHudElement(true, true);
    }

    private void Hide() {
        ShowHudElement(false, true);
    }

    private void ShowHudElement(bool show, bool animated) {
        if (animated) {
            AnimationOperation fadeOperation = new AnimationOperation(panel, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
                fadeSettings = new AnimationOperation.FadeSettings {
                    startAlpha = 0,
                    targetAlpha = 1
                }
            };
            AnimationOperation anchorPositionOperation = new AnimationOperation(panel, UIAnimationType.AnchoredPosition, EaseType.BackInOut, 0, 0.5f) {
                anchoredPositionSettings = new AnimationOperation.AnchoredPositionSettings {
                    startMin = new Vector2(1f, 0f),
                    startMax = new Vector2(1.1f, 0.1f),
                    targetMin = new Vector2(0.9f, 0f),
                    targetMax = new Vector2(1f, 0.1f)
                }
            };
            AnimationSequence showAnimation = new AnimationSequence(e => StartCoroutine(e));
            showAnimation.AddOperation(anchorPositionOperation);
            showAnimation.AddOperation(fadeOperation);
            showAnimation.AddOperation(new AnimationOperation {targetObject = panel, type = UIAnimationType.Activate, activate = true});

            if (show) {
                showAnimation.Play();
            } else {
                AnimationSequence reversedShowAnimation = new AnimationSequence(showAnimation.Reversed(), e => StartCoroutine(e));
                reversedShowAnimation.AddOperation(new AnimationOperation {targetObject = panel, type = UIAnimationType.Activate, activate = false, delay = 0.5f});
                reversedShowAnimation.Play();
            }
        } else {
            panel.SetActive(show);
        }
    }

    private void SetSelectedShotType(ShotType shotType, bool animated) {
        if (shotType == selectedShotType) return;
        
        if (animated) {
            selectedShotType = shotType;
            
            this.DoRoutine(0.25f, endAction: () => {
                shotSelectionText.text = $"{shotType.ToString().ToSpaceBeforeUpperCase()}";
            });
        } else {
            selectedShotType = shotType;
            shotSelectionText.text = $"{shotType.ToString().ToSpaceBeforeUpperCase()}";
        }
    }
    
    private void SetShotSelectionBackgroundImageColour(ShotType shotType, bool animated) {
        Color targetColour = shotType switch {
            ShotType.Flat => flatShotColour,
            ShotType.TopSpin => topSpinShotColour,
            ShotType.Slice => sliceShotColour,
            ShotType.DropOrLob => dropOrLobShotColour,
            _ => Color.white
        };

        if (animated) {
            AnimationOperation colourOperation = new AnimationOperation(shotSelectionBackgroundImage.gameObject, UIAnimationType.Colour, EaseType.SmoothStepSmoother, 0, 0.5f) {
                colourSettings = new AnimationOperation.ColourSettings {
                    startColour = shotSelectionBackgroundImage.color,
                    targetColour = targetColour
                }
            };
            AnimationSequence colourAnimation = new AnimationSequence(e => StartCoroutine(e));
            colourAnimation.AddOperation(colourOperation);
            
            colourAnimation.Play();
        } else {
            shotSelectionBackgroundImage.color = targetColour;
        }
    }
    
    private void UpdateShotSelection(ShotType shotType, bool animated) {
        SetSelectedShotType(shotType, animated);
        SetShotSelectionBackgroundImageColour(shotType, animated);
    }
}