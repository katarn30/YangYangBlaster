using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;

public class RpcServiceImpl : RpcService.RpcServiceBase
{
    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LoginReply { Error = "Hello " + request.Name });
    }

    public void Login()
    {
        // test grpc
        Grpc.Core.Channel channel = new Grpc.Core.Channel("183.99.10.187:50051", Grpc.Core.ChannelCredentials.Insecure);
        var client = new RpcService.RpcServiceClient(channel);
        var reply = client.Login(new LoginRequest { Name = "아에이오우" });
        Debug.Log("========================" + reply.Error);
        channel.ShutdownAsync().Wait();
    }
}
