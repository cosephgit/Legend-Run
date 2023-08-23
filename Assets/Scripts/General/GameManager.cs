using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages scene transitions, load/save, etc
// created 22/8/23
// last modified 22/8/23


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
    }
}
