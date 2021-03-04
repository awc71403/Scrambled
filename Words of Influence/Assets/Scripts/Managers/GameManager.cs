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

    private int m_turn;
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
    #endregion
}
