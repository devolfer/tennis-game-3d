using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShotSessionMultiplier : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private ShotSessionEventRelay shotSessionEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private Image multiplierFill;

    [SerializeField] private Color multiplierX1Colour;
    [SerializeField] private Color multiplierX2Colour;
    [SerializeField] private Color multiplierX4Colour;
    [SerializeField] private Color multiplierX8Colour;

    private int currentCombo;
    private int currentMultiplier;
    private AnimationSequence multiplierAnimation;
    private AnimationOperation scaleOperation;
    private AnimationOperation reversedScaleOperation;

    private void Start() {
        SetMultiplier(0, false);
        SetFill(0, false);
    }

    private void OnEnable() {
        // shotSessionEventRelay.OnBegin += Show;
        shotSessionEventRelay.OnEnd += Hide;

        shotSessionEventRelay.OnComboValueChanged += ChangeMultiplier;
    }

    private void OnDisable() {
        // shotSessionEventRelay.OnBegin -= Show;
        shotSessionEventRelay.OnEnd -= Hide;

        shotSessionEventRelay.OnComboValueChanged -= ChangeMultiplier;
    }

    private void OnDestroy() {
        multiplierAnimation?.Cleanup();
    }

    private void Show() {
        ShowHudElement(true, true);
    }

    private void Hide() {
        if (currentMultiplier > 1) ShowHudElement(false, true);
        
        multiplierAnimation?.Stop(applicationEventRelay.RequestStoppingCoroutine);
        multiplierAnimation = null;
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
                    startMin = new Vector2(-0.12f, 0.84f),
                    startMax = new Vector2(0.08f, 1f),
                    targetMin = new Vector2(0f, 0.84f),
                    targetMax = new Vector2(0.2f, 1f)
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

    private void ChangeMultiplier(int combo) {
        SetFill(combo / 16f, true);
        ChangeFillColour(combo, true);
        SetMultiplier(combo, true);
    }

    private void SetMultiplier(int combo, bool animated) {
        int multiplier = GetScoreMultiplier(combo);

        if (animated) {
            if (multiplierAnimation == null && multiplier == 2) {
                scaleOperation = new AnimationOperation(multiplierText.gameObject, UIAnimationType.Scale, EaseType.SmoothStepSmoother, 0, 0.5f) {
                    scaleSettings = new AnimationOperation.ScaleSettings {
                        startScale = Vector3.one,
                        targetScale = Vector3.one * 1.25f
                    }
                };
                reversedScaleOperation = scaleOperation.Reversed();
                reversedScaleOperation.delay = 0.5f;

                multiplierAnimation = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
                multiplierAnimation.AddOperation(scaleOperation);
                multiplierAnimation.AddOperation(reversedScaleOperation);
                multiplierAnimation.Loop();

                multiplierAnimation.Play();

                Show();
            } 
            
            if (multiplier < currentMultiplier) {
                Hide();
            }

            multiplierText.text = $"x {multiplier}";
            currentCombo = combo;
            currentMultiplier = multiplier;
        } else {
            multiplierText.text = $"x {multiplier}";
            currentCombo = combo;
            currentMultiplier = multiplier;

            ShowHudElement(multiplier > 1, false);
        }
    }

    private int GetScoreMultiplier(int combo) {
        return combo switch {
            int n when n >= 4 && n < 8 => 2,
            int n when n >= 8 && n < 16 => 4,
            int n when n >= 16 => 8,
            _ => 1
        };
    }

    private void SetFill(float value, bool animated) {
        if (animated) {
            float startValue = multiplierFill.fillAmount;
            float targetValue = value;

            this.DoRoutine(0.5f, null,
                t => {
                    multiplierFill.fillAmount = Lerp.Value(startValue, targetValue, t, Easing.SmoothStep.Smoother);
                },
                () => {
                    multiplierFill.fillAmount = targetValue;
                });
        } else {
            multiplierFill.fillAmount = value;
        }
    }

    private void ChangeFillColour(int combo, bool animated) {
        int multiplier = GetScoreMultiplier(combo);
        
        Color targetColour = multiplier switch {
            int n when n >= 4 && n < 8 => multiplierX2Colour,
            int n when n >= 8 && n < 16 => multiplierX4Colour,
            int n when n >= 16 => multiplierX8Colour,
            _ => multiplierX1Colour
        };

        if (animated) {
            Color startColour = multiplierFill.color;

            this.DoRoutine(0.5f, null,
                t => {
                    multiplierFill.color = Lerp.Value(startColour, targetColour, t, Easing.SmoothStep.Smoother);
                    multiplierText.color = Lerp.Value(startColour, targetColour, t, Easing.SmoothStep.Smoother);
                },
                () => {
                    multiplierFill.color = targetColour;
                });
        } else {
            multiplierFill.color = targetColour;
            multiplierText.color = targetColour;
        }
    }
}