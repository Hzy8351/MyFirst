#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor
{

    [MenuItem("Open Scene/Init")]
    public static void OpenInit()
    {
        OpenScene("Init");
    }
    [MenuItem("Open Scene/Home")]
    public static void OpenHome()
    {
        OpenScene("Home");
    }
    [MenuItem("Open Scene/Game")]
    public static void OpenGame()
    {
        OpenScene("Game");
    }
    private static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
        }
    }
}
#endif