using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Variables
    public static SpawnManager m_singleton;

    [SerializeField]
    private SpawnPoint[] m_boardSpawnPoints;
    #endregion

    #region Initialization
    private void Awake() {
        m_singleton = this;
    }
    #endregion

    #region Getter
    public Transform GetBoardSpawnPoint(int i) {
        return m_boardSpawnPoints[i].transform;
    }
    #endregion
}
