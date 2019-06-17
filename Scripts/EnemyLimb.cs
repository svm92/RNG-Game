using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLimb : MonoBehaviour {

    public float rotationLimit;
    public bool constantRotation = false; // If this is true, consider rotationLimit to be the speed
    public float translationLimit;

    RectTransform rt;
    float startingAngle;
    float minimumAngle;
    float maximumAngle;
    float startingPos;
    float minimumPos;
    float maximumPos;

    bool armInitialized = false;
    bool allowMovement = true;

    public void initialize()
    {
        rt = GetComponent<RectTransform>();
        startingAngle = rt.rotation.eulerAngles.z;
        minimumAngle = startingAngle - rotationLimit;
        maximumAngle = startingAngle + rotationLimit;
        startingPos = rt.localPosition.x;
        minimumPos = startingPos - translationLimit;
        maximumPos = startingPos + translationLimit;

        armInitialized = true;
        enableArm();
    }

    private void OnEnable()
    {
        enableArm();
    }

    void enableArm()
    {
        if (!armInitialized) return;
        StartCoroutine(solveSortingOrderIssue());
        if (rotationLimit != 0 && allowMovement && !constantRotation)
            StartCoroutine(rotateLimb());
        if (translationLimit != 0 && allowMovement)
            StartCoroutine(translateLimb());
    }

    IEnumerator solveSortingOrderIssue() // Solves a Unity bug(?) about sorting order in nested canvases
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder++;
            yield return null;
            canvas.sortingOrder--;
        }
    }

    private void Update()
    {
        if (constantRotation) transform.Rotate(Vector3.back, rotationLimit);
    }

    IEnumerator rotateLimb()
    {
        float randomAngle = Random.Range(minimumAngle, maximumAngle);
        Quaternion startRotation = rt.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, randomAngle);
        float rotationTime = Random.Range(0.75f, 2f);
        float timer = 0;
        while (timer <= rotationTime)
        {
            rt.rotation = Quaternion.Lerp(startRotation, targetRotation, timer / rotationTime);
            timer += Time.deltaTime;
            yield return null;
        }
        rt.rotation = targetRotation;
        yield return new WaitForSeconds(Random.Range(0, 1.25f));
        if (allowMovement)
            StartCoroutine(rotateLimb());
    }

    IEnumerator translateLimb()
    {
        float randomTranslation = Random.Range(minimumPos, maximumPos);
        Vector3 currentPosition = rt.localPosition;
        Vector3 targetPosition = new Vector3(randomTranslation, rt.localPosition.y, rt.localPosition.z);
        float translationTime = Random.Range(0.75f, 2f);
        float timer = 0;
        while (timer <= translationTime)
        {
            rt.localPosition = Vector3.Lerp(currentPosition, targetPosition, timer / translationTime);
            timer += Time.deltaTime;
            yield return null;
        }
        rt.localPosition = targetPosition;
        yield return new WaitForSeconds(Random.Range(0, 1.25f));
        if (allowMovement)
            StartCoroutine(translateLimb());
    }

}
