using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Image m_myImage;
    [SerializeField]
    private TextMeshProUGUI m_myHpText;
    [SerializeField]
    private TextMeshProUGUI m_myDmgText;

    [SerializeField]
    private Image m_enemyImage;
    [SerializeField]
    private TextMeshProUGUI m_enemyHpText;
    [SerializeField]
    private TextMeshProUGUI m_enemyDmgText;

    private Unit m_myUnit;
    private Unit m_enemyUnit;

    private Animator m_animator;
    #endregion

    #region Initialization
    private void Awake() {
        m_animator = GetComponent<Animator>();
    }
    #endregion

    #region UI
    public void SetUI(Unit myUnit, Unit enemyUnit) {
        m_myUnit = myUnit;
        m_enemyUnit = enemyUnit;

        m_myImage.sprite = myUnit.GetComponent<SpriteRenderer>().sprite;
        m_myHpText.text = $"{myUnit.GetCurrentHealth}/{m_myUnit.GetMaxHealth}";
        m_myDmgText.text = $"{myUnit.GetDamage}";

        m_enemyImage.sprite = enemyUnit.GetComponent<SpriteRenderer>().sprite;
        m_enemyHpText.text = $"{enemyUnit.GetCurrentHealth}/{m_enemyUnit.GetMaxHealth}";
        m_enemyDmgText.text = $"{enemyUnit.GetDamage}";

        m_myImage.gameObject.SetActive(true);
        m_enemyImage.gameObject.SetActive(true);
        gameObject.SetActive(true);

        m_animator.Play("Fight");
    }

    public void FightUI() {
        int myHP = Mathf.Max(m_myUnit.GetCurrentHealth - m_enemyUnit.GetDamage, 0);
        int enemyHP = Mathf.Max(m_enemyUnit.GetCurrentHealth - m_myUnit.GetDamage, 0);

        m_myHpText.text = $"{myHP}/{m_myUnit.GetMaxHealth}";
        m_enemyHpText.text = $"{enemyHP}/{m_enemyUnit.GetMaxHealth}";

        if (myHP > 0) {
            AnimationManager.m_singleton.Hurt(m_myImage);
        } else {
            AnimationManager.m_singleton.Death(m_myImage);
        }

        if (enemyHP > 0) {
            AnimationManager.m_singleton.Hurt(m_enemyImage);
        }
        else {
            AnimationManager.m_singleton.Death(m_enemyImage);
        }
    }

    public void Reset() {
        m_myUnit = null;
        m_enemyUnit = null;
        gameObject.SetActive(false);
    }
    #endregion
}
