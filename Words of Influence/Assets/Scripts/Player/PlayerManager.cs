﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    private RoomManager m_roomManager;
    private Camera m_camera;
    private PhotonView m_PV;
    private int m_HP;
    private int m_money;

    private int m_winStreak;
    private int m_lossStreak;

    private int m_tilesInHand;
    private int m_ID;

    private TextMeshProUGUI m_moneyText;

    [SerializeField]
    private PlayerUIItem m_playerUIItemPrefab;
    private PlayerUIItem m_playerUIItem;

    [SerializeField]
    private Board m_boardPrefab;
    private Board m_board;

    public static PlayerManager m_localPlayer;

    public const int m_startingHP = 100;
    public const int m_startingMoney = 5;
    #endregion

    #region Initialization
    void Awake() {
        m_roomManager = RoomManager.m_singleton;
        m_camera = GetComponentInChildren<Camera>();
        m_PV = GetComponent<PhotonView>();

        m_HP = m_startingHP;
        m_money = m_startingMoney;

        m_tilesInHand = 0;

        if (m_PV.IsMine) {
            Debug.Log("I'm the local player!");
            m_localPlayer = this;
        }
    }

    void Start() {
        if (!m_PV.IsMine) {
            Destroy(m_camera.gameObject);
        }

        m_moneyText = TileShop.m_singleton.GetMoneyText;
        UpdateMoney();

        GameManager.m_singleton.AddPlayer(this);
        CreatePlayerUI();
        CreateBoard();
        SetPlayerManagerLocation();
    }

    private void CreatePlayerUI() {
        Debug.Log($"CreatePlayerUI called. Player {m_PV.Owner.NickName} has {m_HP} HP.");
        m_playerUIItem = Instantiate(m_playerUIItemPrefab);
        m_playerUIItem.SetUp(this);
        m_playerUIItem.gameObject.transform.SetParent(PlayerUIList.m_singleton.transform);
        m_playerUIItem.gameObject.transform.localScale = new Vector3(1, 1, 1);
        PlayerUIList.m_singleton.UpdateRanking();
    }

    private void CreateBoard() {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) {
            if (players[i].UserId == m_PV.Owner.UserId) {
                m_ID = i;
                m_board = Instantiate(m_boardPrefab);
                m_board.Setup(this);
                m_board.transform.position = SpawnManager.m_singleton.GetBoardSpawnPoint(i).transform.position;
            }
        }
    }

    private void SetPlayerManagerLocation() {
        Vector3 location = m_board.transform.position;
        location.z = location.z - 10;
        transform.position = location;
    }
    #endregion

    #region Getter/Setter
    public PhotonView GetPhotonView {
        get { return m_PV; }
    }

    public int GetHP {
        get { return m_HP; }
    }

    public int GetMoney {
        get { return m_money; }
    }

    public Camera GetCamera {
        get { return m_camera; }
    }

    public Board GetBoard {
        get { return m_board; }
    }

    public int TilesInHand {
        get { return m_tilesInHand; }
        set { m_tilesInHand = value; }
    }
    #endregion

    #region Shop
    public void OnBoughtTile(TileDatabaseSO.TileData tileData) {
        m_PV.RPC("RPC_BuyTile", RpcTarget.All, tileData.m_ID);
        UpdateMoney();
        //Update UI
    }

    public void Income() {
        int bonus = 0;
        //Interest
        bonus += Mathf.Min((m_money / 10), 5);
        //Win/Loss Streak
        if (m_lossStreak >= 2) {
            switch (m_lossStreak) {
                case 2:
                    bonus += 1;
                    break;
                case 3:
                    bonus += 1;
                    break;
                case 4:
                    bonus += 2;
                    break;
                default:
                    bonus += 3;
                    break;
            }
        }
        else if (m_winStreak >= 2) {
            switch (m_winStreak) {
                case 2:
                    bonus += 1;
                    break;
                case 3:
                    bonus += 1;
                    break;
                case 4:
                    bonus += 2;
                    break;
                default:
                    bonus += 3;
                    break;
            }
        }
        m_money += 5 + bonus;
        UpdateMoney();
    }
    #endregion

    #region UI
    private void UpdateMoney() {
        m_moneyText.text = m_money.ToString();
    }
    #endregion

    #region Player
    public void TakeDamage(int damage) {
        Debug.Log("TakeDamage called");
        m_PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    #endregion

    #region RPC
    [PunRPC]
    void RPC_TakeDamage(int damage) {
        m_HP -= damage;
        m_playerUIItem.UpdateHP(m_HP);
    }

    [PunRPC]
    void RPC_BuyTile(int ID) {
        TileDatabaseSO.TileData tileData = GameManager.m_singleton.m_tileDatabase.allTiles[ID];
        m_money -= tileData.m_cost;
        Tile newTile = Instantiate(tileData.m_tilePrefab);
        m_board.GetMyHand.Add(newTile);
    }
    #endregion
}
