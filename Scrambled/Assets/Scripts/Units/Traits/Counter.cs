using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Counter")]
public class Counter : Trait {

    public override void TriggerAbility(PlayerManager caller, Unit unit, Unit target, Effect effect)
    {
        throw new System.NotImplementedException();
    }
}
