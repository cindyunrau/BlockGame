using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBlock : MonoBehaviour
{
    private bool isDragging = false;
    
    public float gridSize = 1f;

    private Transform shadow;
    private Vector3 offset;

    private void Start()
    {
        shadow = this.gameObject.transform.GetChild(0);
    }

    void OnMouseDown()
    {
        isDragging = true;
        // Calculate offset from mouse position to object's center
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 currentWorldPoint = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(currentWorldPoint.x / gridSize) * gridSize,
                Mathf.Round(currentWorldPoint.y / gridSize) * gridSize,
                transform.position.z
            );

            transform.position = currentWorldPoint;
            shadow.position = snappedPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.position = shadow.position;
    }
}