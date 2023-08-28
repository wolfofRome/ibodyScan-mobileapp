using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Dropdown dropdown;
    private bool isHovering, isExpanded;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        isHovering = false;
        isExpanded = false;
    }

    private void Update()
    {
        GameObject g = GameObject.Find("Dropdown_Angle");
        if (isExpanded && !dropdown.IsExpanded)
        {
            Debug.Log("Dropdown closed");
            isExpanded = false;
            if(g != null)
                g.transform.localPosition = new Vector3(520f, 40f, 0f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject g = GameObject.Find("Dropdown_Angle");
        if (g != null)
        {
            if (isHovering)
            {
                //Debug.Log("Dropdown opened");
                g.transform.localPosition = new Vector3(520f, -82f, 0f);
            }
        }
        if (isHovering && !isExpanded)
        {
            Debug.Log("Dropdown opened");
            isExpanded = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
