/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SamplesMainMenu : MonoBehaviour
{

	#region PUBLIC_MEMBERS

    public enum MenuItem
    {
        Coloring3D = 0,
        DrawPrimitive,
        DrawPolygon,
        ImageClip,
        Glow,
    }

    public Canvas AboutCanvas;
    public Text AboutTitle;
    public Text AboutDescription;

    public static bool isAboutScreenVisible = false;

    // initialize static enum with one of the items
    public static MenuItem menuItem = MenuItem.Coloring3D;

    public const string MenuScene = "Menu";
    public const string LoadingScene = "Loading";

    SamplesAboutScreenInfo aboutScreenInfo;

	#endregion // PUBLIC_MEMBERS

    void Start()
    {
        // about screen is hidden when scene reloaded
        // set about screen state to false for nav handler
        isAboutScreenVisible = false;

        if (aboutScreenInfo == null)
        {
            // initialize if null
            aboutScreenInfo = new SamplesAboutScreenInfo();
        }
    }

	#region PUBLIC_METHODS

    public static string GetSceneToLoad()
    {
        return SamplesMainMenu.menuItem.ToString();
    }

    public static void LoadScene(string scene)
    {
        #if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        Application.LoadLevel(scene);
        #else // UNITY_5_3 or above
		UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        #endif
    }

    public void LoadAboutScene(string itemSelected)
    {
        // This method called from list of Sample App menu buttons
        switch (itemSelected)
        {

            case ("Coloring3D"): SamplesMainMenu.menuItem = SamplesMainMenu.MenuItem.Coloring3D; break;
            case ("DrawPrimitive"): SamplesMainMenu.menuItem = SamplesMainMenu.MenuItem.DrawPrimitive; break;
        }

        AboutTitle.text = aboutScreenInfo.GetTitle(SamplesMainMenu.menuItem.ToString());
        AboutDescription.text = aboutScreenInfo.GetDescription(SamplesMainMenu.menuItem.ToString());

        AboutCanvas.transform.parent.transform.position = Vector3.zero; // move canvas into position
        AboutCanvas.sortingOrder = 1; // bring canvas in front of main menu
        isAboutScreenVisible = true;

    }

	#endregion // PUBLIC_METHODS

}
