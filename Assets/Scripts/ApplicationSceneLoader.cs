using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationSceneLoader : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private ApplicationScene persistantScene;
    [SerializeField] private ApplicationScene[] mainMenuScenes;

    [SerializeField] private LoadingScreen loadingScreen;

    private List<AsyncOperation> loadingOperationList = new List<AsyncOperation>();
    private List<AsyncOperation> unloadingOperationList = new List<AsyncOperation>();
    private List<string> scenesToUnload = new List<string>();

    private Coroutine loadScenesRoutine;

    private void OnEnable() {
        Application.targetFrameRate = -1;
        if (applicationEventRelay) applicationEventRelay.OnApplicationStarted += OnApplicationStarted;
        
        if (applicationEventRelay) applicationEventRelay.OnRequestedLoadingScene += LoadScenes;
        if (applicationEventRelay) applicationEventRelay.OnRequestedReloadingScene += ReloadCurrentScenes;
        if (applicationEventRelay) applicationEventRelay.OnRequestedLoadingMainMenu += LoadMainMenu;
    }

    private void OnDisable() {
        if (applicationEventRelay) applicationEventRelay.OnApplicationStarted -= OnApplicationStarted;
        
        if (applicationEventRelay) applicationEventRelay.OnRequestedLoadingScene -= LoadScenes;
        if (applicationEventRelay) applicationEventRelay.OnRequestedReloadingScene -= ReloadCurrentScenes;
        if (applicationEventRelay) applicationEventRelay.OnRequestedLoadingMainMenu -= LoadMainMenu;
    }

    private void OnApplicationStarted() {
        if (SceneManager.GetActiveScene().name != persistantScene.sceneName) return;
        
        LoadMainMenu(false);
        if (applicationEventRelay) applicationEventRelay.FadeScreen(loadingScreen.FadeScreenDuration, false);
    }

    private void LoadMainMenu(bool showLoadingScreen) {
        LoadScenes(mainMenuScenes, showLoadingScreen);
    }

    private void LoadScenes(ApplicationScene[] applicationScenes, bool showLoadingScreen) {
        if (loadScenesRoutine == null) {
            loadScenesRoutine = StartCoroutine(LoadScenesRoutine(applicationScenes, showLoadingScreen));
        } else {
            StartCoroutine(LoadScenesRoutine(applicationScenes, showLoadingScreen, () => loadScenesRoutine == null));
        }
    }

    private IEnumerator LoadScenesRoutine(ApplicationScene[] applicationScenes, bool showLoadingScreen, Func<bool> startCondition = null) {
        if (startCondition != null) {
            yield return new WaitUntil(startCondition);
        }

        if (showLoadingScreen) {
            loadingScreen.Show(true, true);
            yield return new WaitForSeconds(loadingScreen.FadeScreenDuration);
        }

        SetScenesToUnload();

        for (int i = 0; i < applicationScenes.Length; i++) {
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(applicationScenes[i].sceneName, LoadSceneMode.Additive);
            loadingOperation.allowSceneActivation = false;
            
            loadingOperationList.Add(loadingOperation);
        }

        float loadingProgress = 0f;
        while (loadingProgress < 0.89f) {
            loadingProgress = GetTotalLoadingProgress(loadingOperationList);
            // Debug.Log($"Loading Progress: {loadingProgress * 100f:F}");

            if (showLoadingScreen) {
                float loadingBarProgress = loadingProgress / 0.9f;
                loadingScreen.SetBarProgress(loadingBarProgress, true);
            }

            yield return null;
        }
        
        foreach (AsyncOperation loadingOperation in loadingOperationList) {
            loadingOperation.allowSceneActivation = true;
        }
        
        while (loadingProgress < 1f) {
            loadingProgress = GetTotalLoadingProgress(loadingOperationList);
            // Debug.Log($"Init Progress: {loadingProgress * 100f:F}");

            yield return null;
        }

        ActivateLoadedScene(applicationScenes[0].sceneName);
        
        UnloadScenes();
        
        float unloadingProgress = 0f;
        while (unloadingProgress < 1f) {
            unloadingProgress = GetTotalLoadingProgress(unloadingOperationList);
            // Debug.Log($"Unloading Progress: {unloadingProgress * 100f:F}");

            yield return null;
        }
        
        yield return null;

        if (showLoadingScreen) {
            loadingScreen.Show(false, true);
            yield return new WaitForSeconds(loadingScreen.FadeScreenDuration);
        }

        loadingOperationList.Clear();
        unloadingOperationList.Clear();
        
        if (applicationEventRelay) {
            applicationEventRelay.LoadingDone();
        }

        loadScenesRoutine = null;
    }

    private void ReloadCurrentScenes(bool showLoadingScreen) {
        StartCoroutine(ReloadCurrentScenesRoutine(showLoadingScreen));
    }
    
    private IEnumerator ReloadCurrentScenesRoutine(bool showLoadingScreen) {
        if (showLoadingScreen) {
            loadingScreen.Show(true, true);
            yield return new WaitForSeconds(loadingScreen.FadeScreenDuration);
        }
        
        string[] sceneNamesToReload = SetScenesToUnload().ToArray();
        
        UnloadScenes();

        float unloadingProgress = 0f;
        while (unloadingProgress < 1f) {
            unloadingProgress = GetTotalLoadingProgress(unloadingOperationList);
            // Debug.Log($"Unloading Progress: {unloadingProgress * 100f:F}");

            yield return null;
        }

        for (int i = 0; i < sceneNamesToReload.Length; i++) {
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneNamesToReload[i], LoadSceneMode.Additive);
            loadingOperation.allowSceneActivation = false;
            
            loadingOperationList.Add(loadingOperation);
        }

        float loadingProgress = 0f;
        while (loadingProgress < 0.89f) {
            loadingProgress = GetTotalLoadingProgress(loadingOperationList);
            // Debug.Log($"Loading Progress: {loadingProgress * 100f:F}");

            if (showLoadingScreen) {
                float loadingBarProgress = loadingProgress / 0.9f;
                loadingScreen.SetBarProgress(loadingBarProgress, true);
            }

            yield return null;
        }

        foreach (AsyncOperation loadingOperation in loadingOperationList) {
            loadingOperation.allowSceneActivation = true;
        }

        while (loadingProgress < 1f) {
            loadingProgress = GetTotalLoadingProgress(loadingOperationList);
            // Debug.Log($"Init Progress: {loadingProgress * 100f:F}");

            yield return null;
        }

        ActivateLoadedScene(sceneNamesToReload[0]);
        
        if (showLoadingScreen) {
            loadingScreen.Show(false, true);
            yield return new WaitForSeconds(loadingScreen.FadeScreenDuration);
        }

        unloadingOperationList.Clear();
        loadingOperationList.Clear();
        
        if (applicationEventRelay) {
            applicationEventRelay.LoadingDone();
        }
    }

    private void ActivateLoadedScene(string sceneName) {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (!loadedScene.isLoaded) return;

        SceneManager.SetActiveScene(loadedScene);
    }

    private List<string> SetScenesToUnload() {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            string loadedSceneName = SceneManager.GetSceneAt(i).name;
            if (loadedSceneName == persistantScene.sceneName) continue;

            scenesToUnload.Add(loadedSceneName);
        }

        return scenesToUnload;
    }

    private void UnloadScenes() {
        if (scenesToUnload == null || scenesToUnload.Count == 0) return;

        for (int i = scenesToUnload.Count - 1; i >= 0; i--) {
            string unloadSceneName = scenesToUnload[i];
            unloadingOperationList.Add(SceneManager.UnloadSceneAsync(unloadSceneName));
        }

        scenesToUnload.Clear();
    }
    
    private float GetTotalLoadingProgress(List<AsyncOperation> asyncOperationList) {
        if (asyncOperationList == null || asyncOperationList.Count == 0) return 1f;

        float progressSum = 0f;

        foreach (AsyncOperation asyncOperation in asyncOperationList) {
            if (asyncOperation == null) continue;
            progressSum += asyncOperation.progress;
        }

        return progressSum / asyncOperationList.Count;
    }
}