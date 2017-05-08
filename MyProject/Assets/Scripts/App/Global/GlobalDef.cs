using UnityEngine;
using System.Collections;

public class GlobalDef
{
    public static string AppVer = "100000";
    public static string AppId = "123456789";
    public static string AppName = "AppDemo";
    public static string Host = "server.xxx.com";
    public static string ServerAddr = "http://" + Host + "/server/Handle.ashx";

    public static string DeviceId = "";
    //ads config

    //app config
    public const string USER_FILE_NAME = "UsAf";
    public const string kAppVer = "kAppVer";
    public const string kLocalized = "kLocalized";

    public const string kMusic = "kMusic";
    public const string kSound = "kSound";

    private static GlobalDef instanace;
    public static GlobalDef Instance
    {
        get
        {
            if (instanace == null)
            {
                instanace = new GlobalDef();
            }

            return instanace;
        }
    }

    public GlobalDef()
    {
    }
}
