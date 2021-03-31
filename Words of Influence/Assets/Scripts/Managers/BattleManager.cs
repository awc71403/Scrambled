using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region Variables
    private int myIndex;
    private int enemyIndex;

    private PlayerManager m_myPlayer;
    private PlayerManager m_enemyPlayer;

    //Only support one fight and not everyone's fight (technically doesn't need to)
    private List<Unit> m_myUnits;
    //Test
    [SerializeField]
    //
    private List<Unit> m_enemyUnits;

    private List<Tile> m_enemyTileVisual;

    public static BattleManager m_singleton;
    #endregion

    #region Initialization
    private void Awake() {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        m_singleton = this;
    }
    #endregion

    #region Battle
    public void SetUpBattle(PlayerManager me, PlayerManager enemy) {
        myIndex = 0;
        enemyIndex = 0;

        m_myPlayer = me;
        m_enemyPlayer = enemy;

        m_enemyTileVisual = new List<Tile>();

        //Move OrderUnits from PlayerManager to BattleManager
        m_myUnits = me.OrderUnits();
        //Don't need?
        //m_enemyUnits = enemy.OrderUnits();

        StartCoroutine(SetUpVisual());
    }

    private void Fight(Unit myUnit, Unit enemyUnit) {
        int myDamage = myUnit.GetDamage;
        int enemyDamage = enemyUnit.GetDamage;
        if (myUnit.TakeDamage(myDamage)) {
            m_myUnits.Remove(myUnit);
        }
        if (enemyUnit.TakeDamage(enemyDamage)) {
            m_enemyUnits.Remove(enemyUnit);
        }
        //Might mess around with the index that needs to be changed/added
    }

    private void ResetVisual() {
        foreach (Unit unit in m_myPlayer.MyUnits) {
            unit.gameObject.SetActive(true);
        }
        foreach (Unit unit in m_enemyPlayer.MyUnits) {
            unit.gameObject.SetActive(true);
        }
        ClearEnemyVisual();
    }

    //Might be merged into another method later
    private void ClearEnemyVisual() {
        foreach (Tile tile in m_enemyTileVisual) {
            Destroy(tile.gameObject);
        }
    }
    #endregion

    #region Coroutine
    IEnumerator SetUpVisual() {
        m_enemyUnits = new List<Unit>();

        BoardHolder[,] myBoardHolders = m_myPlayer.GetBoard.GetHolderMapArray;
        BoardHolder[,] enemyBoardHolders = m_enemyPlayer.GetBoard.GetHolderMapArray;
        for (int y = Board.BoardColumns / 2; y < Board.BoardColumns; y++) {
            for (int x = 0; x < Board.BoardRows; x++) {
                BoardHolder enemyHolder = enemyBoardHolders[x, y];
                if (enemyHolder.IsOccupied) {
                    (int, int) mirror = Mirror(x, y);
                    BoardHolder myholder = myBoardHolders[mirror.Item1, mirror.Item2];

                    Tile tile = enemyHolder.Tile;
                    Tile newTile = Instantiate(tile);
                    newTile.transform.position = myholder.transform.position;
                    m_enemyTileVisual.Add(newTile);

                    if (newTile.IsFirstHorizontal || newTile.IsSingleTile) {
                        Unit unit = newTile.HorizontalUnit;
                        m_enemyUnits.Add(unit);
                    }
                    if (enemyHolder.Tile.IsFirstVertical) {
                        Unit unit = newTile.VerticalUnit;
                        m_enemyUnits.Add(unit);
                    }
                }
            }
        }

        //Reorganize Firewall
        for (int i = m_enemyUnits.Count - 1; i >= 0; i--) {
            Unit unit = m_enemyUnits[i];
            TileDatabaseSO.TileData.Trait[] traits = unit.GetTraits;
            if (traits[0] == TileDatabaseSO.TileData.Trait.FIREWALL || traits[1] == TileDatabaseSO.TileData.Trait.FIREWALL || traits[2] == TileDatabaseSO.TileData.Trait.FIREWALL) {
                m_enemyUnits.RemoveAt(i);
                m_enemyUnits.Insert(0, unit);
            }
        }
        //Reorganize Blacklist
        for (int i = 0; i < m_enemyUnits.Count; i++) {
            Unit unit = m_enemyUnits[i];
            TileDatabaseSO.TileData.Trait[] traits = unit.GetTraits;
            if (traits[0] == TileDatabaseSO.TileData.Trait.BLACKLIST || traits[1] == TileDatabaseSO.TileData.Trait.BLACKLIST || traits[2] == TileDatabaseSO.TileData.Trait.BLACKLIST) {
                m_enemyUnits.RemoveAt(i);
                m_enemyUnits.Add(unit);
            }
        }

        yield return new WaitForSeconds(1f);
        if (m_myUnits.Count > m_enemyUnits.Count) {
            StartCoroutine(MyCombat());
        }
        else if (m_myUnits.Count < m_enemyUnits.Count) {
            StartCoroutine(EnemyCombat());
        }
        else {
            int random = Random.Range(0, 2);
            if (random == 0) {
                StartCoroutine(MyCombat());
            }
            else {
                StartCoroutine(EnemyCombat());
            }
        }
    }

    IEnumerator MyCombat() {
        //Attack
        Debug.Log("MyCombat called");
        Unit myUnit = m_myUnits[myIndex];
        Unit enemyUnit = m_enemyUnits[0];
        Fight(myUnit, enemyUnit);
        yield return new WaitForSeconds(1f);
        myIndex++;
        if (myIndex >= m_myUnits.Count) {
            myIndex = 0;
        }
        if (m_myUnits.Count == 0) {
            if (m_enemyUnits.Count == 0) {
                //Both Players don't take damage
                Debug.Log("Exit: Tie");
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
            else {
                Debug.Log("Exit: I lost");
                m_myPlayer.TakeDamage(10);
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
        }
        else if (m_enemyUnits.Count == 0) {
            Debug.Log("Exit: I won");
            GameManager.m_singleton.PlayerReadyToProceed();
            ResetVisual();
        }
        else {
            StartCoroutine(EnemyCombat());
        }
    }

    IEnumerator EnemyCombat() {
        //Attack
        Debug.Log("EnemyCombat called");
        Unit myUnit = m_myUnits[0];
        Unit enemyUnit = m_enemyUnits[enemyIndex];
        Fight(myUnit, enemyUnit);
        yield return new WaitForSeconds(1f);
        enemyIndex++;
        if (enemyIndex >= m_enemyUnits.Count) {
            enemyIndex = 0;
        }
        if (m_myUnits.Count == 0) {
            if (m_enemyUnits.Count == 0) {
                //Both Players don't take damage
                Debug.Log("Exit: Tie");
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
            else {
                Debug.Log("Exit: I lost");
                m_myPlayer.TakeDamage(10);
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
        }
        else if (m_enemyUnits.Count == 0) {
            Debug.Log("Exit: I won");
            GameManager.m_singleton.PlayerReadyToProceed();
            ResetVisual();
        }
        else {
            StartCoroutine(MyCombat());
        }
    }
    #endregion

    #region Helper
    private (int, int) Mirror(int x, int y) {
        int returnX = x;
        int returnY = Mathf.Abs(y - (Board.BoardRows - 1));
        Debug.Log($"Mirror of ({x}, {y}) is ({returnX}, {returnY})");
        return (returnX, returnY);
    }
    #endregion
}
