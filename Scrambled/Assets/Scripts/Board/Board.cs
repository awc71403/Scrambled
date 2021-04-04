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
    [SerializeField]
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
        m_holderMapArray = new BoardHolder[BoardRows, BoardColumns];

        for (int y = 0; y < BoardColumns; y++) {
            for (int x = 0; x < BoardRows; x++) {
                m_holderMapArray[x, y] = m_boardHolders[x + y * BoardColumns];
                m_boardHolders[x + y * BoardColumns].X = x;
                m_boardHolders[x + y * BoardColumns].Y = y;
                if (y >= BoardColumns / 2) {
                    if (m_player.GetPhotonView.IsMine) {
                        m_holderMapArray[x, y].IsMine = true;
                    }
                }
                else {
                    m_holderMapArray[x, y].GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
                }
            }
        }

        if (m_player.GetPhotonView.IsMine) {
            foreach (TileHolder holder in m_myHand.GetTileHolders) {
                holder.IsMine = true;
            }
        }

        foreach (TileHolder holder in m_opponentHand.GetTileHolders) {
            holder.GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
        }

        for (int y = 0; y < BoardColumns; y++) {
            for (int x = 0; x < BoardRows; x++) {
                if (x - 1 >= 0) {
                    m_holderMapArray[x, y].Left = m_holderMapArray[x - 1, y];
                }
                if (x + 1 < BoardRows) {
                    m_holderMapArray[x, y].Right = m_holderMapArray[x + 1, y];
                }
                if (y + 1 < BoardColumns) {
                    m_holderMapArray[x, y].Down = m_holderMapArray[x, y + 1];
                }
                if (y - 1 >= 0) {
                    m_holderMapArray[x, y].Up = m_holderMapArray[x, y - 1];
                }
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
