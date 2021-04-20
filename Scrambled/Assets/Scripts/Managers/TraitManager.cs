using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    #region Variables
    public static TraitManager m_singleton;
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
    public void CheckActivation(Unit unit, Unit target, Trait trait, Trait.ActivationType activation) {
        if (trait.m_activationType == activation) {
            //Activate
            //trait.TriggerAbility()
        }
    }

    public void BuffTrait(Trait trait, int[] oldBuff, int[] newBuff) {
        PlayerManager player = PlayerManager.m_localPlayer;
        if (trait.m_traitType == TraitType.LINK) {
            
        } else if (trait.m_traitType == TraitType.FIREWALL) {

        }
    }
    #endregion
}
