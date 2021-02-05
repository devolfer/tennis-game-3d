using System;
using System.Collections.Generic;
using CustomUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingProgramResults : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private TrainingEventRelay trainingEventRelay;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject content;
    [SerializeField] private MenuButton restartButton;
    [SerializeField] private MenuButton quitButton;
    [SerializeField] private ScrollRect sessionScrollRect;
    [SerializeField] private GameObject sessionResultRowPrefab;
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private Transform ratingScaleTransform;
    [SerializeField] private GameObject ratingScaleImagePrefab;
    [SerializeField] private int ratingScaleImageNumber;

    [SerializeField] private AnimationSequence showAnimation;

    private AnimationSequence showAnim;
    private AnimationSequence hideAnim;
    private UIAnimation titleAnimation;
    private UIAnimation contentAnimation;
    private List<AnimationSequence> spinningAnimations = new List<AnimationSequence>();

    private List<Image> ratingScaleImages;

    private void OnEnable() {
        trainingEventRelay.OnShowResults += ShowResults;

        restartButton.onPress.AddListener(Restart);
        quitButton.onPress.AddListener(Quit);
    }

    private void OnDisable() {
        trainingEventRelay.OnShowResults -= ShowResults;

        restartButton.onPress.RemoveListener(Restart);
        quitButton.onPress.RemoveListener(Quit);
    }

    private void Awake() {
        ShowWindow(false, false);

        if (showAnim == null) {
            showAnim = new AnimationSequence(showAnimation, applicationEventRelay.RequestStartingCoroutine);
            hideAnim = new AnimationSequence(showAnimation.Reversed(), applicationEventRelay.RequestStartingCoroutine);
            showAnim.OnFinished(() => {
                restartButton.Show(true, false);
                quitButton.Show(true, false);
                quitButton.Select();
            });
            hideAnim.OnFinished(() => resultsPanel.SetActive(false));
        }

        title.TryGetComponent(out titleAnimation);
        content.TryGetComponent(out contentAnimation);
    }

    private void OnDestroy() {
        showAnim.Cleanup();
        hideAnim.Cleanup();
        foreach (AnimationSequence animationSequence in spinningAnimations) {
            animationSequence.Cleanup();
        }
    }

    private void Start() {
        CreateRatingScaleImages();
        // ShowResults(false, false);
    }

    private void CreateRatingScaleImages() {
        ratingScaleImages = new List<Image>();
        for (int i = 0; i < ratingScaleImageNumber; i++) {
            GameObject gO = Instantiate(ratingScaleImagePrefab, ratingScaleTransform);
            if (!gO.TryGetComponent(out Image ratingImage)) continue;
            ratingScaleImages.Add(ratingImage);
        }
    }

    private void ShowWindow(bool show, bool animated) {
        if (animated) {
            if (show) {
                if (showAnim == null) {
                    showAnim = new AnimationSequence(showAnimation, applicationEventRelay.RequestStartingCoroutine);
                    hideAnim = new AnimationSequence(showAnimation.Reversed(), applicationEventRelay.RequestStartingCoroutine);
                    showAnim.OnFinished(() => {
                        restartButton.Show(true, false);
                        quitButton.Show(true, false);
                        quitButton.Select();
                    });
                    hideAnim.OnFinished(() => resultsPanel.SetActive(false));
                }

                showAnim.Play();
                resultsPanel.SetActive(true);
            } else {
                hideAnim.Play();
            }
        } else {
            ShowContent(show, false);
        }

        applicationEventRelay.CinemaBar(show, true);
        
        if (show) {
            Utility.SetCursor(true, CursorLockMode.None);
        } else {
            Utility.SetCursor(false, CursorLockMode.Locked);
        }
    }

    private void ShowContent(bool show, bool animated) {
        if (animated) {
            if (titleAnimation) titleAnimation.Show(show);
            if (contentAnimation) contentAnimation.Show(show);
        } else {
            title.SetActive(show);
            content.SetActive(show);
        }

        restartButton.Show(show, animated);
        quitButton.Show(show, animated);
        // resultsPanel.SetActive(show);
    }

    private void ShowResults(TrainingProgramData programData) {
        sessionNameText.text = $"{programData.programName} - Results";

        int programTotalScore = programData.GetTotalScore();
        int programMaximumScore = programData.GetMaximumScore();

        totalScoreText.text = $"{programTotalScore}/{programMaximumScore}";

        float ratingValue = (float) programTotalScore / programMaximumScore * 100f;
        float f = 100f / ratingScaleImageNumber;
        float colourDuration = (float) ratingScaleImages.Count / (ratingScaleImages.Count * ratingScaleImages.Count);
        float delay = colourDuration * 1.5f;

        AnimationSequence ratingAnimation = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
        ratingAnimation.AddDelay(1f);
        
        spinningAnimations.Clear();
        AnimationOperation spinningOperation1 = new AnimationOperation(null, UIAnimationType.Rotate, EaseType.Linear, 0, 4f) {
            rotateSettings = new AnimationOperation.RotateSettings {
                startEuler = new Vector3(0, 0, 0),
                targetEuler = new Vector3(0, 0, 180)
            }
        };
        AnimationOperation spinningOperation2 = new AnimationOperation(null, UIAnimationType.Rotate, EaseType.Linear, 3.95f, 4f) {
            rotateSettings = new AnimationOperation.RotateSettings {
                startEuler = new Vector3(0, 0, 180),
                targetEuler = new Vector3(0, 0, 360)
            }
        };

        for (int i = 0; i < ratingScaleImages.Count; i++) {
            if (i * f >= ratingValue) break;

            AnimationOperation colourOperation = new AnimationOperation(ratingScaleImages[i].gameObject, UIAnimationType.Colour, EaseType.CubicIn, delay, colourDuration) {
                colourSettings = new AnimationOperation.ColourSettings {
                    startColour = ratingScaleImages[i].color,
                    targetColour = Color.white
                }
            };

            ratingAnimation.AddOperation(colourOperation);

            AnimationSequence spinningAnim = new AnimationSequence(applicationEventRelay.RequestStartingCoroutine);
            
            spinningAnim.AddOperation(
                new AnimationOperation(spinningOperation1) {targetObject = ratingScaleImages[i].gameObject},
                new AnimationOperation(spinningOperation2) {targetObject = ratingScaleImages[i].gameObject});
            spinningAnim.Loop();
            
            spinningAnimations.Add(spinningAnim);
        }

        foreach (TrainingSessionData session in programData.trainingSessions) {
            GameObject sessionResultRow = Instantiate(sessionResultRowPrefab, sessionScrollRect.content);
            if (!sessionResultRow.TryGetComponent(out SessionResultRow sessionResult)) continue;

            sessionResult.Setup(session.sessionName, $"{session.Score}/{session.MaximumScore}", null);
            // TODO hook session row buttons to show respective session results => set action
        }

        ShowWindow(true, true);
        ratingAnimation.Play();

        foreach (AnimationSequence animationSequence in spinningAnimations) {
            animationSequence.Play();
        }
    }

    private void Restart(MenuButton button) {
        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => {
                ShowWindow(false, true);

                return 1f;
            },
            () => {
                applicationEventRelay.RequestReloadingScene(true);

                return 0f;
            }
        }));
    }

    private void Quit(MenuButton button) {
        applicationEventRelay.RequestStartingCoroutine(Utility.SequenceRoutine(new Func<float>[] {
            () => {
                ShowWindow(false, true);

                return 1f;
            },
            () => {
                applicationEventRelay.RequestLoadingMainMenu(true);
                
                trainingEventRelay.CurrentProgram = null;

                return 0f;
            }
        }));

        foreach (AnimationSequence animationSequence in spinningAnimations) {
            animationSequence.Cleanup();
        }
    }
}