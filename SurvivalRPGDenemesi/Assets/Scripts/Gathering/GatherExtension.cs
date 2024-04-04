using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherExtension : MonoBehaviour
{
    public void Gather(ItemSO toolUsed, InventoryManager inventory)
    {
        if (GetComponentInParent<GatherableObject>() == null)
            return;

        GetComponentInParent<GatherableObject>().Gather(toolUsed, inventory);
    }
}
