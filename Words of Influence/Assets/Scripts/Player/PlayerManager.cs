using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    private RoomManager m_roomManager;
    private Camera m_camera;
    private PhotonView m_PV;
    private int m_HP;
    private int m_money;

    private int m_tilesInHand;

    [SerializeField]
    private PlayerUIItem m_playerUIItemPrefab;
    private PlayerUIItem m_playerUIItem;

    [SerializeField]
    private Board m_boardPrefab;
    private Board m_board;

    public static PlayerManager m_localPlayer;

    public const int m_startingHP = 100;
    #endregion

    #region Initialization
    void Awake() {
        m_roomManager = RoomManager.m_singleton;
        m_camera = GetComponentInChildren<Camera>();
        m_PV = GetComponent<PhotonView>();

        m_HP = m_startingHP;

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
        GameManager.m_singleton.AddPlayer(this);
        CreatePlayerUI();
        CreateBoard();
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
        m_board = Instantiate(m_boardPrefab);
        m_board.Setup(this);
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
        if (m_tilesInHand >= m_board.GetMyHand.GetTileHolders.Length) {
            return;
        }
        m_money -= tileData.m_cost;
        Tile newTile = Instantiate(tileData.m_tilePrefab);
        Debug.Log(m_board);
        Debug.Log(m_board.GetMyHand);
        m_board.GetMyHand.Add(newTile);

        //Update UI
    }

    public void Income() {
        int bonus = 0;
        //Interest
        bonus += Mathf.Min((m_money / 10), 5);
        //Win/Loss Streak
        m_money += 5 + bonus;
    }
    #endregion

    #region UI
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
    #endregion
}
