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

    [SerializeField]
    private Canvas m_canvas;
    [SerializeField]
    private GameObject m_UITrackerPrefab;

    private int m_currentPlayerAmount;
    private int m_turn;

    private bool m_ghostMatched;
    private int m_repetition;

    public const int StartRepetition = 4;
    #endregion

    #region Initialization
    void Awake() {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        m_singleton = this;

        m_turn = 0;
        m_repetition = StartRepetition;

        m_playerList = new List<PlayerManager>();
        m_aliveList = new List<PlayerManager>();
    }

    void Start() {
        if (PlayerManager.m_localPlayer == null) {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity, 0);
        }
    }
    #endregion

    #region Getter
    public Canvas GetCanvas {
        get { return m_canvas; }
    }

    public int GetRepetition {
        get { return m_repetition; }
    }
    #endregion

    #region Players
    public void AddPlayer(PlayerManager player) {
        m_playerList.Add(player);
        m_aliveList.Add(player);

        if (PhotonNetwork.CurrentRoom.PlayerCount == m_playerList.Count) {
            SortPlayers();
        }
    }

    public void PlayerDied(PlayerManager player) {
        m_aliveList.Remove(player);
        CheckLastPlayer();
    }
    #endregion

    #region Sort
    private void SortPlayers() {
        m_playerList.Sort(PlayerIDSort);
    }

    private int PlayerIDSort(PlayerManager player1, PlayerManager player2) {
        return player1.ID.CompareTo(player2.ID);
    }
    #endregion

    #region Game State
    private void NextTurn() {
        m_turn++;
        Debug.Log(m_aliveList.Count);
        foreach (PlayerManager player in m_aliveList) {
            player.Income();
            //Refresh shop
            TileShop.m_singleton.TurnRefresh();
        }
    }

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

    public GameObject CreateTracker() {
        GameObject tracker = Instantiate(m_UITrackerPrefab);
        tracker.transform.SetParent(m_canvas.transform);
        return tracker;
    }
    #endregion

    #region Exit
    public void ExitRoom() {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Matchmaking
    private void Matchmake() {
        foreach (PlayerManager player in m_aliveList) {
            //While I have no opponnet
            if (player.OpponentID == PlayerManager.NoOpponent) {
                //And we're not doing round robin
                if (m_aliveList.Count > 4) {
                    int playerIndices;
                    int random;
                    //If there are even players or ghost is matched
                    if (m_aliveList.Count % 2 == 0 || m_ghostMatched) {
                        //EVEN
                        playerIndices = m_aliveList.Count;
                    }
                    //Else there are odd players and we need to match a ghost
                    else {
                        //ODD
                        playerIndices = m_aliveList.Count + 1;
                    }
                    //Find an opponent that you ahven't fought and isn't matched
                    while (true) {
                        random = Random.Range(0, playerIndices - 1);
                        //If you pick the ghost
                        if (random == playerIndices && !m_ghostMatched) {
                            //GHOST
                            player.OpponentID = Random.Range(0, playerIndices - 2);
                            m_ghostMatched = true;
                            Debug.Log($"Player {player.ID}'s opponent is the Ghost.");
                            break;
                        }
                        if (m_playerList[m_aliveList[random].ID].OpponentID == PlayerManager.NoOpponent && !player.GetOpponentTracker.Contains(m_aliveList[random].ID)) {
                            //If the person you chose does not have an opponent and you have not fought him in X turns
                            player.OpponentID = random;
                            m_aliveList[random].OpponentID = player.ID;
                            Debug.Log($"Player {player.ID}'s opponent is Player {player.OpponentID}.");
                            Debug.Log($"Player {random}'s opponent is Player {m_aliveList[random].OpponentID}.");
                            break;
                        }
                    }
                }
            }
        }
    }

    private void UpdateMatchmaking() {

    }
    #endregion

    #region Photon
    public override void OnLeftRoom() {
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

    public void Test_NextTurn() {
        NextTurn();
    }

    public void Test_PlayersAndID() {
        foreach (PlayerManager player in m_playerList) {
            Debug.LogError($"Player {player.GetPhotonView.Owner.NickName} has ID {player.ID}");
        }
    }

    public void Test_SimulateMatchmaking() {
        Matchmake();
    }
    #endregion
}
