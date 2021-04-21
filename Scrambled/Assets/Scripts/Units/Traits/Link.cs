using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Link")]
public class Link : Trait {

    private const int HEALTHINDEX = 0;
    private const int DAMANGEINDEX = 1;

    public override void TriggerAbility(PlayerManager caller, Unit unit, Unit target, Effect effect)
    {
        int[] oldProperties = FindThresholdProperties(caller.GetPlayerTraits[m_traitType].m_oldThreshold);
        int[] currentProperties = FindThresholdProperties(caller.GetPlayerTraits[m_traitType].m_currentThreshold);
        TraitManager.m_singleton.BuffTrait(caller, this, oldProperties, currentProperties);
    }
}
