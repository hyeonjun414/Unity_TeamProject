﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviourPun
{
    //input 종류 - null(입력실패) , 움직임, 회전, 공격, 방어, 아이템사용, 슬롯체인지
    //null , 슬롯체인지는 영향없음
    //의식의 흐름()=> 매 노트카운트 끝부분 콜라이더에 닿으면 
    //InputCheckManager.Judge() 호출 => MoveJudge => ActualMovement => Judge 
    //=> 플레이어 키 입력 가능하게 초기화
    [HideInInspector]
    public int isReadyCount=0;

    public List<Character> players;


    [Header("Player")]
   
    //살아있는 플레이어 
    public List<Character> alivePlayer;
    //사망한 플레이어
    public List<Character> deadPlayer;


    [Header("Text")]

    public Text resultText;
    public GameObject resultTextObj;
    private void Start() {
    }



    public static BattleManager Instance {get;private set;}
    private void Awake()
    {
        if (Instance == null)  Instance = this;
   
    }

    private void Update() {
        FinalWinner();
    }



    public void RegisterAllPlayer()
    {
        players = FindObjectsOfType<Character>().ToList();

        //게임이 시작했을 때 들어온 모든 플레이어를 살아있는 플레이어 그룹에 넣는다.
        alivePlayer = FindObjectsOfType<Character>().ToList();
      
    }


    public void Judge()
    {
        if(PhotonNetwork.IsMasterClient)
        {

        }
        //CheckPlayersAvailability();
        //AttackJudge();
        //ItemJudge();

    }
    public void CheckPlayersAvailability()
    {
        // 플레이어 명령 기반
        // 입력이 없다면 NULL 

    }

    public void AttackJudge()
    {
        
    }
    public void ItemJudge()
    {
    }
    [PunRPC]
    public void ResetPlayers()
    {
        foreach(Character player in players)
        {
            //player.eCurInput = ePlayerInput.NULL;
        }
        RhythmManager.Instance.isBeat = true;
    }

    //플레이어가 죽었을 때 판정
    public void PlayerOut(Character deadPL){

        //alivePlayer 리스트에서 죽은 플레이어를 뺀다.
        alivePlayer.Remove(deadPL);
        //deadPlayer 리스트에 죽은 플레이어를 더해준다.
        deadPlayer.Add(deadPL);
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
    
    

    //플레이어가 한 명 남았을 때 그라운드를 끝냄.

    public void FinalWinner(){
        if(alivePlayer.Count == 1){
            //각 플레이어들에게 메시지를 보냄.
            //BattleOverMessage();
            Debug.Log("게임이 끝났습니다.");
            photonView.RPC("BattleOverMessage", RpcTarget.All);

        }
    }


    /*
    IEnumerator GameOver(){
        yield return new WaitForSeconds(3f);  
        SceneManager.LoadScene("NewLobbyScene");

    }

    */

}
