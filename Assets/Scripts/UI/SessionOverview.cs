using System;
using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionOverview : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private TrainingEventRelay trainingEventRelay;
    [SerializeField] private GameObject sessionOverviewPanel;
    [SerializeField] private GameObject sessionTitle;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private MenuButton highScoreButton;
    [SerializeField] private MenuButton settingsButton;
    [SerializeField] private MenuButton startSessionButton;
    [SerializeField] private MenuButton trainingOverviewButton;
    [SerializeField] private MenuButton trainingProgressButton;
    [SerializeField] private Image trainingProgressFill;
    [SerializeField] private TextMeshProUGUI trainingProgressText;
    [SerializeField] private MenuButton tasksButton;
    [SerializeField] private MenuButton restartTrainingButton;
    [SerializeField] private MenuButton quitTrainingButton;
    [SerializeField] private float startCountdownDuration;
    [SerializeField] private AnimationSequence initialShowAnimation;

    private AnimationSequence showAnim;
    private AnimationSequence hideAnim;
    private ModalityWindow.ModalityWindowSettings restartModalityWindowSettings;
    private ModalityWindow.ModalityWindowSettings quitModalityWindowSettings;

    private void OnEnable() {
        trainingEventRelay.OnShowSessionOverview += ShowSessionOverview;

        startSessionButton.onPress.AddListener(StartSession);
        restartTrainingButton.onPress.AddListener(ShowModalityWindow);
        quitTrainingButton.onPress.AddListener(ShowModalityWindow);
    }

    private void OnDisable() {
        trainingEventRelay.OnShowSessionOverview -= ShowSessionOverview;

        startSessionButton.onPress.RemoveListener(StartSession);
        restartTrainingButton.onPress.RemoveListener(ShowModalityWindow);
        quitTrainingButton.onPress.RemoveListener(ShowModalityWindow);
    }

    private void Awake() {
        ShowWindow(false, false);

        if (showAnim == null) {
            showAnim = new AnimationSequence(initialShowAnimation, applicationEventRelay.RequestStartingCoroutine);
            startSessionButton.SelectOnShow = true;
            showAnim.OnFinished(() => ShowButtons(true, false));
            // showAnim.OnFinished(() => startSessionButton.Select());
        }

        if (hideAnim == null) {
            hideAnim = new AnimationSequence(initialShowAnimation.Reversed(), applicationEventRelay.RequestStartingCoroutine);
            hideAnim.OnFinished(() => startSessionButton.Deselect());
            hideAnim.OnFinished(() => ShowButtons(false, false));
            hideAnim.OnFinished(() => sessionOverviewPanel.SetActive(false));
        }
        
        restartModalityWindowSettings = new ModalityWindow.ModalityWindowSettings {
            confirmAction = () => {
                applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
                    () => {
                        applicationEventRelay.ShowModalityWindow(false, true);
                        ShowButtons(true, true);
                        ShowWindow(false, true);

                        return 1f;
                    },
                    () => {
                        if (applicationEventRelay) applicationEventRelay.RequestReloadingScene(true);

                        return 0f;
                    }
                }));
            },
            cancelAction = () => {
                applicationEventRelay.ShowModalityWindow(false, true);
                startSessionButton.SelectOnShow = false;
                quitTrainingButton.SelectOnShow = false;
                restartTrainingButton.SelectOnShow = true;
                ShowButtons(true, true);
            }, 
            displayText = "Restart training program?"
        };

        quitModalityWindowSettings = new ModalityWindow.ModalityWindowSettings {
            confirmAction = () => {
                applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
                    () => {
                        applicationEventRelay.ShowModalityWindow(false, true);
                        ShowButtons(true, true);
                        ShowWindow(false, true);

                        return 1f;
                    },
                    () => {
                        if (applicationEventRelay) applicationEventRelay.RequestLoadingMainMenu(true);

                        trainingEventRelay.CurrentProgram = null;

                        return 0f;
                    }
                }));
            },
            cancelAction = () => {
                applicationEventRelay.ShowModalityWindow(false, true);
                startSessionButton.SelectOnShow = false;
                restartTrainingButton.SelectOnShow = false;
                quitTrainingButton.SelectOnShow = true;
                ShowButtons(true, true);
            }, 
            displayText = "Return to menu?"
        };
        
        trainingProgressButton.onSelect.AddListener(SetProgressBar);
    }

    private void OnDestroy() {
        showAnim.Cleanup();
        hideAnim.Cleanup();
        
        trainingProgressButton.onSelect.RemoveListener(SetProgressBar);
    }

    private void ShowWindow(bool show, bool animated) {
        if (animated) {
            if (show) {
                if (showAnim == null) {
                    showAnim = new AnimationSequence(initialShowAnimation, applicationEventRelay.RequestStartingCoroutine);
                    startSessionButton.SelectOnShow = true;
                    showAnim.OnFinished(() => ShowButtons(true, false));
                    // showAnim.OnFinished(() => startSessionButton.Select());
                }
                
                showAnim.Play();

                SetProgressBar(null);
                sessionOverviewPanel.SetActive(true);
                
                Utility.SetCursor(true, CursorLockMode.None);
            } else {
                if (hideAnim == null) {
                    hideAnim = new AnimationSequence(initialShowAnimation.Reversed(), applicationEventRelay.RequestStartingCoroutine);
                    hideAnim.OnFinished(() => startSessionButton.Deselect());
                    hideAnim.OnFinished(() => ShowButtons(false, false));
                    hideAnim.OnFinished(() => sessionOverviewPanel.SetActive(false));
                }
                
                hideAnim.Play();
                
                Utility.SetCursor(false, CursorLockMode.Locked);
            }

            applicationEventRelay.CinemaBar(show, true);
        } else {
            sessionTitle.SetActive(show);

            ShowButtons(show, false);
        }

        // if (show) {
        //     Utility.SetCursor(true, CursorLockMode.None);
        // } else {
        //     Utility.SetCursor(false, CursorLockMode.Locked);
        // }
    }

    private void ShowButtons(bool show, bool animated) {
        highScoreButton.Show(show, animated);
        settingsButton.Show(show, animated);
        startSessionButton.Show(show, animated);
        trainingOverviewButton.Show(show, animated);
        trainingProgressButton.Show(show, animated);
        tasksButton.Show(show, animated);
        restartTrainingButton.Show(show, animated);
        quitTrainingButton.Show(show, animated);
    }

    private void ShowSessionOverview(TrainingSessionData sessionData) {
        titleText.text = sessionData.sessionName;
        startSessionButton.DescriptionText.text = sessionData.description;
        // TODO rest of session data in sub menus

        ShowWindow(true, true);
    }

    private void SetProgressBar(MenuButton button) {
        if (!trainingEventRelay.CurrentProgram) return;
        float currentTrainingProgress = trainingEventRelay.CurrentProgram.GetProgress();
        float showDelay = button == trainingProgressButton ? 0f : 0.75f;

        trainingProgressText.text = $" {currentTrainingProgress:P0}";

        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => showDelay,
            () => {
                applicationEventRelay.RequestStartingCoroutine(Utility.LerpRoutine(
                    1f,
                    () => trainingProgressFill.fillAmount = 0f,
                    t => trainingProgressFill.fillAmount = Lerp.Value(0f, currentTrainingProgress, t, Easing.SmoothStep.Smoother),
                    () => trainingProgressFill.fillAmount = currentTrainingProgress
                ));

                return 0f;
            }
        }));
    }

    private void StartSession(MenuButton button) {
        restartTrainingButton.SelectOnShow = false;
        quitTrainingButton.SelectOnShow = false;
        
        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => {
                ShowWindow(false, true);

                return 2f;
            },
            () => {
                applicationEventRelay.Countdown(startCountdownDuration, true);

                return startCountdownDuration + 1f;
            },
            () => {
                trainingEventRelay.StartPlay();

                return 0f;
            }
        }));
    }

    private void ShowModalityWindow(MenuButton button) {
        ShowButtons(false, true);

        if (button == quitTrainingButton) {
            applicationEventRelay.ShowModalityWindow(true, true, quitModalityWindowSettings);
        } else if (button == restartTrainingButton) {
            applicationEventRelay.ShowModalityWindow(true, true, restartModalityWindowSettings);
        }
    }
}