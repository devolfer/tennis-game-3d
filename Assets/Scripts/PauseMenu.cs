using System;
using CustomUtilities;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private InputEventRelay inputEventRelay;
    [SerializeField] private GameObject panel;
    [SerializeField] private MenuButton resumeButton;
    [SerializeField] private MenuButton quitButton;

    private bool pausingEnabled;
    private bool paused;

    private AnimationSequence showAnimation;
    private AnimationSequence hideAnimation;

    private ModalityWindow.ModalityWindowSettings quitModalityWindowSettings;
    
    private bool mouseShownBefore;
    private CursorLockMode mouseLockStateBefore;

    private void OnEnable() {
        applicationEventRelay.OnPauseEnabled += EnablePausing;
        inputEventRelay.OnPauseEvent += Pause;

        resumeButton.onPress.AddListener(Resume);
        quitButton.onPress.AddListener(ShowModalityWindow);
    }

    private void OnDisable() {
        applicationEventRelay.OnPauseEnabled -= EnablePausing;
        inputEventRelay.OnPauseEvent -= Pause;

        resumeButton.onPress.RemoveListener(Resume);
        quitButton.onPress.RemoveListener(ShowModalityWindow);
    }

    private void Awake() {
        panel.SetActive(false);

        InitModalityWindowSettings();
        InitAnimations();
    }

    private void OnDestroy() {
        showAnimation.Cleanup();
        hideAnimation.Cleanup();
    }

    private void EnablePausing(bool value) {
        pausingEnabled = value;
    }

    private void InitModalityWindowSettings() {
        quitModalityWindowSettings = new ModalityWindow.ModalityWindowSettings {
            confirmAction = () => {
                StartCoroutine(Utility.SequenceRoutine(new Func<float>[] {
                    () => {
                        pausingEnabled = true;
                        applicationEventRelay.ShowModalityWindow(false, true);
                        Pause();

                        return 1f;
                    },
                    () => {
                        applicationEventRelay.RequestLoadingMainMenu(true);

                        return 0f;
                    }
                }, true));
            },
            cancelAction = () => {
                applicationEventRelay.ShowModalityWindow(false, true);
                Show(true, true);
                pausingEnabled = true;
            },
            displayText = "Return to menu?"
        };
    }

    private void InitAnimations() {
        AnimationOperation fadeOperation = new AnimationOperation(panel, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
            fadeSettings = new AnimationOperation.FadeSettings {
                startAlpha = 0,
                targetAlpha = 1
            }
        };
        showAnimation = new AnimationSequence(e => StartCoroutine(e));
        showAnimation.AddOperation(fadeOperation, new AnimationOperation {targetObject = panel, type = UIAnimationType.Activate, activate = true});
        resumeButton.SelectOnShow = true;
        showAnimation.OnFinished(() => {
            resumeButton.Show(true, false);
            quitButton.Show(true, false);
        });
        
        hideAnimation = new AnimationSequence(e => StartCoroutine(e));
        hideAnimation.AddOperation(fadeOperation.Reversed(), new AnimationOperation {targetObject = panel, type = UIAnimationType.Activate, activate = false, delay = 0.5f});
    }

    private void Pause() {
        if (!pausingEnabled) return;
        
        paused = !paused;

        if (paused) {
            Time.timeScale = 0;
            Show(true, true);
        } else {
            this.DoSequence(new Func<float>[] {
                () => {
                    Show(false, true);
            
                    return 0.5f;
                },
                () => {
                    Time.timeScale = 1;
            
                    return 0;
                }
            }, true);
        }
    }

    private void Resume(MenuButton button) {
        Pause();
    }

    private void Show(bool show, bool animated) {
        if (animated) {
            if (show) {
                showAnimation.Play(true);
            } else {
                hideAnimation.Play(true);
            }
        } else {
            panel.SetActive(show);
        }

        if (!pausingEnabled) return;

        if (show) {
            mouseShownBefore = Cursor.visible;
            mouseLockStateBefore = Cursor.lockState;
            Utility.SetCursor(true, CursorLockMode.None);
        } else {
            Utility.SetCursor(mouseShownBefore, mouseLockStateBefore);
        }
    }

    private void ShowModalityWindow(MenuButton button) {
        pausingEnabled = false;

        Show(false, true);

        applicationEventRelay.ShowModalityWindow(true, true, quitModalityWindowSettings);
    }
}