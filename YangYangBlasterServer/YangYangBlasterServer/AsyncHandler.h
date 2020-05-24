#pragma once

#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"

namespace yyb
{
    class IAsyncHandler
    {
    public:
        virtual void Proceed() = 0;
    };

    template<class REQUEST, class REPLY>
	class AsyncHandler : public IAsyncHandler
	{
    public:
        using AsyncHandlerType = AsyncHandler<REQUEST, REPLY>;
        using RESPONDER = grpc::ServerAsyncResponseWriter<REPLY>;
        using REQUEST_FUNC = std::function<void(grpc::ServerContext*,
            REQUEST*, RESPONDER*, grpc::CompletionQueue*, grpc::ServerCompletionQueue*, void*)>;
        using CREATE_FUNC = std::function<AsyncHandlerType*()>;

        AsyncHandler()
            : service_(nullptr), cq_(nullptr), responder_(&ctx_), status_(PROCESS) {}

        void Proceed() override
        {
            // on read
            if (status_ == PROCESS)
            {
                AsyncHandlerType::CreateRequest(service_, cq_, requestFunc_, createFunc_);

                OnRead(request_, reply_);
                                
                status_ = FINISH;

                //responder_.Finish(reply_, grpc::Status::OK, this);
            }
            // on write
            else if (status_ == FINISH)
            {
                OnWrite();

                delete this;
            }
            else
            {
                GPR_ASSERT(false);
            }
        }

        virtual void OnRead(const REQUEST& request, REPLY& reply) {}
        virtual void OnWrite() {}
        
        void Start(RpcService::AsyncService* service,
            grpc::ServerCompletionQueue* cq, REQUEST_FUNC f1, CREATE_FUNC f2)
        {
            service_ = service;
            cq_ = cq;
            requestFunc_ = f1;
            createFunc_ = f2;

            GPR_ASSERT(requestFunc_);
            if (requestFunc_)
            {
                requestFunc_(&ctx_, &request_, &responder_, cq_, cq_, this);
            }
        }

        static AsyncHandlerType* CreateRequest(RpcService::AsyncService* service,
            grpc::ServerCompletionQueue* cq, REQUEST_FUNC f1, CREATE_FUNC f2)
        {
            AsyncHandlerType* handler = nullptr;
            if (f2)
            {
                handler = f2();
                if (handler)
                {
                    handler->Start(service, cq, f1, f2);
                }
            }

            return handler;
        }
        
        void Finish() 
        {
            responder_.Finish(reply_, grpc::Status::OK, this);
        }

    protected:
        RpcService::AsyncService* service_;
        grpc::ServerCompletionQueue* cq_;

        grpc_impl::ServerContext ctx_;
        REQUEST request_;
        REPLY reply_;
        RESPONDER responder_;

        CREATE_FUNC createFunc_;
        REQUEST_FUNC requestFunc_;

        enum AsyncHandlerStatus { CREATE, PROCESS, FINISH };
        AsyncHandlerStatus status_;  // The current serving state.
	};
}

