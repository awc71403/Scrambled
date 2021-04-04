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

    private int m_level;
    private int m_currentEXP;
    [SerializeField]
    private int[] m_expThreshold;

    private int m_winStreak;
    private int m_lossStreak;

    [SerializeField]
    private int m_tilesInHand;
    [SerializeField]
    private int m_tilesInPlay;
    private int m_ID;

    private int m_opponentID;
    private int m_ghostID;

    [SerializeField]
    private List<int> m_opponentTracker;

    private int m_tileIDCreator;

    private TextMeshProUGUI m_moneyText;

    [SerializeField]
    private Unit m_unitPrefab;
    [SerializeField]
    private List<Unit> m_myUnits;

    [SerializeField]
    private PlayerUIItem m_playerUIItemPrefab;
    private PlayerUIItem m_playerUIItem;

    [SerializeField]
    private Board m_boardPrefab;
    private Board m_board;

    public static PlayerManager m_localPlayer;

    public const int TilesPerLevel = 2;
    public const int StartingHP = 100;
    public const int StartingMoney = 2;
    public const int StartingLevel = 1;
    public const int NoOpponent = -1;
    public const int DefaultGhost = -2;
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

        m_level = StartingLevel;
        m_currentEXP = 0;

        m_tilesInHand = 0;
        m_tilesInPlay = 0;

        m_opponentID = NoOpponent;

        m_myUnits = new List<Unit>();
        m_opponentTracker = new List<int>();

        if (m_PV.IsMine) {
            m_localPlayer = this;
            UIManager.m_singleton.UpdateTiles(m_tilesInPlay);
        }
    }

    void Start() {
        if (!m_PV.IsMine) {
            Destroy(m_camera.gameObject);
        }

        CreatePlayerUI();
        CreateBoard();

        GameManager.m_singleton.AddPlayer(this);

        m_moneyText = TileShop.m_singleton.GetMoneyText;
        UpdateMoney();

        UIManager.m_singleton.UpdatePlayerLevel();

    }
        
    private void CreatePlayerUI() {
        m_playerUIItem = Instantiate(m_playerUIItemPrefab);
        m_playerUIItem.SetUp(this);
        m_playerUIItem.gameObject.transform.SetParent(PlayerUIList.m_singleton.transform);
        m_playerUIItem.gameObject.transform.localScale = new Vector3(1, 1, 1);
        PlayerUIList.m_singleton.UpdateRanking();
    }

    private void CreateBoard() {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) {
            if (players[i].ActorNumber == m_PV.Owner.ActorNumber && m_board == null) {
                m_ID = i;
                m_board = Instantiate(m_boardPrefab);
                m_board.Setup(this);
                m_board.transform.position = SpawnManager.m_singleton.GetBoardSpawnPoint(m_ID).transform.position;
            }
        }
        SetPlayerManagerLocation();
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

    public int GetLevel {
        get { return m_level; }
    }

    public int GetCurrentExp {
        get { return m_currentEXP; }
    }

    public int[] GetExpThreshold {
        get { return m_expThreshold; }
    }

    public Camera GetCamera {
        get { return m_camera; }
    }

    public Board GetBoard {
        get { return m_board; }
    }

    public int GhostID {
        get { return m_ghostID; }
        set { m_ghostID = value; }
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
    public int GetTilesInPlay {
        get { return m_tilesInPlay; }
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
        //Will need to update globally when showing interest to other players
        UpdateMoney();
        //Update UI
    }

    public void OnSoldTile(Tile tile) {
        TileShop.m_singleton.AddToPool(tile.GetData.m_cost, tile.GetData.m_ID);
        tile.RemoveTileUnits();
        m_PV.RPC("RPC_SellTile", RpcTarget.All, tile.OccupiedHolder.X, tile.OccupiedHolder.Y);
        UIManager.m_singleton.UpdateTiles(m_tilesInPlay);
        UpdateMoney();
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
        AddMoney(TileShop.TurnIncome + bonus);
    }

    public void UsedRefresh() {
        m_PV.RPC("RPC_UsedRefresh", RpcTarget.All);
    }

    public void UsedEXPGain() {
        m_PV.RPC("RPC_UsedBuyEXP", RpcTarget.All);
    }
    #endregion

    #region UI
    private void UpdateMoney() {
        TileShop.m_singleton.CanButton();
        m_moneyText.text = m_money.ToString();
    }
    #endregion

    #region Player
    public void TakeDamage(int damage) {
        Debug.Log("TakeDamage called");
        m_PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    public void AddMoney(int amount) {
        m_money += amount;
        UpdateMoney();
    }

    public void UseMoney(int amount) {
        m_money -= amount;
        UpdateMoney();
    }

    public void IncreaseExp(int amount) {
        Debug.Log("IncreaseExp");
        if (m_level > m_expThreshold.Length) {
            return;
        }
        m_currentEXP += amount;
        int threshold = m_expThreshold[m_level - 1];
        if (m_currentEXP >= threshold) {
            Debug.Log("Level Up");
            m_level++;
            m_currentEXP -= threshold;
        }
        UIManager.m_singleton.UpdatePlayerLevel();
    }
    #endregion

    #region Matchmaking
    //Add a way to permenately remove soem stuff

    public void SetGhostOpponent(int opponentID) {
        m_ghostID = opponentID;
        m_opponentID = DefaultGhost;

        TrackerUpdate();
    }

    public void SetOpponent(int opponentID) {
        m_opponentID = opponentID;
        m_opponentTracker.Add(opponentID);

        TrackerUpdate();
    }

    public static void OnPlayerDeath() {
        if (m_repitition > 1) {
            m_repitition--;
        }
    }

    public void TrackerUpdate() {
        if (m_opponentTracker.Count > m_repitition) {
            m_opponentTracker.RemoveAt(0);
        }
    }

    public void SetTracker(List<int> tracker) {
        m_opponentTracker = tracker;
    }
    #endregion

    #region Board

    #endregion

    #region Tile
    public void MoveTile(Tile tile, TileHolder targetHolder) {
        TileHolder currentHolder = tile.OccupiedHolder;
        m_PV.RPC("RPC_MoveTile", RpcTarget.All, currentHolder.X, currentHolder.Y, targetHolder.X, targetHolder.Y);
        UIManager.m_singleton.UpdateTiles(m_tilesInPlay);
    }

    public void SwapTiles(Tile tile, TileHolder targetHolder) {
        TileHolder currentHolder = tile.OccupiedHolder;
        m_PV.RPC("RPC_SwapTiles", RpcTarget.All, currentHolder.X, currentHolder.Y, targetHolder.X, targetHolder.Y);
    }

    private void UpdateUnits(int fromX, int fromY, int targetX, int targetY, bool isSwapping = false, bool isRemoving = false) {
        //From: Words left and right of letter being moved
        //Horizontal

        //For Testing
        //m_myUnits = new List<Unit>();

        //If the tile is on the board
        if (isRemoving) {
            if (fromY != Board.HandYPosition) {
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
                    fromLeftHBool = WordCheck(fromLeftH, true);
                }
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                newStart = newStart.Right;
                if (newStart != null && newStart.Tile != null) {
                    fromRightH = ScanHorizontal(newStart);
                    fromRightHBool = WordCheck(fromRightH, true);
                }
                //Vertical
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                while (newStart.Up != null && newStart.Up.Tile != null) {
                    newStart = newStart.Up;
                }
                if (newStart.Y != fromY) {
                    fromUpV = ScanVertical(newStart);
                    fromUpVBool = WordCheck(fromUpV, false);
                }
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                newStart = newStart.Down;
                if (newStart != null && newStart.Tile != null) {
                    fromDownV = ScanVertical(newStart);
                    fromDownVBool = WordCheck(fromDownV, false);
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
        } else if (isSwapping) {
            if (fromY != Board.HandYPosition) {
                //Horizontal
                TileHolder newStart = m_board.GetHolderMapArray[fromX, fromY];
                while (newStart.Left != null && newStart.Left.Tile != null) {
                    newStart = newStart.Left;
                }

                Tile[] fromH = ScanHorizontal(newStart);
                bool fromHBool = WordCheck(fromH, true);

                //Vertical
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                while (newStart.Up != null && newStart.Up.Tile != null) {
                    newStart = newStart.Up;
                }

                Tile[] fromV = ScanVertical(newStart);
                bool fromVBool = WordCheck(fromV, false);

                //Singles
                if (!fromHBool) {
                    MakeSingleUnits(fromH);
                }
                if (!fromVBool) {
                    MakeSingleUnits(fromV);
                }
            }
            else {
                m_board.GetMyHand.GetTileHolders[fromX].Tile.RemoveTileUnits();
            }
            if (targetY != Board.HandYPosition) {
                //Horizontal
                TileHolder newStart = m_board.GetHolderMapArray[targetX, targetY];
                while (newStart.Left != null && newStart.Left.Tile != null) {
                    newStart = newStart.Left;
                }

                Tile[] targetH = ScanHorizontal(newStart);
                bool targetHBool = WordCheck(targetH, true);

                //Vertical
                newStart = m_board.GetHolderMapArray[targetX, targetY];
                while (newStart.Up != null && newStart.Up.Tile != null) {
                    newStart = newStart.Up;
                }

                Tile[] targetV = ScanVertical(newStart);
                bool targetVBool = WordCheck(targetV, false);

                //Singles
                if (!targetHBool) {
                    MakeSingleUnits(targetH);
                }
                if (!targetVBool) {
                    MakeSingleUnits(targetV);
                }
            }
            else {
                m_board.GetMyHand.GetTileHolders[targetX].Tile.RemoveTileUnits();
            }
        }
        else {
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

                //While there are still tiles on the left of where we are from, move left
                TileHolder newStart = m_board.GetHolderMapArray[fromX, fromY];
                while (newStart.Left != null && newStart.Left.Tile != null) {
                    newStart = newStart.Left;
                }
                //If the furthest left time is not the one we're from
                if (newStart.X != fromX) {
                    fromLeftH = ScanHorizontal(newStart);
                    fromLeftHBool = WordCheck(fromLeftH, true);
                }
                //If there is a tile on the right of where we are from
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                newStart = newStart.Right;
                if (newStart != null && newStart.Tile != null) {
                    fromRightH = ScanHorizontal(newStart);
                    fromRightHBool = WordCheck(fromRightH, true);
                }
                //Vertical
                //While there are still tiles above where we are from, move up
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                while (newStart.Up != null && newStart.Up.Tile != null) {
                    newStart = newStart.Up;
                }
                if (newStart.Y != fromY) {
                    fromUpV = ScanVertical(newStart);
                    fromUpVBool = WordCheck(fromUpV, false);
                }
                //If there is a tile below where we are from
                newStart = m_board.GetHolderMapArray[fromX, fromY];
                newStart = newStart.Down;
                if (newStart != null && newStart.Tile != null) {
                    fromDownV = ScanVertical(newStart);
                    fromDownVBool = WordCheck(fromDownV, false);
                }

                //Singles
                Debug.Log("FromSingle");
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
            //If the place you're putting the tile isn't your hand
            if (targetY != Board.HandYPosition) {
                Debug.Log("Target Update");
                //Horizontal
                TileHolder newStart = m_board.GetHolderMapArray[targetX, targetY];
                while (newStart.Left != null && newStart.Left.Tile != null) {
                    newStart = newStart.Left;
                }

                Tile[] targetH = ScanHorizontal(newStart);
                bool targetHBool = WordCheck(targetH, true);

                //Vertical
                newStart = m_board.GetHolderMapArray[targetX, targetY];
                while (newStart.Up != null && newStart.Up.Tile != null) {
                    newStart = newStart.Up;
                }

                Tile[] targetV = ScanVertical(newStart);
                bool targetVBool = WordCheck(targetV, false);

                //Singles
                Debug.Log("TargetSingle");
                if (!targetHBool) {
                    MakeSingleUnits(targetH);
                }
                if (!targetVBool) {
                    MakeSingleUnits(targetV);
                }
            }
            else {
                m_board.GetMyHand.GetTileHolders[targetX].Tile.RemoveTileUnits();
            }
        }

        Debug.Log($"MyUnits has a length of {m_myUnits.Count}."); 
        Debug.Log($"Here are my units in order:");
        foreach (Unit unit in m_myUnits) {
            Debug.Log(TileToString(unit.GetLetters));
        }
        Debug.Log("-------------------------------------");

    }

    //Creates an array of all the tiles in a row
    private Tile[] ScanHorizontal(TileHolder leftMost) {
        List<Tile> listTiles = new List<Tile>();
        while (leftMost != null && leftMost.Tile != null) {
            listTiles.Add(leftMost.Tile);
            leftMost.Tile.RemoveHorizontalUnit();
            leftMost = leftMost.Right;
        }
        string test = "";
        foreach (Tile tile in listTiles) {
            test += tile.GetName;
        }
        Debug.Log($"ScanHorizontal: {test}");
        return listTiles.ToArray();
    }

    //Creates an array of all the tiles in a column
    private Tile[] ScanVertical(TileHolder upMost) {
        List<Tile> listTiles = new List<Tile>();
        while (upMost != null && upMost.Tile != null) {
            listTiles.Add(upMost.Tile);
            upMost.Tile.RemoveVerticalUnit();
            upMost = upMost.Down;
        }
        string test = "";
        foreach (Tile tile in listTiles) {
            test += tile.GetName;
        }
        Debug.Log($"ScanVertical: {test}");
        return listTiles.ToArray();
    }

    private bool WordCheck(Tile[] tiles, bool isHorizontal) {
        if (WordManager.IsWord(TileToString(tiles))) {
            Unit newUnit = Instantiate(m_unitPrefab).GetComponent<Unit>();
            newUnit.Setup(tiles, isHorizontal);
            m_myUnits.Add(newUnit);
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
            Unit newUnit = Instantiate(m_unitPrefab).GetComponent<Unit>();
            newUnit.Setup(array, true);
            m_myUnits.Add(newUnit);
            Debug.Log($"Tile {tile.GetName} turned into a Unit");
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

    #region Unit
    public List<Unit> OrderUnits() {
        List<Unit> orderedUnits = new List<Unit>();
        BoardHolder[,] boardHolders = m_board.GetHolderMapArray;
        //Add Everything
        for (int y = Board.BoardColumns / 2; y < Board.BoardColumns; y++) {
            for (int x = 0; x < Board.BoardRows; x++) {
                BoardHolder holder = boardHolders[x, y];
                if (holder.IsOccupied) {
                    Tile tile = holder.Tile;
                    if (tile.IsFirstHorizontal || tile.IsSingleTile) {
                        Unit unit = tile.HorizontalUnit;
                        orderedUnits.Add(unit);
                    }
                    if (holder.Tile.IsFirstVertical) {
                        Unit unit = tile.VerticalUnit;
                        orderedUnits.Add(unit);
                    }
                }
            }
        }
        //Reorganize Firewall
        //for (int i = orderedUnits.Count - 1; i >= 0; i--) {
        //    Unit unit = orderedUnits[i];
        //    TileDatabaseSO.TileData.Trait[] traits = unit.GetTraits;
        //    if (traits[0] == TileDatabaseSO.TileData.Trait.FIREWALL || traits[1] == TileDatabaseSO.TileData.Trait.FIREWALL || traits[2] == TileDatabaseSO.TileData.Trait.FIREWALL) {
        //        orderedUnits.RemoveAt(i);
        //        orderedUnits.Insert(0, unit);
        //    }
        //}
        //Reorganize Blacklist
        //for (int i = 0; i < orderedUnits.Count; i++) {
        //    Unit unit = orderedUnits[i];
        //    TileDatabaseSO.TileData.Trait[] traits = unit.GetTraits;
        //    if (traits[0] == TileDatabaseSO.TileData.Trait.BLACKLIST || traits[1] == TileDatabaseSO.TileData.Trait.BLACKLIST || traits[2] == TileDatabaseSO.TileData.Trait.BLACKLIST) {
        //        orderedUnits.RemoveAt(i);
        //        orderedUnits.Add(unit);
        //    }
        //}

        return orderedUnits;
    }
    #endregion

    #region RPC
    [PunRPC]
    void RPC_UsedRefresh() {
        UseMoney(TileShop.RefreshCost);
    }

    [PunRPC]
    void RPC_UsedBuyEXP() {
        IncreaseExp(TileShop.EXPGain);

        UseMoney(TileShop.EXPCost);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage) {
        m_HP -= damage;
        m_playerUIItem.UpdateHP(m_HP);
        if (m_HP <= 0) {
            GameManager.m_singleton.PlayerDied(this);
        }
    }

    [PunRPC]
    void RPC_BuyTile(int ID) {
        TileDatabaseSO.TileData tileData = GameManager.m_singleton.m_tileDatabase.allTiles[ID];
        UseMoney(tileData.m_cost);
        Tile newTile = Instantiate(tileData.m_tilePrefab);
        newTile.Setup(tileData, this);
        m_board.GetMyHand.Add(newTile);
    }

    [PunRPC]
    void RPC_SellTile(int x, int y) {
        Debug.Log($"SellTile: {x} and {y}");
        Tile tile;
        if (y != Board.HandYPosition) {
            tile = m_board.GetHolderMapArray[x, y].Tile;
            m_tilesInPlay--;
        }
        else {
            tile = m_board.GetMyHand.GetTileHolders[x].Tile;
            m_tilesInHand--;
        }
        TileHolder tileHolder = tile.OccupiedHolder;
        tileHolder.IsOccupied = false;
        tileHolder.Tile = null;
        //Sell Tile
        AddMoney(tile.GetData.m_cost);
        //Might cause bugs
        Destroy(tile.gameObject);
        //Update Board
        UpdateUnits(x, y, 0, 0, false, true);
    }

    [PunRPC]
    void RPC_MoveTile(int tileX, int tileY, int targetX, int targetY) {
        Tile chosenTile;
        if (tileY != Board.HandYPosition) {
            chosenTile = m_board.GetHolderMapArray[tileX, tileY].Tile;
        }
        else {
            if (targetY != Board.HandYPosition) {
                m_tilesInPlay++;
            }
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
            m_tilesInHand++;
            m_tilesInPlay--;
        }

        targetHolder.Tile = chosenTile;
        targetHolder.IsOccupied = true;
        chosenTile.OccupiedHolder.Tile = null;
        chosenTile.OccupiedHolder.IsOccupied = false;
        chosenTile.OccupiedHolder = targetHolder;
        chosenTile.transform.position = targetHolder.transform.position;

        UpdateUnits(tileX, tileY, targetX, targetY);
    }

    [PunRPC]
    void RPC_SwapTiles(int tileX, int tileY, int targetX, int targetY) {
        Tile chosenTile;
        if (tileY != Board.HandYPosition) {
            chosenTile = m_board.GetHolderMapArray[tileX, tileY].Tile;
        }
        else {
            chosenTile = m_board.GetMyHand.GetTileHolders[tileX].Tile;
        }

        TileHolder targetHolder;
        if (targetY != Board.HandYPosition) {
            targetHolder = m_board.GetHolderMapArray[targetX, targetY];
        }
        else {
            targetHolder = m_board.GetMyHand.GetTileHolders[targetX];
        }

        targetHolder.Tile.transform.position = chosenTile.OccupiedHolder.transform.position;
        targetHolder.Tile.OccupiedHolder = chosenTile.OccupiedHolder;
        Tile temp = targetHolder.Tile;

        targetHolder.Tile = chosenTile;

        chosenTile.OccupiedHolder.Tile = temp;
        chosenTile.OccupiedHolder = targetHolder;
        chosenTile.transform.position = targetHolder.transform.position;

        UpdateUnits(tileX, tileY, targetX, targetY, true);
    }
    #endregion
}
