#pragma once

#include "health_check_service.pb.h"
#include "health_check_service.grpc.pb.h"

namespace yyb
{
	class HealthCheckServiceImpl final 
		: public grpc::health::v1::Health::Service
	{
	public:
		grpc::Status Check(grpc_impl::ServerContext* context,
			const grpc::health::v1::HealthCheckRequest* request, 
			grpc::health::v1::HealthCheckResponse* reply) override;

		grpc::Status Watch(grpc::ServerContext* context, 
			const grpc::health::v1::HealthCheckRequest* request, 
			grpc::ServerWriter<grpc::health::v1::HealthCheckResponse>* writer) 
			override;

		void SetStatus(const grpc::string& service_name,
			grpc::health::v1::HealthCheckResponse::ServingStatus status);
		void SetAll(grpc::health::v1::HealthCheckResponse::ServingStatus status);

		void Shutdown();

	private:
		std::mutex mu_;
		bool shutdown_ = false;
		std::map<const grpc::string, 
            grpc::health::v1::HealthCheckResponse::ServingStatus>
			status_map_;
	};

    //A custom implementation of the health checking service interface. This is
    //used to test that it prevents the server from creating a default service and
    //also serves as an example of how to override the default service.
    class CustomHealthCheckService : public grpc::HealthCheckServiceInterface {
    public:
        explicit CustomHealthCheckService(yyb::HealthCheckServiceImpl* impl)
            : impl_(impl)
        {
            impl_->SetStatus("yyb", grpc::health::v1::HealthCheckResponse::SERVING);
        }

        void SetServingStatus(const grpc::string& service_name,
            bool serving) override
        {
            impl_->SetStatus(service_name, serving ? grpc::health::v1::HealthCheckResponse::SERVING
                : grpc::health::v1::HealthCheckResponse::NOT_SERVING);
        }

        void SetServingStatus(bool serving) override
        {
            impl_->SetAll(serving ? grpc::health::v1::HealthCheckResponse::SERVING
                : grpc::health::v1::HealthCheckResponse::NOT_SERVING);
        }

        void Shutdown() override { impl_->Shutdown(); }

    private:
        yyb::HealthCheckServiceImpl* impl_;  // not owned
    };
}