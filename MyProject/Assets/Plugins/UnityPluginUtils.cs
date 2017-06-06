using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class UnityPluginUtils : AndroidBehaviour<UnityPluginUtils>
{
    [DllImport("__Internal")]
    private static extern void ShowToast();

    protected override string javaClassName
    {
        get { return "com.maxx.Utils"; }
    }

    public static string GetMacAddr()
    {
        string macAddr = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        macAddr = instance.CallStatic<string>("getMac");
#elif UNITY_IPHONE && !UNITY_EDITOR
      
#endif

        return macAddr;
    }

    public static void ShowToast(string message, bool isLong)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
         instance.CallStatic("ShowToast", message, isLong);
#elif UNITY_IPHONE && !UNITY_EDITOR
        ShowToast();
#endif

    }
}