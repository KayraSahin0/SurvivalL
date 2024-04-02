using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV_Handler : MonoBehaviour
{
    private Camera cam;
    public Weapon weapon;
    public float fovTransitionSpeed = 10f;
    [HideInInspector] public float defaultFOV;

    private void Start()
    {
        cam = GetComponent<Camera>();
        defaultFOV = cam.fieldOfView;
    }

    
    private void Update()
    {
        if (weapon != null)
        {
            if (weapon.isAiming)
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, weapon.weaponData.zoomFOV, fovTransitionSpeed * Time.deltaTime);
            else
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFOV, fovTransitionSpeed * Time.deltaTime);
        }
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFOV, fovTransitionSpeed * Time.deltaTime);
    }
}
