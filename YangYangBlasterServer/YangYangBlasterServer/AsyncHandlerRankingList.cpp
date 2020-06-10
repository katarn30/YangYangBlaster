#include "AsyncHandlerRankingList.h"
#include "GlobalDefine.h"
#include "Cache.h"

namespace yyb
{
	void AsyncHandlerRankingList::OnRead(
		const RankingListRequest& request,
		RankingListReply& reply)
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;

		auto redis = Cache::Instance().GetCache(CACHE_INDEX_GLOBAL);
		if (redis)
		{
			std::string key = CACHE_KEY_RANKING;
			std::string value;
			std::string start = "0";
			std::string stop = "14"; // max

			auto user = GetUser();
			if (user)
			{
				value = user->GetNickName();
			}

			// 랭킹, 점수 획득
			auto futureReply1 = redis->zrevrange(key, start, stop, true);
			auto futureReply2 = redis->zrevrank(key, value);
			auto futureReply3 = redis->zscore(key, value);

			redis->sync_commit();

			// start ~ stop 까지 랭킹 가져옴
			auto reply1 = futureReply1.get();
			if (reply1)
			{
				auto arr = reply1.as_array();

				for (int i = 0; i < arr.size(); i += 2)
				{
					std::string nickName = arr[i].as_string();
					int score = boost::lexical_cast<int>(arr[i + 1].as_string());
					int rank = i / 2 + 1;

					auto ranking = reply.add_rankings();
					ranking->set_nickname(nickName);
					ranking->set_rank(rank);
					ranking->set_score(score);
				}
			}

			// 내 닉네임
			reply.mutable_myranking()->set_nickname(value);

			// 내 랭크 몇인지
			auto reply2 = futureReply2.get();
			if (reply2)
			{
				int rank = reply2.as_integer();
				rank += 1;

				reply.mutable_myranking()->set_rank(rank);
			}

			// 내 점수 몇인지
			auto reply3 = futureReply3.get();
			if (reply3)
			{
				int score = boost::lexical_cast<int>(reply3.as_string());

				reply.mutable_myranking()->set_score(score);
			}
		}
	}

	void AsyncHandlerRankingList::OnWrite()
	{

	}
}