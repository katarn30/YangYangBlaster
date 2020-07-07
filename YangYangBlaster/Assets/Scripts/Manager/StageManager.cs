using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingleTon<StageManager>
{
    [Header("Stage Background")]
    public List<SpriteRenderer> stageUpList = new List<SpriteRenderer>();
    public List<SpriteRenderer> stageDownList = new List<SpriteRenderer>();

    [Header("Weather")]
    public GameObject rainEffect;
    public GameObject snowEffect;
    public GameObject windStormEffect;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyStage()
    {
        WeatherInit();
    }

    public void SetIngameStage()
    {
        SetWeather();
    }

    public void SetWeather()
    {
        WeatherInit();

        int rnd = Random.Range(0,4);

        if (rnd == 1)
        {
            rainEffect.SetActive(true);
        }
        else if (rnd == 2)
        {
            snowEffect.SetActive(true);
        }
        else if (rnd == 3)
        {
            windStormEffect.SetActive(true);
        }
    }

    void WeatherInit()
    {
        rainEffect.SetActive(false);
        snowEffect.SetActive(false);
        windStormEffect.SetActive(false);
    }
}
