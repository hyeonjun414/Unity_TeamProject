﻿using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class PlayerResultInfo
{
    public PlayerResultInfo(int actorNumber, string name, int kill, int death, int score, int index)
    {
        this.actorNumber = actorNumber;
        this.name = name;
        this.kill = kill;
        this.death = death;
        this.score = score;
        this.index = index;
    }
    public int actorNumber;
    public string name;
    public int kill;
    public int death;
    public int score;
    public int index;
}
public class ResultSceneManager : MonoBehaviour
{
    private int _syncFinished = 0;
    public int syncFinished
    {
        get
        {
            return _syncFinished;
        }
        set
        {
            _syncFinished = value;
            Debug.Log(_syncFinished + "명의 플레이어 정보 동기화 완료");
        }
    }
    public Transform[] WinnersSpawnPoint;
    public Transform[] LosersSpawnPoint;
    public GameObject[] skeletons;

    public ResultUnit resultUnitPrefab;
    public Transform resultContentPos;
    public List<ResultUnit> resultUnits;

    List<PlayerResultInfo> resultInfoList;
    private void Awake()
    {
        CreateResultUnit();
        InitPlayers();
        SetInformation();
        SetPlayer();

    }
    private void CreateResultUnit()
    {
        resultInfoList = new List<PlayerResultInfo>();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            resultUnits.Add(Instantiate(resultUnitPrefab, resultContentPos));
        }
    }

    // 이전 플레이어 정보 갱신
    private void InitPlayers()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object kill;
            p.CustomProperties.TryGetValue(GameData.PLAYER_KILL, out kill);

            object death;
            p.CustomProperties.TryGetValue(GameData.PLAYER_DEAD, out death);

            object score;
            p.CustomProperties.TryGetValue(GameData.PLAYER_SCORE, out score);

            object index;
            p.CustomProperties.TryGetValue(GameData.PLAYER_INDEX, out index);

            int _kill = (int)kill;
            int _death = (int)death;
            int _score = (int)score;
            int _index = (int)index;
            resultInfoList.Add(new PlayerResultInfo(p.ActorNumber, p.NickName, _kill, _death, _score, _index));
        }

        // 전부 갱신하고 리스트를 정렬함.
        resultInfoList = resultInfoList.OrderByDescending(x => x.score).ToList();
    }
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        StartCoroutine(LeaveAndReturnToRoom());

        StartCoroutine(WaitForReadyToWrite());

    }

    private void SetInformation()
    {
        for(int i = 0; i< resultUnits.Count; i++)
        {
            resultUnits[i].SetUp(resultInfoList[i], i+1);
        }
    }
    private void SetPlayer()
    {
        List<PlayerResultInfo> infos = resultInfoList;

        for (int i = 0; i < resultInfoList.Count; ++i)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("CharactersEmpty/Character");
            int index = resultInfoList[i].index + 1;
            if (resultInfoList[i].index + 1 == 19) index = 1;
            builder.Append((index).ToString("D2"));
            string dummyPlayer = builder.ToString();
            DummyPlayer player = null;
            if (i == 0)
            {
                player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), WinnersSpawnPoint[0]);
                player.gameObject.SetActive(true);
                StartCoroutine(PlayerAnimPlay(player, "Victory"));
            }
            else
            {
                player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), LosersSpawnPoint[i - 1]);
                player.gameObject.SetActive(true);
                skeletons[i - 1].gameObject.SetActive(true);
                StartCoroutine(PlayerAnimPlay(player, "Crying"));
            }
        }

    }
    private void SyncUpdatedInformation(string nickName)
    {

        Debug.Log("0번성공");
        DBManager.Instance.GetUserIDbyNickName(nickName, (str) =>
        {
            Debug.Log("1번성공");
            DBManager.Instance.ReadPlayerInfo(str, true, (str2) =>
            {
                Debug.Log("2번성공 : " + str + " str2: " + str2);

                String value = str2;

                string[] words = value.Split('$');
                //이메일 $ 닉네임 $ 총판수 $ 승리수
                for (int i = 0; i < resultInfoList.Count; ++i)
                {
                    Debug.Log("resultInfoList.count: " + resultInfoList.Count + " i: " + i);
                    Debug.Log(resultInfoList[i].name + " :: " + words[1]);
                    if (resultInfoList[i].name == words[1])
                    {
                        Debug.Log("resultInfoList[i].rank: " + resultInfoList[i].score);
                        if (resultInfoList[i].score == 1)
                        {
                            Debug.Log("2.5번성공; 승리카운트 올라가야함");
                            int wins = int.Parse(words[3]);
                            ++wins;
                            words[3] = wins.ToString();
                        }
                        int totals = int.Parse(words[2]);
                        Debug.Log("totals: " + totals + " words[2]: " + words[2]);
                        ++totals;
                        words[2] = totals.ToString();
                        Debug.Log("3번성공");
                        DBManager.Instance.WriteExistingPlayerDB(str, words[0], words[1], words[2], words[3]);
                        Debug.Log("4번성공");
                    }
                }

                Debug.Log("5번성공");
                ++syncFinished;
                Debug.Log("6번성공");
            });
        });
    }
    private void WriteDB()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isMail;
            if(p.CustomProperties.TryGetValue(GameData.IS_EMAIL, out isMail))
            {
                if (p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) continue;
                //자기 데이터는 자기가 쓰도록 수정
                if ((bool)isMail)
                {
                    SyncUpdatedInformation(p.NickName);
                }
            }
        }
        Debug.Log("resultInfoList.Count: " + resultInfoList.Count);
        for (int i = 0; i < resultInfoList.Count; ++i)
        {
            Debug.Log(resultInfoList[i].name + " : " + resultInfoList[i].kill + " : " + resultInfoList[i].death + " : " + resultInfoList[i].score);
        }
    }


    private void ResetCustomProperties()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {

            Hashtable prop1 = new Hashtable() { { GameData.PLAYER_READY, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            Hashtable prop2 = new Hashtable() { { GameData.PLAYER_KILL, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            Hashtable prop3 = new Hashtable() { { GameData.PLAYER_DEAD, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            Hashtable prop4 = new Hashtable() { { GameData.PLAYER_SCORE, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            Hashtable prop5 = new Hashtable() { { GameData.PLAYER_LOAD, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop5);



        }

    }
    IEnumerator WaitForReadyToWrite()
    {
        yield return new WaitForSeconds(4f);
        WriteDB();

    }
    // 플레이어 승리, 패배 애니메이션 재생
    IEnumerator PlayerAnimPlay(DummyPlayer player, string whatToPlay)
    {
        yield return new WaitForSeconds(0.5f);
        player.anim.Play(whatToPlay);
    }
    IEnumerator LeaveAndReturnToRoom()
    {
        yield return new WaitForSeconds(10f);
        ResetCustomProperties();
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            
            PhotonNetwork.LoadLevel("NewLobbyScene");

        }



    }





}
