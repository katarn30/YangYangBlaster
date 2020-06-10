#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class AsyncHandlerRankingList 
		: public AsyncHandler<RankingListRequest, RankingListReply>
	{
    public:
		void OnRead(const RankingListRequest& request,
			RankingListReply& reply) override;

		void OnWrite() override;
	};
}