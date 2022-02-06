using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShotSessionTaskDisplay : MonoBehaviour {
    [SerializeField] private ShotSessionEventRelay shotSessionEventRelay;
    [SerializeField] private GameObject taskPanel;
    [SerializeField] private Image taskShotBackgroundImage;
    [SerializeField] private TextMeshProUGUI taskShotText;

    [SerializeField] private Color anyShotColour;
    [SerializeField] private Color flatShotColour;
    [SerializeField] private Color topSpinShotColour;
    [SerializeField] private Color sliceShotColour;
    [SerializeField] private Color dropOrLobShotColour;
    [SerializeField] private Color volleyShotColour;

    private void OnEnable() {
        shotSessionEventRelay.OnBegin += ShowDisplay;
        shotSessionEventRelay.OnEnd += HideDisplay;

        shotSessionEventRelay.OnTaskChanged += UpdateTask;
    }

    private void OnDisable() {
        shotSessionEventRelay.OnBegin -= ShowDisplay;
        shotSessionEventRelay.OnEnd -= HideDisplay;

        shotSessionEventRelay.OnTaskChanged -= UpdateTask;
    }

    private void Start() {
        ShowTask(false, false);
    }

    private void SetTaskShotText(ShotType shotType, bool animated) {
        if (animated) {
            this.DoRoutine(0.25f, endAction: () => {
                taskShotText.text = $"{shotType.ToString().ToSpaceBeforeUpperCase()}";
            });
        } else {
            taskShotText.text = shotType.ToString().ToSpaceBeforeUpperCase();
        }
    }

    private void SetTaskShotBackgroundImageColour(ShotType shotType, bool animated) {
        Color targetColour = shotType switch {
            ShotType.Any => anyShotColour,
            ShotType.Flat => flatShotColour,
            ShotType.TopSpin => topSpinShotColour,
            ShotType.Slice => sliceShotColour,
            ShotType.DropOrLob => dropOrLobShotColour,
            ShotType.Volley => volleyShotColour,
            _ => Color.white
        };

        if (animated) {
            AnimationOperation colourOperation = new AnimationOperation(taskShotBackgroundImage.gameObject, UIAnimationType.Colour, EaseType.SmoothStepSmoother, 0, 0.5f) {
                colourSettings = new AnimationOperation.ColourSettings {
                    startColour = taskShotBackgroundImage.color,
                    targetColour = targetColour
                }
            };
            AnimationSequence colourAnimation = new AnimationSequence(e => StartCoroutine(e));
            colourAnimation.AddOperation(colourOperation);
            
            colourAnimation.Play();
        } else {
            taskShotBackgroundImage.color = targetColour;
        }
    }

    private void ShowTask(bool show, bool animated) {
        if (animated) {
            AnimationOperation fadeOperation = new AnimationOperation(taskPanel, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
                fadeSettings = new AnimationOperation.FadeSettings {
                    startAlpha = 0,
                    targetAlpha = 1
                }
            };
            AnimationOperation anchorPositionOperation = new AnimationOperation(taskPanel, UIAnimationType.AnchoredPosition, EaseType.BackInOut, 0, 0.5f) {
                anchoredPositionSettings = new AnimationOperation.AnchoredPositionSettings {
                    startMin = new Vector2(0f, 0.1f),
                    startMax = new Vector2(1f, 1.1f),
                    targetMin = new Vector2(0f, 0f),
                    targetMax = new Vector2(1f, 1f)
                }
            };
            AnimationSequence showAnimation = new AnimationSequence(e => StartCoroutine(e));
            showAnimation.AddOperation(fadeOperation);
            showAnimation.AddOperation(anchorPositionOperation);
            showAnimation.AddOperation(new AnimationOperation {targetObject = taskPanel, type = UIAnimationType.Activate, activate = true});

            if (show) {
                showAnimation.Play();
            } else {
                AnimationSequence reversedShowAnimation = new AnimationSequence(showAnimation.Reversed(), e => StartCoroutine(e));
                reversedShowAnimation.AddOperation(new AnimationOperation {targetObject = taskPanel, type = UIAnimationType.Activate, activate = false, delay = 0.5f});
                reversedShowAnimation.Play();
            }
        } else {
            taskPanel.SetActive(show);
        }
    }

    private void ShowDisplay() {
        ShowTask(true, true);
    }

    private void HideDisplay() {
        ShowTask(false, true);
    }

    private void UpdateTask(ShotTask task) {
        SetTaskShotText(task.TaskShotType, true);
        SetTaskShotBackgroundImageColour(task.TaskShotType, true);
    }
}