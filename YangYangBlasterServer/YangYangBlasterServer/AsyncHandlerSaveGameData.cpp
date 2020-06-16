#include "AsyncHandlerSaveGameData.h"
#include "GlobalDefine.h"
#include "Cache.h"
#include "DB.h"

namespace yyb
{
	void AsyncHandlerSaveGameData::OnRead(
		const GameDataRequest& request,
		GameDataReply& reply)
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;

		user_ptr user = GetUser();
		if (user)
		{
			DB::QueryScopeTran tran(DB_POOL_INDEX_GLOBAL);

			for (const auto& item : request.items())
			{
				if (false == UpdateItem(tran, user->GetUsn(), item))
				{
					if (false == InsertItem(tran, user->GetUsn(), item))
					{
						reply.set_error(ERROR_CODE_FAILED_TO_SAVE_ITEM);
						return;
					}
				}
			}

			for (const auto& mercenary : request.mercenaries())
			{
				if (false == UpdateMercenary(tran, user->GetUsn(), 
					mercenary))
				{
					if (false == InsertMercenary(tran, user->GetUsn(),
						mercenary))
					{
						reply.set_error(ERROR_CODE_FAILED_TO_SAVE_MERCENARY);
						return;
					}
				}
			}

			if (false == UpdateStage(tran, user->GetUsn(), request.stage()))
			{
				if (false == InsertStage(tran, user->GetUsn(), request.stage()))
				{
					reply.set_error(ERROR_CODE_FAILED_TO_SAVE_STAGE);
					return;
				}
			}

			if (false == UpdateUpgradePlayer(tran, user->GetUsn(),
				request.upgradeplayer()))
			{
				if (false == InsertUpgradePlayer(tran, user->GetUsn(),
					request.upgradeplayer()))
				{
					reply.set_error(ERROR_CODE_FAILED_TO_SAVE_UPGRADE_PLAYER);
					return;
				}
			}

			tran.Commit();
		}
	}

	void AsyncHandlerSaveGameData::OnWrite()
	{

	}

	bool AsyncHandlerSaveGameData::UpdateItem(
		DB::QueryScopeTran& tran,
		int usn, const Item& item)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"UPDATE item SET item_name=:item_name, "
					"item_type=:item_type, "
					"item_category=:item_category, "
					"item_count=:item_count "
					"WHERE usn=:usn",
					soci::use(item.itemname(), "item_name"),
					soci::use(static_cast<int>(item.itemtype()), 
						"item_type"),
					soci::use(static_cast<int>(item.itemcategory()), 
						"item_category"),
					soci::use(static_cast<int>(item.itemcount()), 
						"item_count"),
					soci::use(usn, "usn"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::InsertItem(
		DB::QueryScopeTran& tran,
		int usn, const Item& item)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"INSERT INTO item(usn, item_name, item_type, "
					"item_category, item_count) VALUES(:usn, item_name, "
					":item_type, :item_category, :item_count)",
					soci::use(usn, "usn"),
					soci::use(item.itemname(),
						"item_name"),
					soci::use(static_cast<int>(item.itemtype()),
						"item_type"),
					soci::use(static_cast<int>(item.itemcategory()),
						"item_category"),
					soci::use(static_cast<int>(item.itemcount()),
						"item_count"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::UpdateMercenary(
		DB::QueryScopeTran& tran,
		int usn, const Mercenary& mercenary)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"UPDATE mercenary SET "
					"mercenary_name=:mercenary_name, "
					"mercenary_level=:mercenary_level "
					"WHERE usn=:usn",
					soci::use(mercenary.mercenaryname(), 
						"mercenary_name"),
					soci::use(static_cast<int>(mercenary.mercenarylevel()), 
						"mercenary_level"),
					soci::use(usn, "usn"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::InsertMercenary(
		DB::QueryScopeTran& tran,
		int usn, const Mercenary& mercenary)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"INSERT INTO mercenary(usn, mercenary_name, "
					"mercenary_level) VALUES(:usn, "
					":mercenary_name, :mercenary_level)",
					soci::use(usn, "usn"),
					soci::use(mercenary.mercenaryname(),
						"mercenary_name"),
					soci::use(static_cast<int>(mercenary.mercenarylevel()),
						"mercenary_level"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::UpdateStage(
		DB::QueryScopeTran& tran, 
		int usn, const Stage& stage)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"UPDATE stage SET "
					"stage_num=:stage_num, "
					"stage_score=:stage_score "
					"WHERE usn=:usn",
					soci::use(static_cast<int>(stage.stagenum()),
						"stage_num"),
					soci::use(static_cast<int>(stage.stagescore()),
						"stage_score"),
					soci::use(usn, "usn"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::InsertStage(
		DB::QueryScopeTran& tran,
		int usn, const Stage& stage)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"INSERT INTO stage(usn, stage_num, "
					"stage_score) VALUES(:usn, "
					":stage_num, :stage_score)",
					soci::use(usn, "usn"),
					soci::use(static_cast<int>(stage.stagenum()),
						"stage_num"),
					soci::use(static_cast<int>(stage.stagescore()),
						"stage_score"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::UpdateUpgradePlayer(DB::QueryScopeTran& tran, 
		int usn, const UpgradePlayer& upgradePlayer)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"UPDATE upgrade_player SET "
					"power_level=:power_level, "
					"attack_speed_level=:attack_speed_level, "
					"critical_level=:critical_level, "
					"buff_duration_level=:buff_duration_level, "
					"free_coin_level=:free_coin_level "
					"WHERE usn=:usn",
					soci::use(static_cast<int>(upgradePlayer.powerlevel()),
						"power_level"),
					soci::use(static_cast<int>(upgradePlayer.attackspeedlevel()),
						"attack_speed_level"),
					soci::use(static_cast<int>(upgradePlayer.criticallevel()),
						"critical_level"),
					soci::use(static_cast<int>(upgradePlayer.buffdurationlevel()),
						"buff_duration_level"),
					soci::use(static_cast<int>(upgradePlayer.freecoinlevel()),
						"free_coin_level"),
					soci::use(usn, "usn"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerSaveGameData::InsertUpgradePlayer(DB::QueryScopeTran& tran,
		int usn, const UpgradePlayer& upgradePlayer)
	{
		bool queryResult = tran.Execute([&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"INSERT INTO upgrade_player(usn, power_level, "
					"attack_speed_level, critical_level, "
					"buff_duration_level, free_coin_level) VALUES("
					":usn, :power_level, :attack_speed_level, "
					":critical_level, :buff_duration_level, "
					":free_coin_level)",
					soci::use(usn, "usn"),
					soci::use(static_cast<int>(upgradePlayer.powerlevel()),
						"power_level"),
					soci::use(static_cast<int>(upgradePlayer.attackspeedlevel()),
						"attack_speed_level"),
					soci::use(static_cast<int>(upgradePlayer.criticallevel()),
						"critical_level"),
					soci::use(static_cast<int>(upgradePlayer.buffdurationlevel()),
						"buff_duration_level"),
					soci::use(static_cast<int>(upgradePlayer.freecoinlevel()),
						"free_coin_level"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}
}