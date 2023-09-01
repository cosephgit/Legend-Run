using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// manages scene transitions, load/save, etc
// created 22/8/23
// last modified 22/8/23


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private AudioMixer audioMixer;
    [Header("Karmic balance")]
    [SerializeField] private float diffKarmaBalance = 0.2f;
    public float volBGM { get; private set; }
    public float volSFX { get; private set; }
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

        SetVolumeBGM(1f);
        SetVolumeSFX(1f);
    }

    public void SetVolumeBGM(float volume)
    {
        volBGM = volume;
        audioMixer.SetFloat("volumeBGM", CoSephUtils.VolumeToDecibels(volBGM));
    }

    public void SetVolumeSFX(float volume)
    {
        volSFX = volume;
        audioMixer.SetFloat("volumeSFX", CoSephUtils.VolumeToDecibels(volSFX));
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
}
