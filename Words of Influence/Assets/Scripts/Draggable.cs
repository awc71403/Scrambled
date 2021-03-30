using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables
    public LayerMask m_releaseMask;

    private Camera m_cam;
    private SpriteRenderer m_spriteRenderer;

    private Vector3 m_oldPosition;
    private int m_oldSortingOrder;

    public bool m_isDragging = false;
    #endregion

    #region Initialization
    private void Start() {
        m_cam = PlayerManager.m_localPlayer.GetCamera;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Dragging
    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        Tile thisTile = GetComponent<Tile>();
        if (thisTile != null) {
            if (!thisTile.GetPlayer.GetPhotonView.IsMine) {
                return;
            }
        }
        m_oldPosition = this.transform.position;
        m_oldSortingOrder = m_spriteRenderer.sortingOrder;

        m_spriteRenderer.sortingOrder = 1;
        m_isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!m_isDragging) {
            return;
        }

        Vector3 newPosition = m_cam.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = newPosition.z + 20;
        this.transform.position = newPosition;
    }


    public void OnEndDrag(PointerEventData eventData) {
        if (!m_isDragging) {
            return;
        }

        if (!TryRelease()) {
            this.transform.position = m_oldPosition;
        }

        m_spriteRenderer.sortingOrder = m_oldSortingOrder;

        m_isDragging = false;
    }

    //Needs to be multiplayer ready
    private bool TryRelease() {
        RaycastHit2D hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, m_releaseMask);
        if (hit.collider != null) {
            TileHolder holder = hit.collider.GetComponent<TileHolder>();
            if (holder != null) {
                //If the Draggable is a Tile (will need to add Items later on)
                Tile thisTile = GetComponent<Tile>();
                if (thisTile != null) {
                    if (holder.IsMine) {
                        //If it is occupied
                        if (holder.IsOccupied) {
                            //Probably create a Trade function?
                            Debug.Log("SWAP");
                            PlayerManager.m_localPlayer.SwapTiles(thisTile, holder);
                        }
                        else {
                            //If it is not occupied
                            PlayerManager.m_localPlayer.MoveTile(thisTile, holder);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion
}
