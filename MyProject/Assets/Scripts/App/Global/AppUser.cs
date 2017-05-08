using UnityEngine;
using System.Collections;
using System;
using LitJson;
using System.Collections.Generic;

public class AppUser : MonoBehaviour
{
    private string userName_ = "";
    private string userId_ = "";
    private AppData appData_ = new AppData();
    private JsonData appConfig_ = new JsonData();
    private JsonData gameConfig_ = new JsonData();

    private static AppUser instance;
    public static AppUser Instance
    {
        get
        {
            if (instance) return instance;

            instance = GameObject.FindObjectOfType<AppUser>();
            if (instance) return instance;

            GameObject obj = new GameObject(typeof(AppUser).ToString());
            //切场景不消除
            GameObject.DontDestroyOnLoad(obj);
            instance = obj.AddComponent<AppUser>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void OnApplicationFocus(bool focusStatus)
    {
        //Debug.Log("OnApplicationFocus" + focusStatus);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("OnApplicationPause " + pauseStatus);
        if (pauseStatus)
        {
            OnPause();
        }
        else
        {
            OnResume();
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    public void SetAppConfig(string key, string value,bool save = false)
    {
        appConfig_[key] = value;
        if (save)
        {
            OnSaveArchive();
        }
    }

    public string GetAppConfig(string key)
    {
        return appConfig_[key].ToString();
    }

    public void UpdateGold(int gold, bool save)
    {
        //appData_.gold += gold;
        if (save)
        {
            OnSaveArchive();
        }
    }

    public void OnStart() {
        OnLoadArchive();

        //network
        //HttpService.GetAppConfig();
        //HttpService.GetPaidList();

        //Platform.CleanAllNotification();
        //Platform.ScheduleAction();

        //AppDef.Instance.ResetConfig();
    }

    public void OnResume() {
        OnLoadArchive();

        //HttpService.GetAppConfig();
        //HttpService.GetPaidList();

        //Platform.CleanAllNotification();
        //Platform.ScheduleAction();
    }

    public void OnPause() {
        //this.leavelAppTime = Utils.millisecondNow();
        //Platform.AddNotification();

        OnSaveArchive();
    }

    private void OnInitArchive() {
        userId_ = "100001";
        userName_ = "user001";

        appConfig_[GlobalDef.kAppVer] = GlobalDef.AppVer;
        appConfig_[GlobalDef.kMusic] = "1";
        appConfig_[GlobalDef.kSound] = "1";
        appConfig_[GlobalDef.kLocalized] = "zh";

        OnSaveArchive();
    }

    private void OnUpdateArchive()
    {

    }

    private void OnLoadArchive() {
        //bool ret = Utils.IsExistFile(Utils.GetWritePath(),GlobalDef.USER_FILE_NAME);
        //if (ret)
        //{
        //    var strArchive = Utils.LoadFile(Utils.GetWritePath(), GlobalDef.USER_FILE_NAME);
        //    OnDecode(strArchive);

        //    var appVer = GetAppConfig(GlobalDef.kAppVer);
        //    if (int.Parse(appVer) < int.Parse(GlobalDef.AppVer))
        //    {
        //        OnUpdateArchive();
        //    }
        //}
        //else
        //{
        //    OnInitArchive();
        //}
    }

    private void OnSaveArchive() {
        //var jsonArchive = this.OnEncode();
        //var strArchive = JsonMapper.ToJson(jsonArchive);

        ////cryptArchive = Utils.encrypt(strArchive);
        //Utils.DeleteFile(Utils.GetWritePath(), GlobalDef.USER_FILE_NAME);
        //Utils.CreateORwriteConfigFile(Utils.GetWritePath(), GlobalDef.USER_FILE_NAME,strArchive);
    }

    private JsonData OnEncode() {
        JsonData user = new JsonData();

        user["userName"] = userName_;
        user["userId"] = userId_;

        JsonData archive = new JsonData();
        archive["user"] = user;
        archive["appData"] = appData_.OnEncode();
        archive["appConfig"] = appConfig_;
        archive["gameConfig"] = gameConfig_;

        Console.WriteLine("");
        return archive;
    }

    private void OnDecode(string archiveJson)
    {
        JsonData archive = JsonMapper.ToObject(archiveJson);

        JsonData user = archive["user"];
        userId_ = user["userId"].ToString();
        userName_ = user["userName"].ToString();

        appData_.OnDecode(archive["appData"]);
        appConfig_ = archive["appConfig"];
        gameConfig_ = archive["gameConfig"];

        Console.WriteLine("");
    }
}

public class AppData {
    private int bestScore;
    private int gold;

    private int currentScore;
    private int currentGold;


    public JsonData OnEncode()
    {
        JsonData data = new JsonData();

        data["bestScore"] = bestScore;
        data["gold"] = gold;

        data["currentScore"] = currentScore;
        data["currentGold"] = currentGold;

        return data;
    }

    public void OnDecode(JsonData data)
    {
        bestScore = int.Parse(data["bestScore"].ToString());
        gold = int.Parse(data["gold"].ToString());

        currentScore = int.Parse(data["currentScore"].ToString());
        currentGold = int.Parse(data["currentGold"].ToString());
    }
}
