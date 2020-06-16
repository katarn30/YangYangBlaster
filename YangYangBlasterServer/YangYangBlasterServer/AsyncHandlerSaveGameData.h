#pragma once

#include "AsyncHandler.h"
#include "DB.h"

namespace yyb
{
	class AsyncHandlerSaveGameData
		: public AsyncHandler<GameDataRequest, GameDataReply>
	{
    public:
		void OnRead(const GameDataRequest& request,
			GameDataReply& reply) override;

		void OnWrite() override;

		bool UpdateItem(DB::QueryScopeTran& tran, 
			int usn, const Item& item);
		bool InsertItem(DB::QueryScopeTran& tran,
			int usn, const Item& item);
		bool UpdateMercenary(DB::QueryScopeTran& tran,
			int usn, const Mercenary& mercenary);
		bool InsertMercenary(DB::QueryScopeTran& tran,
			int usn, const Mercenary& mercenary);
		bool UpdateStage(DB::QueryScopeTran& tran, 
			int usn, const Stage& stage);
		bool InsertStage(DB::QueryScopeTran& tran,
			int usn, const Stage& stage);
		bool UpdateUpgradePlayer(DB::QueryScopeTran& tran, 
			int usn, const UpgradePlayer& upgradePlayer);
		bool InsertUpgradePlayer(DB::QueryScopeTran& tran,
			int usn, const UpgradePlayer& upgradePlayer);
	};
}