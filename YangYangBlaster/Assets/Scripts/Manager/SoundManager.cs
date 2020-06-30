using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleTon<SoundManager>
{
    [Header("Lobby Sound")]
    public List<AudioClip> lobbyBGMList = new List<AudioClip>();

    [Header("Ingame Sound")]
    public List<AudioClip> ingameBGMList = new List<AudioClip>();

    [Header("Ingame Boss Sound")]
    public List<AudioClip> ingameBossBGMList = new List<AudioClip>();

    [Header("Player Sound")]
    public AudioClip playerBulletSound;

    [Header("Monster Sound")]
    public AudioClip monsterDeadSound;

    [Header("ETC Sound")]
    public List<AudioClip> coinSoundList = new List<AudioClip>();

    [Header("Audio Source")]
    public AudioSource bgm;
    public AudioSource playerSound;
    public AudioSource monsterSound;
    public AudioSource etcSound;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    #region BGM
    #region LobbySound
    public void LobbyBGMSound()
    {
        int lobbyBGMRand = Random.Range(0, lobbyBGMList.Count);

        bgm.clip = lobbyBGMList[lobbyBGMRand];
        bgm.loop = true;
        bgm.Play();
    }
    #endregion

    #region IngameSound
    public void StageBGMSound()
    {
        int ingameBGMRand = Random.Range(0, ingameBGMList.Count);

        bgm.clip = ingameBGMList[ingameBGMRand];
        bgm.loop = true;
        bgm.Play();
    }

    public void StageBossBGMSound()
    {
        int ingameBossBGMRand = Random.Range(0, ingameBossBGMList.Count);

        bgm.clip = ingameBossBGMList[ingameBossBGMRand];
        bgm.loop = true;
        bgm.Play();
    }
    #endregion
    #endregion

    #region Player
    public void PlayerBulletSound()
    {
        playerSound.clip = playerBulletSound;
        playerSound.loop = false;
        playerSound.Play();
    }
    #endregion

    #region Monster
    public void MonsterDeadSound()
    {
        monsterSound.clip = monsterDeadSound;
        monsterSound.loop = false;
        monsterSound.Play();
    }
    #endregion

    #region ETC
    public void CoinSound()
    {
        int coinSoundRand = Random.Range(0, coinSoundList.Count);

        etcSound.clip = coinSoundList[coinSoundRand];
        etcSound.loop = false;
        etcSound.Play();
    }
    #endregion

    #region GameMute
    public void BGMMute(bool _mute)
    {
        bgm.mute = _mute;
    }

    public void OtherMute(bool _mute)
    {
        playerSound.mute = _mute;
        monsterSound.mute = _mute;
        etcSound.mute = _mute;
    }
    #endregion
}
