using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks {
    #region Variables
    public TileDatabaseSO m_tileDatabase;

    public static GameManager m_singleton;

    private PhotonView m_PV;

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

    private List<int> m_roundRobin;
    private List<int> m_currentRobin;

    public TextMeshProUGUI seedText;

    [SerializeField]
    private Slider m_timerSlider;
    [SerializeField]
    private TextMeshProUGUI m_timerSliderText;

    private bool m_isBuffer;
    private float m_currentTimer;
    private Phase m_currentPhase;

    private int m_readyToProceed;

    // 30 but testing 10
    public const float BufferTime = 2;
    public const float BuyTime = 30;
    public const float FightTime = 30;
    public const int StartRepetition = 4;
    public const int ExpPerRound = 2;
    #endregion

    #region Enum
    public enum Phase { BUY, FIGHT};
    #endregion

    #region Initialization
    void Awake() {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        m_singleton = this;

        m_PV = GetComponent<PhotonView>();

        m_turn = 0;
        m_repetition = StartRepetition;

        m_readyToProceed = 0;
        m_isBuffer = false;

        InitializePhase();

        m_playerList = new List<PlayerManager>();
        m_aliveList = new List<PlayerManager>();
    }

    void Start() {
        if (PlayerManager.m_localPlayer == null) {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity, 0);
        }

        seedText.text = $"Seed: {RoomManager.Seed.ToString()}";
    }
    #endregion

    #region Update
    private void Update() {
        if (m_currentTimer > 0) {
            m_currentTimer -= Time.deltaTime;
            UpdateTimerUI();
        }
        else if (m_isBuffer) {
            m_isBuffer = false;
            SwapPhase();
        }
        else {
            //PlayerReadyToProceed();
        }
    }
    #endregion

    #region Getter
    public List<PlayerManager> GetPlayerList {
        get { return m_playerList; }
    }

    public Canvas GetCanvas {
        get { return m_canvas; }
    }

    public int GetRepetition {
        get { return m_repetition; }
    }

    public Phase GetCurrentPhase {
        get { return m_currentPhase; }
    }

    public bool GetIsBuffer {
        get { return m_isBuffer; }
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
        m_aliveList.Sort(PlayerIDSort);
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
            player.IncreaseExp(ExpPerRound);
        }
        TileShop.m_singleton.TurnRefresh();
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

    private void UpdateTimerUI() {
        m_timerSlider.value = m_currentTimer / BuyTime;
        string phase;
        if (m_currentPhase == Phase.BUY) {
            phase = "BUY: ";
        }
        else {
            phase = "FIGHT: ";
        }
        m_timerSliderText.text = phase + ((int)m_currentTimer).ToString();
    }
    #endregion

    #region Exit
    public void ExitRoom() {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Matchmaking
    private void Matchmake() {
        int[] chosenEnemyID = new int[m_playerList.Count];
        for (int i = 0; i < chosenEnemyID.Length; i++) {
            chosenEnemyID[i] = PlayerManager.NoOpponent;
        }
        int alive = m_aliveList.Count;
        //foreach (PlayerManager player in m_aliveList) {
        if (alive > 4) {
            //While I have no opponnet
            //if (chosenEnemyID[player.ID] == PlayerManager.NoOpponent) {
            foreach (PlayerManager player in m_aliveList) {
                //And we're not doing round robin
                //if (m_aliveList.Count > 4) {
                if (chosenEnemyID[player.ID] == PlayerManager.NoOpponent) {
                    List<PlayerManager> possibleOpponents = new List<PlayerManager>(m_aliveList);
                    int playerIndices = possibleOpponents.Count;
                    int playerAgainstGhost = PlayerManager.GhostID;
                    int chosenGhost;
                    int random;
                    bool odd;
                    //If there are even players or ghost is matched
                    if (m_aliveList.Count % 2 == 0) {
                        //EVEN
                        odd = false;
                    }
                    //Else there are odd players and we need to match a ghost
                    else {
                        //ODD
                        odd = true;
                    }
                    //Find an opponent that you haven't fought and isn't matched
                    //Need to test with int for loop instead of Random.Range
                    bool found = false;
                    //playerIndices != 0
                    while (playerIndices != 0) {
                        if (!odd || m_ghostMatched) {
                            random = Random.Range(0, playerIndices);
                        }
                        else {
                            random = Random.Range(0, playerIndices + 1);
                        }
                        //If you pick the ghost
                        if (random == playerIndices && !player.GetOpponentTracker.Contains(PlayerManager.GhostID)) {
                            //GHOST
                            //Add another while loop incase you picked yourself
                            int opponentID = Random.Range(0, playerIndices - 1);
                            if (player.ID == opponentID) {
                                //If the ghost is not yourself
                                possibleOpponents.RemoveAt(opponentID);
                                playerIndices--;
                                opponentID = Random.Range(0, playerIndices - 1);
                            }
                            playerAgainstGhost = player.ID;
                            chosenGhost = opponentID;
                            chosenEnemyID[player.ID] = PlayerManager.GhostID;
                            m_ghostMatched = true;
                            found = true;
                            Debug.Log($"Player {player.ID} matched with the Ghost of Player {possibleOpponents[opponentID].ID}.");
                            break;
                        }
                        if (chosenEnemyID[possibleOpponents[random].ID] == PlayerManager.NoOpponent && !player.GetOpponentTracker.Contains(possibleOpponents[random].ID) && player.ID != possibleOpponents[random].ID) {
                            //If the person you chose does not have an opponent and you have not fought him in X turns and if your opponent is not yourself
                            chosenEnemyID[player.ID] = possibleOpponents[random].ID;
                            chosenEnemyID[possibleOpponents[random].ID] = player.ID;
                            Debug.Log($"Player {player.ID} matched with Player {chosenEnemyID[player.ID]}.");
                            Debug.Log($"Player {possibleOpponents[random].ID} matched with Player {chosenEnemyID[possibleOpponents[random].ID]}.");
                            found = true;
                            break;
                        }
                        else {
                            possibleOpponents.RemoveAt(random);
                            playerIndices--;
                        }
                    }
                    if (!found) {
                        PlayerManager fix = player;
                        possibleOpponents = new List<PlayerManager>(m_aliveList);
                        playerIndices = possibleOpponents.Count;
                        Debug.LogError("No matching at all");
                        //playerIndices != 0
                        while (playerIndices != 0) {
                            if (!odd) {
                                random = Random.Range(0, playerIndices);
                            }
                            else {
                                random = Random.Range(0, playerIndices + 1);
                            }
                            if (random == playerIndices && !fix.GetOpponentTracker.Contains(PlayerManager.GhostID)) {
                                int opponentID = Random.Range(0, playerIndices - 1);
                                if (fix.ID == opponentID) {
                                    possibleOpponents.RemoveAt(opponentID);
                                    playerIndices--;
                                    opponentID = Random.Range(0, playerIndices - 1);
                                }
                                chosenEnemyID[fix.ID] = PlayerManager.GhostID;
                                chosenGhost = opponentID;
                                int nextFix = playerAgainstGhost;
                                playerAgainstGhost = fix.ID;

                                if (nextFix == PlayerManager.GhostID) {
                                    break;
                                }

                                possibleOpponents = new List<PlayerManager>(m_aliveList);
                                playerIndices = possibleOpponents.Count;
                                Debug.Log($"NextFix: {nextFix}");
                                fix = m_playerList[nextFix];
                                break;
                            }
                            else if (random == playerIndices) {
                                continue;
                            }

                            Debug.Log($"Random: {random} and PlayerIndices: {playerIndices}");
                            if (!fix.GetOpponentTracker.Contains(possibleOpponents[random].ID) && fix.ID != possibleOpponents[random].ID) {
                                chosenEnemyID[fix.ID] = possibleOpponents[random].ID;
                                int nextFix = chosenEnemyID[possibleOpponents[random].ID];
                                chosenEnemyID[possibleOpponents[random].ID] = fix.ID;

                                if (nextFix == PlayerManager.NoOpponent) {
                                    Debug.LogError("We have fixed it!");
                                    break;
                                }

                                if (nextFix == PlayerManager.GhostID) {
                                    nextFix = playerAgainstGhost;

                                    chosenGhost = possibleOpponents[random].ID;
                                    playerAgainstGhost = fix.ID;
                                }

                                possibleOpponents = new List<PlayerManager>(m_aliveList);
                                playerIndices = possibleOpponents.Count;

                                Debug.Log($"NextFix: {nextFix}");
                                if (nextFix == PlayerManager.GhostID) {
                                    break;
                                }

                                fix = m_playerList[nextFix];
                            }
                            else {
                                possibleOpponents.RemoveAt(random);
                                playerIndices--;
                            }
                        }
                    }
                }
            }
        }
        else if (alive == 4) {
            if (m_currentRobin.Count == 0) {
                m_currentRobin = new List<int>(m_roundRobin);
            }
            if (m_currentRobin[0] == m_aliveList[0].ID) {
                m_currentRobin.RemoveAt(0);
                if (m_currentRobin.Count == 0) {
                    m_currentRobin = new List<int>(m_roundRobin);
                }
            }
            chosenEnemyID[m_aliveList[0].ID] = m_currentRobin[0];
            chosenEnemyID[m_currentRobin[0]] = m_aliveList[0].ID;
            m_currentRobin.RemoveAt(0);

            for (int i = 1; i < alive; i++) {
                if (chosenEnemyID[m_aliveList[i].ID] == PlayerManager.NoOpponent) {
                    for (int j = i + 1; j < alive; j++) {
                        if (chosenEnemyID[m_aliveList[j].ID] == PlayerManager.NoOpponent) {
                            chosenEnemyID[m_aliveList[i].ID] = m_aliveList[j].ID;
                            chosenEnemyID[m_aliveList[j].ID] = m_aliveList[i].ID;
                        }
                    }
                }
            }

        }
        else if (alive == 3) {
            Debug.Log("Calling Alive = 3");
            List<int> players = new List<int>(m_roundRobin);
            int chosenGhost = PlayerManager.GhostID;
            if (m_currentRobin.Count == 0) {
                m_currentRobin = new List<int>(m_roundRobin);
            }
            Debug.Log($"CurrentRobin: {m_currentRobin[0]} and AliveList: {m_aliveList[0].ID}");
            if (m_currentRobin[0] == m_aliveList[0].ID) {
                //If you are fighting yourself, fight the ghost
                Debug.Log("Ghost Time");
                int random = Random.Range(0, players.Count);
                if (players[random] == m_aliveList[0].ID) {
                    players.RemoveAt(random);
                    random = Random.Range(0, players.Count);
                }
                chosenGhost = random;
                chosenEnemyID[m_aliveList[0].ID] = PlayerManager.GhostID;

                chosenEnemyID[m_aliveList[1].ID] = m_aliveList[2].ID;
                chosenEnemyID[m_aliveList[2].ID] = m_aliveList[1].ID;
            }
            else {
                Debug.Log("Not Ghost Time");
                chosenEnemyID[m_aliveList[0].ID] = m_currentRobin[0];
                chosenEnemyID[m_currentRobin[0]] = m_aliveList[0].ID;

                for (int i = 1; i < alive; i++) {
                    if (chosenEnemyID[m_aliveList[i].ID] == PlayerManager.NoOpponent) {
                        int random = Random.Range(0, players.Count);
                        if (players[random] == m_aliveList[i].ID) {
                            players.RemoveAt(random);
                            random = Random.Range(0, players.Count);
                        }
                        chosenGhost = random;
                        chosenEnemyID[m_aliveList[i].ID] = PlayerManager.GhostID;
                    }
                }
            }
            m_currentRobin.RemoveAt(0);
        }
        else if (alive == 2) {
            Debug.Log("Calling Alive = 2");
            chosenEnemyID[0] = m_aliveList[1].ID;
            chosenEnemyID[1] = m_aliveList[0].ID;
        }
        for (int i = 0; i < chosenEnemyID.Length; i++) {
            if (chosenEnemyID[i] != PlayerManager.NoOpponent) {
                m_playerList[i].SetOpponent(chosenEnemyID[i]);
                Debug.Log($"Player {m_playerList[i].ID}'s TRUE opponent is Player {m_playerList[i].OpponentID}.");
            }
        }

        Debug.Log("--------------------------------------------------------------------------");
    }

    public void KillPlayer() {
        int random = Random.Range(0, m_aliveList.Count);
        m_aliveList[random].gameObject.SetActive(false);
        m_aliveList.RemoveAt(random);
        PlayerManager.OnPlayerDeath();
        foreach (PlayerManager player in m_aliveList) {
            player.TrackerUpdate();
        }
        if (m_aliveList.Count == 4) {
            UpdateMatchmakeSystem();
        }
        else if (m_aliveList.Count == 3) {
            UpdateMatchmakeSystem();
        }
        Debug.Log($"Player {random} has died");
        Debug.Log("--------------------------------------------------------------------------");
    }

    private void UpdateMatchmakeSystem() {
        Debug.Log("Update Matchmaking");
        List<PlayerManager> players = new List<PlayerManager>(m_aliveList);
        List<int> randomizedPlayerIDs = new List<int>();

        for (int i = 0; i < m_aliveList.Count; i++) {
            int random = Random.Range(0, players.Count);
            randomizedPlayerIDs.Add(players[random].ID);
            players.RemoveAt(random);
        }

        if (m_aliveList.Count == 4) {
            PlayerManager firstPlayer = m_aliveList[0];
            int firstID = randomizedPlayerIDs[0];
            if (firstID == firstPlayer.ID || firstID == firstPlayer.OpponentID) {
                UpdateMatchmakeSystem();
                return;
            }
        }

        m_currentRobin = new List<int>();
        m_roundRobin = randomizedPlayerIDs;
        Debug.Log("The order is:");
        for (int i = 0; i < m_roundRobin.Count; i++) {
            Debug.Log(m_roundRobin[i]);
        }
    }
    #endregion

    #region Phase
    private void InitializePhase() {
        m_currentPhase = Phase.BUY;
        m_currentTimer = BuyTime;
    }

    private void SwapPhase() {
        if (m_currentPhase == Phase.BUY) {
            m_currentPhase = Phase.FIGHT;
            //For Testing
            Matchmake();
            BattleManager.m_singleton.SetUpBattle(PlayerManager.m_localPlayer, m_playerList[PlayerManager.m_localPlayer.OpponentID]);
        }
        else {
            NextTurn();
            m_currentPhase = Phase.BUY;
            m_currentTimer = BuyTime;
        }
    }

    public void PlayerReadyToProceed() {
        m_PV.RPC("RPC_PlayerReadyToProceed", RpcTarget.MasterClient);
    }

    private void BufferNextPhase() {
        m_PV.RPC("RPC_BufferNextPhase", RpcTarget.All);
    }
    #endregion

    #region RPC
    [PunRPC]
    void RPC_PlayerReadyToProceed() {
        m_readyToProceed++;
        if (m_readyToProceed == m_aliveList.Count) {
            m_readyToProceed = 0;
            BufferNextPhase();
        }
    }

    [PunRPC]
    void RPC_BufferNextPhase() {
        m_isBuffer = true;
        m_currentTimer = BufferTime;
    }
    #endregion

    #region Photon
    public override void OnLeftRoom() {
        base.OnLeftRoom();
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
