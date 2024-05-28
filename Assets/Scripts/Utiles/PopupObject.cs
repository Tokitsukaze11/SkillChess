using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public event Action<bool,string> OnMouseOverPopup;
    public void InitDescription(DescriptObject descriptObject)
    {
        description = descriptObject.Description;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOverPopup?.Invoke(true,description);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseOverPopup?.Invoke(false,null);
    }
}
