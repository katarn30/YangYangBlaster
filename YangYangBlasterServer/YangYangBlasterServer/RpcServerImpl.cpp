#include "RpcServerImpl.h"
#include "AsyncHandler.h"
#include "AsyncHandlerRpcServiceExample.h"
#include "AsyncHandlerLogin.h"

namespace yyb
{
	RpcServerImpl::~RpcServerImpl()
	{
		server_->Shutdown();

		for (auto& cq : scqs_)
		{
			cq->Shutdown();
		}
	}

    void RpcServerImpl::Run()
    {
        std::string server_address("0.0.0.0:20051");
        //RpcServiceImpl service;

        grpc::EnableDefaultHealthCheckService(true);
        grpc::reflection::InitProtoReflectionServerBuilderPlugin();
        grpc::ServerBuilder builder;
        // Listen on the given address without any authentication mechanism.
        builder.AddListeningPort(server_address, grpc::InsecureServerCredentials());
        // Register "service" as the instance through which we'll communicate with
        // clients. In this case it corresponds to an *synchronous* service.
        builder.RegisterService(&service_);
        // Get hold of the completion queue used for the asynchronous communication
        // with the gRPC runtime.

        int completionThreadCount = std::thread::hardware_concurrency() * 2;
        for (int i = 0; i < completionThreadCount; ++i)
        {
            scqs_.push_back(std::move(builder.AddCompletionQueue()));
            //thread_group_.create_thread(boost::bind(handleRpcs, &service_, scqs_[i].get()));
        }
        /*grpc::ResourceQuota rq;
        rq.SetMaxThreads(static_cast<int>(std::thread::hardware_concurrency()) * 2);
        builder.SetResourceQuota(rq);*/
        // Finally assemble the server.
        server_ = builder.BuildAndStart();
        std::cout << "Server listening on " << server_address << std::endl;

        // Wait for the server to shutdown. Note that some other thread must be
        // responsible for shutting down the server for this call to ever return.
        //server->Wait();

        for (int i = 0; i < completionThreadCount; ++i)
        {
            //scqs_.push_back(std::move(builder.AddCompletionQueue()));
            thread_group_.create_thread(boost::bind(handleRpcs, &service_, scqs_[i].get()));
        }

        thread_group_.join_all();
    }

	void RpcServerImpl::createHandlers(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq)
	{
		AsyncHandlerRpcServiceExample::CreateRequest(service, cq,
			std::move(std::bind(&RpcService::AsyncService::RequestRpcServiceExample, service,
				std::placeholders::_1, std::placeholders::_2,
				std::placeholders::_3, std::placeholders::_4,
				std::placeholders::_5, std::placeholders::_6)),
			[] {return new AsyncHandlerRpcServiceExample; });

        AsyncHandlerLogin::CreateRequest(service, cq,
            std::move(std::bind(&RpcService::AsyncService::RequestLogin, service,
                std::placeholders::_1, std::placeholders::_2,
                std::placeholders::_3, std::placeholders::_4,
                std::placeholders::_5, std::placeholders::_6)),
            [] {return new AsyncHandlerLogin; });
	}

    void RpcServerImpl::handleRpcs(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq)
    {
        createHandlers(service, cq);

        void* tag = nullptr;
        bool ok = false;

        while (true)
        {
            try
            {
                GPR_ASSERT(cq->Next(&tag, &ok));
                //GPR_ASSERT(ok);
                if (ok)
                {
                    static_cast<IAsyncHandler*>(tag)->Proceed();
                }
                else
                {
                    std::cout << "Server completion error" << std::endl;
                }
            }
            catch (std::exception& const e)
            {
                std::cout << e.what() << std::endl;
            }
        }
    }
}