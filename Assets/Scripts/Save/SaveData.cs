using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int coins;
    public int gems;
    public int distance; // longest run record
    public int flags;
    public string[] owned; // string array of unique item strings which have been bought
    public DateTime lastAdRewards;
    public DateTime lastFbRewards;

    public SaveData(int coins, int gems, int distance, int flags, string[] owned)
    {
        this.coins = coins;
        this.gems = gems;
        this.distance = distance;
        this.owned = owned;
        this.flags = flags;
        lastAdRewards = DateTime.Now.Subtract(new TimeSpan(0, 0, GlobalVars.DAILYDELAY));
        lastFbRewards = lastAdRewards;
    }

    public void SetFlag(int flag)
    {
        flags = flags | flag;
    }
    public void ClearFlag(int flag)
    {
        flags = flags & ~flag;
    }

    public bool GetFlag(int flag)
    {
        return ((flags & flag) != 0);
    }

    public void DEBUGWIPE()
    {
        coins = 0;
        gems = 0;
        distance = 0;
        owned = new string[0];
        flags = 0;
        lastAdRewards = DateTime.Now.Subtract(new TimeSpan(0, 0, GlobalVars.DAILYDELAY));
        lastFbRewards = lastAdRewards;
    }
}
