#include "RpcServerImpl.h"
#include "HealthCheckServiceImpl.h"
#include "AsyncHandler.h"
#include "UserManager.h"
#include "User.h"
#include "CreateHandler.h"

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
        HealthCheckServiceImpl healthCheckService;

        grpc::EnableDefaultHealthCheckService(false);
        grpc::reflection::InitProtoReflectionServerBuilderPlugin();
        grpc::ServerBuilder builder;

        // Options..
        /*builder.AddChannelArgument(GRPC_ARG_KEEPALIVE_TIME_MS, 2000);
        builder.AddChannelArgument(GRPC_ARG_KEEPALIVE_TIMEOUT_MS, 1000);
        builder.AddChannelArgument(GRPC_ARG_HTTP2_BDP_PROBE, 1);*/
        std::unique_ptr<grpc::HealthCheckServiceInterface> service(
            new CustomHealthCheckService(&healthCheckService));
        std::unique_ptr<grpc::ServerBuilderOption> option(
            new grpc::HealthCheckServiceServerBuilderOption(std::move(service)));
        builder.SetOption(std::move(option));

        // Listen on the given address without any authentication mechanism.
        builder.AddListeningPort(server_address, grpc::InsecureServerCredentials());
        // Register "service" as the instance through which we'll communicate with
        // clients. In this case it corresponds to an *synchronous* service.
        builder.RegisterService(&service_);
        builder.RegisterService(&healthCheckService);

        /*std::vector<
            std::unique_ptr<grpc::experimental::ServerInterceptorFactoryInterface>>
            creators;
        creators.push_back(
            std::unique_ptr<grpc::experimental::ServerInterceptorFactoryInterface>(
                new SyncSendMessageVerifierFactory()));
        builder.experimental().SetInterceptorCreators(std::move(creators));*/

        // Get hold of the completion queue used for the asynchronous communication
        // with the gRPC runtime.

        int completionThreadCount = std::thread::hardware_concurrency();
        for (int i = 0; i < completionThreadCount; ++i)
        {
            scqs_.push_back(std::move(builder.AddCompletionQueue()));
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
            thread_group_.create_thread(boost::bind(&boost::asio::io_service::run, &io_service_));
        }

        for (int i = 0; i < completionThreadCount; ++i)
        {
            thread_group_.create_thread(boost::bind(handleRpcs, &service_, scqs_[i].get(), &io_service_));
        }

        thread_group_.join_all();
    }

#define HANDLER_MACRO(KEWORD)   \
    AsyncHandler##KEWORD##::CreateRequest(service, cq, io_service,  \
    std::move(std::bind(&RpcService::AsyncService::Request##KEWORD##, service,   \
        std::placeholders::_1, std::placeholders::_2,   \
        std::placeholders::_3, std::placeholders::_4,   \
        std::placeholders::_5, std::placeholders::_6)), \
        [] {return new AsyncHandler##KEWORD##; })

	void RpcServerImpl::createHandlers(RpcService::AsyncService* service, 
        grpc::ServerCompletionQueue* cq, boost::asio::io_service* io_service)
	{
        CreateHandler(service, cq, io_service);
	}

    void RpcServerImpl::handleRpcs(RpcService::AsyncService* service, 
        grpc::ServerCompletionQueue* cq, boost::asio::io_service* io_service)
    {
        createHandlers(service, cq, io_service);
        //createHandler(service, cq, io_service);

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

    //void SyncSendMessageVerifier::Intercept(
    //    grpc::experimental::InterceptorBatchMethods* methods)
    //{
    //    if (methods->QueryInterceptionHookPoint(
    //        grpc::experimental::InterceptionHookPoints::POST_RECV_INITIAL_METADATA)) {
    //        // Make sure that the changes made in SyncSendMessageTester persisted
    //        /*std::string old_msg =
    //            static_cast<const grpc::health::v1::HealthCheckRequest*>(
    //                methods->GetSendMessage())->message();*/

    //        //const auto* metadata = methods->GetRecvInitialMetadata();
    //        //if (metadata)
    //        //{
    //        //    auto metadataIter = metadata->find("access_key");
    //        //    if (metadataIter != metadata->end())
    //        //    {
    //        //        //std::string accessKey = metadataIter->second.data();
    //        //        std::string accessKey(metadataIter->second.begin(),
    //        //            metadataIter->second.end());
    //        //        yyb::user_ptr user = yyb::UserManager::Instance().GetUser(
    //        //            accessKey);

    //        //        if (user)
    //        //        {
    //        //        }
    //        //    }
    //        //}

    //        //methods->GetInterceptedChannel()->GetState(true);
    //        //methods->Hijack();
    //        methods->FailHijackedRecvMessage();

    //        //EXPECT_EQ(old_msg.find("World"), 0u);

    //        // Remove the "World" part of the string that we added earlier
    //        /*new_msg_.set_message(old_msg.erase(0, 5));
    //        methods->ModifySendMessage(&new_msg_);*/

    //        // LoggingInterceptor verifies that changes got reverted
    //    }
    //    methods->Proceed();
    //}
}