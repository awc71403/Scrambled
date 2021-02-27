using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using TMPro;

public class BuyTile : MonoBehaviour
{
    #region Variables
    private ButtonManager m_buyButtonManager;
    private Button m_buyButton;
    [SerializeField]
    private TextMeshProUGUI m_healthText;
    [SerializeField]
    private TextMeshProUGUI m_attackText;
    [SerializeField]
    private TextMeshProUGUI m_costText;
    private int m_health;
    private int m_attack;
    private int m_cost;

    private TileDatabaseSO.TileData m_myData;
    private TileShop m_shopRef;
    #endregion

    #region Initialization
    private void Awake() {
        m_buyButtonManager = GetComponent<ButtonManager>();
        m_buyButton = GetComponent<Button>();
    }

    public void Setup(TileDatabaseSO.TileData myData, TileShop shopRef) {
        m_health = myData.m_health;
        m_attack = myData.m_attack;
        m_cost = myData.m_cost;

        m_healthText.text = m_health.ToString();
        m_attackText.text = m_attack.ToString();
        m_costText.text = m_cost.ToString();

        m_buyButtonManager.buttonText = myData.m_name;

        m_myData = myData;
        m_shopRef = shopRef;
    }
    #endregion

    #region Update
    private void Update() {
        //CanBuy();
    }
    #endregion

    #region Buy
    public void Buy() {
        m_shopRef.BuyTile(m_myData);
    }

    private void CanBuy() {
        if (PlayerManager.m_localPlayer.GetMoney < m_cost) {
            m_buyButton.interactable = false;
        }
        else {
            m_buyButton.interactable = true;
        }
    }
    #endregion
}
