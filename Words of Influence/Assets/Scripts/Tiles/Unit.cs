using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Aka Word
public class Unit
{
    #region Variables
    private Tile[] m_letters;
    private Tile m_firstLetter;
    private int m_unitLength;

    private int m_damage;
    private int m_health;

    private bool m_isHorizontal;
    #endregion

    #region Initialization
    public void Setup(Tile[] tiles, bool isHorizontal) {
        m_letters = tiles;
        m_firstLetter = tiles[0];

        m_unitLength = m_letters.Length;
        m_isHorizontal = isHorizontal;
        if (m_unitLength == 1) {
            Debug.Log($"{tiles[0].GetName} is a single tile");
            tiles[0].IsSingleTile = true;
        }
        else if (isHorizontal) {
            tiles[0].IsFirstHorizontal = true;
        }
        else {
            tiles[0].IsFirstVertical = true;
        }

        foreach (Tile tile in tiles) {
            if (isHorizontal) {
                tile.HorizontalDamage = m_unitLength;
                tile.HorizontalHealth = m_unitLength;
                tile.HorizontalUnit = this;
            }
            else {
                tile.VerticalDamage = m_unitLength;
                tile.VerticalHealth = m_unitLength;
                tile.VerticalUnit = this;
            }
        }
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

    public Tile[] GetLetters {
        get { return m_letters; }
    }

    public int GetUnitLength {
        get { return m_unitLength; }
    }
    #endregion
}
