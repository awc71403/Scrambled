using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    public static UIManager m_singleton;

    [SerializeField]
    private Image m_unitUI;
    [SerializeField]
    private Image m_unitImage;
    [SerializeField]
    private TextMeshProUGUI m_hpText;
    [SerializeField]
    private TextMeshProUGUI m_dmgText;

    [SerializeField]
    private TextMeshProUGUI m_expText;
    [SerializeField]
    private Slider m_expBar;

    [SerializeField]
    private TextMeshProUGUI m_unitsText;

    #endregion

    #region Initialization
    private void Awake() {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        m_singleton = this;
    }
    #endregion

    #region Update
    #endregion

    #region Player
    public void UpdatePlayerLevel() {
        PlayerManager player = PlayerManager.m_localPlayer;
        int currentExp = player.GetCurrentExp;
        int expThreshold = player.GetExpThreshold[player.GetLevel - 1];
        m_expText.text = $"LV {player.GetLevel}: {currentExp}/{expThreshold}";
        m_expBar.value = (float)currentExp / (float)expThreshold;
    }
    #endregion

    #region Unit
    public void UpdateUnit(Unit unit) {
        m_unitUI.gameObject.SetActive(true);
        m_unitImage.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        m_hpText.text = $"{unit.GetCurrentHealth}/{unit.GetMaxHealth}";
        m_dmgText.text = unit.GetDamage.ToString();
    }

    public void CloseUnitUI() {
        m_unitUI.gameObject.SetActive(false);
    }
    #endregion

    #region Tiles
    public void UpdateTiles(int tilesInPlay) {
        m_unitsText.text = $"Tiles: {tilesInPlay}/{PlayerManager.m_localPlayer.GetLevel * PlayerManager.TilesPerLevel}";
    }
    #endregion
}
