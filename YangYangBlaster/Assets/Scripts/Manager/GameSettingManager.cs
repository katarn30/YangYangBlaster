using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingManager : SingleTon<GameSettingManager>
{
    [Header("Sound & Vibration")]
    public bool isMute = false;
    public bool isVibration = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }


    public void SetSoundMute(bool _isMute)
    {
        isMute = _isMute;

        SoundManager.Instance.BGMMute(isMute);
        SoundManager.Instance.OtherMute(isMute);
    }

    public void SetVibration(bool _isVibration)
    {
        isVibration = _isVibration;
    }
}
