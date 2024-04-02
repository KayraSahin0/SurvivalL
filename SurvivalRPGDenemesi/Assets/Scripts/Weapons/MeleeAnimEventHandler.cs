using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimEventHandler : MonoBehaviour
{
    public void CheckForHit()
    {
        GetComponentInParent<Weapon>().CheckForHit();
    }

    public void ExecuteHit()
    {
        GetComponentInParent<Weapon>().ExecuteHit();
    }
}
