using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Database", menuName= "CustomSO/TileDatabase")]
public class TileDatabaseSO : ScriptableObject {

    [System.Serializable]
    public struct TileData {
        public Tile m_tilePrefab;
        public int m_ID;
        public string m_name;
        public Trait m_trait1;
        public Trait m_trait2;
        public Trait m_trait3;

        public int m_cost;

        public int m_health;
        public int m_attack;
    }

    [SerializeField]
    public List<TileData> allTiles;
}
