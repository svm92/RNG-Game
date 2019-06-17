using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour {

    public const float maxAspectRatio = 16f / 9f;

    private void Awake()
    {
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // For wider aspect ratios
        if (currentAspectRatio > maxAspectRatio)
        {
            Camera cam = GetComponent<Camera>();
            Rect camRect = cam.rect;
            camRect.width = maxAspectRatio / currentAspectRatio;
            camRect.x = (1 - camRect.width) / 2f; // x is measured from the left side
            cam.rect = camRect;
        } else // OTherwise, destroy this script
        {
            Destroy(this);
        }
    }

}
