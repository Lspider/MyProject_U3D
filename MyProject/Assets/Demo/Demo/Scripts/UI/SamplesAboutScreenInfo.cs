/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System.Collections.Generic;

public class SamplesAboutScreenInfo
{

	#region PRIVATE_MEMBERS

    private Dictionary<string, string> titles;
    private Dictionary<string, string> descriptions;

	#endregion // PRIVATE_MEMBERS


	#region PUBLIC_METHODS

    public string GetTitle(string titleKey)
    {
        return getValuefromDictionary(titles, titleKey);
    }

    public string GetDescription(string descriptionKey)
    {
        return getValuefromDictionary(descriptions, descriptionKey);
    }
	
	#endregion // PUBLIC_METHODS


	#region PRIVATE_METHODS

    private string getValuefromDictionary(Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            string value;
            dictionary.TryGetValue(key, out value);
            return value;
        }
        else
        {
            return "Key not found.";
        }
    }

	#endregion // PRIVATE_METHODS


	#region CONSTRUCTOR

    public SamplesAboutScreenInfo()
    {

        // Init our Title Strings

        titles = new Dictionary<string, string>();

        titles.Add("Coloring3D", "Coloring 3D");

        // Init our Common Cache Strings

        string keyFunctionality = "<size=26>Key Functionality:</size>";
        string targets = "<size=26>Targets:</size>";
        string instructions = "<size=26>Instructions:</size>";
        //string baseurl = "http://www.le-more.com";
        string footer = 
			"<size=26>Terms of Use:</size>\n" +
            "http://www.le-more.com" +
            "\n\n" +
            "© 2016 Maxx. All Rights Reserved.";

        // Init our Description Strings

        descriptions = new Dictionary<string, string>();

        // Coloring3D

        descriptions.Add("Coloring3D",
            "\nThe Coloring 3D sample shows how to detect an image " +
            "target and render a simple 3D object on top of it.\n" +
            "\n" +
            keyFunctionality + "\n" +
            "• Simultaneous detection and tracking of multiple targets\n" +
            "• Load and activate multiple device databases\n" +
            "• Activate Extended Tracking\n" +
            "• Manage camera functions: flash and continuous autofocus\n" +
            "\n" +
            targets + "\n" +
            "\n" +
            instructions + "\n" +
            "• Point camera at target to view\n" +
            "• Single tap to focus\n" +
            "• Double tap to access options menu\n" +
            "\n" +
            footer + "\n");

    }

	#endregion // CONSTRUCTOR

}
