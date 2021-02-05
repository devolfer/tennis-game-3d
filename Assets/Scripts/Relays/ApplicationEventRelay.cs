using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplicationEventRelay", menuName = "Scriptable Object/Event Relay/Game")]
public class ApplicationEventRelay : ScriptableObject {
    public event Action OnApplicationStarted;
    public event Action<bool> OnApplicationPaused;
    public event Action OnApplicationEnded;

    public event Action<bool> OnPauseEnabled;
    
    public event Action<ApplicationScene[], bool> OnRequestedLoadingScene;
    public event Action<bool> OnRequestedReloadingScene;
    public event Action<bool> OnRequestedLoadingMainMenu;
    public event Action OnLoadingDone;
    
    public event Action OnSceneExit;

    public event Action<float, bool> OnScreenFade; 
    public event Action<float, bool> OnCountdown;
    public event Action<bool, bool> OnCinemaBar;
    public event Action<bool, bool, ModalityWindow.ModalityWindowSettings> OnModalityWindow; 

    public event Action<IEnumerator> OnRequestedStartingCoroutine;
    public event Action<IEnumerator> OnRequestedStoppingCoroutine;

    public void StartApplication() {
        OnApplicationStarted?.Invoke();
    }

    public void PauseApplication(bool flag) {
        OnApplicationPaused?.Invoke(flag);
    }

    public void EndApplication() {
        OnApplicationEnded?.Invoke();
    }

    public void ExitScene() {
        OnSceneExit?.Invoke();
    }
    
    public void EnablePause(bool flag) {
        OnPauseEnabled?.Invoke(flag);
    }

    public void RequestLoadingScene(ApplicationScene[] scenesToLoad, bool showLoadingScreen) {
        OnRequestedLoadingScene?.Invoke(scenesToLoad, showLoadingScreen);
    }
    
    public void RequestReloadingScene(bool showLoadingScreen) {
        OnRequestedReloadingScene?.Invoke(showLoadingScreen);
    }
    
    public void RequestLoadingMainMenu(bool showLoadingScreen) {
        OnRequestedLoadingMainMenu?.Invoke(showLoadingScreen);
    }
    
    public void LoadingDone() {
        OnLoadingDone?.Invoke();
    }
    
    public void FadeScreen(float duration, bool show) {
        OnScreenFade?.Invoke(duration, show);
    }

    public void Countdown(float duration, bool showAsWholeSeconds) {
        OnCountdown?.Invoke(duration, showAsWholeSeconds);
    }
    
    public void CinemaBar(bool show, bool blurBackground) {
        OnCinemaBar?.Invoke(show, blurBackground);
    }

    public void RequestStartingCoroutine(IEnumerator routine) {
        OnRequestedStartingCoroutine?.Invoke(routine);
    }
    
    public void RequestStoppingCoroutine(IEnumerator routine) {
        OnRequestedStoppingCoroutine?.Invoke(routine);
    }

    public void ShowModalityWindow(bool show, bool animated, ModalityWindow.ModalityWindowSettings settings = null) {
        OnModalityWindow?.Invoke(show, animated, settings);
    }
}