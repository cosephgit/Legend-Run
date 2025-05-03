using UnityEngine;
using UnityEngine.UI;

// UIRewardButton
// manages a button which is subject to daily rewards
// created 3/5/25
// modified 3/5/25

public class UIRewardButton : MonoBehaviour
{
    [SerializeField] private Button rewardButton;
    [SerializeField] private Transform sparkleSpinner;

    public void SetEnabled()
    {
        rewardButton.interactable = true;
    }
    public void SetDisabled()
    {
        rewardButton.interactable = false;
    }
}
