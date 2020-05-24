using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;
using Yyb;

public class RpcServiceImpl : RpcService.RpcServiceBase
{
	public override Task<RpcServiceExampleReply> RpcServiceExample(RpcServiceExampleRequest request, ServerCallContext context)
	{
		return Task.FromResult(new RpcServiceExampleReply());
	}
	public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LoginReply());
    }
}
