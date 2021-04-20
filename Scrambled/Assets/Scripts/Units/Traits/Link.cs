using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trait/Link")]
public class Link : Trait {

    private const int HEALTHINDEX = 0;
    private const int DAMANGEINDEX = 1;

    public override void TriggerAbility(Unit unit, Unit target, Effect effect)
    {
        throw new System.NotImplementedException();
    }
}
