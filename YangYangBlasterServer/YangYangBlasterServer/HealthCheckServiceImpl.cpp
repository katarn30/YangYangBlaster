#include "stdafx.h"
#include "HealthCheckServiceImpl.h"
#include "UserManager.h"
#include "User.h"
#include "DB.h"
#include "GlobalDefine.h"

namespace yyb
{
	grpc::Status HealthCheckServiceImpl::Check(
		grpc_impl::ServerContext* context,
		const grpc::health::v1::HealthCheckRequest* request,
		grpc::health::v1::HealthCheckResponse* reply)
	{
		std::cout << __FUNCTION__ << std::endl;

        const auto& metadata = context->client_metadata();
        auto metadataIter = metadata.find("access_key");
        if (metadataIter != metadata.end())
        {
            //std::string accessKey = metadataIter->second.data();
            std::string accessKey(metadataIter->second.begin(), 
                metadataIter->second.end());
            user_ptr user = UserManager::Instance().GetUser(accessKey);

			if (user)
			{
                int accessKeyUpdateTime = 0;

				bool queryResult = DB::Instance().QueryScope(DB_POOL_INDEX_GLOBAL,
					[&](soci::session& sql)
					{
                        soci::statement stmt = (sql.prepare <<
                            "UPDATE user SET "
                            "access_key_update_time = @temp_time := NOW() "
                            "WHERE access_key=:access_key AND is_deleted=0 LIMIT 1",
                            soci::use(accessKey, "access_key"));

						stmt.execute();

						auto affected_rows = stmt.get_affected_rows();
						if (affected_rows <= 0)
						{
							return false;
						}

                        sql << "SELECT UNIX_TIMESTAMP(@temp_time)", 
                            soci::into(accessKeyUpdateTime);

						return true;
					});

                user->SetAccessKeyUpdateTime(accessKeyUpdateTime);
			}
		}

        //for (auto iter : metadata)
        //{

        //}
        //for (auto iter = metadata.begin(); iter != metadata.end(); ++iter) 
        //{
        //    std::cout << "Header key: " << iter->first << ", value: ";
        //    // Check for binary value
        //    size_t isbin = iter->first.find("-bin");
        //    if ((isbin != std::string::npos) && (isbin + 4 == iter->first.size())) 
        //    {
        //        std::cout << std::hex;
        //        for (auto c : iter->second) 
        //        {
        //            std::cout << static_cast<unsigned int>(c);
        //        }
        //        std::cout << std::dec;
        //    }
        //    else 
        //    {
        //        std::cout << iter->second;
        //    }
        //    std::cout << std::endl;
        //}

        std::lock_guard<std::mutex> lock(mu_);
        auto iter = status_map_.find(request->service());
        if (iter == status_map_.end()) 
        {
            return grpc::Status(grpc::StatusCode::NOT_FOUND, "");
        }
        reply->set_status(iter->second);

		return grpc::Status::OK;
	}

	grpc::Status HealthCheckServiceImpl::Watch(
		grpc::ServerContext* context,
		const grpc::health::v1::HealthCheckRequest* request,
		grpc::ServerWriter<grpc::health::v1::HealthCheckResponse>* writer)
	{
		std::cout << __FUNCTION__ << std::endl;

        std::string accessKey;

        const auto& metadata = context->client_metadata();
        auto metadataIter = metadata.find("access_key");
        if (metadataIter != metadata.end())
        {
            accessKey.assign(metadataIter->second.begin(),
                metadataIter->second.end());
            user_ptr user = UserManager::Instance().GetUser(accessKey);

            if (user)
            {
                if (user->accessKey_ != accessKey)
                {
                    // 에러 처리
                }
            }
            else
            {
                // 에러 처리
            }
        }

        
        auto deadlineTime = gpr_time_add(gpr_now(GPR_CLOCK_MONOTONIC),
            gpr_time_from_millis(10000, GPR_TIMESPAN));

        auto last_state = grpc::health::v1::HealthCheckResponse::UNKNOWN;
        while (!context->IsCancelled()) 
        {
            auto now = gpr_now(GPR_CLOCK_MONOTONIC);

            if (gpr_time_cmp(deadlineTime, now) < 0)
            {
                break;
            }
            // 여기서 accessKeyUpdateTime 체크하지 않고 재로그인 시도때 체크하자
            //user_ptr user = UserManager::Instance().GetUser(accessKey);

            //if (user)
            //{
            //    int accessKeyUpdateTime = user->GetAccessKeyUpdateTime();

            //    if (accessKeyUpdateTime + XX시간 < NOW())
            //    {
            //        // 
            //    }
            //}

            {
                std::lock_guard<std::mutex> lock(mu_);
                grpc::health::v1::HealthCheckResponse response;
                auto iter = status_map_.find(request->service());
                if (iter == status_map_.end()) {
                    response.set_status(response.SERVICE_UNKNOWN); // SERVICE_UNKNOWN
                }
                else {
                    response.set_status(iter->second);
                }
                if (response.status() != last_state) {
                    writer->Write(response, ::grpc::WriteOptions());
                    last_state = response.status();
                }
            }
            
            gpr_sleep_until(gpr_time_add(gpr_now(GPR_CLOCK_MONOTONIC),
                gpr_time_from_millis(1000, GPR_TIMESPAN)));
        }

		return grpc::Status::OK;
	}

    void HealthCheckServiceImpl::SetStatus(
        const grpc::string& service_name,
        grpc::health::v1::HealthCheckResponse::ServingStatus status) {
        std::lock_guard<std::mutex> lock(mu_);
        if (shutdown_) {
            status = grpc::health::v1::HealthCheckResponse::NOT_SERVING;
        }
        status_map_[service_name] = status;
    }

    void HealthCheckServiceImpl::SetAll(
        grpc::health::v1::HealthCheckResponse::ServingStatus status) 
    {
        std::lock_guard<std::mutex> lock(mu_);
        if (shutdown_) 
        {
            return;
        }
        for (auto iter = status_map_.begin(); iter != status_map_.end(); ++iter) 
        {
            iter->second = status;
        }
    }

    void HealthCheckServiceImpl::Shutdown() 
    {
        std::lock_guard<std::mutex> lock(mu_);
        if (shutdown_) 
        {
            return;
        }
        shutdown_ = true;
        for (auto iter = status_map_.begin(); iter != status_map_.end(); ++iter) 
        {
            iter->second = grpc::health::v1::HealthCheckResponse::NOT_SERVING;
        }
    }
}