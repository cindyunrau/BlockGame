using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    private Color m_ShadowColor;
    private Color m_ActiveColor;

    private bool active = true;
    public int row;
    public int col;

    public bool ghost;

    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_ShadowColor = Color.gray;
        m_ActiveColor = Color.white;

        m_SpriteRenderer.color = m_ActiveColor;

        ghost = false;
    }

    private void Update()
    {
        if (ghost)
        {
            m_SpriteRenderer.color = m_ShadowColor;
        }
        else
        {
            m_SpriteRenderer.color = m_ActiveColor;
        }
    }

    //public void InShadow(bool isInShadow)
    //{
    //    if (isInShadow)
    //    {
    //        m_SpriteRenderer.color = m_ShadowColor;
    //    }
    //    else
    //    {
    //        m_SpriteRenderer.color = m_ActiveColor;
    //    }
    //}

    public bool IsEmpty()
    {
        return active;
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

}