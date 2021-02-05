using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingEventRelay", menuName = "Scriptable Object/Event Relay/Training/Base")]
public class TrainingEventRelay : ScriptableObject {
    public TrainingProgramData CurrentProgram { get; set; }

    public event Action OnStartPlay;
    public event Action OnEndPlay;

    public event Action<TrainingSessionData> OnShowSessionOverview; 
    public event Action<TrainingProgramData> OnShowResults; 
    
    public void StartPlay() {
        OnStartPlay?.Invoke();
    }

    public void EndPlay() {
        OnEndPlay?.Invoke();
    }

    public void ShowSessionOverview(TrainingSessionData sessionData) {
        OnShowSessionOverview?.Invoke(sessionData);
    }

    public void ShowResults(TrainingProgramData programData) {
        OnShowResults?.Invoke(programData);
    }
}