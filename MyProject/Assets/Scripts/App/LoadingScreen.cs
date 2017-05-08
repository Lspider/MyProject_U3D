using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour 
{
    #region PRIVATE_MEMBER_VARIABLES
    private bool mChangeLevel = true;
    private RawImage mUISpinner;
    #endregion // PRIVATE_MEMBER_VARIABLES


    #region MONOBEHAVIOUR_METHODS
    void Start () 
    {
        mUISpinner = FindSpinnerImage();
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        mChangeLevel = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        ResourceManager.loadResouce += LoadResource;
        ResourceManager.Instance.LoadAsset<Object>("Scenes/GameScene");
#else
        StartCoroutine(AsyncLoad(1.0f));
#endif
    }

    void Update () 
    {
        mUISpinner.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);

        if (mChangeLevel)
        {
            //LoadNextSceneAsync();
            mChangeLevel = false;
        }
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS
    private void LoadNextSceneAsync()
    {
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        Application.LoadLevelAsync(Application.loadedLevel+1);
#else // UNITY_5_3 or above
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex+1);
#endif
    }

    private RawImage FindSpinnerImage()
    {
        RawImage[] images = FindObjectsOfType<RawImage>();
        foreach (var img in images)
        {
            if (img.name.Contains("Spinner"))
            {
                return img;
            }
        }
        return null;
    }
    #endregion // PRIVATE_METHODS

    void OnDisable()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        ResourceManager.loadResouce -= LoadResource;
#endif
    }

    IEnumerator AsyncLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResourceManager.Instance.LoadAsset<Object>("Scenes/Main");
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        Application.LoadLevel("GameScene");
#else
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
#endif
    }

    public void LoadResource(List<string> pathList)
    {
        StartCoroutine(LoadAssetBundle(pathList, 0));
    }

    IEnumerator LoadAssetBundle(List<string> pathList, int index)
    {
        WWW www = WWW.LoadFromCacheOrDownload(pathList[index], 0);
        yield return www;

        if (www.error == null)
        {
            if (index < pathList.Count - 1)
            {
                ++index;
                StartCoroutine(LoadAssetBundle(pathList, index));
            }
            else
            {
                AssetBundle ab = www.assetBundle;
                AsyncOperation asy = SceneManager.LoadSceneAsync("GameScene");
                yield return asy;
            }
        }
        else
        {
            Debug.LogError(www.error);
        }
    }
}
