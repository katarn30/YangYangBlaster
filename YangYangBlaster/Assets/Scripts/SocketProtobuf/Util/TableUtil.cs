using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableUtil
{
    
    public static int[] LoadInts(string data)
    {
        int[] decodeData = FromJson<int[]>(data);
        return decodeData;
    }


    public static Dictionary<string,int> LoadDictString2Int(string data)
    {
        Dictionary<string, int> decodeData = JsonUtility.FromJson<Dictionary<string, int>>(data);
        return decodeData;
    }

    /// <summary> Json해석 </summary>
    /// <typeparam name="T">유형</typeparam>
    /// <param name="json">Json문자열</param>
    public static T FromJson<T>(string json)
    {
        if (json == "null" && typeof(T).IsClass) return default(T);

        if (typeof(T).GetInterface("IList") != null)
        {
            json = "{\"data\":{data}}".Replace("{data}", json);
            Pack<T> Pack = JsonUtility.FromJson<Pack<T>>(json);
            return Pack.data;
        }

        return JsonUtility.FromJson<T>(json);
    }

    /// <summary> 팩 </summary>
    private class Pack<T>
    {
        public T data;
    }
}
