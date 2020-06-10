using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using Grpc.Health.V1;

public class ServerTestManager : SingleTon<ServerTestManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void ServerTest()
    {
        Debug.Log("Server Test Button");

        HealthCheckRequest request = new HealthCheckRequest();
        request.Service = "yyb";

        // 요청
        RpcServiceManager.Instance.Check(request, (HealthCheckResponse reply) =>
        {
            // 응답
            Debug.Log("HealthCheckResponse : " + reply.ToString());
        });

        // 요청
        RpcServiceManager.Instance.Watch(request, 
            (HealthCheckResponse reply) =>
        {
            // 응답
            Debug.Log("HealthCheckResponse : " + reply.ToString());
        });
    }
}
