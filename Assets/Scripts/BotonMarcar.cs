using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BotonMarcar : MonoBehaviour, IPointerDownHandler
{
    Image imageButton;
    bool active=false;
    bool Interaction;

    private void Start()
    {
        imageButton = GetComponent<Image>();
        SwipeRotation.OnMarcarCelda += resetColor;
        
    }

    

    public delegate void MarcarButtonDown(Celda celda);
    public static event MarcarButtonDown OnMarcarButtonDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        active = !active;
        Debug.Log("boton interraccion presionado");
        imageButton.color = new Color(imageButton.color.r, imageButton.color.g, imageButton.color.b,0.5f);
        if (OnMarcarButtonDown != null)
            OnMarcarButtonDown(null);
    }

    void resetColor(Celda celda)
    {
        imageButton.color = new Color(imageButton.color.r, imageButton.color.g, imageButton.color.b, 1f);
    }

    

    

}