using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Blacklist")]
public class Blacklist : Trait {

    public override void TriggerAbility(PlayerManager caller, Unit unit, Unit target, Effect effect)
    {
        throw new System.NotImplementedException();
    }
}
