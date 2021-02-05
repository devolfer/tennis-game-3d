using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuEventRelay", menuName = "Scriptable Object/Event Relay/Main Menu")]
public class MainMenuEventRelay : ScriptableObject {
    public event Action OnReturnToBase;
    
    public event Action OnStatisticsPressed;
    public event Action OnQuickMatchPressed;
    public event Action OnCareerPressed;
    public event Action OnOnlinePressed;
    public event Action OnTrainingPressed;
    public event Action OnSettingsPressed;
    
    public void ReturnToBase() {
        OnReturnToBase?.Invoke();
    }

    public void OpenMenuStatistics() {
        OnStatisticsPressed?.Invoke();
    }

    public void OpenMenuQuickMatch() {
        OnQuickMatchPressed?.Invoke();
    }

    public void OpenMenuCareer() {
        OnCareerPressed?.Invoke();
    }

    public void OpenMenuOnline() {
        OnOnlinePressed?.Invoke();
    }

    public void OpenMenuTraining() {
        OnTrainingPressed?.Invoke();
    }

    public void OpenMenuSettings() {
        OnSettingsPressed?.Invoke();
    }
}