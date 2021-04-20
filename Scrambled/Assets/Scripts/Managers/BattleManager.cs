using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region Variables
    private int m_myIndex;
    private int m_enemyIndex;

    private bool m_timerFinished;

    private PlayerManager m_myPlayer;
    private PlayerManager m_enemyPlayer;

    //Only support one fight and not everyone's fight (technically doesn't need to)
    private List<Unit> m_myUnits;
    //Test
    [SerializeField]
    //
    private List<Unit> m_enemyUnits;

    private List<Tile> m_enemyTileVisual;

    private BattleUI m_battleUI;

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

    private void Start() {
        m_battleUI = UIManager.m_singleton.GetBattleUI;
    }
    #endregion

    #region Battle
    public void SetUpBattle(PlayerManager me, PlayerManager enemy) {
        m_myIndex = 0;
        m_enemyIndex = 0;

        m_myPlayer = me;
        m_enemyPlayer = enemy;

        UIManager.m_singleton.Versus(enemy.GetPhotonView.Owner.NickName);

        Debug.LogError($"I, Player {me.GetPhotonView.Owner.NickName}, am against Player {enemy.GetPhotonView.Owner.NickName}");

        m_timerFinished = false;

        if (m_enemyTileVisual != null) {
            ClearEnemyVisual();
        }
        m_enemyTileVisual = new List<Tile>();

        //Move OrderUnits from PlayerManager to BattleManager
        m_myUnits = me.OrderUnits();
        //Don't need?
        //m_enemyUnits = enemy.OrderUnits();
        StartCoroutine(SetUpVisual());


    }

    private bool Fight(Unit myUnit, Unit enemyUnit) {
        m_battleUI.SetUI(myUnit, enemyUnit);

        bool myUnitDied = false;
        int myDamage = myUnit.GetDamage;
        int enemyDamage = enemyUnit.GetDamage;
        Debug.LogError($"myUnit: {myUnit.GetCurrentHealth}/{myDamage} enemyUnit: {enemyUnit.GetCurrentHealth}/{enemyDamage}");
        if (myUnit.TakeDamage(enemyDamage)) {
            m_myUnits.Remove(myUnit);
            myUnitDied = true;
        }
        if (enemyUnit.TakeDamage(myDamage)) {
            m_enemyUnits.Remove(enemyUnit);
        }
        return myUnitDied;
        //Might mess around with the index that needs to be changed/added
    }

    private void ResetVisual() {
        foreach (Unit unit in m_myPlayer.MyUnits) {
            unit.gameObject.SetActive(true);
            unit.ResetHP();
        }
        foreach (Unit unit in m_enemyPlayer.MyUnits) {
            unit.gameObject.SetActive(true);
            unit.ResetHP();
        }
        ClearEnemyVisual();
        UIManager.m_singleton.ClearVersus();
    }

    //Might be merged into another method later
    private void ClearEnemyVisual() {
        foreach (Tile tile in m_enemyTileVisual) {
            Destroy(tile.gameObject);
        }
    }

    public void TimerFinished() {
        m_timerFinished = true;
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

        foreach (Unit unit in m_enemyUnits) {
            unit.ResetHP();
        }

        //Reorganize Firewall
        //for (int i = m_enemyUnits.Count - 1; i >= 0; i--) {
        //    Unit unit = m_enemyUnits[i];
        //    TileDatabaseSO.TileData.Trait[] traits = unit.GetTraits;
        //    if (traits[0] == TileDatabaseSO.TileData.Trait.FIREWALL || traits[1] == TileDatabaseSO.TileData.Trait.FIREWALL || traits[2] == TileDatabaseSO.TileData.Trait.FIREWALL) {
        //        m_enemyUnits.RemoveAt(i);
        //        m_enemyUnits.Insert(0, unit);
        //    }
        //}
        //Reorganize Blacklist
        //for (int i = 0; i < m_enemyUnits.Count; i++) {
        //    Unit unit = m_enemyUnits[i];
        //    TileDatabaseSO.TileData.Trait[] traits = unit.GetTraits;
        //    if (traits[0] == TileDatabaseSO.TileData.Trait.BLACKLIST || traits[1] == TileDatabaseSO.TileData.Trait.BLACKLIST || traits[2] == TileDatabaseSO.TileData.Trait.BLACKLIST) {
        //        m_enemyUnits.RemoveAt(i);
        //        m_enemyUnits.Add(unit);
        //    }
        //}

        yield return new WaitForSeconds(1f);
        if (m_myUnits.Count > m_enemyUnits.Count) {
            StartCoroutine(MyCombat());
        }
        else if (m_myUnits.Count < m_enemyUnits.Count) {
            StartCoroutine(EnemyCombat());
        }
        else {
            int random = Random.Range(0, 2);
            //Done for syncing reasons
            if (m_myPlayer.ID > m_enemyPlayer.ID) {
                random = 1 - random;
            }
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
        //IF A UNIT DIES, THE INDEX MIGHT BE BROKEN
        Debug.Log("MyCombat called");
        if (m_myIndex >= m_myUnits.Count) {
            m_myIndex = 0;
        }

        Unit myUnit = m_myUnits[m_myIndex];
        Unit enemyUnit = m_enemyUnits[0];

        if (!Fight(myUnit, enemyUnit)) {
            m_myIndex++;
        }

        yield return new WaitForSeconds(3f);

        if (m_timerFinished) {
            m_myPlayer.TakeDamage(10);
            m_enemyPlayer.TakeDamage(10);
            Debug.LogError("MyCombat: TimerFinished PlayerReadyToProceed");
            GameManager.m_singleton.PlayerReadyToProceed();
            ResetVisual();
            Debug.LogError("Exit: Timer finished");
            yield break;
        }

        if (m_myUnits.Count == 0) {
            if (m_enemyUnits.Count == 0) {
                //Both Players don't take damage
                Debug.LogError("Exit: Tie");
                Debug.LogError("MyCombat: Tie PlayerReadyToProceed");
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
            else {
                Debug.LogError("Exit: I lost");
                Debug.LogError("MyCombat: My Loss PlayerReadyToProceed");
                m_myPlayer.TakeDamage(10);
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
        }
        else if (m_enemyUnits.Count == 0) {
            Debug.LogError("Exit: I won");
            Debug.LogError("MyCombat: My Win PlayerReadyToProceed");
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

        if (m_enemyIndex >= m_enemyUnits.Count) {
            m_enemyIndex = 0;
        }

        Unit myUnit = m_myUnits[0];
        Unit enemyUnit = m_enemyUnits[m_enemyIndex];

        if (!Fight(myUnit, enemyUnit)) {
            m_enemyIndex++;
        }

        yield return new WaitForSeconds(3f);

        if (m_timerFinished) {
            m_myPlayer.TakeDamage(10);
            m_enemyPlayer.TakeDamage(10);
            Debug.LogError("MyCombat: TimerFinished PlayerReadyToProceed");
            GameManager.m_singleton.PlayerReadyToProceed();
            ResetVisual();
            Debug.LogError("Exit: Timer finished");
            yield break;
        }

        if (m_myUnits.Count == 0) {
            if (m_enemyUnits.Count == 0) {
                //Both Players don't take damage
                Debug.LogError("Exit: Tie");
                Debug.LogError("EnemyCombat: Tie PlayerReadyToProceed");
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
            else {
                Debug.LogError("Exit: I lost");
                Debug.LogError("EnemyCombat: Loss PlayerReadyToProceed");
                m_myPlayer.TakeDamage(10);
                GameManager.m_singleton.PlayerReadyToProceed();
                ResetVisual();
            }
        }
        else if (m_enemyUnits.Count == 0) {
            Debug.LogError("Exit: I won");
            Debug.LogError("EnemyCombat: Won PlayerReadyToProceed");
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
