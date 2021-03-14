using System.Collections;
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
    private static int m_repitition;

    private RoomManager m_roomManager;
    private Camera m_camera;
    private PhotonView m_PV;
    private int m_HP;
    private int m_money;

    private int m_winStreak;
    private int m_lossStreak;

    private int m_tilesInHand;
    private int m_ID;

    private int m_opponentID;

    [SerializeField]
    private List<int> m_opponentTracker;

    private int m_tileIDCreator;

    private TextMeshProUGUI m_moneyText;

    [SerializeField]
    private List<Unit> m_myUnits;

    [SerializeField]
    private PlayerUIItem m_playerUIItemPrefab;
    private PlayerUIItem m_playerUIItem;

    [SerializeField]
    private Board m_boardPrefab;
    private Board m_board;

    public static PlayerManager m_localPlayer;

    public const int StartingHP = 100;
    public const int StartingMoney = 5;
    public const int NoOpponent = -1;
    public const int GhostID = -1;
    public const int StartRepetition = 4;
    #endregion

    #region Initialization
    void Awake() {
        m_repitition = StartRepetition;

        m_roomManager = RoomManager.m_singleton;
        m_camera = GetComponentInChildren<Camera>();
        m_PV = GetComponent<PhotonView>();

        m_HP = StartingHP;
        m_money = StartingMoney;

        m_tilesInHand = 0;

        m_opponentID = NoOpponent;

        m_myUnits = new List<Unit>();
        m_opponentTracker = new List<int>();

        if (m_PV.IsMine) {
            m_localPlayer = this;
        }
    }

    void Start() {
        if (!m_PV.IsMine) {
            Destroy(m_camera.gameObject);
        }

        //m_moneyText = TileShop.m_singleton.GetMoneyText;
        //UpdateMoney();

        //GameManager.m_singleton.AddPlayer(this);
        //CreatePlayerUI();
        //CreateBoard();
        //SetPlayerManagerLocation();

    }
        
    private void CreatePlayerUI() {
        m_playerUIItem = Instantiate(m_playerUIItemPrefab);
        m_playerUIItem.SetUp(this);
        m_playerUIItem.gameObject.transform.SetParent(PlayerUIList.m_singleton.transform);
        m_playerUIItem.gameObject.transform.localScale = new Vector3(1, 1, 1);
        PlayerUIList.m_singleton.UpdateRanking();
    }

    private void CreateBoard() {
        Debug.Log("CreateBoard called");
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) {
            if (players[i].UserId == m_PV.Owner.UserId && m_board == null) {
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

    public int ID {
        get { return m_ID; }
        set { m_ID = value; }
    }

    public int OpponentID {
        get { return m_opponentID; }
        set { m_opponentID = value; }
    }

    public int TilesInHand {
        get { return m_tilesInHand; }
        set { m_tilesInHand = value; }
    }

    public List<Unit> MyUnits {
        get { return m_myUnits; }
        set { m_myUnits = value; }
    }

    public List<int> GetOpponentTracker {
        get { return m_opponentTracker; }
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

    public void UsedRefresh() {
        m_PV.RPC("RPC_Refresh", RpcTarget.All);
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

    #region Battle
    //Add a way to permenately remove soem stuff

    public void SetOpponent(int opponentID) {
        m_opponentID = opponentID;
        m_opponentTracker.Add(opponentID);

        if (m_opponentTracker.Count > m_repitition) {
            m_opponentTracker.RemoveAt(0);
        }
    }

    public void OnPlayerDeath() {
        m_opponentTracker.RemoveAt(0);
    }
    #endregion

    #region Tile
    public void MoveTile(Tile tile, TileHolder targetHolder) {
        TileHolder currentHolder = tile.OccupiedHolder;
        m_PV.RPC("RPC_PlaceTile", RpcTarget.All, currentHolder.X, currentHolder.Y, targetHolder.X, targetHolder.Y);
    }

    private void UpdateUnits(int fromX, int fromY, int targetX, int targetY) {
        //From: Words left and right of letter being moved
        //Horizontal

        //For Testing
        //m_myUnits = new List<Unit>();

        if (fromY != Board.HandYPosition) {
            Debug.Log("From Update");

            Tile[] fromLeftH = null;
            bool fromLeftHBool = true;

            Tile[] fromRightH = null;
            bool fromRightHBool = true;

            Tile[] fromUpV = null;
            bool fromUpVBool = true;

            Tile[] fromDownV = null;
            bool fromDownVBool = true;

            TileHolder newStart = m_board.GetHolderMapArray[fromX, fromY];
            while (newStart.Left != null && newStart.Left.Tile != null) {
                newStart = newStart.Left;
            }
            if (newStart.X != fromX) {
                fromLeftH = ScanHorizontal(newStart);
                fromLeftHBool = WordCheck(fromLeftH);
            }
            newStart = m_board.GetHolderMapArray[fromX, fromY];
            newStart = newStart.Right;
            if (newStart != null && newStart.Tile != null) {
                fromRightH = ScanHorizontal(newStart);
                fromRightHBool = WordCheck(fromRightH);
            }
            //Vertical
            newStart = m_board.GetHolderMapArray[fromX, fromY];
            while (newStart.Up != null && newStart.Up.Tile != null) {
                newStart = newStart.Up;
            }
            if (newStart.Y != fromY) {
                fromUpV = ScanVertical(newStart);
                fromUpVBool = WordCheck(fromUpV);
            }
            newStart = m_board.GetHolderMapArray[fromX, fromY];
            newStart = newStart.Down;
            if (newStart != null && newStart.Tile != null) {
                fromDownV = ScanVertical(newStart);
                fromDownVBool = WordCheck(fromDownV);
            }

            //Singles
            if (fromLeftH != null && !fromLeftHBool) {
                MakeSingleUnits(fromLeftH);
            }
            if (fromRightH != null && !fromRightHBool) {
                MakeSingleUnits(fromRightH);
            }
            if (fromUpV != null && !fromUpVBool) {
                MakeSingleUnits(fromUpV);
            }
            if (fromDownV != null && !fromDownVBool) {
                MakeSingleUnits(fromDownV);
            }
        }

        //Target: Words all combined in row and column
        //Horizontal
        if (targetY != Board.HandYPosition) {
            Debug.Log("Target Update");
            TileHolder newStart = m_board.GetHolderMapArray[targetX, targetY];
            while (newStart.Left != null && newStart.Left.Tile != null) {
                newStart = newStart.Left;
            }

            Tile[] targetH = ScanHorizontal(newStart);
            bool targetHBool = WordCheck(targetH);

            //Vertical
            newStart = m_board.GetHolderMapArray[targetX, targetY];
            while (newStart.Up != null && newStart.Up.Tile != null) {
                newStart = newStart.Up;
            }

            Tile[] targetV = ScanVertical(newStart);
            bool targetVBool = WordCheck(targetV);

            //Singles
            if (!targetHBool) {
                MakeSingleUnits(targetH);
            }
            if (!targetVBool) {
                MakeSingleUnits(targetV);
            }
        }
        else {
            m_board.GetMyHand.GetTileHolders[targetX].Tile.RemoveTileUnit(true);
        }

        Debug.Log($"MyUnits has a length of {m_myUnits.Count}."); 
        Debug.Log($"Here are my units in order:");
        foreach (Unit unit in m_myUnits) {
            Debug.Log(TileToString(unit.GetLetters));
        }
        Debug.Log("-------------------------------------");

    }

    private Tile[] ScanHorizontal(TileHolder leftMost) {
        List<Tile> listTiles = new List<Tile>();
        while (leftMost != null && leftMost.Tile != null) {
            listTiles.Add(leftMost.Tile);
            leftMost.Tile.RemoveTileUnit(true);
            leftMost = leftMost.Right;
        }
        return listTiles.ToArray();
    }

    private Tile[] ScanVertical(TileHolder upMost) {
        List<Tile> listTiles = new List<Tile>();
        while (upMost != null && upMost.Tile != null) {
            listTiles.Add(upMost.Tile);
            upMost.Tile.RemoveTileUnit(false);
            upMost = upMost.Down;
        }
        return listTiles.ToArray();
    }

    private bool WordCheck(Tile[] tiles) {
        if (WordManager.IsWord(TileToString(tiles))) {
            Unit newWord = new Unit();
            newWord.Setup(tiles, true);
            m_myUnits.Add(newWord);
            return true;
        }
        return false;
    }

    private void MakeSingleUnits(Tile[] tiles) {
        foreach (Tile tile in tiles) {
            if (tile.IsSingleTile || tile.HorizontalUnit != null || tile.VerticalUnit != null) {
                continue;
            }
            Tile[] array = new Tile[1];
            array[0] = tile;
            Unit newWord = new Unit();
            newWord.Setup(array, true);
            m_myUnits.Add(newWord);
        }
    }

    private string TileToString(Tile[] tiles) {
        string word = "";
        for (int i = 0; i < tiles.Length; i++) {
            word = word + tiles[i].GetName;
        }
        return word;
    }
    #endregion

    #region RPC
    [PunRPC]
    void RPC_Refresh() {
        m_money -= 2;
    }

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
        newTile.Setup(tileData, this);
        m_board.GetMyHand.Add(newTile);
    }

    [PunRPC]
    void RPC_PlaceTile(int tileX, int tileY, int targetX, int targetY) {
        Tile chosenTile;
        if (tileY != Board.HandYPosition) {
            chosenTile = m_board.GetHolderMapArray[tileX, tileY].Tile;
        }
        else {
            chosenTile = m_board.GetMyHand.GetTileHolders[tileX].Tile;
            m_board.GetMyHand.GetTileHolders[tileX].IsOccupied = false;
            m_tilesInHand--;
        }

        TileHolder targetHolder;
        if (targetY != Board.HandYPosition) {
            targetHolder = m_board.GetHolderMapArray[targetX, targetY];
        }
        else {
            targetHolder = m_board.GetMyHand.GetTileHolders[targetX];
        }


        targetHolder.Tile = chosenTile;
        chosenTile.OccupiedHolder.Tile = null;
        chosenTile.OccupiedHolder.IsOccupied = false;
        chosenTile.OccupiedHolder = targetHolder;
        chosenTile.transform.position = targetHolder.transform.position;

        UpdateUnits(tileX, tileY, targetX, targetY);
    }
    #endregion
}
