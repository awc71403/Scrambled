using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer m_spriteRenderer;
    [HideInInspector]
    public int m_baseDamage;
    [HideInInspector]
    public int m_baseHealth;

    private TileHolder m_occupiedHolder;
    private int m_column;
    private int m_row;

    #region Getter
    public TileHolder OccupiedHolder {
        get { return m_occupiedHolder; }
        set { m_occupiedHolder = value; }
    }
    #endregion
}
