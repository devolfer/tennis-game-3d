using UnityEngine;

[CreateAssetMenu(fileName = "TrainingProgram", menuName = "Scriptable Object/Training/Program")]
public class TrainingProgramData : ScriptableObject {
    public string programName;
    [TextArea] public string description;
    public Sprite menuSprite;
    public TrainingSessionData[] trainingSessions;
    
    public int GetTotalScore() {
        int sum = 0;

        foreach (TrainingSessionData session in trainingSessions) {
            sum += session.Score;
        }

        return sum;
    }
    
    public int GetMaximumScore() {
        int sum = 0;

        foreach (TrainingSessionData session in trainingSessions) {
            sum += session.MaximumScore;
        }

        return sum;
    }

    public void ResetScores() {
        foreach (TrainingSessionData session in trainingSessions) {
            session.ResetScore();
        }
    }

    public float GetProgress() {
        int numSessions = trainingSessions.Length;
        if (numSessions == 0) return 1f;
        
        float sum = 0;

        foreach (TrainingSessionData session in trainingSessions) {
            sum += session.GetProgress();
        }

        return sum / numSessions;
    }

    public void ResetProgress() {
        foreach (TrainingSessionData session in trainingSessions) {
            session.ResetProgress();
        }
    }
}