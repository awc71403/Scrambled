﻿using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileShop : MonoBehaviour
{
    #region Structure
    [System.Serializable]
    public struct PoolRate {
        public int oneRate;
        public int twoRate;
        public int threeRate;
        public int fourRate;
        public int fiveRate;
    }
    #endregion 

    #region Variables
    public static TileShop m_singleton;

    public List<BuyTile> m_buyTiles;

    [SerializeField]
    private TextMeshProUGUI m_moneyText;
    [SerializeField]
    private Button m_refreshButton;
    [SerializeField]
    private Button m_expButton;
    [SerializeField]
    private SwitchManager m_switch;

    private TileDatabaseSO m_tileDatabase;

    private Dictionary<int, List<int>> m_poolDictionary;

    [SerializeField]
    private PoolRate[] m_poolRates;

    private const int onePoolSize = 60;
    private const int twoPoolSize = 40;
    private const int threePoolSize = 40;
    private const int fourPoolSize = 20;
    private const int fivePoolSize = 20;
    #endregion

    #region Initialization
    private void Awake() {
        m_singleton = this;
    }

    private void Start() {
        m_tileDatabase = GameManager.m_singleton.m_tileDatabase;
        SetupShop();
        GenerateTiles();
    }

    public void GenerateTiles() {
        for (int i = 0; i < m_buyTiles.Count; i++) {
            //m_buyTiles[i].Setup(m_tileDatabase.allTiles[Random.Range(0, m_tileDatabase.allTiles.Count)], this);
            m_buyTiles[i].gameObject.SetActive(true);
        }
    }

    public void SetupShop() {
        List<int> onePool = new List<int>();
        List<int> twoPool = new List<int>();
        List<int> threePool = new List<int>();
        List<int> fourPool = new List<int>();
        List<int> fivePool = new List<int>();

        m_poolDictionary = new Dictionary<int, List<int>>();

        foreach (TileDatabaseSO.TileData tile in TileDatabaseSO.allTiles) {
            switch (tile.m_cost) {
                case 1:
                    for (int i = 0; i < onePoolSize; i++) {
                        onePool.Add(tile.m_ID);
                    }
                    break;
                case 2:
                    for (int i = 0; i < twoPoolSize; i++) {
                        twoPool.Add(tile.m_ID);
                    }
                    break;
                case 3:
                    for (int i = 0; i < threePoolSize; i++) {
                        threePool.Add(tile.m_ID);
                    }
                    break;
                case 4:
                    for (int i = 0; i < fourPoolSize; i++) {
                        fourPool.Add(tile.m_ID);
                    }
                    break;
                case 5:

                    for (int i = 0; i < fivePoolSize; i++) {
                        fivePool.Add(tile.m_ID);
                    }
                    break;
            }
        }

        m_poolDictionary.Add(1, onePool);
        m_poolDictionary.Add(2, twoPool);
        m_poolDictionary.Add(3, threePool);
        m_poolDictionary.Add(4, fourPool);
        m_poolDictionary.Add(5, fivePool);
    }
    #endregion

    #region Update
    private void Update() {
        
    }
    #endregion

    #region Getter
    public TextMeshProUGUI GetMoneyText {
        get { return m_moneyText; }
    }
    #endregion

    #region Shop
    private int Roll() {
        int level = PlayerManager.m_localPlayer.GetLevel;
        PoolRate rates = m_poolRates[level - 1];

        int chosenCost;
        int random = Random.Range(1, 100);
        if (random <= rates.oneRate) {
            chosenCost = 1;
        }
        else if (random <= rates.oneRate + rates.twoRate) {
            chosenCost = 2;
        }
        else if (random <= rates.oneRate + rates.twoRate + rates.threeRate) {
            chosenCost = 3;
        }
        else if (random <= rates.oneRate + rates.twoRate + rates.threeRate + rates.fourRate) {
            chosenCost = 4;
        }
        else {
            chosenCost = 5;
        }
        return 0;
    }

    public void BuyTile(TileDatabaseSO.TileData cardData) {
        PlayerManager.m_localPlayer.OnBoughtTile(cardData);
    }

    public void CanRefresh() {
        if (PlayerManager.m_localPlayer.GetMoney < 2) {
            m_refreshButton.interactable = false;
        }
        else {
            m_refreshButton.interactable = true;
        }
    }

    public void TurnRefresh() {
        if (m_switch.isOn) {
            m_switch.AnimateSwitch();
            return;
        }
        GenerateTiles();
    }

    public void Refresh() {
        PlayerManager.m_localPlayer.UsedRefresh();
        GenerateTiles();
    }
    #endregion
}
