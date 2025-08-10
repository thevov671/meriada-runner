using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [SerializeField] private Image _itemIcon; 

    public void Init(Sprite icon)
    {
        _itemIcon.sprite = icon;   
    }
}
