using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolder : MonoBehaviour
{
    #region Variables
    private Tile m_tile;
    private bool m_isOccupied;
    private int m_x;
    private int m_y;
    #endregion

    #region Getter/Setter
    public Tile Tile {
        get { return m_tile; }
        set { m_tile = value; }
    }

    public bool IsOccupied {
        get { return m_isOccupied; }
        set { m_isOccupied = value; }
    }

    public int X {
        get { return m_x; }
        set { m_x = value; }
    }

    public int Y {
        get { return m_y; }
        set { m_y = value; }
    }
    #endregion


}
