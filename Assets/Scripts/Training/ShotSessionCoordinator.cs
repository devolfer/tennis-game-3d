using System;
using System.Collections;
using System.Collections.Generic;
using CustomUtilities;
using UnityEngine;

public class ShotSessionCoordinator : MonoBehaviour, ISessionCoordinator {
    [SerializeField] private ShotSessionEventRelay shotSessionEventRelay;
    [SerializeField] private BallMachine ballMachine;
    [SerializeField] private ShotZoneHighlighter shotZoneHighlighter;
    [SerializeField] private Transform trainingBallsParent;
    [SerializeField] private TennisBall ballPrefab;
    [SerializeField] private int numberPooledBalls;
    
    [SerializeField] private int hitZoneReward;
    [SerializeField] private int correctShotReward;

    public event Action OnSessionComplete;

    private ShotSessionData shotSession;
    private ShotTask[] shotTasks;
    private ShotTask previousShotTask;
    private ShotTask currentShotTask;
    private ShotTask nextShotTask;

    private int currentShotTaskIndex;
    private float shootInterval;

    private Queue<TennisBall> queuedBalls;
    private TennisBall currentPlayBall;

    private Coroutine shotSessionRoutine;

    private int currentScore;
    private int currentCombo;
    private int bestCombo;

    private void OnEnable() {
        shotZoneHighlighter.OnBallInShotZone += ValidateBallInShotZone;
    }

    private void OnDisable() {
        shotZoneHighlighter.OnBallInShotZone -= ValidateBallInShotZone;
    }

    public void SetupSession(TrainingSessionData sessionData) {
        shotSession = (ShotSessionData) sessionData;
        shootInterval = shotSession.shootInterval;
        shotTasks = shotSession.shotTaskList.ToArray();
        currentShotTaskIndex = 0;
        previousShotTask = null;
        currentShotTask = shotTasks[currentShotTaskIndex];
        nextShotTask = currentShotTaskIndex + 1 < shotTasks.Length ? shotTasks[currentShotTaskIndex + 1] : null;

        shotSession.MaximumScore = GetMaxScore(shotSession);

        queuedBalls = Utility.CreateMonoBehaviourPool<TennisBall>(ballPrefab, numberPooledBalls, trainingBallsParent);

        ballMachine.MoveTo(currentShotTask.BallMachineShot.StartPosition);
        ballMachine.AimAt(currentShotTask.BallMachineShot.TargetPosition);
        shotZoneHighlighter.ScaleAndPositionZone(currentShotTask.TaskCourtZone);
        shotZoneHighlighter.ColourZone(currentShotTask.TaskShotType);
        shotSessionEventRelay.ChangeTask(currentShotTask);
        shotSessionEventRelay.ChangeProgress(0);
        shotSessionEventRelay.ChangeScore(0);
        shotSessionEventRelay.ChangeCombo(0);

        gameObject.SetActive(true);
    }

    public void StartSession() {
        if (shotSessionRoutine != null) return;
        shotSessionEventRelay.Begin();
        shotSessionRoutine = StartCoroutine(ShotSessionRoutine());
    }

    public void EndSession() {
        shotSessionEventRelay.End();
        
        if (shotSessionRoutine == null) return;
        StopCoroutine(shotSessionRoutine);
    }

    public void CleanupSession() {
        shotSession = null;
        shootInterval = 0;
        shotTasks = null;
        currentShotTaskIndex = 0;
        previousShotTask = null;
        currentShotTask = null;
        nextShotTask = null;
        currentScore = 0;
        currentCombo = 0;
        bestCombo = 0;

        currentPlayBall = null;
        foreach (TennisBall ball in queuedBalls) {
            Destroy(ball.gameObject);
        }

        queuedBalls.Clear();

        gameObject.SetActive(false);
    }

    private IEnumerator ShotSessionRoutine() {
        while (currentShotTaskIndex < shotTasks.Length) {
            currentPlayBall = queuedBalls.Dequeue();
            queuedBalls.Enqueue(currentPlayBall);
            ballMachine.ShootBall(currentPlayBall, currentShotTask.BallMachineShot);

            nextShotTask = currentShotTaskIndex + 1 < shotTasks.Length ? shotTasks[currentShotTaskIndex + 1] : null;

            if (nextShotTask != null) {
                yield return new WaitForSeconds(shootInterval * 0.75f);

                ballMachine.MoveTo(nextShotTask.BallMachineShot.StartPosition, true, shootInterval * 0.25f);
                ballMachine.AimAt(nextShotTask.BallMachineShot.TargetPosition, true, shootInterval * 0.25f);
                shotZoneHighlighter.ScaleAndPositionZone(nextShotTask.TaskCourtZone, true, shootInterval * 0.25f);
                shotZoneHighlighter.ColourZone(nextShotTask.TaskShotType, true, shootInterval * 0.25f);

                shotSessionEventRelay.ChangeTask(nextShotTask);

                yield return new WaitForSeconds(shootInterval * 0.25f);
            }

            yield return new WaitWhile(() => currentPlayBall.CanBeHit);

            if (!currentShotTask.Played) {
                currentShotTask.Played = true;
                UpdateScore(currentShotTask);
                shotSessionEventRelay.ChangeProgress((currentShotTaskIndex + 1f) / shotTasks.Length);
            }

            previousShotTask = currentShotTask;
            if (nextShotTask != null) currentShotTask = nextShotTask;

            currentShotTaskIndex += 1;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitWhile(() => currentPlayBall.CanBeHit);

        CompleteSession();
    }

    private void CompleteSession() {
        this.DoSequence(new Func<float>[] {
            () => 2f,
            () => {
                shotSessionRoutine = null;
                OnSessionComplete?.Invoke();

                return 0f;
            }
        });
    }

    private void UpdateScore(ShotTask shotTask) {
        if (shotTask == null) return;
    
        int increment = 0;
    
        if (shotTask.ShotZoneTaskAccomplished) {
            currentCombo += 1;
            increment += hitZoneReward;
        } else {
            currentCombo = 0;
        }
        
        bestCombo = Mathf.Max(currentCombo, bestCombo);
    
        if (shotTask.ShotTypeTaskAccomplished) {
            increment += correctShotReward;
        }

        int scoreMultiplier = GetScoreMultiplier(currentCombo);

        currentScore += increment * scoreMultiplier;
        
        shotSession.Score = currentScore;
        
        shotSessionEventRelay.ChangeScore(currentScore);
        shotSessionEventRelay.ChangeCombo(currentCombo);
    }

    private int GetScoreMultiplier(int combo) {
        return combo switch {
            int n when n >= 4 && n < 8 => 2,
            int n when n >= 8 && n < 16 => 4,
            int n when n >= 16 => 8,
            _ => 1
        };
    }

    private int GetMaxScore(ShotSessionData shotSessionData) {
        int scoreSum = 0;
        int combo = 0;

        for (int i = 0; i < shotSessionData.shotTaskList.Count; i++) {
            combo += 1;
            scoreSum += (hitZoneReward + correctShotReward) * GetScoreMultiplier(combo);
        }

        return scoreSum;
    }

    private void ValidateBallInShotZone(TennisBall ball) {
        if (ball != currentPlayBall) return;
        if (!ball.CanBeHit) return;

        currentShotTask.ShotZoneTaskAccomplished = true;

        if (ball.CurrentShot.TypeOfShot == currentShotTask.TaskShotType) {
            currentShotTask.ShotTypeTaskAccomplished = true;
        }

        currentShotTask.Played = true;
        currentPlayBall.CanBeHit = false;
        
        UpdateScore(currentShotTask);
        shotSessionEventRelay.ChangeProgress((currentShotTaskIndex + 1f) / shotTasks.Length);
    }
}