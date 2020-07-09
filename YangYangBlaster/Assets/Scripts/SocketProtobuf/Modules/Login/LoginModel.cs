using UnityEngine;
using System.Collections;
using Msg;

public class LoginModel : BaseModel<LoginModel>
{
    protected override void InitAddTocHandler()
    {
        AddTocHandler(typeof(LoginReply), STocLoginReply);
    }

    private void STocLoginReply(object data)
    {
        LoginReply reply = data as LoginReply;

        if (ERROR_CODE.Ok == reply.Error)
        {
            Debug.Log("Ok");

            GameDataManager.Instance.userData.loginKey = reply.LoginKey;
            GameDataManager.Instance.userData.accessKey = reply.AccessKey;
            GameDataManager.Instance.userData.nickName = reply.NickName;
            //usn_ = reply.Usn;

            GameDataManager.Instance.SaveUserDataLoginParts();
            GameDataManager.Instance.LoadGameData();

            //// 헬스체크 시작
            //HealthCheckRequest healthCheckRequest = new HealthCheckRequest();
            //healthCheckRequest.Service = "yyb";

            //// 요청
            //RpcServiceManager.Instance.Check(healthCheckRequest, 
            //    (HealthCheckResponse HealthCheckReply) =>
            //{
            //    // 응답
            //    Debug.Log("Check Response : " + HealthCheckReply.ToString());
            //});

            //// 요청
            //RpcServiceManager.Instance.Watch(healthCheckRequest, 
            //    (HealthCheckResponse HealthCheckReply) =>
            //{
            //    // 응답
            //    Debug.Log("Watch Response : " + HealthCheckReply.ToString());
            //});
        }
        else
        {
            Debug.Log(reply.Error);
            GameDataManager.Instance.CheckUpdateVersion();
            //DeletePlayerPrefs();
        }
    }

    public void CTosLoginRequest(Msg.LoginRequest.Types.LOGIN_TYPE loginType, 
        string loginKey, string nickName, string idToken)
    {
        LoginRequest request = new LoginRequest();
        request.LoginKey = loginKey;
        request.LoginType = loginType;
        request.NickName = nickName;
        request.IdToken = idToken;
        request.Version = string.Format("{0}_{1}", Application.version, Application.identifier);

        SendTos(request);
    }
}
