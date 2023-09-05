using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

// manages scene transitions, load/save, etc
// created 22/8/23
// last modified 5/9/23

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private const string SAVEVOLUMEBGM = "VolBGM";
    private const string SAVEVOLUMESFX = "VolSFX";
    private const string SAVECOINS = "Coins";
    private const string SAVEDISTANCE = "Distance";
    [SerializeField] private AudioMixer audioMixer;
    [Header("Karmic balance")]
    [SerializeField] private float diffKarmaBalance = 0.2f;
    public float volBGM { get; private set; }
    public float volSFX { get; private set; }
    public int coinsStash { get; private set; }
    public int distanceBest { get; private set; }
    // pathing difficulty balancer
    private float diffKarma = 0f;

    private void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else instance = this;

        SetVolumeBGM(PlayerPrefs.GetFloat(SAVEVOLUMEBGM, 1f), false);
        SetVolumeSFX(PlayerPrefs.GetFloat(SAVEVOLUMESFX, 1f), false);
        coinsStash = PlayerPrefs.GetInt(SAVECOINS, 0);
        distanceBest = PlayerPrefs.GetInt(SAVEDISTANCE, 0);
    }

    public void SetVolumeBGM(float volume, bool save = true)
    {
        volBGM = volume;
        audioMixer.SetFloat("volumeBGM", CoSephUtils.VolumeToDecibels(volBGM));
        if (save) PlayerPrefs.SetFloat(SAVEVOLUMEBGM, volBGM);
    }

    public void SetVolumeSFX(float volume, bool save = true)
    {
        volSFX = volume;
        audioMixer.SetFloat("volumeSFX", CoSephUtils.VolumeToDecibels(volSFX));
        if (save) PlayerPrefs.SetFloat(SAVEVOLUMESFX, volSFX);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    // add coins to the player's collection
    public void AddCoins(int coins)
    {
        coinsStash += coins;
        PlayerPrefs.SetInt(SAVECOINS, coinsStash);
    }

    // add a new distance high score
    public void AddDistance(int distance)
    {
        if (distance > distanceBest)
        {
            distanceBest = distance;
            PlayerPrefs.SetInt(SAVEDISTANCE, distanceBest);
        }
    }

    public bool KarmicChance(float chanceBase = 0.5f)
    {
        float chance = chanceBase + diffKarma;

        if (Random.Range(0f, 1f) < chance)
        {
            diffKarma -= (1f - chanceBase) * diffKarmaBalance;
            return true;
        }
        else
        {
            diffKarma += chanceBase * diffKarmaBalance;
            return false;
        }
    }

    // standardised strings for displaying certain kinds of values
    public string DisplayCoins(int coins)
    {
        if (coins > 999999)
            return coins.ToString("E2");
        else
            return coins.ToString("N0");
    }

    public string DisplayDistance(int distance)
    {
        if (distance < 1000)
        {
            return distance.ToString("N0") + " m";
        }
        else if (distance < 1000000)
        {
            // you're not going to get here, but just in case
            return (Mathf.Floor(distance / 10f) / 100f).ToString("N2") + " km";
        }
        else
        {
            return (Mathf.Floor(distance / 10f) / 100f).ToString("E2") + " km";
        }
    }
}
