using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Yyb;

public class LoginManager : SingleTon<LoginManager>
{
    public bool isWaitLogin = false;
    public UnityEngine.UI.InputField nicknameField = null;

    private LoginRequest.Types.LOGIN_TYPE loginType_ = 0;
    private string loginKey_ = "";
    private string nickName_ = "";
    private int usn_ = 0;

    private const string PREFIX_PREFS = "yyb_";
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        //if (Application.platform == RuntimePlatform.Android)
        {
            PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
                .RequestIdToken()
                .RequestServerAuthCode(false)
                .Build());
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        LoadPlayerPrefs();
        DoAutoLogin();
    }

    public void LoadPlayerPrefs()
    {
        loginKey_ = PlayerPrefs.GetString(PREFIX_PREFS + "login_key", "");
        loginType_ = (LoginRequest.Types.LOGIN_TYPE)PlayerPrefs.GetInt(
            PREFIX_PREFS + "login_type",
            (int)LoginRequest.Types.LOGIN_TYPE.NonCert);
        nickName_ = PlayerPrefs.GetString(PREFIX_PREFS + "nick_name", "");

        nicknameField.text = nickName_;
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetString(PREFIX_PREFS + "login_key", loginKey_);
        PlayerPrefs.SetInt(PREFIX_PREFS + "login_type", (int)loginType_);
        PlayerPrefs.SetString(PREFIX_PREFS + "nick_name", nickName_);
    }

    public void DoAutoLogin()
    {
        if (loginKey_.Equals(""))
        {
            return;
        }

        if (LoginRequest.Types.LOGIN_TYPE.NonCert == loginType_)
        {
            NonCertLogin();
        }
        else if (LoginRequest.Types.LOGIN_TYPE.Google == loginType_)
        {
            GoogleLogin();
        }
    }

    public void NonCertLogin()
    {
        //int usn = PlayerPrefs.GetInt("usn", 0);
        //if (nickName_.Equals(""))
        {
            nickName_ = nicknameField.text == null ? "" : nicknameField.text;
        }

        loginType_ = LoginRequest.Types.LOGIN_TYPE.NonCert;

        RpcLogin(loginType_, loginKey_, nickName_, "");
    }

    public void GoogleLogin()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool bSuccess) =>
            {
                if (bSuccess)
                {
                    Debug.Log("Login : " + Social.localUser.userName);

                    //if (nickName_.Equals(""))
                    {
                        nickName_ = nicknameField.text == null ? "" : nicknameField.text;
                    }

                    loginType_ = LoginRequest.Types.LOGIN_TYPE.Google;

                    string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken() == null ?
                    "" : ((PlayGamesLocalUser)Social.localUser).GetIdToken();

                    RpcLogin(loginType_, loginKey_, nickName_, idToken);
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

            if (LoginReply.Types.ERROR_CODE.Ok == reply.Error)
            {
                Debug.Log("Ok");

                //private LoginRequest.Types.LOGIN_TYPE loginType_ = 0;
                loginKey_ = reply.LoginKey;
                nickName_ = reply.NickName;
                usn_ = reply.Usn;

                SavePlayerPrefs();
            }
            else if (LoginReply.Types.ERROR_CODE.EmptyNickname == reply.Error)
            {
                Debug.Log("EmptyNickname");
            }
            else if (LoginReply.Types.ERROR_CODE.DupNickname == reply.Error)
            {
                Debug.Log("DupNickname");
            }
            else if (LoginReply.Types.ERROR_CODE.NicknameHaveSpecialCharacters == reply.Error)
            {
                Debug.Log("NicknameHaveSpecialCharacters");
            }
            else if (LoginReply.Types.ERROR_CODE.UnableToCreateUser == reply.Error)
            {
                Debug.Log("UnableToCreateUser");
            }
            else if (LoginReply.Types.ERROR_CODE.FailedToAcquireUserInfo == reply.Error)
            {
                Debug.Log("FailedToAcquireUserInfo");
            }
            else if (LoginReply.Types.ERROR_CODE.GoogleAuthFailed == reply.Error)
            {
                Debug.Log("GoogleAuthFailed");
            }
            else
            {
                Debug.Log("Unkown error code");
            }
        });
    }

    public void OnLogout()
    {
        PlayGamesPlatform.Instance.SignOut();

        Debug.Log("Logout : " + Social.localUser.userName);
    }
}
