using System.Collections;
using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject tennisBallImage;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float fadeScreenDuration;

    public float FadeScreenDuration => fadeScreenDuration;
    private float barCurrentTargetValue;
    private IEnumerator progressRoutine;

    private AnimationSequence showAnim;
    private AnimationSequence hideAnim;

    private AnimationSequence textBlinkAnim;
    private AnimationSequence spinningBallAnim;

    private void Awake() {
        if (showAnim != null) return;
        
        InitAnims();
    }

    private void OnDestroy() {
        showAnim.Cleanup();
        hideAnim.Cleanup();
        textBlinkAnim.Cleanup();
    }

    public void Show(bool show, bool animated) {
        if (animated) {
            if (show) {
                applicationEventRelay.FadeScreen(fadeScreenDuration, true);
                if (showAnim == null) {
                    InitAnims();
                }
                
                showAnim.Play();
                textBlinkAnim.Play();
                spinningBallAnim.Play();
            } else {
                applicationEventRelay.FadeScreen(fadeScreenDuration, false);
                if (hideAnim == null) {
                    InitAnims();
                }
                
                textBlinkAnim.Stop(applicationEventRelay.RequestStoppingCoroutine);
                spinningBallAnim.Stop(applicationEventRelay.RequestStoppingCoroutine);
                hideAnim.Play();
            }
        } else {
            panel.SetActive(show);
        }
    }

    public void SetBarProgress(float value, bool animated) {
        if (animated) {
            if (progressRoutine != null) {
                applicationEventRelay.RequestStoppingCoroutine(progressRoutine);
            }
            
            progressRoutine = Utility.LerpRoutine(1f, null, t => {
                progressBarFill.fillAmount = Lerp.Value(progressBarFill.fillAmount, barCurrentTargetValue, t, Easing.Exponential.In);
            }, () => progressRoutine = null);
            
            applicationEventRelay.RequestStartingCoroutine(progressRoutine);
            
            barCurrentTargetValue = value;
        } else {
            progressBarFill.fillAmount = value; 
        }
    }

    private void InitAnims() {
        showAnim = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        
        AnimationOperation activateOperation = new AnimationOperation(panel, UIAnimationType.Activate, EaseType.None, fadeScreenDuration, 0) {
            activate = true
        };
        AnimationOperation fadeOperation = new AnimationOperation(panel, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 1f) {
            fadeSettings = new AnimationOperation.FadeSettings {
                startAlpha = 0,
                targetAlpha = 1
            }
        };
        showAnim.AddOperation(activateOperation, fadeOperation);

        hideAnim = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        AnimationOperation deactivateOperation = new AnimationOperation(panel, UIAnimationType.Activate, EaseType.None, 1, 0) {
            activate = false
        };

        hideAnim.AddOperation(new AnimationOperation(fadeOperation.Reversed()), deactivateOperation);
        hideAnim.OnFinished(() => {
            progressBarFill.fillAmount = 0;
            if (progressRoutine != null) applicationEventRelay.RequestStoppingCoroutine(progressRoutine);
            barCurrentTargetValue = 0;
        });
        
        textBlinkAnim = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        AnimationOperation blinkOperation = new AnimationOperation(fadeOperation) {
            targetObject = loadingText.gameObject
        };
        textBlinkAnim.AddOperation(blinkOperation);
        textBlinkAnim.PingPong();
        textBlinkAnim.Loop();
        
        AnimationOperation spinningOperation1 = new AnimationOperation(tennisBallImage, UIAnimationType.Rotate, EaseType.Linear, 0, 4f) {
            rotateSettings = new AnimationOperation.RotateSettings {
                startEuler = new Vector3(0, 0, 0),
                targetEuler = new Vector3(0, 0, 180)
            }
        };
        AnimationOperation spinningOperation2 = new AnimationOperation(tennisBallImage, UIAnimationType.Rotate, EaseType.Linear, 3.95f, 4f) {
            rotateSettings = new AnimationOperation.RotateSettings {
                startEuler = new Vector3(0, 0, 180),
                targetEuler = new Vector3(0, 0, 360)
            }
        };
        
        spinningBallAnim = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        spinningBallAnim.AddOperation(spinningOperation1, spinningOperation2);
        spinningBallAnim.Loop();
    }
}