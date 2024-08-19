using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GlobalVars
{
    public const string SAVEVOLUMEBGM = "VolBGM";
    public const string SAVEVOLUMESFX = "VolSFX";
    public const string SAVECOINS = "Coins";
    public const string SAVEGEMS = "Gems";
    public const string SAVEDISTANCE = "Distance";
    public const string SAVETUTORIAL = "Tutorial";
    public const string SAVESETTINGS = "Settings";
    public const string SAVEPROGRESS = "Progress";
    // save data flags
    public const int SAVEFLAGTUTORIAL = 1; // 1
    public const int SAVEFLAGCOINS = 1 << 1; // 2
    public const int SAVEFLAGGEMS = 1 << 2; // 4
    public const int SAVEFLAGDISTANCE = 1 << 3; // 8

    // standardised strings for displaying certain kinds of values
    public static string DisplayCoins(int coins)
    {
        if (coins > 999999)
            return coins.ToString("E2");
        else
            return coins.ToString("N0");
    }

    public static string DisplayGems(int gems)
    {
        if (gems > 999999)
            return gems.ToString("E2");
        else
            return gems.ToString("N0");
    }
    // premium expenses are stored in cents/pennies, expressed in dollars/pounds
    public static string DisplayPremium(int cost)
    {
        return ("$" + (cost / 100f).ToString("F2"));
    }

    public static string DisplayDistance(int distance)
    {
        if (distance < 1000)
        {
            return distance.ToString("N0") + " m";
        }
        else if (distance < 1000000)
        {
            // you're not going to get here, but just in case
            return (Mathf.Floor(distance / 100f) / 10f).ToString("N2") + " km";
        }
        else
        {
            return (Mathf.Floor(distance / 10f) / 100f).ToString("E2") + " km";
        }
    }

    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
