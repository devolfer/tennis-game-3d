using System;
using CustomUtilities;
using TMPro;
using UnityEngine;

public class ShotSessionScore : MonoBehaviour {
    [SerializeField] private ShotSessionEventRelay shotSessionEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreChange;
    [SerializeField] private TextMeshProUGUI scoreChangeText;

    private int currentScore;

    private void Start() {
        SetScore(0, false);
        ShowScoreChange(false, false);
        ShowHudElement(false, false);
    }

    private void OnEnable() {
        shotSessionEventRelay.OnBegin += Show;
        shotSessionEventRelay.OnEnd += Hide;
        
        shotSessionEventRelay.OnScoreValueChanged += ChangeScore;
    }

    private void OnDisable() {
        shotSessionEventRelay.OnBegin -= Show;
        shotSessionEventRelay.OnEnd -= Hide;
        
        shotSessionEventRelay.OnScoreValueChanged -= ChangeScore;
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
                    startMin = new Vector2(1f, 0.8f),
                    startMax = new Vector2(1.1f, 1f),
                    targetMin = new Vector2(0.9f, 0.8f),
                    targetMax = new Vector2(1f, 1f)
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

    private void ChangeScore(int newScore) {
        int difference = newScore - currentScore;

        if (difference == 0) return;

        this.DoSequence(new Func<float>[] {
            () => {
                SetScoreChange(difference, true);
                ShowScoreChange(true, true);

                return 0.5f;
            },
            () => {
                SetScore(newScore, true);

                return 0.5f;
            },
            () => {
                ShowScoreChange(false, true);

                return 0f;
            }
        });
    }

    private void SetScore(int newScore, bool animated) {
        if (animated) {
            this.DoRoutine(0.5f, null, t => {
                float value = Lerp.Value(currentScore, newScore, t, Easing.SmoothStep.Smoother);
                scoreText.text = $"{(int) value}";
            }, () => {
                scoreText.text = $"{newScore}";
                currentScore = newScore;
            });
        } else {
            scoreText.text = $"{newScore}";
            currentScore = newScore;
        }
    }

    private void SetScoreChange(int difference, bool animated) {
        if (animated) {
            // TODO
            scoreChangeText.text = difference >= 0 ? $"+{difference}" : $"-{difference}";
        } else {
            scoreChangeText.text = difference >= 0 ? $"+{difference}" : $"-{difference}";
        }
    }

    private void ShowScoreChange(bool show, bool animated) {
        if (animated) {
            AnimationOperation showFadeOperation = new AnimationOperation(scoreChange, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
                fadeSettings = new AnimationOperation.FadeSettings {
                    startAlpha = 0,
                    targetAlpha = 1
                }
            };
            AnimationSequence fadeAnimation = new AnimationSequence(e => StartCoroutine(e));
            fadeAnimation.AddOperation(showFadeOperation);
            fadeAnimation.AddOperation(new AnimationOperation {targetObject = scoreChange, type = UIAnimationType.Activate, activate = true});

            if (show) {
                fadeAnimation.Play();
            } else {
                AnimationSequence reversedAnimation = new AnimationSequence(fadeAnimation.Reversed(), e => StartCoroutine(e));
                reversedAnimation.AddOperation(new AnimationOperation {targetObject = scoreChange, type = UIAnimationType.Activate, activate = false, delay = 0.5f});
                reversedAnimation.Play();
            }
        } else {
            scoreChange.SetActive(show);
        }
    }
}