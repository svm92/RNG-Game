using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleIllusionistManager : MonoBehaviour {

    ParticleSystem ps;
    ParticleSystem.ShapeModule shp;
    Transform arm1Tr, arm2Tr;

    const float maxRadiusWidth = 1.6f; // Radius of the shape module that covers the entire lower arm
    const float lowerArmWidth = 300;
    const float correctionFactor = 0.35f; // To correct displacements from coordinate centers + others

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        shp = ps.shape;
        arm1Tr = transform.parent.GetChild(0);
        arm2Tr = transform.parent.GetChild(1);


        StartCoroutine(checkIfFlipFlame());
        StartCoroutine(reorderFlameInLayer());
        StartCoroutine(alterRadius());
    }

    IEnumerator checkIfFlipFlame()
    {
        yield return null;
        // Reposition flame if the arm is on the right side
        float enemyX = transform.parent.parent.parent.parent.position.x;
        float handX = transform.position.x;
        if (handX > enemyX)
            shp.rotation += Vector3.forward * 180;
    }

    IEnumerator reorderFlameInLayer()
    {
        yield return null;
        int canvasOrder = transform.parent.parent.GetComponent<Canvas>().sortingOrder;
        ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
        psr.sortingOrder = canvasOrder - 1; // -1 because the canvas sortingOrder is displaced by 1
    }

    IEnumerator alterRadius()
    {
        float arm1Pos = arm1Tr.localPosition.x;
        float arm2Pos = arm2Tr.localPosition.x;
        shp.radius = maxRadiusWidth * Mathf.Abs(arm2Pos - arm1Pos) / lowerArmWidth * correctionFactor;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(alterRadius());
    }

}
