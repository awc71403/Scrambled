using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Worm")]
public class Worm : Trait {

    private const int LIFESTEALINDEX = 0;

    public override void TriggerAbility(PlayerManager caller, Unit unit, Unit target, Effect effect)
    {
        int[] currentProperties = FindThresholdProperties(caller.GetPlayerTraits[m_traitType].m_currentThreshold);

        //Need whole number
        unit.Heal((unit.GetDamage + unit.DamageBuff) * currentProperties[LIFESTEALINDEX]);
    }
}
