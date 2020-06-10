#include "AsyncHandlerRanking.h"
#include "GlobalDefine.h"
#include "Cache.h"

namespace yyb
{
	void AsyncHandlerRanking::OnRead(
		const RankingRequest& request,
		RankingReply& reply)
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;

		auto redis = Cache::Instance().GetCache(CACHE_INDEX_GLOBAL);
		if (redis)
		{
			std::string key = "ranking";
			std::string value;

			auto user = GetUser();
			if (user)
			{
				value = user->GetNickName();
			}

			//// 내 저장된 랭킹 점수
			//auto reply = redis->zrank(key, value);
			//redis->sync_commit();

			//if (reply.get().is_integer())
			//{
			//	storedScore = reply.get().as_integer();
			//}

			//// 내 저장된 랭킹 점수와 지금 획득한 점수 비교
			//if (storedScore < score)
			//{
			//	std::vector< std::string > members;

			//	members.push_back(value);

			//	// 지금 획득한 점수가 더 크면 저장된 랭킹 점수 제거
			//	redis->zrem(key, members);
			//}

			if (false == value.empty())
			{
				// 지금 획득한 점수 저장
				std::multimap<std::string, std::string> score_members;
				std::vector<std::string> options;

				score_members.insert({ std::to_string(request.score()), value });

				redis->zadd(key, options, score_members);

				redis->commit();
			}
		}
	}

	void AsyncHandlerRanking::OnWrite()
	{

	}
}