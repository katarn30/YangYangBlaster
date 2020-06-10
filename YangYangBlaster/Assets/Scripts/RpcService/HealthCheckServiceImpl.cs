using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;
using Yyb;
using Grpc.Health.V1;

public class HealthCheckServiceImpl : Grpc.Health.V1.Health.HealthBase
{
	//public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
	//{
	//	return Task.FromResult(new HealthCheckResponse());
	//}
    //public override Task<LoginReply> Watch(LoginRequest request, ServerCallContext context)
    //   {
    //       return Task.FromResult(new LoginReply());
    //   }

    private readonly object myLock = new object();
    private readonly Dictionary<string, HealthCheckResponse.Types.ServingStatus> statusMap =
        new Dictionary<string, HealthCheckResponse.Types.ServingStatus>();

    /// <summary>
    /// Sets the health status for given service.
    /// </summary>
    /// <param name="service">The service. Cannot be null.</param>
    /// <param name="status">the health status</param>
    public void SetStatus(string service, HealthCheckResponse.Types.ServingStatus status)
    {
        lock (myLock)
        {
            statusMap[service] = status;
        }
    }

    /// <summary>
    /// Clears health status for given service.
    /// </summary>
    /// <param name="service">The service. Cannot be null.</param>
    public void ClearStatus(string service)
    {
        lock (myLock)
        {
            statusMap.Remove(service);
        }
    }

    /// <summary>
    /// Clears statuses for all services.
    /// </summary>
    public void ClearAll()
    {
        lock (myLock)
        {
            statusMap.Clear();
        }
    }

    /// <summary>
    /// Performs a health status check.
    /// </summary>
    /// <param name="request">The check request.</param>
    /// <param name="context">The call context.</param>
    /// <returns>The asynchronous response.</returns>
    public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        lock (myLock)
        {
            var service = request.Service;

            HealthCheckResponse.Types.ServingStatus status;
            if (!statusMap.TryGetValue(service, out status))
            {
                // TODO(jtattermusch): returning specific status from server handler is not supported yet.
                throw new RpcException(new Status(StatusCode.NotFound, ""));
            }
            return Task.FromResult(new HealthCheckResponse { Status = status });
        }
    }
}
