#pragma once

namespace yyb
{
#define OUT

    enum CACHE_INDEX
    {
        CACHE_INDEX_GLOBAL,
    };

    enum DB_POOL_INDEX
    {
        DB_POOL_INDEX_GLOBAL,
        DB_POOL_INDEX_USER
    };

    enum MARKET_PLATFORM_TYPE
    {
        MARKET_PLATFORM_TYPE_DEV,
        MARKET_PLATFORM_TYPE_PLAY_STORE,
        MARKET_PLATFORM_TYPE_APP_STORE,
        MARKET_PLATFORM_TYPE_T_STORE,
    };

    const std::string CACHE_KEY_USER_ID_TOKEN = "user_id_token";
}