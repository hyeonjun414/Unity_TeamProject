﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    [Header("HUD")]
    public PlayerStatusUI statusUI;
    public ItemSlotUI itemSlotUI;
    public TopTextUI topTextUI;

    [Header("Battle Result")]
    public BattleResultPanel battleResultPanel;
    public ResultTextUI resultTextUI;

    [Header("Revive UI")]
    public RegenUI regenUI;

    private void Awake()
    {
        if(_instance == null)
            _instance = this;
    }

}
