using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIList : MonoBehaviour
{
    #region Variables
    private PlayerUIItem[] m_playerUIs;

    public static PlayerUIList m_singleton;
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

    #region UI
    public void UpdateRanking() {
        Debug.Log("Sorting");
        m_playerUIs = gameObject.GetComponentsInChildren<PlayerUIItem>();

        foreach (PlayerUIItem item in m_playerUIs) {
            Debug.Log($"Player {item.GetName} has {item.GetHP} HP.");
        }

        Debug.Log(m_playerUIs.Length);

        for (int j = 0; j < m_playerUIs.Length - 1; j++)
            for (int i = 0; i < m_playerUIs.Length - j - 1; i++) {
                // Check if the element is bigger than the element that comes after
                if (GreaterThan(m_playerUIs[i], m_playerUIs[i + 1])) {
                    PlayerUIItem temp = m_playerUIs[i];
                    m_playerUIs[i] = m_playerUIs[i + 1];
                    m_playerUIs[i + 1] = temp;
                }
            }

        // Display the ordered list
        int counter = 1;
        for (int k = 0; k < m_playerUIs.Length; k++) {
            m_playerUIs[k].gameObject.transform.SetAsFirstSibling();
            counter++;
        }
    }
    #endregion

    #region Helper
    private bool GreaterThan(PlayerUIItem item1, PlayerUIItem item2) {
        Debug.Log("GreaterThan called");
        Debug.Log($"Comparing {item1.GetName}: {item1.GetHP} and {item2.GetName}: {item2.GetHP}");
        if (item1.GetHP > item2.GetHP) {
            return true;
        }
        else if (item1.GetHP < item2.GetHP) {
            return false;
        }
        else {
            Debug.Log("HP is equal");
            if (item1.GetName.CompareTo(item2.GetName) <= 0) {
                Debug.Log($"{item1.GetName} comes before {item2.GetName}");
                return true;
            }
            else {
                Debug.Log($"{item2.GetName} comes before {item1.GetName}");
                return false;
            }
        }
    }
    #endregion
}
