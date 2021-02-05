using UnityEngine;

public abstract class TrainingSessionData : ScriptableObject {
    public string sessionName;
    [TextArea] public string description;
    public Sprite menuSprite;
    public ApplicationScene[] uiScenes; // TODO not here maybe

    public int Score;
    public int MaximumScore;

    public void ResetScore() {
        Score = 0;
        MaximumScore = 0;
    }

    public abstract float GetProgress();
    public abstract void ResetProgress();
}