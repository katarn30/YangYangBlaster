#include "AsyncHandlerLoadGameData.h"
#include "GlobalDefine.h"
#include "Cache.h"
#include "DB.h"

namespace yyb
{
	void AsyncHandlerLoadGameData::OnRead(
		const GameDataRequest& request,
		GameDataReply& reply)
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;

		user_ptr user = GetUser();
		if (user)
		{
			std::vector<Item> items;

			if (false == SelectItem(user->GetUsn(), items))
			{
				reply.set_error(ERROR_CODE_FAILED_TO_LOAD_ITEM);
				return;
			}

			std::vector<Mercenary> mercenaries;

			if (false == SelectMercenary(user->GetUsn(),
				mercenaries))
			{
				reply.set_error(ERROR_CODE_FAILED_TO_LOAD_MERCENARY);
				return;
			}

			Stage stage;

			if (false == SelectStage(user->GetUsn(), stage))
			{
				reply.set_error(ERROR_CODE_FAILED_TO_LOAD_STAGE);
				return;
			}

			UpgradePlayer upgradePlayer;

			if (false == SelectUpgradePlayer(user->GetUsn(),
				upgradePlayer))
			{
				reply.set_error(ERROR_CODE_FAILED_TO_LOAD_UPGRADE_PLAYER);
				return;
			}

			for (const auto& item : items)
			{
				reply.add_items()->CopyFrom(item);
			}

			for (const auto& mercenary : mercenaries)
			{
				reply.add_mercenaries()->CopyFrom(mercenary);
			}

			reply.mutable_stage()->CopyFrom(stage);
			reply.mutable_upgradeplayer()->CopyFrom(upgradePlayer);
			return;
		}
	}

	void AsyncHandlerLoadGameData::OnWrite()
	{

	}

	bool AsyncHandlerLoadGameData::SelectItem(int usn, 
		OUT std::vector<Item>& outItems)
	{
		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				soci::rowset<soci::row> rs
				{ 
					(sql.prepare << "SELECT item_name, item_type, "
					"item_category, item_count "
					"FROM item "
					"WHERE usn=:usn", soci::use(usn, "usn"))
				};

				for (const auto& r: rs)
				{
					Item item;

					if (r.get_indicator("item_name") != soci::i_null)
					{
						item.set_itemname(r.get<std::string>("item_name"));
					}
					if (r.get_indicator("item_type") != soci::i_null)
					{
						item.set_itemtype(
							static_cast<ITEM_TYPE>(
								r.get<long long>("item_type")));
					}
					if (r.get_indicator("item_category") != soci::i_null)
					{
						item.set_itemcategory(
							static_cast<ITEM_CATEGORY>(
								r.get<long long>("item_category")));
					}
					if (r.get_indicator("item_count") != soci::i_null)
					{
						item.set_itemcount(
							r.get<unsigned long long>("item_count"));
					}

					outItems.push_back(std::move(item));
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerLoadGameData::SelectMercenary(int usn, 
		OUT std::vector<Mercenary>& outMercenaries)
	{
		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				soci::rowset<soci::row> rs
				{
					(sql.prepare << "SELECT mercenary_name, "
					"mercenary_level "
					"FROM mercenary "
					"WHERE usn=:usn", soci::use(usn, "usn"))
				};

				for (const auto& r : rs)
				{
					Mercenary mercenary;

					if (r.get_indicator("mercenary_name") != soci::i_null)
					{
						mercenary.set_mercenaryname(
							r.get<std::string>("mercenary_name"));
					}
					if (r.get_indicator("mercenary_level") != soci::i_null)
					{
						mercenary.set_mercenarylevel(
							r.get<long long>("mercenary_level"));
					}

					outMercenaries.push_back(std::move(mercenary));
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerLoadGameData::SelectStage(int usn, 
		OUT Stage& outStage)
	{
		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				int stageNum = 0;
				int stageScore = 0;

				sql << "SELECT stage_num, stage_score "
					"FROM stage "
					"WHERE usn=:usn",
					soci::into(stageNum), 
					soci::into(stageScore), 
					soci::use(usn, "usn");

				outStage.set_stagenum(stageNum);
				outStage.set_stagescore(stageScore);

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerLoadGameData::SelectUpgradePlayer(int usn, 
		OUT UpgradePlayer& outUpgradePlayer)
	{
		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				int powerLevel = 0;
				int attackSpeedLevel = 0;
				int criticalLevel = 0;
				int buffDurationLevel = 0;
				int freeCoinLevel = 0;

				sql << "SELECT power_level, attack_speed_level, "
					"critical_level, buff_duration_level, "
					"free_coin_level "
					"FROM upgrade_player "
					"WHERE usn=:usn",
					soci::into(powerLevel),
					soci::into(attackSpeedLevel),
					soci::into(criticalLevel),
					soci::into(buffDurationLevel),
					soci::into(freeCoinLevel),
					soci::use(usn, "usn");

				outUpgradePlayer.set_powerlevel(powerLevel);
				outUpgradePlayer.set_attackspeedlevel(attackSpeedLevel);
				outUpgradePlayer.set_criticallevel(criticalLevel);
				outUpgradePlayer.set_buffdurationlevel(buffDurationLevel);
				outUpgradePlayer.set_freecoinlevel(freeCoinLevel);

				return true;
			});

		return queryResult;
	}
}