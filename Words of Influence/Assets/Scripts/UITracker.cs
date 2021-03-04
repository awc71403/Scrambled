using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITracker : MonoBehaviour {

    #region Private Variables
    private GameObject m_trackObject;

    [SerializeField]
    private Vector3 m_offset;
    #endregion

    #region Getter
    public GameObject TrackObject {
        get { return m_trackObject; }
        set { m_trackObject = value; }
    }
    #endregion

    #region Update
    void Update() {
        if (m_trackObject != null) {
            gameObject.transform.position = Camera.main.WorldToScreenPoint(m_trackObject.transform.position) + m_offset;
        }
    }
    #endregion
}
