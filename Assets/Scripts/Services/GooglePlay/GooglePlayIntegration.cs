using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GooglePlayIntegration : MonoBehaviour
{
    private bool initComplete = false;

    public void Initialise()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        initComplete = false;
    }

    public bool IsInitComplete()
    {
        return initComplete;
    }
    
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            Debug.Log("GooglePlayIntegration: sign-in success!");
            initComplete = true;
            UIMainMenu.instance.LoginComplete(true);
        }
        else
        {
            Debug.LogError("GooglePlayIntegration: sign-in failed!");
            // TODO probably kill the app at this point? should always be able to! but what about offline play?

            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            initComplete = false;
            UIMainMenu.instance.LoginComplete(false);
        }
    }
}
