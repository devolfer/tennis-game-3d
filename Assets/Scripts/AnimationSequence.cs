using System;
using System.Collections;
using System.Collections.Generic;
using CustomUtilities;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AnimationSequence {
    [SerializeField] private bool loop;
    [SerializeField] private bool pingPong;

    [FormerlySerializedAs("animationSequence")] 
    [SerializeField] private AnimationOperation[] animationOperations;

    private Action<IEnumerator> playDelegate;
    private Func<float>[] playFunctions;

    private event Action OnDone;
    private Func<float>[] doneFunctions;

    private IEnumerator playEnumerator;
    private IEnumerator doneEnumerator;

    private bool useUnscaledTime;

    public float Duration {
        get {
            float sum = 0;
            float greatestDurationBefore = 0;
            float operationDuration = 0;
            float operationDelay = 0;

            foreach (AnimationOperation operation in animationOperations) {
                operationDuration = operation.duration;
                operationDelay = operation.delay;
                if (operationDelay > 0) {
                    if (operationDuration + operationDelay > greatestDurationBefore) {
                        sum += operationDuration + operationDelay - greatestDurationBefore;
                    } else {
                        if (operationDuration > greatestDurationBefore) {
                            sum += operationDuration - greatestDurationBefore;
                        } else {
                            sum += operationDelay;
                        }
                    }
                } else {
                    if (operation.type != UIAnimationType.Activate && operationDuration > greatestDurationBefore) {
                        sum += operationDuration - greatestDurationBefore;
                    }
                }
                
                if (operation.type != UIAnimationType.Activate) greatestDurationBefore = Mathf.Max(greatestDurationBefore, operationDuration);
            }

            return sum;
        }
    }

    public AnimationSequence(Action<IEnumerator> delegateForPlay) {
        animationOperations = new AnimationOperation[0];
        playDelegate = delegateForPlay;
    }

    public AnimationSequence(AnimationOperation[] operations, Action<IEnumerator> delegateForPlay) {
        animationOperations = operations;
        playDelegate = delegateForPlay;

        playFunctions = PlayFunctions();
        doneFunctions = DoneFunctions();
    }

    public AnimationSequence(AnimationSequence animationSequence, Action<IEnumerator> delegateForPlay) {
        animationOperations = animationSequence.animationOperations;
        playDelegate = delegateForPlay;

        playFunctions = PlayFunctions();
        doneFunctions = DoneFunctions();

        if (animationSequence.pingPong) PingPong();
        if (animationSequence.loop) Loop();
    }

    public AnimationSequence Reversed() {
        return new AnimationSequence(Reverse(animationOperations), playDelegate);
    }

    private AnimationOperation[] Reverse(AnimationOperation[] operations) {
        List<AnimationOperation> list = new List<AnimationOperation>();
        foreach (AnimationOperation operation in operations) {
            list.Add(new AnimationOperation(operation.Reversed()));
        }

        list.Reverse();

        return list.ToArray();
    }

    public void PingPong() {
        List<AnimationOperation> normalList = new List<AnimationOperation>(animationOperations);
        List<AnimationOperation> reversedList = new List<AnimationOperation>(Reverse(animationOperations));

        float normalDuration = Duration;
        float normalDelaySum = 0;
        foreach (AnimationOperation operation in animationOperations) {
            normalDelaySum += operation.delay;
        }

        AnimationOperation waitOperation = new AnimationOperation(reversedList[0]) {
            type = UIAnimationType.Activate,
            easing = EaseType.None,
            delay = normalDelaySum > 0 ? Mathf.Abs(normalDuration - normalDelaySum) : normalDuration
        };

        // Debug.Log(waitOperation.delay);

        normalList.Add(waitOperation);

        foreach (AnimationOperation operation in reversedList) {
            normalList.Add(operation);
        }

        animationOperations = normalList.ToArray();

        playFunctions = PlayFunctions();
        doneFunctions = DoneFunctions();
    }
    
    public void Play() {
        useUnscaledTime = false;
        playDelegate?.Invoke(playEnumerator = Utility.SequenceRoutine(playFunctions));
        playDelegate?.Invoke(doneEnumerator = Utility.SequenceRoutine(doneFunctions));
    }

    public void Play(bool unscaledTime) {
        useUnscaledTime = unscaledTime;
        playDelegate?.Invoke(playEnumerator = Utility.SequenceRoutine(playFunctions, unscaledTime));
        playDelegate?.Invoke(doneEnumerator = Utility.SequenceRoutine(doneFunctions, unscaledTime));
    }

    private Func<float>[] PlayFunctions() {
        if (animationOperations == null || animationOperations.Length == 0) return null;

        int numAnimationOperations = animationOperations.Length;
        Func<float>[] sequenceFunctions = new Func<float>[numAnimationOperations + 1];

        sequenceFunctions[0] = () => animationOperations[0].delay;

        for (int i = 1; i < sequenceFunctions.Length; i++) {
            AnimationOperation animationOperation = animationOperations[i - 1];
            float delay = i < numAnimationOperations ? animationOperations[i].delay : 0f;

            sequenceFunctions[i] = () => {
                animationOperation.PlayWithDelegate(playDelegate, useUnscaledTime);

                return delay;
            };
        }

        return sequenceFunctions;
    }

    private Func<float>[] DoneFunctions() {
        return new Func<float>[] {
            () => Duration,
            () => {
                OnDone?.Invoke();

                return 0f;
            }
        };
    }

    public void AddDelay(float delayDuration) {
        if (delayDuration <= 0) return;

        AddOperation(new AnimationOperation {delay = delayDuration});
    }

    public void AddOperation(AnimationOperation operation, params AnimationOperation[] additionalToAdd) {
        List<AnimationOperation> operations = new List<AnimationOperation>(animationOperations) {operation};
        foreach (AnimationOperation additionalOperation in additionalToAdd) {
            if (additionalOperation == null) continue;
            operations.Add(additionalOperation);
        }

        animationOperations = operations.ToArray();

        playFunctions = PlayFunctions();
        doneFunctions = DoneFunctions();

        if (!loop) return;
        OnDone -= Play;
        OnDone += Play;
    }

    public void RemoveOperation(AnimationOperation operation) {
        List<AnimationOperation> operations = new List<AnimationOperation>(animationOperations);
        if (!operations.Remove(operation)) return;
        animationOperations = operations.ToArray();

        playFunctions = PlayFunctions();
        doneFunctions = DoneFunctions();

        if (!loop) return;
        OnDone -= Play;
        OnDone += Play;
    }

    public void Loop() {
        loop = true;
        OnDone += Play;
    }

    public void OnFinished(Action actionToTake) {
        if (OnDone != null) {
            foreach (Delegate action in OnDone.GetInvocationList()) {
                Action handler = action as Action;
                if (handler == null) continue;
                if (handler == actionToTake) return;
            }
        }

        OnDone += actionToTake;
    }

    public void RemoveOnFinishedAction(Action actionToRemove) {
        if (OnDone == null) return;

        foreach (Delegate action in OnDone.GetInvocationList()) {
            Action handler = action as Action;
            if (handler == null) continue;
            if (handler != actionToRemove) continue;
            OnDone -= actionToRemove;
            return;
        }
    }

    public void Stop(Action<IEnumerator> stopDelegate) {
        stopDelegate?.Invoke(playEnumerator);
        stopDelegate?.Invoke(doneEnumerator);
    }

    public void Cleanup() {
        if (OnDone == null) return;

        foreach (Delegate action in OnDone.GetInvocationList()) {
            Action handler = action as Action;
            if (handler == null) continue;

            OnDone -= handler;
        }
    }
}