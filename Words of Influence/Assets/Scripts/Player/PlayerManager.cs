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

    [SerializeField]
    private PlayerUIItem m_playerUIItemPrefab;
    private PlayerUIItem m_playerUIItem;

    public static PlayerManager m_localPlayer;

    public const int m_startingHP = 100;
    #endregion

    #region Initialization
    void Awake() {
        m_roomManager = RoomManager.m_singleton;
        m_camera = GetComponentInChildren<Camera>();
        m_PV = GetComponent<PhotonView>();

        m_HP = m_startingHP;

        if (m_PV.IsMine) {
            m_localPlayer = this;
        }
    }

    void Start() {
        if (!m_PV.IsMine) {
            Destroy(m_camera.gameObject);
        }
        GameManager.m_singleton.AddPlayer(this);
        CreatePlayerUI();
    }

    private void CreatePlayerUI() {
        Debug.Log($"CreatePlayerUI called. Player {m_PV.Owner.NickName} has {m_HP} HP.");
        m_playerUIItem = Instantiate(m_playerUIItemPrefab);
        m_playerUIItem.SetUp(this);
        m_playerUIItem.gameObject.transform.SetParent(PlayerUIList.m_singleton.transform);
        m_playerUIItem.gameObject.transform.localScale = new Vector3(1, 1, 1);
        PlayerUIList.m_singleton.UpdateRanking();
    }
    #endregion

    #region Getter
    public PhotonView GetPhotonView {
        get { return m_PV; }
    }

    public int GetHP {
        get { return m_HP; }
    }

    public Camera GetCamera {
        get { return m_camera; }
    }
    #endregion

    #region UI
    #endregion
}
