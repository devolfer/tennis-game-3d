using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotSessionEventRelay", menuName = "Scriptable Object/Event Relay/Training/Shot Session")]
public class ShotSessionEventRelay : ScriptableObject {
    public event Action OnBegin;
    public event Action OnEnd;
    
    public event Action<int> OnScoreValueChanged;
    public event Action<int> OnComboValueChanged;
    public event Action<ShotTask> OnTaskChanged;
    public event Action<float> OnProgressChanged; 
    
    public void Begin() {
        OnBegin?.Invoke();
    }
    
    public void End() {
        OnEnd?.Invoke();
    }

    public void ChangeScore(int value) {
        OnScoreValueChanged?.Invoke(value);
    }

    public void ChangeCombo(int value) {
        OnComboValueChanged?.Invoke(value);
    }

    public void ChangeTask(ShotTask shotTask) {
        OnTaskChanged?.Invoke(shotTask);
    }

    public void ChangeProgress(float percentage) {
        OnProgressChanged?.Invoke(percentage);
    }
}