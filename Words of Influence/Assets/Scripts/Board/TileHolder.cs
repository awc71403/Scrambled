using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolder : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Tile m_tile;
    private bool m_isOccupied;
    private bool m_isMine;
    private int m_x;
    private int m_y;
    
    private TileHolder m_leftHolder;
    private TileHolder m_rightHolder;
    private TileHolder m_upHolder;
    private TileHolder m_downHolder;
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

    public bool IsMine {
        get { return m_isMine; }
        set { m_isMine = value; }
    }

    public int X {
        get { return m_x; }
        set { m_x = value; }
    }

    public int Y {
        get { return m_y; }
        set { m_y = value; }
    }

    public TileHolder Left {
        get { return m_leftHolder; }
        set { m_leftHolder = value; }
    }

    public TileHolder Right {
        get { return m_rightHolder; }
        set { m_rightHolder = value; }
    }

    public TileHolder Up {
        get { return m_upHolder; }
        set { m_upHolder = value; }
    }

    public TileHolder Down {
        get { return m_downHolder; }
        set { m_downHolder = value; }
    }
    #endregion


}
