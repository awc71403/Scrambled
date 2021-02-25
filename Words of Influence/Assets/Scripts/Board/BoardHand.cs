using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHand : MonoBehaviour
{
    #region Variables
    private bool m_isEnemyHand;

    //Temporary. Change the Transform into a class to hold the tile and have a drag/dropper.
    [SerializeField]
    private Transform[] m_tileHolders;
    private bool[] m_occupied;
    #endregion

    #region Initialization
    private void Awake() {
        m_occupied = new bool[m_tileHolders.Length];
    }
    #endregion

    #region Getter
    public Transform[] GetTileHolders {
        get { return m_tileHolders; }
    }
    #endregion

    #region Tiles
    public void Add(Tile tile) {
        for (int i = 0; i < m_occupied.Length; i++) {
            if (!m_occupied[i]) {
                m_occupied[i] = true;
                Transform setLocation = m_tileHolders[i];
                tile.gameObject.transform.SetPositionAndRotation(setLocation.transform.position, setLocation.transform.rotation);
                break;
            }
        }
    }
    #endregion
}
