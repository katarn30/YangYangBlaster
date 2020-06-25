using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Yyb;
using Grpc.Health.V1;

public class LoginManager : SingleTon<LoginManager>
{
    public bool isWaitLogin = false;
    public UnityEngine.UI.InputField nicknameField = null;

    //private LoginRequest.Types.LOGIN_TYPE loginType_ = 0;
    //private string loginKey_ = "";
    //private string accessKey_ = "";
    //private string nickName_ = "";
    //private int usn_ = 0;

    //private const string PREFIX_PREFS = "yyb_";
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        #if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
                .RequestIdToken()
                .RequestServerAuthCode(false)
                .Build());
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }
        #endif
        //DoAutoLogin();
    }

    //public void LoadPlayerPrefs()
    //{
    //    loginKey_ = PlayerPrefs.GetString(PREFIX_PREFS + "login_key", "");
    //    loginType_ = (LoginRequest.Types.LOGIN_TYPE)PlayerPrefs.GetInt(
    //        PREFIX_PREFS + "login_type",
    //        (int)LoginRequest.Types.LOGIN_TYPE.NonCert);
    //    nickName_ = PlayerPrefs.GetString(PREFIX_PREFS + "nick_name", "");
    //    GameDataManager.Instance.userData.nickName = 
    //        PlayerPrefs.GetString(PREFIX_PREFS + "nick_name", "");

    //    //nicknameField.text = nickName_;
    //}

    //public void SavePlayerPrefs()
    //{
    //    PlayerPrefs.SetString(PREFIX_PREFS + "login_key", loginKey_);
    //    PlayerPrefs.SetInt(PREFIX_PREFS + "login_type", (int)loginType_);
    //    PlayerPrefs.SetString(PREFIX_PREFS + "nick_name", nickName_);
    //}

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void DoAutoLogin()
    {
        GameDataManager.Instance.LoadUserDataLoginParts();

        var loginType = GameDataManager.Instance.userData.loginType;
        var loginKey = GameDataManager.Instance.userData.loginKey;

        if (LoginRequest.Types.LOGIN_TYPE.Google == loginType)
        {
            if (loginKey.Equals(""))
            {
                NonCertLogin();
                return;
            }
#if UNITY_ANDROID
            GoogleLogin();
#endif
        }
        else if (LoginRequest.Types.LOGIN_TYPE.Facebook == loginType)
        {
            NonCertLogin();
        }
        else
        {
            NonCertLogin();
        }
    }

    public void NonCertLogin()
    {
        GameDataManager.Instance.userData.loginType = LoginRequest.Types.LOGIN_TYPE.NonCert;

        var loginType = GameDataManager.Instance.userData.loginType;
        var loginKey = GameDataManager.Instance.userData.loginKey;
        var nickName = GameDataManager.Instance.userData.nickName;
        
        RpcLogin(loginType, loginKey, nickName, "");
    }

#if UNITY_ANDROID
    public void GoogleLogin()
    {
        GameDataManager.Instance.userData.loginType = LoginRequest.Types.LOGIN_TYPE.Google;

        var loginType = GameDataManager.Instance.userData.loginType;
        var loginKey = GameDataManager.Instance.userData.loginKey;
        var nickName = GameDataManager.Instance.userData.nickName;

        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool bSuccess) =>
            {
                if (bSuccess)
                {
                    Debug.Log("Login : " + Social.localUser.userName);

                    string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken() == null ?
                    "" : ((PlayGamesLocalUser)Social.localUser).GetIdToken();

                    RpcLogin(loginType, loginKey, nickName, idToken);
                }
                else
                {
                    Debug.Log("Fail");
                }
            });
        }
        else
        {
            Debug.Log("You already logged in");
        }
    }
#endif

    public void RpcLogin(LoginRequest.Types.LOGIN_TYPE loginType,
        string loginKey, string nickName, string idToken)
    {
        // 로그인
        LoginRequest request = new LoginRequest();
        request.LoginType = loginType;
        //request.Usn = usn;//Social.localUser.userName == null ? "null" : Social.localUser.userName;
        request.LoginKey = loginKey;
        request.NickName = nickName;
        request.IdToken = idToken;

        // 요청
        RpcServiceManager.Instance.Login(request, (LoginReply reply) =>
        {
            // 응답
            Debug.Log("LoginReply : " + reply.ToString());

            if (ERROR_CODE.Ok == reply.Error)
            {
                Debug.Log("Ok");

                GameDataManager.Instance.userData.loginKey = reply.LoginKey;
                GameDataManager.Instance.userData.accessKey = reply.AccessKey;
                GameDataManager.Instance.userData.nickName = reply.NickName;
                //usn_ = reply.Usn;

                GameDataManager.Instance.SaveUserDataLoginParts();
                GameDataManager.Instance.LoadGameData();

                // 헬스체크 시작
                HealthCheckRequest healthCheckRequest = new HealthCheckRequest();
                healthCheckRequest.Service = "yyb";

                // 요청
                RpcServiceManager.Instance.Check(healthCheckRequest, 
                    (HealthCheckResponse HealthCheckReply) =>
                {
                    // 응답
                    Debug.Log("Check Response : " + HealthCheckReply.ToString());
                });

                // 요청
                RpcServiceManager.Instance.Watch(healthCheckRequest, 
                    (HealthCheckResponse HealthCheckReply) =>
                {
                    // 응답
                    Debug.Log("Watch Response : " + HealthCheckReply.ToString());
                });
            }
            else
            {
                Debug.Log(reply.Error);

                DeletePlayerPrefs();
            }
        });
    }

    public void OnLogout()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.SignOut();
#endif
        Debug.Log("Logout : " + Social.localUser.userName);
    }

}
