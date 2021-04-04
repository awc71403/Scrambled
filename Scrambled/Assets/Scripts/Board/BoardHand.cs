using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHand : MonoBehaviour
{
    #region Variables
    private bool m_isEnemyHand;
    private Board m_board;
    private PlayerManager m_player;

    //Temporary. Change the Transform into a class to hold the tile and have a drag/dropper.
    [SerializeField]
    private HandHolder[] m_handHolders;
    #endregion

    #region Initialization
    private void Awake() {
        m_board = GetComponentInParent<Board>();
        m_handHolders = GetComponentsInChildren<HandHolder>();
        for (int i = 0; i < m_handHolders.Length; i++) {
            m_handHolders[i].X = i;
            m_handHolders[i].Y = Board.HandYPosition;
        }
    }

    public void Setup(PlayerManager player) {
        m_player = player;
    }
    #endregion

    #region Getter
    public TileHolder[] GetTileHolders {
        get { return m_handHolders; }
    }
    #endregion

    #region Tiles
    public void Add(Tile tile) {
        m_player.TilesInHand++;
        for (int i = 0; i < m_handHolders.Length; i++) {
            HandHolder tileHolder = m_handHolders[i];
            if (!tileHolder.IsOccupied) {
                tileHolder.IsOccupied = true;
                tileHolder.Tile = tile;
                tile.OccupiedHolder = tileHolder;
                Transform setLocation = tileHolder.transform;
                tile.gameObject.transform.SetPositionAndRotation(setLocation.transform.position, setLocation.transform.rotation);
                break;
            }
        }
    }
    #endregion
}
