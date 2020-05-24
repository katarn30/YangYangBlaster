using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class LoginManager : SingleTon<LoginManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Application.platform == RuntimePlatform.Android)
        {
            PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }        
    }

    public void GoogleLogin()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool bSuccess) =>
            {
                if (bSuccess)
                {
                    Debug.Log("Success : " + Social.localUser.userName);                    
                }
                else
                {
                    Debug.Log("Fall");                    
                }
            });
        }
    }
}
