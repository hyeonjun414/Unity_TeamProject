﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class BattleManager : MonoBehaviourPun
{
    [HideInInspector]
    public int isReadyCount = 0;
    public bool isResultButtonClicked = false;
    public BattleResultPanel battleResultPanel;

    public List<Character> players;

    [Header("Player")]

    //살아있는 플레이어 
    public List<Character> alivePlayer;
    //사망한 플레이어
    public List<Character> deadPlayer;

    [Header("Text")]
    public Text resultText;
    public GameObject resultTextObj;

    [Header("UI")]
    public RegenUI regenUI;

    [Header("Mode")]
    public ModeType mode;
  
    public static BattleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        

        
    }
    private void Start()
    {
        battleResultPanel = UIManager.Instance.battleResultPanel;
    }

    public void SetUpMode()
    {
        object modeData;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MODE, out modeData))
        {
            mode = (ModeType)modeData;
            switch (mode)
            {
                case ModeType.LastFighter:
                    SetUpDeathMatch();
                    break;
                case ModeType.OneShot:
                    SetUpOneShotMode();
                    break;
                case ModeType.TimeToKill:
                    SetUpTimerMode();
                    break;
            }
        }
    }
    
    private void Update() 
    {
        ShowBattleStatus();

    }
    public void ShowBattleStatus()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            battleResultPanel.gameObject.SetActive(!battleResultPanel.gameObject.activeSelf);
        }
    }

    public void SetUpDeathMatch()
    {
        //HUDUIManager.Instance.DeathMatch();

    }
    
    public void SetUpOneShotMode()
    {
            //HUDUIManager.Instance.OnShotMatch();

        // 한대 맞으면 죽는 데스매치
        foreach(Character p in players)
        {
            p.stat.hp = 1;
        }
    }
    public void SetUpTimerMode()
    {

        //HUDUIManager.Instance.TimerMatch();
    //    TimeManager.Instance.limitTime = 180f;

        foreach (Character p in players)
        {
            p.isRegen = true;

        }
     //   TimeManager.Instance.TimeOver();


    }

    public void RegisterAllPlayer()
    {
        players = FindObjectsOfType<Character>().ToList();

        
        

        alivePlayer = FindObjectsOfType<Character>().ToList();


        // 등록이 완료되었다면 모드 설정에 따라 설정을 진행한다.
        SetUpMode();

    }



    //플레이어가 죽었을 때 판정 || 플레이어가 disconnect되었을 때 호출

    public void PlayerOut(Character deadPL)
    {
        // 시간제 게임일 경우 계산안함.
        if (mode == ModeType.TimeToKill) return;
        //alivePlayer 리스트에서 죽은 플레이어를 뺀다.
        alivePlayer.Remove(deadPL);

        Debug.Log("playerOut: "+deadPL.playerId);
        
        //deadPlayer 리스트에 죽은 플레이어를 더해준다.
        deadPlayer.Add(deadPL);

        if(alivePlayer.Count ==1)
        {
            photonView.RPC("BattleOverMessage", RpcTarget.All);
            StartCoroutine(GameOver());
        }
        
    }

    [PunRPC]
    public void BattleOverMessage()
    {
        resultTextObj.SetActive(true);

        string myNickName = PhotonNetwork.LocalPlayer.NickName;

        //플레이어가 죽지 않았을 때
        foreach(Character player in alivePlayer){
            if(player.nickName == myNickName)
            {
                resultText.text = "YOU WIN!";
                return;
            }
       }
        //플레이어가 죽었을 때
        foreach(Character player in deadPlayer){
            if(player.nickName == myNickName)
            {
                resultText.text = "YOU LOSE!";
                return;
            }
        }

        
        
    }
    private void SetBattleResult()
    {
    }

    IEnumerator GameOver(){
        yield return new WaitForSeconds(3f);  
        
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            
            if(alivePlayer[0].photonView.Owner.ActorNumber == p.ActorNumber)
            {
                Debug.Log("alive: "+p.NickName+": "+p.GetScore()); 
                Debug.Log("alivePlayer[0]"+alivePlayer[0].nickName+": "+alivePlayer[0].stat.deathCount);   
            }
            for(int i=0; i<deadPlayer.Count;++i)
            {
 
                if(deadPlayer[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    Debug.Log("dead: "+p.NickName+": "+p.GetScore());
                    Debug.Log("deadPlayer[i]"+deadPlayer[i].nickName+": "+deadPlayer[i].stat.deathCount);
                }
            }
            
        }
        SetBattleResult();

        yield return new WaitForSeconds(5f);
        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.LoadLevel("Result");
        }
        

    }
    //플레이어가 한 명 남았을 때 그라운드를 끝냄.

    public void FinalWinner(){
        //if(alivePlayer.Count == 1){
            //각 플레이어들에게 메시지를 보냄.
            //BattleOverMessage();
            Debug.Log("게임이 끝났습니다.");
            photonView.RPC("BattleOverMessage", RpcTarget.All);

        //}
    }



    IEnumerator TimeOver(){
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LoadLevel("Result");

      
    }

    public bool CheckPlayersTimeOver(){
        if(!PhotonNetwork.IsMasterClient){
            return false;
        }

        foreach(Player p in PhotonNetwork.PlayerList){
                //플레이어의 타이머가 0 이하가 되면 종료 및 다른 씬으로 전환
        }

        return true;
    }
}
