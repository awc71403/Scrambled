using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer m_spriteRenderer;
    public int m_baseDamage;
    public int m_baseHealth;

    public TileHolder m_occupiedHolder;

    #region Getter
    public TileHolder OccupiedHolder {
        get { return m_occupiedHolder; }
        set { m_occupiedHolder = value; }
    }
    #endregion
}
