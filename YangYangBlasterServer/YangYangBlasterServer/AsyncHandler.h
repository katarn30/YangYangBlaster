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

    /*using CREATE_REQUEST = std::function<void(::grpc::ServerContext*,
        ::yyb::RpcServiceExampleRequest*,
        ::grpc::ServerAsyncResponseWriter*,
        ::grpc::CompletionQueue*,
        ::grpc::ServerCompletionQueue*,
        void* tag)>;

    std::function<void(std::placeholders::_1, std::placeholder::_2, std::placeholder::_3, std::placeholder::_4, std::placeholder::_5, std::placeholder::_6)>
        ;
    std::*/
    template<class REQUEST, class REPLY>
	class AsyncHandler : public IAsyncHandler
	{
    public:
        //AsyncHandler() = delete;
        AsyncHandler(/*RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq*/)
            : service_(nullptr), cq_(nullptr), responder_(&ctx_), status_(CREATE) {
            // Invoke the serving logic right away.
            //Proceed();
        }

        virtual void Proceed()
        {
            if (status_ == CREATE)
            {
                OnRead(request_, reply_);
                //service_->CREATE_REQUEST(&ctx_, &request_, &responder_, cq_, cq_, this);
                status_ = PROCESS;
            }
            // on read
            else if (status_ == PROCESS)
            {
                OnWrite();

                //new CallRpcServiceExample(service_, cq_);

                //reply_.set_error("async error");

                status_ = FINISH;

                responder_.Finish(reply_, grpc::Status::OK, this);
            }
            // on write
            else if (status_ == FINISH)
            {
                delete this;
            }
            else
            {
                GPR_ASSERT(false);
            }
        }

        virtual void OnRead(const REQUEST& request, REPLY& reply) {}
        virtual void OnWrite() {}

        void Start(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq)
        {
            service_ = service;
            cq_ = cq;

            Proceed();
        }

    protected:
        RpcService::AsyncService* service_;
        grpc::ServerCompletionQueue* cq_;

        grpc_impl::ServerContext ctx_;
        REQUEST request_;
        REPLY reply_;
        grpc::ServerAsyncResponseWriter<REPLY> responder_;

        enum AsyncHandlerStatus { CREATE, PROCESS, FINISH };
        AsyncHandlerStatus status_;  // The current serving state.
	};
}

