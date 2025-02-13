﻿using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class AuthManager : Singleton<AuthManager>
{
    public bool isFireBaseReady {get;  private set;}
    public bool isSignInOnProgress {get; private set;}
    public TMP_InputField idField;
    public TMP_InputField passwordField;
    public Button signInBtn;
    public FirebaseApp firebaseApp;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser user;
    public GameObject errorTextPanel;
    public GameObject loginPanel;


    [Header("SignInPanel")]
    public GameObject signInPanel;
    public TMP_InputField nickNameField;
    public TMP_InputField idCreateField;
    public TMP_InputField passwordCreateField;
    public TMP_InputField passwordCreateFieldConfirm;
    public Button createIDBtn;
    public Button cancelSignInPanelBtn;
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }
    private void Start()
    {
        SoundManager.Instance.TitleRoomBGM();
        CheckAvailableFirebase();
    }
    public void CheckAvailableFirebase()
    {
        signInBtn.interactable = false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var result = task.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.LogError(message: result.ToString());
                isFireBaseReady = false;
            }
            else
            {
                isFireBaseReady = true;
                firebaseApp = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            signInBtn.interactable = isFireBaseReady;
        });
    }
    public void LogInWithEmail()
    {
        if(!isFireBaseReady ||  isSignInOnProgress || user !=null)
        {
            return;
        }

        isSignInOnProgress = true;
        signInBtn.interactable = false;

        firebaseAuth.SignInWithEmailAndPasswordAsync(idField.text,passwordField.text).
            ContinueWithOnMainThread((task=>
            {
                isSignInOnProgress = false;
                signInBtn.interactable=true;


                if(task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                    StartCoroutine(ErrorMessage("잘못된 입력입니다. ID와 비밀번호를 확인해주세요"));//task.Exception.ToString()));
                }
                else if(task.IsCanceled)
                {
                    Debug.LogError(message:"Sign-in canceled");
                }
                else
                {
                    user = task.Result;
                    DBManager.isLoginEmail=true;
                    DBManager.Instance.GetUserID(idField.text,(str)=>{
                        DBManager.userID = str;
                        LobbyManager.instance.loginPanel.NickNameSet();
                        
                    });
                    StartCoroutine(ErrorMessage(user.Email+"확인"));

                    ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable() { { GameData.IS_EMAIL, true } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
                    Debug.Log(user.Email);
                    
                    SceneManager.LoadScene("NewLobbyScene");
                }

            }));
    }

    public void LogInAnonymously()
    {
        if(!isFireBaseReady ||  isSignInOnProgress || user !=null)
        {
            return;
        }
        isSignInOnProgress = true;
        signInBtn.interactable = false;

        firebaseAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task => {

            isSignInOnProgress = false;
            signInBtn.interactable=true;

            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            else if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            else
            {
                ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable() { { GameData.IS_EMAIL, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

                SceneManager.LoadScene("NewLobbyScene");
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            }


        });
    }

    public void NickNameCheck()
    {
        if(passwordCreateField.text.ToString() != passwordCreateFieldConfirm.text.ToString())
        {
            StartCoroutine(ErrorMessage("비밀번호를 다시 확인해주세요"));
             return;
        }

            DBManager.Instance.NickNameDuplicateCheck(nickNameField.text,(str)=>{
                if(str == "nullString")
                {
                    CreateUserId();
                }
                else
                {
                    StartCoroutine(ErrorMessage("이미 사용중인 닉네임입니다"));
                }
            });

    }

    public void CreateUserId()
    {

        firebaseAuth.CreateUserWithEmailAndPasswordAsync(idCreateField.text,passwordCreateField.text)
            .ContinueWithOnMainThread(task=>
            {
                if(task.IsCanceled)
                {
                    Debug.LogError("task Canceled");
                    return;
                }
                else if(task.IsFaulted)
                {
                    StartCoroutine(ErrorMessage("제대로 된 입력이 아닙니다"));
                    Debug.Log("task is Faulted"+task.Exception);

                    return;
                }
                else//성공
                {
                    //userCreated
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("Firebase user created successfully: {0}({1})",
                        newUser.DisplayName,newUser.UserId);
                    
                    DBManager.Instance.WriteNewPlayerDB(newUser.UserId,idCreateField.text,nickNameField.text);
                    
                    signInPanel.SetActive(false);

                    StartCoroutine(ErrorMessage("가입이 완료되었습니다"));
                    idField.text = idCreateField.text;

                    nickNameField.text = "";
                    idCreateField.text = "";
                    passwordCreateField.text = "";
                }

            });
    }

    public void SetSignInPanel()
    {
        signInPanel.SetActive(true);
        nickNameField.text = "";
        idField.text = "";
        passwordField.text = "";
    }
    public void CancelOnCreateIDPanel()
    {
        signInPanel.SetActive(false);
    }
    public IEnumerator ErrorMessage(string errorMessage)
    {
        TMP_Text error = errorTextPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        errorTextPanel.SetActive(true);
        loginPanel.SetActive(false);
        createIDBtn.interactable=false;
        cancelSignInPanelBtn.interactable=false;

        error.text = errorMessage;//errorMessage;
        yield return new WaitForSeconds(2.5f);
        errorTextPanel.SetActive(false);
        loginPanel.SetActive(true);
        createIDBtn.interactable=true;
        cancelSignInPanelBtn.interactable=true;
        error.text = "";
    }
}
