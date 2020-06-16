using Grpc.Health.V1;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yyb;
using Grpc.Core;
using System.Threading.Tasks;
using System.Threading;

public class RpcServiceManager : SingleTon<RpcServiceManager>
{
    private Grpc.Core.Channel channel;
    private RpcService.RpcServiceClient client;
    private Health.HealthClient health;
    private Thread healthCheckThread;
    private Thread healthWatchThread;
    private bool running = true;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        Run();
    }

    //로비 상태로 바뀔떄
    public void SetLobbyInit()
    {
        //// 로그인
        //LoginRequest request = new LoginRequest();
        //request.Name = "이름이름이름이름이름";
        //request.SerialKey = "키키키키키키키";

        //// 요청
        //Login(request, delegate (LoginReply reply)
        //{
        //    // 응답
        //    Debug.Log("RpcService : " + reply);
        //});
    }

    public void Run()
    {
        if (channel == null || client == null)
        {
            // 서버 연결
            string localHost = "127.0.0.1:20051";
            string inhouseHost = "183.99.10.187:20051";
            string liveHost = "";

#if UNITY_EDITOR
            string host = localHost;
#else
            string host = inhouseHost;
#endif
            channel = new Grpc.Core.Channel(host, Grpc.Core.ChannelCredentials.Insecure);
            client = new RpcService.RpcServiceClient(channel);
            health = new Health.HealthClient(channel);
        }
    }

    protected override void OnApplicationQuit()
    {
        running = false;

        if (null != channel)
        {
            channel.ShutdownAsync().Wait(1);
        }

        if (null != healthCheckThread)
        {
            healthCheckThread.Abort();
        }

        if (null != healthWatchThread)
        {
            //healthThread.Interrupt();
            healthWatchThread.Abort();
        }

        base.OnApplicationQuit();
    }

    public delegate void RPCHandler<T>(T reply);

    // HealthCheck - Check
    public void Check(HealthCheckRequest request, RPCHandler<HealthCheckResponse> handler)
    {
        healthCheckThread = new Thread(() =>
        {
            int intervalTime = 60 * 1000;//Time.time + (10.0f * 60.0f);

            try
            {
                while (running)
                {
                    //if (Time.time < intervalTime)
                    //{
                    //    continue;
                    //}

                    var metaData = new Metadata
                    {
                        { "access_key", GameDataManager.Instance.userData.accessKey }
                    };

                    var reply = health.Check(request, metaData);

                    handler(reply);

                    Thread.Sleep(intervalTime);
                }
            }
            catch (Grpc.Core.RpcException e)
            {
                Debug.LogError("RPC failed " + e);
            }
        });


        healthCheckThread.Start();
        //StartCoroutine(coCheck(request, handler));
    }

    //IEnumerator coCheck(HealthCheckRequest request, RPCHandler<HealthCheckResponse> handler)
    //{
    //    try
    //    {
    //        while (running)
    //        {
    //            //if (Time.time < intervalTime)
    //            //{
    //            //    continue;
    //            //}

    //            var metadata = new Metadata
    //            {
    //                { "access_key", GameDataManager.Instance.userData.accessKey }
    //            };

    //            var reply = health.Check(request, metadata);

    //            handler(reply);
    //        }
    //    }
    //    catch (Grpc.Core.RpcException e)
    //    {
    //        Debug.LogError("RPC failed " + e);
    //    }

    //    yield return null;
    //}

    // HealthCheck - Watch
    public void Watch(HealthCheckRequest request, 
        RPCHandler<HealthCheckResponse> handler)
    {
        //coWatch(request, handler);

        //await task;
        //StartCoroutine(coWatch(request, handler));
        //await coWatch(request, handler);

        healthWatchThread = new Thread(async () =>
        {
            try
            {
                while (running)
                {
                    var metaData = new Metadata
                    {
                        { "access_key", GameDataManager.Instance.userData.accessKey }
                    };

                    using (var call = health.Watch(request, metaData))
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            HealthCheckResponse reply = call.ResponseStream.Current;
                            handler(reply);
                        }
                    }
                }
            }
            catch (Grpc.Core.RpcException e)
            {
                Debug.LogError("RPC failed " + e);
            }
        });

        healthWatchThread.Start();
    }

    //void coWatch(HealthCheckRequest request, 
    //    RPCHandler<HealthCheckResponse> handler)
    //{
    //    Thread thread = new Thread(async () =>
    //    {
    //        try
    //        {
    //            using (var call = health.Watch(request))
    //            {
    //                while (await call.ResponseStream.MoveNext())
    //                {
    //                    HealthCheckResponse reply = call.ResponseStream.Current;
    //                    handler(reply);
    //                }
    //            }

    //            //var reply = health.Watch(request);

    //            //handler(reply);
    //        }
    //        catch (Grpc.Core.RpcException e)
    //        {
    //            Debug.LogError("RPC failed " + e);
    //        }
    //    });
    //}

    // RpcServiceExample
    public void RpcServiceExample(RpcServiceExampleRequest request, RPCHandler<RpcServiceExampleReply> handler)
    {
        StartCoroutine(coRpcServiceExample(request, handler));
    }
    IEnumerator coRpcServiceExample(RpcServiceExampleRequest request, RPCHandler<RpcServiceExampleReply> handler)
    {
        try
        {
            var metaData = new Metadata
            {
                { "access_key", GameDataManager.Instance.userData.accessKey }
            };

            var reply = client.RpcServiceExample(request, metaData);

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

    // LoadGameData
    public void LoadGameData(GameDataRequest request, RPCHandler<GameDataReply> handler)
    {
        StartCoroutine(coLoadGameData(request, handler));
    }
    IEnumerator coLoadGameData(GameDataRequest request, RPCHandler<GameDataReply> handler)
    {
        try
        {
            var metaData = new Metadata
            {
                { "access_key", GameDataManager.Instance.userData.accessKey }
            };

            var reply = client.LoadGameData(request, metaData);

            handler(reply);
        }
        catch (Grpc.Core.RpcException e)
        {
            Debug.LogError("RPC failed " + e);
        }

        yield return null;
    }

    // SaveGameData
    public void SaveGameData(GameDataRequest request, RPCHandler<GameDataReply> handler)
    {
        StartCoroutine(coSaveGameData(request, handler));
    }
    IEnumerator coSaveGameData(GameDataRequest request, RPCHandler<GameDataReply> handler)
    {
        try
        {
            var metaData = new Metadata
            {
                { "access_key", GameDataManager.Instance.userData.accessKey }
            };

            var reply = client.SaveGameData(request, metaData);

            handler(reply);
        }
        catch (Grpc.Core.RpcException e)
        {
            Debug.LogError("RPC failed " + e);
        }

        yield return null;
    }

    // Ranking
    public void Ranking(RankingRequest request, RPCHandler<RankingReply> handler)
    {
        StartCoroutine(coRanking(request, handler));
    }
    IEnumerator coRanking(RankingRequest request, RPCHandler<RankingReply> handler)
    {
        try
        {
            var metaData = new Metadata
            {
                { "access_key", GameDataManager.Instance.userData.accessKey }
            };

            var reply = client.Ranking(request, metaData);

            handler(reply);
        }
        catch (Grpc.Core.RpcException e)
        {
            Debug.LogError("RPC failed " + e);
        }

        yield return null;
    }

    // RankingList
    public void RankingList(RankingListRequest request, RPCHandler<RankingListReply> handler)
    {
        StartCoroutine(coRankingList(request, handler));
    }
    IEnumerator coRankingList(RankingListRequest request, RPCHandler<RankingListReply> handler)
    {
        try
        {
            var metaData = new Metadata
            {
                { "access_key", GameDataManager.Instance.userData.accessKey }
            };

            var reply = client.RankingList(request, metaData);

            handler(reply);
        }
        catch (Grpc.Core.RpcException e)
        {
            Debug.LogError("RPC failed " + e);
        }

        yield return null;
    }
}
