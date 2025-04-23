using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages the options sub-menu
// created 31/8/23
// last modified 31/8/23

public class UIOptionsMenu : MonoBehaviour
{
    [Header("Menu sounds")]
    [SerializeField] private float audioSliderDelay = 0.1f;
    [Header("Objects references")]
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Slider sliderBGM;
    private UIMainMenu menuMain;
    private float audioSliderNextPip;
    private bool audioReady;

    // called by the owning menu to set it up
    public void Initialise(UIMainMenu menuMain)
    {
        this.menuMain = menuMain;
        sliderSFX.value = GameManager.instance.volSFX;
        sliderBGM.value = GameManager.instance.volBGM;
        audioReady = true;
    }

    private void SoundSlider()
    {
        if (audioReady)
        {
            if (audioSliderNextPip > 0) return;
            AudioManager.instance.SoundPlayMenuSliderPing();
            audioSliderNextPip = audioSliderDelay;
        }
    }

    public void SliderSFX(System.Single volume)
    {
        GameManager.instance.SetVolumeSFX(volume);
        SoundSlider();
    }
    public void SliderBGM(System.Single volume)
    {
        GameManager.instance.SetVolumeBGM(volume);
        SoundSlider();
    }

    private void Update()
    {
        if (audioSliderNextPip > 0)
        {
            audioSliderNextPip -= Time.unscaledDeltaTime;
        }
    }
}
