using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionUIController : MonoBehaviour
{
    [Header("SOUND")]
    public Image soundToggleOn;
    public Image soundToggleOff;

    [Header("VIBRATION")]
    public Image vibrationToggleOn;
    public Image vibrationToggleOff;

    public void SoundToggleButton(int _num)
    {
        if (_num == 0)
        {
            soundToggleOn.gameObject.SetActive(true);
            soundToggleOff.gameObject.SetActive(false);

            GameSettingManager.Instance.SetSoundMute(false);
        }
        else if (_num == 1)
        {
            soundToggleOn.gameObject.SetActive(false);
            soundToggleOff.gameObject.SetActive(true);

            GameSettingManager.Instance.SetSoundMute(true);
        }
    }

    public void VibrationToggleButton(int _num)
    {
        if (_num == 0)
        {
            vibrationToggleOn.gameObject.SetActive(true);
            vibrationToggleOff.gameObject.SetActive(false);

            GameSettingManager.Instance.SetVibration(true);
        }
        else if (_num == 1)
        {
            vibrationToggleOn.gameObject.SetActive(false);
            vibrationToggleOff.gameObject.SetActive(true);

            GameSettingManager.Instance.SetVibration(false);
        }
    }

    public void QuitButton()
    {
        gameObject.SetActive(false);

        if (GameManager.Instance.state == GameManager.GameState.InGame)
        {
            GameManager.Instance.isPause = false;
        }
    }

    public void ExitLobbyButton()
    {
        GameManager.Instance.isPause = false;
        gameObject.SetActive(false);
        GameManager.Instance.ResultReward();
        GameManager.Instance.ChangeGameState(GameManager.GameState.Lobby);
    }
}
