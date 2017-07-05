using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    #region PRIVATE_METHODS
    private IEnumerator LoadNextSceneAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        Application.LoadLevel(Application.loadedLevel + 1);
#else
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
#endif
    }
    #endregion //PRIVATE_METHODS

    public void PlaySound(AudioClip ac, bool loop)
    {
        this.GetComponent<AudioSource>().clip = ac;
        this.GetComponent<AudioSource>().Play();
        this.GetComponent<AudioSource>().loop = loop;
    }
}
