using System;
using System.Collections;
using CustomUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private MainMenuEventRelay mainMenuEventRelay;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private MenuButton statisticsButton;
    [SerializeField] private MenuButton quickMatchButton;
    [SerializeField] private MenuButton careerButton;
    [SerializeField] private MenuButton onlineButton;
    [SerializeField] private MenuButton trainingButton;
    [SerializeField] private MenuButton settingsButton;

    private AnimationSequence statisticsButtonSequence;
    private Action action;

    private void OnEnable() {
        if (applicationEventRelay) applicationEventRelay.OnLoadingDone += SceneLoadedShow;
        if (mainMenuEventRelay) mainMenuEventRelay.OnReturnToBase += ShowMenu;

        trainingButton.onPress.AddListener(ShowTrainingMenu);
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnLoadingDone -= SceneLoadedShow;
        if (mainMenuEventRelay) mainMenuEventRelay.OnReturnToBase -= ShowMenu;

        trainingButton.onPress.RemoveListener(ShowTrainingMenu);
    }

    private void Awake() {
        statisticsButton.Show(false, false);
        quickMatchButton.Show(false, false);
        careerButton.Show(false, false);
        onlineButton.Show(false, false);
        trainingButton.Show(false, false);
        settingsButton.Show(false, false);

        backgroundImage.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        statisticsButtonSequence?.Cleanup();
    }

    private void SceneLoadedShow() {
        // Maybe do this in via the editor...
        GameObject qMButton = quickMatchButton.gameObject;
        AnimationOperation qMFadeOperation = new AnimationOperation(qMButton, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 0.5f) {
            fadeSettings = new AnimationOperation.FadeSettings {
                startAlpha = 0f,
                targetAlpha = 1f
            }
        };
        AnimationOperation qMScaleOperation = new AnimationOperation(qMButton, UIAnimationType.Scale, EaseType.SmoothStepSmoother, 0, 0.5f) {
            scaleSettings = new AnimationOperation.ScaleSettings {
                startScale = Vector3.zero,
                targetScale = Vector3.one
            }
        };
        AnimationOperation qMActivateOperation = new AnimationOperation(qMButton, UIAnimationType.Activate, EaseType.None, 0, 0.5f) {
            activate = true
        };

        AnimationSequence quickMatchButtonSequence = new AnimationSequence(RelayRoutine);
        quickMatchButtonSequence.AddOperation(qMFadeOperation, qMScaleOperation, qMActivateOperation);

        GameObject cButton = careerButton.gameObject;
        AnimationOperation cFadeOperation = new AnimationOperation(qMFadeOperation) {
            targetObject = cButton,
            delay = 0.25f
        };
        AnimationOperation cScaleOperation = new AnimationOperation(qMScaleOperation) {
            targetObject = cButton
        };
        AnimationOperation cActivateOperation = new AnimationOperation(qMActivateOperation) {
            targetObject = cButton
        };
        AnimationSequence careerButtonSequence = new AnimationSequence(RelayRoutine);
        careerButtonSequence.AddOperation(cFadeOperation, cScaleOperation, cActivateOperation);

        GameObject oButton = onlineButton.gameObject;
        AnimationOperation oFadeOperation = new AnimationOperation(qMFadeOperation) {
            targetObject = oButton,
            delay = 0.35f
        };
        AnimationOperation oScaleOperation = new AnimationOperation(qMScaleOperation) {
            targetObject = oButton
        };
        AnimationOperation oActivateOperation = new AnimationOperation(qMActivateOperation) {
            targetObject = oButton
        };
        AnimationSequence onlineButtonSequence = new AnimationSequence(RelayRoutine);
        onlineButtonSequence.AddOperation(oFadeOperation, oScaleOperation, oActivateOperation);

        GameObject tButton = trainingButton.gameObject;
        AnimationOperation tFadeOperation = new AnimationOperation(qMFadeOperation) {
            targetObject = tButton,
            delay = 0.45f
        };
        AnimationOperation tScaleOperation = new AnimationOperation(qMScaleOperation) {
            targetObject = tButton
        };
        AnimationOperation tActivateOperation = new AnimationOperation(qMActivateOperation) {
            targetObject = tButton
        };
        AnimationSequence trainingButtonSequence = new AnimationSequence(RelayRoutine);
        trainingButtonSequence.AddOperation(tFadeOperation, tScaleOperation, tActivateOperation);

        GameObject seButton = settingsButton.gameObject;
        AnimationOperation seFadeOperation = new AnimationOperation(qMFadeOperation) {
            targetObject = seButton,
            delay = 0.6f
        };
        AnimationOperation seScaleOperation = new AnimationOperation(qMScaleOperation) {
            targetObject = seButton
        };
        AnimationOperation seActivateOperation = new AnimationOperation(qMActivateOperation) {
            targetObject = seButton
        };
        AnimationSequence settingsButtonSequence = new AnimationSequence(RelayRoutine);
        settingsButtonSequence.AddOperation(seFadeOperation, seScaleOperation, seActivateOperation);

        GameObject stButton = statisticsButton.gameObject;
        AnimationOperation stFadeOperation = new AnimationOperation(qMFadeOperation) {
            targetObject = stButton,
            delay = 0.75f
        };
        AnimationOperation stScaleOperation = new AnimationOperation(qMScaleOperation) {
            targetObject = stButton
        };
        AnimationOperation stActivateOperation = new AnimationOperation(qMActivateOperation) {
            targetObject = stButton
        };
        statisticsButtonSequence = new AnimationSequence(RelayRoutine);
        statisticsButtonSequence.AddOperation(stFadeOperation, stScaleOperation, stActivateOperation);
        statisticsButtonSequence.OnFinished(() => trainingButton.Select());

        quickMatchButtonSequence.Play();
        careerButtonSequence.Play();
        onlineButtonSequence.Play();
        trainingButtonSequence.Play();
        settingsButtonSequence.Play();
        statisticsButtonSequence.Play();

        statisticsButton.CanBePressed = true;
        quickMatchButton.CanBePressed = true;
        careerButton.CanBePressed = true;
        onlineButton.CanBePressed = true;
        trainingButton.CanBePressed = true;
        settingsButton.CanBePressed = true;
        
        Utility.SetCursor(true, CursorLockMode.None);
    }

    private void ShowMenu() {
        // AnimationSequence animationSequence = new AnimationSequence(RelayRoutine);
        //
        // GameObject backgroundImageGameObject = backgroundImage.gameObject;
        // AnimationOperation fadeBackgroundAnimation = new AnimationOperation(backgroundImageGameObject, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 1) {
        //     fadeSettings = new AnimationOperation.FadeSettings {startAlpha = 0f, targetAlpha = 1f}
        // };
        // AnimationOperation activateBackground = new AnimationOperation(backgroundImageGameObject, UIAnimationType.Activate, EaseType.None, 0, 0) {
        //     activate = true
        // };
        //
        // animationSequence.AddOperation(fadeBackgroundAnimation);
        // animationSequence.AddOperation(activateBackground);
        // animationSequence.Play();

        statisticsButton.Show(true, true);
        quickMatchButton.Show(true, true);
        careerButton.Show(true, true);
        onlineButton.Show(true, true);
        trainingButton.Show(true, true);
        settingsButton.Show(true, true);
    }

    private void HideMenu() {
        statisticsButton.Show(false, true);
        quickMatchButton.Show(false, true);
        careerButton.Show(false, true);
        onlineButton.Show(false, true);
        trainingButton.Show(false, true);
        settingsButton.Show(false, true);

        // AnimationSequence animationSequence = new AnimationSequence(RelayRoutine);
        //
        // GameObject backgroundImageGameObject = backgroundImage.gameObject;
        // AnimationOperation fadeBackgroundAnimation = new AnimationOperation(backgroundImageGameObject, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 1) {
        //     fadeSettings = new AnimationOperation.FadeSettings {startAlpha = 1f, targetAlpha = 0f}
        // };
        // AnimationOperation deactivateBackground = new AnimationOperation(backgroundImageGameObject, UIAnimationType.Activate, EaseType.None, 1, 0) {
        //     activate = true
        // };
        //
        // animationSequence.AddOperation(fadeBackgroundAnimation, deactivateBackground);
        // animationSequence.Play();
    }

    private void ShowTrainingMenu(MenuButton button) {
        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => 0.2f,
            () => {
                HideMenu();

                return 0.25f;
            },
            () => {
                mainMenuEventRelay.OpenMenuTraining();

                return 0f;
            }
        }));
    }

    private void RelayRoutine(IEnumerator e) {
        applicationEventRelay.RequestStartingCoroutine(e);
    }
}