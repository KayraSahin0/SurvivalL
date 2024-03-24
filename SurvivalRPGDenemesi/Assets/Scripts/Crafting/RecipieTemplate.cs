using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipieTemplate : MonoBehaviour, IPointerDownHandler
{
    private CraftingManager crafting;
    [HideInInspector] public CraftingRecipieSO recipie;

    public Image icon;
    public Text nameText;
    public Text requirementText;
    public Text timerText;

    private void Start()
    {
        crafting = GetComponentInParent<CraftingManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            crafting.Try_Craft(this);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            crafting.Cancel(this);
        }
    }
}
