#pragma once

#include "stdafx.h"
#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"
//#include "RpcServiceImpl.h"
#include "AsyncHandler.h"
#include "AsyncHandlerRpcServiceExample.h"

namespace yyb
{
    //class CallRpcServiceExample
    //{
    //public:
    //    CallRpcServiceExample(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq)
    //        : service_(service), cq_(cq), responder_(&ctx_), status_(CallStatus::CREATE) {
    //        // Invoke the serving logic right away.
    //        Proceed();
    //    }

    //    void Proceed()
    //    {
    //        std::cout << __FUNCTION__ << " : " << status_ << std::endl;

    //        if (status_ == CREATE)
    //        {
    //            service_->RequestRpcServiceExample(&ctx_, &request_, &responder_, cq_, cq_, this);
    //            status_ = PROCESS;
    //        }
    //        else if (status_ == PROCESS)
    //        {
    //            new CallRpcServiceExample(service_, cq_);

    //            reply_.set_error("async error");

    //            status_ = FINISH;

    //            responder_.Finish(reply_, grpc::Status::OK, this);
    //        }
    //        else if (status_ == FINISH)
    //        {
    //            delete this;
    //        }
    //        else
    //        {
    //            GPR_ASSERT(false);
    //        }
    //    }

    //    RpcService::AsyncService* service_;
    //    grpc::ServerCompletionQueue* cq_;
    //    grpc_impl::ServerContext ctx_;

    //    RpcServiceExampleRequest request_;
    //    RpcServiceExampleReply reply_;
    //    grpc::ServerAsyncResponseWriter<RpcServiceExampleReply> responder_;

    //    enum CallStatus { CREATE, PROCESS, FINISH };
    //    CallStatus status_;  // The current serving state.
    //};
    using scq_ptr = std::unique_ptr<grpc::ServerCompletionQueue>;

	class RpcServerImpl final
	{
	public:
		~RpcServerImpl()
		{
			server_->Shutdown();

            for (auto& cq : scqs_)
            {
                cq->Shutdown();
            }
		}

        void Run() {
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
                thread_group_.create_thread(boost::bind(handleRpcs, &service_, scqs_[i].get()));
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

            thread_group_.join_all();
        }

	private:
        static void handleRpcs(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq)
        {
            /*std::unique_ptr<CallRpcServiceExample> call =
                std::make_unique<CallRpcServiceExample>(&service_, cq_.get());*/
            (new AsyncHandlerRpcServiceExample())->Start(service, cq);

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

        RpcService::AsyncService service_;
        std::unique_ptr<grpc::Server> server_;
        std::vector<scq_ptr> scqs_;
        boost::thread_group thread_group_;
	};
}

