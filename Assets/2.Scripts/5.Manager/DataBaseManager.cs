﻿using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using System;

public class DataBaseManager : Singleton<DataBaseManager>
{
    string DBURL = "https://unity-team-project-trim-default-rtdb.firebaseio.com/";
    DatabaseReference reference;
    private void Awake()
    {
        if (_instance == null) _instance = this;
        
    }
    private void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBURL);


        //WriteDB();
        //ReadDB();
        //WriteNickName("epsqjqhdl@naver.com","abcde");
        Debug.Log(ReadNickName("epsqjqhdl@naver.com"));
    }

    public void WriteNickName(string emailID, string nickName)
    {
        //UserNickName name = new UserNickName(nickName);
        //string jsonData = JsonUtility.ToJson(name);
        //reference.Child("Users").Child(emailID).Child("username").SetValueAsync(nickName);
        //ContinueWith(task=>
        // {
        //     if(task.IsCompleted)
        //     {
        //         Debug.Log("saveSuccess");
        //     }
        //     else
        //     {
        //         Debug.Log("saveNotSuccess");
        //     }
        // });
        String value = emailID;
        string[] words = value.Split('.');
        value = words[0];
        reference.Child("USERS").Child(value).Child("UserNickname").SetValueAsync(nickName);
        //reference.Child("useruser").SetValueAsync(nickName);
    }
    public string ReadNickName(string emailID)
    {
        String key = emailID;
        string[] words = key.Split('.');
        key = words[0];
        Debug.Log(key);
        string returnValue=null;
        
        reference = FirebaseDatabase.DefaultInstance.RootReference;
       // var getValue = reference.EqualTo(value);
       // Query query;

        reference.Child(key)
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
            
                //Debug.Log(snapshot.Child(key).Child("UserNickname").Value);
                Debug.Log((snapshot.Child("UserNickName").Value).ToString());
            }
        });
        // FirebaseDatabase.DefaultInstance.GetReference("USERS").GetValueAsync().ContinueWithOnMainThread(task=>
        // {
        //     if(task.IsFaulted)
        //     {

        //     }
        //     else if(task.IsCompleted)
        //     {
        //         DataSnapshot snapshot = task.Result;
        //         for(int i=0; i<snapshot.ChildrenCount;++i)
        //         {
        //             snapshot.GetRawJsonValue().ToString();
        //         }
        //     }
        // });
        // reference.Child(key).GetValueAsync().ContinueWithOnMainThread(task=>
        // {
        //     if(task.IsCompleted)
        //     {
        //         Debug.Log("성공");
        //         DataSnapshot snapshot = task.Result;
        //         returnValue = snapshot.Value.ToString();
        //     }
        //     else
        //     {
        //         AuthManager.Instance.StartCoroutine(AuthManager.Instance.ErrorMessage("별명을 불러오지 못했습니다"));
                
        //     }
        // });
        
        
        return returnValue;
    }
    public class UserNickName
    {
        //public string 
        public string nickName;
        public UserNickName(string nickName)
        {
            //this.emailID = emailID;
            this.nickName = nickName;
        }
    }
}
