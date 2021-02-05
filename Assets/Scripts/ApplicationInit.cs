using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationInit : MonoBehaviour {
    [SerializeField] private ApplicationEventRelay applicationEventRelay;
    [SerializeField] private ApplicationScene persistantScene;

    private void Start() {
        if (SceneManager.GetActiveScene().name != persistantScene.sceneName) return;
        
        // Debug.Log("Application Init");
        if (applicationEventRelay) applicationEventRelay.StartApplication();
    }
}