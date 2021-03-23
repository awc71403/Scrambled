using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region Variables
    private int myIndex;
    private int enemyIndex;

    private List<Unit> myUnits;
    private List<Unit> enemyUnits;

    private static BattleManager m_singleton;
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

        myUnits = me.OrderUnits();
        enemyUnits = enemy.OrderUnits();

        StartCoroutine(MyCombat());
    }
    #endregion

    #region Coroutine
    IEnumerator MyCombat() {
        //Attack
        yield return new WaitForSeconds(1f);
        myIndex++;
    }

    IEnumerator EnemyCombat() {
        //Attack
        yield return new WaitForSeconds(1f);
        enemyIndex++;
    }
    #endregion
}
