using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShop : MonoBehaviour
{
    public List<BuyTile> m_allTiles;

    private TileDatabaseSO m_tileDatabase;

    private void Start() {
        m_tileDatabase = GameManager.m_singleton.m_tileDatabase;
        GenerateTiles();
    }

    public void GenerateTiles() {
        for (int i = 0; i < m_allTiles.Count; i++) {
            m_allTiles[i].Setup(m_tileDatabase.allTiles[Random.Range(0, m_tileDatabase.allTiles.Count)], this);
        }
    }

    public void BuyTile(TileDatabaseSO.TileData cardData) {
        Debug.Log(PlayerManager.m_localPlayer);
        PlayerManager.m_localPlayer.OnBoughtTile(cardData);
    }
}
