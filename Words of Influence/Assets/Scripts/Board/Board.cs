using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private BoardHolder[] m_boardHolders;

    private BoardHolder[,] m_holderMapArray;

    private PlayerManager m_player;

    [SerializeField]
    private BoardHand m_myHand;
    private BoardHand m_opponentHand;

    public const int BoardRows = 10;
    public const int BoardColumns = 10;
    public const int HandYPosition = -1;
    #endregion

    #region Initialization
    //Tile generation needs to be made first
    //Should be Awake but keep as Start for now until the Tile generation has been moved (Just to prevent errors)
    private void Start() {
        m_boardHolders = GetComponentsInChildren<BoardHolder>();
        Debug.Log(m_boardHolders.Length);
        m_holderMapArray = new BoardHolder[BoardRows, BoardColumns];

        for (int y = 0; y < BoardColumns; y++) {
            for (int x = 0; x < BoardRows; x++) {
                m_holderMapArray[x, y] = m_boardHolders[x + y * BoardColumns];
                m_boardHolders[x + y * BoardColumns].X = x;
                m_boardHolders[x + y * BoardColumns].Y = y;
            }
        }
    }

    public void Setup(PlayerManager player) {
        m_player = player;
        m_myHand.Setup(player);
    }
    #endregion

    #region Getter
    public BoardHolder[,] GetHolderMapArray {
        get { return m_holderMapArray; }
    }

    public BoardHand GetMyHand {
        get { return m_myHand; }
    }
    #endregion
}
