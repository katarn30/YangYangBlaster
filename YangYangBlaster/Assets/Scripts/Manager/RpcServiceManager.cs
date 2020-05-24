using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yyb;
public class RpcServiceManager : SingleTon<RpcServiceManager>
{
    private Grpc.Core.Channel channel;
    private RpcService.RpcServiceClient client;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        Run();
    }

    //로비 상태로 바뀔떄
    public void SetLobbyInit()
    {
        // 로그인
        LoginRequest request = new LoginRequest();
        request.Name = "이름이름이름이름이름";
        request.SerialKey = "키키키키키키키";

        // 요청
        Login(request, delegate (LoginReply reply)
        {
            // 응답
            Debug.Log("RpcService : " + reply);
        });
    }

    public void Run()
    {
        if (channel == null || client == null)
        {
            // 서버 연결
            string liveHost = "183.99.10.187:20051";
            string inhouseHost = "127.0.0.1:20051";

            channel = new Grpc.Core.Channel(inhouseHost, Grpc.Core.ChannelCredentials.Insecure);
            client = new RpcService.RpcServiceClient(channel);
        }
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        channel.ShutdownAsync().Wait();
    }

    public delegate void RPCHandler<T>(T reply);

    // RpcServiceExample
    public void RpcServiceExample(RpcServiceExampleRequest request, RPCHandler<RpcServiceExampleReply> handler)
    {
        StartCoroutine(coRpcServiceExample(request, handler));
    }
    IEnumerator coRpcServiceExample(RpcServiceExampleRequest request, RPCHandler<RpcServiceExampleReply> handler)
    {
        try
        {
            var reply = client.RpcServiceExample(request);

            handler(reply);
        }
        catch (Grpc.Core.RpcException e)
        {
            Debug.LogError("RPC failed " + e);
        }

        yield return null;
    }

    // Login
    public void Login(LoginRequest request, RPCHandler<LoginReply> handler)
    {
        StartCoroutine(coLogin(request, handler));
    }
    IEnumerator coLogin(LoginRequest request, RPCHandler<LoginReply> handler)
    {
        try
        {
            var reply = client.Login(request);

            handler(reply);
        }
        catch (Grpc.Core.RpcException e)
        {
            Debug.LogError("RPC failed " + e);
        }
        
        yield return null;
    }
}
