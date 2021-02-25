using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Tile[] m_tiles;

    private Tile[,] m_tileMapArray;

    private PlayerManager m_player;

    [SerializeField]
    private BoardHand m_myHand;
    private BoardHand m_opponentHand;

    public const int m_boardRows = 10;
    public const int m_boardColumns = 10;
    #endregion

    #region Initialization
    //Should be Awake but keep as Start for now until the Tile generation has been moved (Just to prevent errors)
    private void Start() {
        m_tileMapArray = new Tile[m_boardRows, m_boardColumns];

        for (int y = 0; y < m_boardColumns; y++) {
            for (int x = 0; x < m_boardRows; x++) {
                //m_tileMapArray[x, y] = m_tiles[x + y * m_boardColumns];
            }
        }
    }
    #endregion

    #region Getter
    public BoardHand GetMyHand {
        get { return m_myHand; }
    }
    #endregion
}
