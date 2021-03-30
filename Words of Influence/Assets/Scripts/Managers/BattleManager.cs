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
        m_enemyUnits = enemy.OrderUnits();

        StartCoroutine(SetUpVisual());
    }

    private void Fight(Unit myUnit, Unit enemyUnit) {
        if (myUnit.TakeDamage(enemyUnit.GetDamage)) {
            m_myUnits.Remove(myUnit);
        }
        if (enemyUnit.TakeDamage(myUnit.GetDamage)) {
            m_enemyUnits.Remove(enemyUnit);
        }
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
        BoardHolder[,] myBoardHolders = m_myPlayer.GetBoard.GetHolderMapArray;
        BoardHolder[,] enemyBoardHolders = m_enemyPlayer.GetBoard.GetHolderMapArray;
        for (int y = Board.BoardColumns / 2; y < Board.BoardColumns; y++) {
            for (int x = 0; x < Board.BoardRows; x++) {
                BoardHolder enemyholder = enemyBoardHolders[x, y];
                if (enemyholder.IsOccupied) {
                    (int, int) mirror = Mirror(x, y);
                    BoardHolder myholder = myBoardHolders[mirror.Item1, mirror.Item2];

                    Tile tile = enemyholder.Tile;
                    Tile newTile = Instantiate(tile);
                    newTile.transform.position = myholder.transform.position;
                    m_enemyTileVisual.Add(newTile);
                }
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
        Unit myUnit = m_myUnits[myIndex];
        Unit enemyUnit = m_enemyUnits[0];
        Fight(myUnit, enemyUnit);
        yield return new WaitForSeconds(1f);
        myIndex++;
        if (m_myUnits.Count == 0) {
            if (m_enemyUnits.Count == 0) {
                //Both Players don't take damage
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
            else {
                m_myPlayer.TakeDamage(10);
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
        }
        else if (m_enemyUnits.Count == 0) {
            GameManager.m_singleton.PlayerReadyToProceed();
            ResetVisual();
        }
        else {
            StartCoroutine(EnemyCombat());
        }
    }

    IEnumerator EnemyCombat() {
        //Attack
        Unit myUnit = m_myUnits[0];
        Unit enemyUnit = m_enemyUnits[enemyIndex];
        Fight(myUnit, enemyUnit);
        yield return new WaitForSeconds(1f);
        enemyIndex++;
        if (m_myUnits.Count == 0) {
            if (m_enemyUnits.Count == 0) {
                //Both Players don't take damage
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
            else {
                m_myPlayer.TakeDamage(10);
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
        }
        else if (m_enemyUnits.Count == 0) {
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
        int returnX = Mathf.Abs(x - (Board.BoardColumns - 1));
        int returnY = Mathf.Abs(y - (Board.BoardRows - 1));
        Debug.Log($"Mirror of ({x}, {y}) is ({returnX}, {returnY})");
        return (returnX, returnY);
    }
    #endregion
}
