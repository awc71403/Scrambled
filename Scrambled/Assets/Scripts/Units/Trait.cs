using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitType { LINK, WORM, VIRUS, ENCRYPTER, FIREWALL, BLACKLIST, TROJAN, RELAY, MACRO, BOTNET, COUNTER, OPS, CIPHER, ZERO}

public abstract class Trait : ScriptableObject
{
    #region Struct
    [System.Serializable]
    public struct TraitThresholds {
        #region Variables
        public int m_threshold1;
        public int m_threshold2;
        public int m_threshold3;

        public const int NOTHRESHOLD = -1;
        #endregion
    }
    #endregion

    public enum ActivationType { NONE, SUMMON, DEATH, DAMAGE, ATTACK }
    public TraitType m_traitType;
    public ActivationType m_activationType;

    public TraitThresholds m_traitThresholds;

    public int[] m_threshold1Properties;
    public int[] m_threshold2Properties;
    public int[] m_threshold3Properties;

    public abstract void TriggerAbility(Unit unit, Unit target, Effect effect);

    public int[] FindThresholdProperties(int threshold) {
        if (threshold < m_traitThresholds.m_threshold1) {
            return null;
        }
        else if (threshold < m_traitThresholds.m_threshold2 && m_traitThresholds.m_threshold2 != TraitThresholds.NOTHRESHOLD)
        {
            return m_threshold1Properties;
        }
        else if (threshold < m_traitThresholds.m_threshold3 && m_traitThresholds.m_threshold2 != TraitThresholds.NOTHRESHOLD)
        {
            return m_threshold2Properties;
        }
        else
        {
            return m_threshold3Properties;
        }
    }
}
