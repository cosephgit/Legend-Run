using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// general methods used by all menus
// created 31/8/23
// last modified 31/8/23

public class UIMainMenu : MonoBehaviour
{
    [Header("General menu references")]
    [SerializeField] protected GameObject menuMain;
    [SerializeField] protected UIOptionsMenu menuOptions;
    [SerializeField] private AudioClip audioButton;
    [SerializeField] private AudioClip menuMusic;
    private bool audioReady;

    private void Awake()
    {
        ButtonBack();
    }

    private void Start()
    {
        if (menuMusic)
            AudioManager.instance.MusicPlay(menuMusic);
        audioReady = true;
    }

    public void SoundButton()
    {
        if (audioReady)
        {
            AudioManager.instance.SoundPlayEven(audioButton, Vector2.zero);
        }
    }

    public void ButtonPlay()
    {
        SoundButton();
        SceneManager.LoadScene(1);
    }
    public virtual void ButtonOptions()
    {
        SoundButton();
        menuMain.SetActive(false);
        menuOptions.gameObject.SetActive(true);
    }
    public virtual void ButtonQuit()
    {
        SoundButton();
        Application.Quit();
        Debug.Log("quit");
    }
    public virtual void ButtonBack()
    {
        SoundButton();
        menuMain.SetActive(true);
        menuOptions.gameObject.SetActive(false);
    }
}
