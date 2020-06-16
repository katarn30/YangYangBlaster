#pragma once

#include "AsyncHandler.h"
#include "AsyncHandlerRpcServiceExample.h"
#include "AsyncHandlerLogin.h"
#include "AsyncHandlerRanking.h"
#include "AsyncHandlerRankingList.h"
#include "AsyncHandlerLoadGameData.h"
#include "AsyncHandlerSaveGameData.h"

namespace yyb
{
#define HANDLER_MACRO(KEWORD)   \
    AsyncHandler##KEWORD##::CreateRequest(service, cq, io_service,  \
    std::move(std::bind(&RpcService::AsyncService::Request##KEWORD##, service,   \
    std::placeholders::_1, std::placeholders::_2,   \
    std::placeholders::_3, std::placeholders::_4,   \
    std::placeholders::_5, std::placeholders::_6)), \
    [] {return new AsyncHandler##KEWORD##; })

	void CreateHandler(RpcService::AsyncService* service,
		grpc::ServerCompletionQueue* cq, boost::asio::io_service* io_service)
	{
        HANDLER_MACRO(RpcServiceExample);
        //HANDLER_MACRO(Listen);
        HANDLER_MACRO(Login);
        HANDLER_MACRO(Ranking);
        HANDLER_MACRO(LoadGameData);
        HANDLER_MACRO(SaveGameData);
	}
}