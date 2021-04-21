using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Trojan")]
public class Trojan : Trait {

    public override void TriggerAbility(PlayerManager caller, Unit unit, Unit target, Effect effect)
    {
        throw new System.NotImplementedException();
    }
}
