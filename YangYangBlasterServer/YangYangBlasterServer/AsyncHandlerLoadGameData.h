#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class AsyncHandlerLoadGameData
		: public AsyncHandler<GameDataRequest, GameDataReply>
	{
    public:
		void OnRead(const GameDataRequest& request,
			GameDataReply& reply) override;

		void OnWrite() override;

		bool SelectItem(int usn, OUT std::vector<Item>& outItems);
		bool SelectMercenary(int usn, OUT std::vector<Mercenary>& outMercenaries);
		bool SelectStage(int usn, OUT Stage& outStage);
		bool SelectUpgradePlayer(int usn, OUT UpgradePlayer& outUpgradePlayer);
	};
}