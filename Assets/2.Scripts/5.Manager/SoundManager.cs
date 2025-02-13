﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;


public class SoundManager : Singleton<SoundManager>
{

    public AudioMixer mixer;
    public AudioSource bgSound;
    public AudioClip[] bgSoundlist;

    public BGMDB inGameBgms;

    private void Awake()
    {
        if (_instance == null) _instance = this;

        var obj = FindObjectsOfType<SoundManager>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bgSoundlist.Length; i++)
        {
            if (arg0.name == bgSoundlist[i].name)
                BGSoundPlay(bgSoundlist[i], 1);
        }
    }

    public void TitleRoomBGM()
    {
        BGSoundPlay(bgSoundlist[1], 0);
    }

    public void PlayerRoomBGM()
    {
        BGSoundPlay(bgSoundlist[0], 0);
    }

    public void BGSoundVolume(float volume)
    {
        mixer.SetFloat("BGSoundVolume", Mathf.Log10(volume) * 20);
    }

    public void SFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void BGSoundPlay(AudioClip clip, float delay)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.2f;
        Invoke("Play", delay);
    }

    public void Play()
    {
        bgSound.Play();
    }
    public void Stop()
    {
        bgSound.Stop();
    }

    public int RandomBGMIndex()
    {
        return Random.Range(0, inGameBgms.bgms.Length);
    }
}
