using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileShop : MonoBehaviour
{
    #region Variables
    public static TileShop m_singleton;

    public List<BuyTile> m_allTiles;

    [SerializeField]
    private TextMeshProUGUI m_moneyText;
    [SerializeField]
    private Button m_refreshButton;
    [SerializeField]
    private Button m_expButton;

    private TileDatabaseSO m_tileDatabase;
    #endregion

    #region Initialization
    private void Awake() {
        m_singleton = this;
    }

    private void Start() {
        m_tileDatabase = GameManager.m_singleton.m_tileDatabase;
        GenerateTiles();
    }

    public void GenerateTiles() {
        for (int i = 0; i < m_allTiles.Count; i++) {
            m_allTiles[i].Setup(m_tileDatabase.allTiles[Random.Range(0, m_tileDatabase.allTiles.Count)], this);
            m_allTiles[i].gameObject.SetActive(true);
        }
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
        GenerateTiles();
    }

    public void Refresh() {
        PlayerManager.m_localPlayer.UsedRefresh();
        GenerateTiles();
    }
    #endregion
}
