#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class AsyncHandlerRanking 
		: public AsyncHandler<RankingRequest, RankingReply>
	{
    public:
		void OnRead(const RankingRequest& request,
			RankingReply& reply) override;

		void OnWrite() override;
	};
}