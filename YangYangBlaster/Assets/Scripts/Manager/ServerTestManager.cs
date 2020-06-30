using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Grpc.Core;
//using Grpc.Health.V1;
using Msg;

public class ServerTestManager : SingleTon<ServerTestManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q))
        {
            GameManager.Instance.StageClear();
        }
#endif
    }

    public void ServerTest()
    {
        Debug.Log("Server Test Button");

        PlayerPrefs.DeleteAll();

        //string idToken = "abcdefg";

        //// 로그인
        //LoginRequest request = new LoginRequest();
        //request.LoginType = LoginRequest.Types.LOGIN_TYPE.Google;
        ////request.Usn = usn;//Social.localUser.userName == null ? "null" : Social.localUser.userName;
        //request.LoginKey = "oljijho;laji";
        //request.NickName = "user1111";
        //request.IdToken = idToken;

        //// 요청
        //RpcServiceManager.Instance.Login(request, (LoginReply reply) =>
        //{
        //    // 응답
        //    Debug.Log("LoginReply : " + reply.ToString());

        //    if (ERROR_CODE.Ok == reply.Error)
        //    {
        //        Debug.Log("Ok");
        //    }
        //    else
        //    {
        //        Debug.Log(reply.Error);
        //    }
        //});
    }
}
