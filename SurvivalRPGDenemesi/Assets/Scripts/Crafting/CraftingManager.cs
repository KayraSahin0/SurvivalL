using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private InventoryManager inventory;

    public RecipieTemplate recipieTemplate;

    public CraftingRecipieSO[] recipies;
    public Transform contentHolder;

    // PRIVATE VARIABLES
    public RecipieTemplate recipieInCraft;
    private bool isCrafting;
    private float currentTimer;

    private void Start()
    {
        inventory = GetComponentInParent<InventoryManager>();
        GenerateRecipies();
    }

    private void Update()
    {
        if (isCrafting)
        {
            if(currentTimer > 0)
            {
                recipieInCraft.timerText.text = currentTimer.ToString("f2");
            }
            else
            {
                recipieInCraft.timerText.text = "";
                inventory.AddItem(recipieInCraft.recipie.outcome, recipieInCraft.recipie.outcomeAmount);
                isCrafting = false;
            }

            currentTimer -= Time.deltaTime;
        }
    }

    public void GenerateRecipies()
    {
        for(int i = 0; i < recipies.Length; i++)
        {
            RecipieTemplate recipie = Instantiate(recipieTemplate.gameObject, contentHolder).GetComponent<RecipieTemplate>();

            recipie.recipie = recipies[i];
            recipie.icon.sprite = recipies[i].icon;
            recipie.nameText.text = recipies[i].recipieName;
            recipie.timerText.text = "";

            for(int b = 0; b < recipies[i].requirements.Length; b++)
            {
                if(b == 0)
                    recipie.requirementText.text = $"{recipies[i].requirements[b].data.itemName} {recipies[i].requirements[b].amountNeeded}";
                else
                    recipie.requirementText.text = $"{recipie.requirementText.text}, {recipies[i].requirements[b].data.itemName} {recipies[i].requirements[b].amountNeeded}";
            }
        }
    }

    public void Try_Craft(RecipieTemplate template)
    {
        if (!HasResources(template.recipie) || isCrafting)
            return;

        TakeResources(template.recipie);

        recipieInCraft = template;
        isCrafting = true;
        currentTimer = template.recipie.craftingTime;
    }

    public void Cancel(RecipieTemplate template)
    {
        if (!isCrafting)
            return;

        for(int i = 0; i < template.recipie.requirements.Length; i++)
        {
            inventory.AddItem(template.recipie.requirements[i].data, template.recipie.requirements[i].amountNeeded);
        }

        isCrafting = false;
        recipieInCraft.timerText.text = "";
    }

    public bool HasResources(CraftingRecipieSO recipie)
    {
        bool canCraft = true;

        int[] stacksNeeded = null;
        int[] stacksAvailable = null;
        List<int> stacksNeededList = new List<int>();

        // GET STACKS NEEDED
        for(int i = 0; i < recipie.requirements.Length; i++)
        {
            stacksNeededList.Add(recipie.requirements[i].amountNeeded);
        }
        stacksNeeded = stacksNeededList.ToArray();
        stacksAvailable = new int[stacksNeeded.Length];

        // CHECK FOR ITEMS
        for (int b = 0; b < recipie.requirements.Length; b++)
        {
            for (int i = 0; i < inventory.inventorySlots.Length; i++)
            {
                if (inventory.inventorySlots[i].data == recipie.requirements[b].data)
                {
                    stacksAvailable[b] += inventory.inventorySlots[i].stackSize;
                }
            }
        }

        // CHECK IF IT CAN CRAFT
        for(int i = 0; i < stacksAvailable.Length; i++)
        {
            if (stacksAvailable[i] < stacksNeeded[i])
            {
                canCraft = false;
                break;
            }
        }

        return canCraft;
    }

    public void TakeResources(CraftingRecipieSO recipie)
    {
        int[] stacksNeeded = null;
        List<int> stacksNeededList = new List<int>();

        // GET STACKS NEEDED
        for (int i = 0; i < recipie.requirements.Length; i++)
        {
            stacksNeededList.Add(0);
        }
        stacksNeeded = stacksNeededList.ToArray();

        // TAKE ITEMS
        for(int i = 0; i < recipie.requirements.Length; i++)
        {
            for(int b = 0; b < inventory.inventorySlots.Length; b++)
            {
                if (!inventory.inventorySlots[b].IsEmpty)
                {
                    if (inventory.inventorySlots[b].data == recipie.requirements[i].data)
                    {
                        if (stacksNeeded[i] < recipie.requirements[i].amountNeeded)
                        {
                            if (stacksNeeded[i] + inventory.inventorySlots[b].stackSize > recipie.requirements[i].amountNeeded)
                            {
                                int amountLeftOnSlot = (inventory.inventorySlots[b].stackSize + stacksNeeded[i]) - recipie.requirements[i].amountNeeded;
                                inventory.inventorySlots[b].stackSize = amountLeftOnSlot;
                                stacksNeeded[i] = recipie.requirements[i].amountNeeded;
                            }
                            else
                            {
                                stacksNeeded[i] += inventory.inventorySlots[b].stackSize;
                                inventory.inventorySlots[b].Clean();
                            }
                        }

                        inventory.inventorySlots[b].UpdateSlot();
                    }
                }
            }
        }
    }
}
