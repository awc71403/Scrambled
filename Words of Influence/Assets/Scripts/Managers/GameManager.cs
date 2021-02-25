using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class GameManager : MonoBehaviourPunCallbacks {
    #region Variables
    public TileDatabaseSO m_tileDatabase;

    public static GameManager m_singleton;

    [SerializeField]
    private ModalWindowManager m_modalWindow;

    [SerializeField]
    private List<PlayerManager> m_playerList;
    private List<PlayerManager> m_aliveList;

    private GameObject[,] m_tileMapArray;
    [SerializeField]
    private GameObject m_tilePrefab;
    private GameObject m_tilesObject;
    private int m_mapXSize;
    private int m_mapYSize;
    private float m_tileSize;

    public const int m_boardRows = 10;
    public const int m_boardColumns = 10;
    #endregion

    #region Initialization
    void Awake() {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        m_singleton = this;

        m_playerList = new List<PlayerManager>();
        m_aliveList = new List<PlayerManager>();

        CreateTiles();
    }

    void Start() {
        if (PlayerManager.m_localPlayer == null) {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity, 0);
        }
    }
    #endregion

    #region Getter
    #endregion

    #region Board
    public void CreateTiles() {

        m_tilesObject = new GameObject();
        m_tilesObject.name = "Tiles";

        m_mapXSize = m_boardColumns;
        m_mapYSize = m_boardRows;

        m_tileSize = m_tilePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        // Fill mapArray, which should be empty at first.
        m_tileMapArray = new GameObject[m_mapXSize, m_mapYSize];

        // Calculate the size of the map.
        float mapWidth = m_mapXSize * m_tileSize;
        float mapHeight = m_mapYSize * m_tileSize;

        // Finds the top left corner.
        Vector3 worldStart = new Vector3(-mapWidth / 2.0f + (0.5f * m_tileSize), mapHeight / 2.0f - (0.5f * m_tileSize) + m_tileSize);

        // Nested for loop that creates mapYSize * mapXSize tiles.
        for (int y = 0; y < m_mapYSize; y++) {
            for (int x = 0; x < m_mapXSize; x++) {
                PlaceTile(x, y, worldStart);
            }
        }

        //for (int y = 0; y < m_mapYSize; y++) {
        //    if (player1side) {
        //        player1side.playerside = 1;
        //    }
        //    if (player2side) {
        //        player2side.playerside = 2;
        //    }
        //}
    }

    // Places a tile at position (x, y).
    private void PlaceTile(int x, int y, Vector3 worldStart) {
        GameObject newTile = Instantiate(m_tilePrefab);

        //Put under tile object in Hierarchy
        newTile.transform.SetParent(m_tilesObject.transform);

        // Calculates where it should go.
        float newX = worldStart.x + (m_tileSize * x);
        float newY = worldStart.y - (m_tileSize * y);

        // Puts it there.
        newTile.transform.position = new Vector3(newX, newY, 0);
        //newTile.GetComponent<TileBehavior>().xPosition = x;
        //newTile.GetComponent<TileBehavior>().yPosition = y;

        // Adds it to mapArray so we can keep track of it later.
        m_tileMapArray[x, y] = newTile;
    }
    #endregion

    #region Players
    public void AddPlayer(PlayerManager player) {
        m_playerList.Add(player);
        m_aliveList.Add(player);
    }

    public void PlayerDied(PlayerManager player) {
        m_aliveList.Remove(player);
        CheckLastPlayer();
    }
    #endregion

    #region Game State
    private void CheckLastPlayer() {
        if (m_aliveList.Count == 1) {
            Win(m_aliveList[0]);
        }
    }

    public void Win(PlayerManager player) {
        //Doesn't need RPC
        //RPC for Take Damage
        //Locally check if a player dies
        //Locally check if last player
        //Locally call win
        OpenWinMenu(player.GetPhotonView.Owner.NickName);
    }
    #endregion

    #region UI
    public void OpenWinMenu(string playerName) {
        Debug.Log(playerName);
        m_modalWindow.descriptionText = $"{playerName} wins!";
        m_modalWindow.UpdateUI();
        m_modalWindow.OpenWindow();
    }
    #endregion

    #region Exit
    public void ExitRoom() {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Photon
    public override void OnLeftRoom() {
        Destroy(RoomManager.m_singleton.gameObject);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
    #endregion

    #region Testing
    public void Test_Win() {
        foreach (PlayerManager player in m_aliveList) {
            if (!player.GetPhotonView.IsMine) {
                PlayerDied(player);
            }
            CheckLastPlayer();
        }
    }
    #endregion
}
