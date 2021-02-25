using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Database", menuName= "CustomSO/TileDatabase")]
public class TileDatabaseSO : ScriptableObject {

    [System.Serializable]
    public struct TileData {
        public Tile m_tilePrefab;
        public string m_name;

        public int m_health;
        public int m_attack;
        public int m_cost;
    }

    [SerializeField]
    public List<TileData> allTiles;
}
