using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    #region Variables
    public static TraitManager m_singleton;

    public const int HEALTHINDEX = 0;
    public const int DAMANGEINDEX = 1;
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

    #region Trait
    public void CheckActivation(PlayerManager caller, Unit unit, Unit target, Trait trait, Trait.ActivationType activation) {
        if (trait.m_activationType == activation) {
            trait.TriggerAbility(caller, unit, target, null);
        }
    }

    public void BuffTrait(PlayerManager caller, Trait trait, int[] oldBuff, int[] newBuff) {
        List<Unit> contributors = caller.GetPlayerTraits[trait.m_traitType].m_contributors;

        foreach (Unit unit in contributors) {
            unit.HealthBuff -= oldBuff[HEALTHINDEX];
            unit.DamageBuff -= oldBuff[DAMANGEINDEX];

            unit.HealthBuff += newBuff[HEALTHINDEX];
            unit.DamageBuff += newBuff[DAMANGEINDEX];
        }
    }

    #endregion
}
