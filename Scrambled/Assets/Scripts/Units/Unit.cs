using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Aka Word
public class Unit: MonoBehaviour
{
    #region Variables
    private Tile[] m_letters;
    private Tile m_firstLetter;
    private int m_unitLength;

    [SerializeField]
    private int m_damage;
    [SerializeField]
    private int m_maxHealth;
    private int m_currHealth;

    private int m_healthBuffs;
    private int m_damageBuffs;

    private SpriteRenderer m_spriteRenderer;

    private bool m_token;

    [SerializeField]
    private Trait[] m_traits;

    private bool m_isHorizontal;
    #endregion

    #region Initialization
    private void Awake() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Tile[] tiles, bool isHorizontal) {
        m_damage = 0;
        m_maxHealth = 0;
        m_token = false;

        m_letters = tiles;
        m_firstLetter = tiles[0];

        m_traits = new Trait[3];
        m_traits[0] = m_firstLetter.GetData.m_trait1;
        m_traits[1] = m_firstLetter.GetData.m_trait2;
        m_traits[2] = m_firstLetter.GetData.m_trait3;

        SetLocation(m_firstLetter);

        m_unitLength = m_letters.Length;
        m_isHorizontal = isHorizontal;
        if (m_unitLength == 1) {
            tiles[0].IsSingleTile = true;
        }
        else if (isHorizontal) {
            tiles[0].IsFirstHorizontal = true;
        }
        else {
            tiles[0].IsFirstVertical = true;
        }

        if (tiles[0].IsSingleTile) {
            m_damage = tiles[0].GetDamage;
            m_maxHealth = tiles[0].GetHealth;
            tiles[0].HorizontalUnit = this;
        }
        else {
            foreach (Tile tile in tiles) {
                if (isHorizontal) {
                    //Change later
                    tile.HorizontalDamage = m_unitLength;
                    tile.HorizontalHealth = m_unitLength;
                    tile.HorizontalUnit = this;
                }
                else {
                    //Change later
                    tile.VerticalDamage = m_unitLength;
                    tile.VerticalHealth = m_unitLength;
                    tile.VerticalUnit = this;
                }

                m_damage += tile.GetDamage + tile.VerticalDamage + tile.HorizontalDamage;
                m_maxHealth += tile.GetHealth + tile.VerticalHealth + tile.HorizontalHealth;
            }
        }

        ResetHP();
    }
    #endregion

    #region Update
    public void UpdateWord() {

    }
    #endregion

    #region Getter
    public Tile GetFirstLetter {
        get { return m_firstLetter; }
    }

    public Trait[] GetTraits {
        get { return m_traits; }
    }

    public Tile[] GetLetters {
        get { return m_letters; }
    }

    public int GetUnitLength {
        get { return m_unitLength; }
    }

    public int GetMaxHealth {
        get { return m_maxHealth; }
    }

    public int GetCurrentHealth {
        get { return m_currHealth; }
    }

    public int GetDamage {
        get { return m_damage; }
    }
    #endregion

    #region Transform
    private void SetLocation(Tile tile) {
        transform.SetParent(tile.transform);
        transform.position = tile.transform.position;
    }
    #endregion

    #region Battle
    //Returns if dead
    public bool TakeDamage(int damage) {
        m_currHealth -= damage;
        if (m_currHealth > 0) {
            AnimationManager.m_singleton.Hurt(m_spriteRenderer);
            return false;
        }
        else {
            AnimationManager.m_singleton.Death(m_spriteRenderer);
            return true;
        }
    }

    public void ResetHP() {
        m_currHealth = m_maxHealth;
    }
    #endregion

    #region Trait
    public void CheckAbilityCond(Trait.ActivationType type) {
    }
    #endregion

    #region Mouse
    private void OnMouseOver() {
        UIManager.m_singleton.UpdateUnit(this);
    }
    private void OnMouseExit() {
        UIManager.m_singleton.CloseUnitUI();
    }
    #endregion
}
