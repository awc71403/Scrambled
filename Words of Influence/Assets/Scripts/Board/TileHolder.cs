using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolder : MonoBehaviour
{
    #region Variables
    private Tile m_tile;
    private bool m_isOccupied;
    #endregion

    #region Getter/Setter
    public Tile GetTile {
        get { return m_tile; }
    }

    public bool IsOccupied {
        get { return m_isOccupied; }
        set { m_isOccupied = value; }
    }
    #endregion


}
