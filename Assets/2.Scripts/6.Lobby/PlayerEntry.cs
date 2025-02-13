﻿using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using TMPro;

public class PlayerEntry : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerNameText;
    public Image characterPanel;
    [Header("Character")]
    public GameObject characterSet;
    [HideInInspector]
    public int characterIndex;
    [HideInInspector]
    public int characterDataSize;
    public Button rightClickButton;
    public Button leftClickButton;
    

    [Header("Ready")]
    public GameObject readyImage;

    [Header("Icon")]
    public GameObject masterIcon;
    public GameObject localIcon;

    private int ownerId;
    private bool isPlayerReady;

    private void Start()
    {
        characterDataSize = 18;
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            rightClickButton.gameObject.SetActive(false);
            leftClickButton.gameObject.SetActive(false);
        }
        else
        {
            object value;
            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameData.PLAYER_INDEX, out value))
            {
                characterIndex = (int)value;
            }
            else
            {
                characterIndex = Random.Range(0, characterDataSize);
            }
            SetPlayerCharacter(characterIndex);
            Hashtable props = new Hashtable() { { GameData.PLAYER_INDEX, characterIndex } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

    }

    public void OnInfoButtonClicked()
    {
        
        DBManager.Instance.GetUserIDbyNickName(playerNameText.text,(str)=>{
            
            if(str == "nullString")
            {
                LobbyManager.instance.ShowError("게스트 계정의 프로필 정보는 제공되지 않습니다");
                
            }
            else
            {
                LobbyManager.instance.ShowPlayerInfo(str);
            }

        });
        
        //게스트계정 클릭 시 => 게스트 계정의 정보는 제공되지 않습니다.
    }
    public void OnRightButtonClicked()
    {
        if (isPlayerReady) return;

        ++characterIndex;
        if (characterIndex >= characterDataSize) characterIndex = 0;
        SetPlayerCharacter(characterIndex);
        Hashtable props = new Hashtable() { { GameData.PLAYER_INDEX, characterIndex } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }
    public void OnLeftButtonClicked()
    {
        if (isPlayerReady) return;

        --characterIndex;
        if (characterIndex <= 0) characterIndex = characterDataSize - 1;
        SetPlayerCharacter(characterIndex);
        Hashtable props = new Hashtable() { { GameData.PLAYER_INDEX, characterIndex } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        playerNameText.text = playerName;
    }

    public void SetPlayerReadyImage(bool playerReady)
    {
        isPlayerReady = playerReady;
        readyImage.SetActive(playerReady);
    }

    public void SetPlayerCharacter(int index)
    {
        for (int i = 0; i < characterDataSize; ++i)
        {
            characterSet.transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject charac = characterSet.transform.GetChild(index).gameObject;
        charac.SetActive(true);
        DummyPlayer dummy = charac.GetComponent<DummyPlayer>();
        int randomAnim = Random.Range(1, 4);
        switch (randomAnim)
        {
            case 1: dummy.anim.SetTrigger("Wave Hand"); break;
            case 2: dummy.anim.SetTrigger("Clapping"); break;
            case 3: dummy.anim.SetTrigger("Victory"); break;
        }

    }
     
}
