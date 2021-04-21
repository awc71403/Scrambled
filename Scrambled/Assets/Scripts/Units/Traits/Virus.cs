using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Virus")]
public class Virus : Trait {

    private const int HEALTHINDEX = 0;
    private const int DAMANGEINDEX = 1;

    public override void TriggerAbility(PlayerManager caller, Unit unit, Unit target, Effect effect)
    {
        int[] currentProperties = FindThresholdProperties(caller.GetPlayerTraits[m_traitType].m_currentThreshold);

        BattleManager.m_singleton.AddMinion(currentProperties[HEALTHINDEX], currentProperties[DAMANGEINDEX], caller == PlayerManager.m_localPlayer);
    }
}
