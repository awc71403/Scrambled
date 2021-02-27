﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
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
        m_cam = Camera.main;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
    #endregion


    #region Dragging
    public void OnStartDrag() {
        m_oldPosition = this.transform.position;
        m_oldSortingOrder = m_spriteRenderer.sortingOrder;

        m_spriteRenderer.sortingOrder = 1; 
        m_isDragging = true;
    }

    public void OnDragging() {
        if (!m_isDragging) {
            return;
        }

        Vector3 newPosition = m_cam.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;
        this.transform.position = newPosition;
    }

    public void OnEndDrag() {
        if (!m_isDragging) {
            return;
        }

        if (!TryRelease()) {
            this.transform.position = m_oldPosition;
        }

        m_spriteRenderer.sortingOrder = m_oldSortingOrder;

        m_isDragging = false;
    }

    private bool TryRelease() {
        RaycastHit2D hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, m_releaseMask);
        if (hit != null && hit.collider != null) {
            TileHolder holder = hit.collider.GetComponent<TileHolder>();
            if (holder != null) {
                //If the Draggable is a Tile (will need to add Items later on)
                Tile thisTile = GetComponent<Tile>();
                if (thisTile != null) {
                    if (!holder.IsOccupied) {
                        thisTile.OccupiedHolder.IsOccupied = false;
                        thisTile.OccupiedHolder = holder;
                        holder.IsOccupied = true;
                        thisTile.transform.position = holder.transform.position;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion
}
