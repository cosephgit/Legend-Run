using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIHealthBar
// updates the player's health bar
// created 22/8/23
// last modified 7/9/23


public class UIHealthBar : UIBase
{
    [SerializeField] private Slider healthFadeSlider; // the fade to show health loss
    [SerializeField] private Slider healthActualSlider; // the actual health level
    [SerializeField] private Slider healthFillSlider; // the fill slider to show health gain
    [SerializeField] private float healthFadePerSecond = 1f; // how quickly the health fade moves
    [SerializeField] private bool DEBUGPRESENTATIONMODE = false;
    private float healthFade;
    private float healthActual;
    private float healthFill;

    public void InitialiseHealth()
    {
        healthFill = 1;
        healthFillSlider.value = 1;
        healthFade = 1;
        healthFadeSlider.value = 1;
        healthActual = 1;
        healthActualSlider.value = 1;
    }

    public void SetHealth(float health, bool forcefill = false)
    {
        if (DEBUGPRESENTATIONMODE) return;

        if (health == healthActual) return;
        gameObject.SetActive(true);

        if (forcefill || health > healthActual)
        {
            healthFade = health;
            healthFadeSlider.value = health;
        }
        if (forcefill || health < healthActual)
        {
            healthFill = health;
            healthFillSlider.value = health;
        }
        healthActual = health;
        healthActualSlider.value = health;
        if (!forcefill) AddShake(4f);
    }

    protected override void Update()
    {
        base.Update();
        if (healthFade > healthActual)
        {
            healthFade = Mathf.Max(healthFade - (Time.unscaledDeltaTime * healthFadePerSecond), healthActual);
            healthFadeSlider.value = healthFade;
        }
        if (healthFill < healthActual)
        {
            healthFill = Mathf.Min(healthActual, healthFill + (Time.unscaledDeltaTime * healthFadePerSecond));
            healthFillSlider.value = healthFill;
        }
    }
}
