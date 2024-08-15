using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SettingData
// class used to contain save data for loading and saving through ES3
// created 30/7/24
// modified 30/7/24

public class SettingData
{
    public float volumeMaster;
    public float volumeBGM;
    public float volumeSFX;

    public SettingData(float volumeMaster, float volumeBGM, float volumeSFX)
    {
        this.volumeMaster = volumeMaster;
        this.volumeBGM = volumeBGM;
        this.volumeSFX = volumeSFX;
    }
}
