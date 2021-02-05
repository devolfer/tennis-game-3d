using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorInitPersistantScene : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private ApplicationScene persistantScene;
    [SerializeField] private ApplicationScene[] additionalScenesToLoad;

    private IEnumerator Start() {
        if (SceneIsLoaded(persistantScene)) yield break;

        float loadingProgress = 0;
        List<AsyncOperation> loadOperations = new List<AsyncOperation> {
            SceneManager.LoadSceneAsync(persistantScene.sceneName, LoadSceneMode.Additive),
        };
        foreach (ApplicationScene applicationScene in additionalScenesToLoad) {
            if (SceneIsLoaded(applicationScene)) continue;
            loadOperations.Add(SceneManager.LoadSceneAsync(applicationScene.sceneName, LoadSceneMode.Additive));
        }
        
        while (loadingProgress < 1f) {
            loadingProgress = GetTotalLoadingProgress(loadOperations);
            yield return null;
        }
        
        if (applicationEventRelay) applicationEventRelay.LoadingDone();
    }
    
    private float GetTotalLoadingProgress(List<AsyncOperation> asyncOperationList) {
        if (asyncOperationList == null || asyncOperationList.Count == 0) return 1f;

        float progressSum = 0f;

        foreach (AsyncOperation asyncOperation in asyncOperationList) {
            progressSum += asyncOperation.progress;
        }

        return progressSum / asyncOperationList.Count;
    }

    private bool SceneIsLoaded(ApplicationScene appScene) {
        for (int i = 0; i < SceneManager.sceneCount; ++i) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == appScene.sceneName) {
                return true;
            }
        }

        return false;
    }
}