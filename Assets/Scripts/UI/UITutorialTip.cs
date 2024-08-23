using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UITutorialFinger
// manages the finger which shows swiping motions
// created 21/8/24
// modified 21/8/24

public class UITutorialTip : MonoBehaviour
{
    [field: SerializeField] public Transform swipeTipStart { get; private set; }
    [field: SerializeField] public Transform swipeTipEnd { get; private set; }
    [field: SerializeField] public bool swipeOscillate { get; private set; }
}
