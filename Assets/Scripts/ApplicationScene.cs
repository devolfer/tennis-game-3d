using UnityEngine;

[CreateAssetMenu(fileName = "ApplicationScene", menuName = "Scriptable Object/Scenes/Application Scene")]
public class ApplicationScene : ScriptableObject {
    public string sceneName;
    [TextArea] public string description;
}