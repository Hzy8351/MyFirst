using UnityEditor;

[InitializeOnLoad]
public class GlobalConfig
{
    static GlobalConfig()
    {
        PlayerSettings.Android.keystorePass = "nab123456";
        //Fish
        PlayerSettings.Android.keyaliasName = "nab";
        PlayerSettings.Android.keyaliasPass = "nab123456";
    }
}