using System;
using CustomUtilities;
using TMPro;
using UnityEngine;

public class TrainingProgramCoordinator : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private TrainingEventRelay trainingEventRelay;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI trainerMessageText;
    [SerializeField] private TrainingProgramData fallBackProgram;

    [SerializeField] private ShotSessionCoordinator shotSessionCoordinator;
    // add more coordinators once there are more training session types

    private TrainingProgramData trainingProgram;
    private TrainingSessionData[] trainingSessions;
    private TrainingSessionData currentSession;
    private ISessionCoordinator currentSessionCoordinator;

    private int currentSessionIndex;

    private void OnEnable() {
        applicationEventRelay.OnLoadingDone += InitProgram;
        trainingEventRelay.OnStartPlay += StartCurrentSession;
        trainingEventRelay.OnStartPlay += EnablePausing;
        trainingEventRelay.OnEndPlay += DisablePausing;
    }

    private void OnDisable() {
        applicationEventRelay.OnLoadingDone -= InitProgram;
        trainingEventRelay.OnStartPlay -= StartCurrentSession;
        trainingEventRelay.OnStartPlay -= EnablePausing;
        trainingEventRelay.OnEndPlay -= DisablePausing;
    }

    private void EnablePausing() {
        applicationEventRelay.EnablePause(true);
    }
    
    private void DisablePausing() {
        applicationEventRelay.EnablePause(false);
    }

    private void InitProgram() {
        if (trainingEventRelay.CurrentProgram) {
            trainingProgram = trainingEventRelay.CurrentProgram;
        } else {
            trainingProgram = fallBackProgram;
            trainingEventRelay.CurrentProgram = fallBackProgram;
        }

        trainingProgram.ResetProgress();
        trainingProgram.ResetScores();
        trainingSessions = trainingProgram.trainingSessions;
        currentSessionIndex = 0;
        currentSession = trainingSessions[0];

        trainingEventRelay.ShowSessionOverview(currentSession);

        this.DoSequence(new Func<float>[] {
            () => 1f,
            () => {
                SetupSessionCoordinator(currentSession);

                return 0f;
            }
        });
    }

    private void SetupSessionCoordinator(TrainingSessionData sessionData) {
        Type sessionType = sessionData.GetType();

        if (sessionType == typeof(ShotSessionData)) {
            currentSessionCoordinator = shotSessionCoordinator;
            currentSessionCoordinator.SetupSession(sessionData);
            return;
        }

        // add handling more coordinators in the future
    }

    private void StartCurrentSession() {
        currentSessionCoordinator.StartSession();
        currentSessionCoordinator.OnSessionComplete += OnSessionComplete;
    }

    private void EndCurrentSession() {
        currentSessionCoordinator.EndSession();
        currentSessionCoordinator.OnSessionComplete -= OnSessionComplete;

        trainingEventRelay.EndPlay();
    }

    private void OnSessionComplete() {
        EndCurrentSession();

        if (currentSessionIndex + 1 < trainingSessions.Length) {
            SetupNextSession();
        } else {
            EndTrainingProgram();
        }
    }

    private void SetupNextSession() {
        currentSessionIndex += 1;
        currentSession = trainingSessions[currentSessionIndex];

        this.DoSequence(new Func<float>[] {
            () => {
                ShowMessage("Next Session!");

                return 2f;
            },
            () => {
                trainingEventRelay.ShowSessionOverview(currentSession);

                currentSessionCoordinator.CleanupSession();
                SetupSessionCoordinator(currentSession);

                return 0f;
            }
        });
    }

    private void EndTrainingProgram() {
        this.DoSequence(new Func<float>[] {
            () => {
                ShowMessage("Done!");

                return 2f;
            },
            () => {
                trainingEventRelay.ShowResults(trainingProgram);

                return 0f;
            }
        });
    }

    private void ShowMessage(string message) {
        trainerMessageText.text = message;
        
        AnimationOperation scaleOperation = new AnimationOperation(messagePanel, UIAnimationType.Scale, EaseType.SmoothStepSmoother, 0, 1f) {
            scaleSettings = new AnimationOperation.ScaleSettings {
                startScale = Vector3.zero,
                targetScale = Vector3.one
            }
        };
        
        AnimationOperation fadeOperation = new AnimationOperation(messagePanel, UIAnimationType.Fade, EaseType.SmoothStepSmoother, 0, 1f) {
            fadeSettings = new AnimationOperation.FadeSettings {
                startAlpha = 0,
                targetAlpha = 1
            }
        };
        
        AnimationOperation flipOperation = new AnimationOperation(messagePanel, UIAnimationType.Rotate, EaseType.SmoothStepSmoother, 0, 0.5f) {
            rotateSettings = new AnimationOperation.RotateSettings {
                startEuler = Vector3.zero,
                targetEuler = new Vector3(90, 0, 0)
            }
        };
        
        AnimationSequence showMessageAnimation = new AnimationSequence(e => StartCoroutine(e));
        showMessageAnimation.AddOperation(scaleOperation, fadeOperation, new AnimationOperation {targetObject = messagePanel, type = UIAnimationType.Activate, activate = true});
        showMessageAnimation.AddDelay(2f);
        showMessageAnimation.AddOperation(new AnimationOperation(fadeOperation.Reversed()) {duration = 0.5f}, flipOperation, new AnimationOperation {targetObject = messagePanel, type = UIAnimationType.Activate, activate = false, delay = 0.5f}, new AnimationOperation {targetObject = messagePanel, type = UIAnimationType.Rotate, rotateSettings = new AnimationOperation.RotateSettings {targetEuler = Vector3.zero}, delay = 0.5f});
        
        showMessageAnimation.Play();
    }
}