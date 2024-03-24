using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipie", menuName = "Survival Game/Crafting/New Recipie")]
public class CraftingRecipieSO : ScriptableObject
{
    public Sprite icon;
    public string recipieName;

    public CraftingRequirement[] requirements;
    [Space]
    public float craftingTime;
    [Space]
    public ItemSO outcome;
    public int outcomeAmount = -1;
}
