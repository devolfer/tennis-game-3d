using System;
using System.Collections.Generic;
using CustomUtilities;
using UnityEngine;

public class MainMenuTraining : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private MainMenuEventRelay mainMenuEventRelay;
    [SerializeField] private TrainingEventRelay trainingEventRelay;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject background;
    [SerializeField] private MenuButton backButton;
    [SerializeField] private Transform loadButtonsContainer;
    [SerializeField] private MenuButton menuButtonScrollPrefab;

    [SerializeField] private TrainingProgramData[] trainingPrograms;
    [SerializeField] private ApplicationScene trainingScene;

    private List<MenuButton> overviewButtons = new List<MenuButton>();

    private AnimationSequence backgroundShowAnimation;
    private AnimationSequence backgroundHideAnimation;

    private void Awake() {
        ShowOverview(false, false);

        backButton.onPress.AddListener(Back);
        
        backgroundShowAnimation = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        backgroundHideAnimation = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        
        AnimationOperation fadeOperation = new AnimationOperation(background, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
            fadeSettings = new AnimationOperation.FadeSettings {startAlpha = 0, targetAlpha = 1}
        };
        
        backgroundShowAnimation.AddOperation(fadeOperation, new AnimationOperation {targetObject = background, type = UIAnimationType.Activate, activate = true});
        backgroundHideAnimation.AddOperation(fadeOperation.Reversed(), new AnimationOperation {targetObject = background, type = UIAnimationType.Activate, activate = false, delay = fadeOperation.duration});
    }

    private void OnEnable() {
        if (applicationEventRelay) applicationEventRelay.OnLoadingDone += LoadingDone;
        if (mainMenuEventRelay) mainMenuEventRelay.OnTrainingPressed += ShowMenu;
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnLoadingDone -= LoadingDone;
        if (mainMenuEventRelay) mainMenuEventRelay.OnTrainingPressed -= ShowMenu;
    }

    private void OnDestroy() {
        foreach (MenuButton menuButton in overviewButtons) {
            menuButton.onClick.RemoveAllListeners();
        }

        backButton.onPress.RemoveListener(Back);
        
        backgroundShowAnimation.Cleanup();
        backgroundHideAnimation.Cleanup();
    }

    private void LoadingDone() {
        foreach (TrainingProgramData program in trainingPrograms) {
            MenuButton menuButton = Instantiate(menuButtonScrollPrefab, loadButtonsContainer);
            menuButton.HeaderText.text = program.programName;
            menuButton.DescriptionText.text = program.description;
            if (program.menuSprite) menuButton.ButtonImage.sprite = program.menuSprite;
            menuButton.Show(false, false);
            overviewButtons.Add(menuButton);

            menuButton.onPress.AddListener(button => LoadTrainingProgram(program));
        }

        overviewButtons[0].SelectOnShow = true;
        backButton.Show(false, false);
    }

    private void ShowOverview(bool show, bool animated) {
        if (animated) {
            if (show) {
                backgroundShowAnimation.Play();
                foreach (MenuButton button in overviewButtons) {
                    button.Show(true, true);
                }

                backButton.Show(true, true);
                panel.SetActive(true);
            } else {
                applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
                    () => {
                        backgroundHideAnimation.Play();
                        
                        foreach (MenuButton button in overviewButtons) {
                            button.Show(false, true);
                        }
                        
                        backButton.Show(false, true);

                        return 0.5f;
                    },
                    () => {
                        panel.SetActive(false);

                        return 0f;
                    }
                }));
            }
        } else {
            foreach (MenuButton button in overviewButtons) {
                button.Show(show,false);
            }
            
            backButton.Show(show, false);
            background.SetActive(show);
            panel.SetActive(show);
        }
    }

    private void ShowMenu() {
        ShowOverview(true, true);
    }

    private void HideMenu() {
        ShowOverview(false, true);
    }

    private void LoadTrainingProgram(TrainingProgramData programData) {
        List<ApplicationScene> scenesToLoad = new List<ApplicationScene> {trainingScene};
        foreach (TrainingSessionData session in programData.trainingSessions) {
            foreach (ApplicationScene uiScene in session.uiScenes) {
                if (scenesToLoad.Contains(uiScene)) continue;
                scenesToLoad.Add(uiScene);
            }
        }

        // TODO tell global instance which training program is played!
        trainingEventRelay.CurrentProgram = programData;

        HideMenu();
        applicationEventRelay.RequestLoadingScene(scenesToLoad.ToArray(), true);
    }

    private void Back(MenuButton button) {
        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => {
                HideMenu();

                return 0.25f;
            },
            () => {
                mainMenuEventRelay.ReturnToBase();

                return 0f;
            }
        }));
    }
}