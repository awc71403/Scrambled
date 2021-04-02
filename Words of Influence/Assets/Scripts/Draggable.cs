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
            if (GameManager.m_singleton.GetCurrentPhase == GameManager.Phase.FIGHT && thisTile.OccupiedHolder.Y != Board.HandYPosition) {
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
                        //Check to see if you can put it down or not during certain Phases
                        if ((GameManager.m_singleton.GetCurrentPhase == GameManager.Phase.FIGHT || GameManager.m_singleton.GetIsBuffer) && (thisTile.OccupiedHolder.Y != Board.HandYPosition || holder.Y != Board.HandYPosition)) {
                            return false;
                        }
                        //If it is occupied
                        PlayerManager player = PlayerManager.m_localPlayer;
                        if (holder.IsOccupied) {
                            if (holder == thisTile.OccupiedHolder) {
                                return false;
                            }
                            //Probably create a Trade function?
                            player.SwapTiles(thisTile, holder);
                        }
                        else {
                            //If it is not occupied
                            //And if we are not capped on tiles in play and the tile isn't coming from the hand to the board
                            if (player.GetTilesInPlay >= player.GetLevel * PlayerManager.TilesPerLevel && thisTile.OccupiedHolder.Y == Board.HandYPosition && holder.Y != Board.HandYPosition) {
                                return false;
                            }
                            player.MoveTile(thisTile, holder);
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
