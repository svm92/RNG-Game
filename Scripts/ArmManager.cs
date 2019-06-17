using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmManager : MonoBehaviour {

    public GameObject armNormal;
    public GameObject armHook;
    public GameObject armPowerGlow;
    public GameObject armBlade;
    public GameObject armBuzzsaw;
    public GameObject armShield;
    public GameObject armFan;
    public GameObject armGrapnel;
    public GameObject armSalve;
    public GameObject armForbiddenRelic;
    public GameObject armSiphon;
    public GameObject armBattery;
    public GameObject armMiasma;
    public GameObject armAegis;
    public GameObject armCartilage;
    public GameObject armFloe;
    public GameObject armIllusionist;

    public GameObject particleSilver;
    public GameObject particleGold;

    public Material silverMaterial;
    public Material goldMaterial;

    public static ArmManager amInstance;

    // N of combinations: (r+n-1)!/(r! * (n-1)!) for n=different types of arms, r=arms combined at once
    public enum Arm { NORMAL, HOOK, POWER_GLOW, BLADE, BUZZSAW, SHIELD, FAN, GRAPNEL, SALVE, FORBIDDEN_RELIC,
        SIPHON, BATTERY, MIASMA, AEGIS, CARTILAGE, FLOE, ILLUSIONIST };

    public enum ArmAspect { NORMAL, SILVER, GOLD };

    enum Model { STANDARD, // Upper arm, gear, lower arm, hand
        NON_SYMMETRICAL, // Upper arm, gear, lower arm, hand [all mirrored for non-symmetrical designs]
        THREE_SEGMENTS, // Upper arm, upper gear, middle arm, lower gear, lower arm
        NO_GEAR }; // Upper arm, lower arm, hand
    enum Side { LEFT, RIGHT, UPPER_LEFT, LOWER_LEFT, UPPER_RIGHT, LOWER_RIGHT };

    private void Awake()
    {
        amInstance = this;
    }

    public List<Arm> assignArms(Transform tr, Arm[] possibleArms, int nOfArms, List<ArmAspect> armAspects)
    {
        //assignArmsDebug(tr, Arm.BLADE); return new List<Arm>() { }; // Debug, force specific arms
        List<Arm> armTypes = new List<Arm>();
        List<Side> spawnPoints;

        switch (nOfArms)
        {
            case 2:
                spawnPoints = new List<Side>() { Side.LEFT, Side.RIGHT };
                break;
            case 4:
                spawnPoints = new List<Side>() { Side.UPPER_LEFT, Side.UPPER_RIGHT, Side.LOWER_LEFT, Side.LOWER_RIGHT };
                break;
            case 6:
                spawnPoints = new List<Side>() { Side.UPPER_LEFT, Side.UPPER_RIGHT, Side.LEFT, Side.RIGHT,
                    Side.LOWER_LEFT, Side.LOWER_RIGHT };
                break;
            case 8:
                spawnPoints = new List<Side>() { Side.UPPER_LEFT, Side.UPPER_LEFT, Side.UPPER_RIGHT, Side.UPPER_RIGHT,
                    Side.LOWER_LEFT, Side.LOWER_LEFT, Side.LOWER_RIGHT, Side.LOWER_RIGHT};
                break;
            default:
                spawnPoints = new List<Side>();
                break;
        }

        Arm arm;

        for (int i = 0; i < nOfArms; i++)
        {
            arm = getRandomArm(possibleArms);
            armTypes.Add(arm);
            assignSingleArm(tr, arm, spawnPoints[i], armAspects[i]);
        }

        return armTypes;
    }

    public void assignArmsDebug(Transform tr, Arm arm1)
    {
        assignArmsDebug(tr, arm1, arm1);
    }

    public void assignArmsDebug(Transform tr, Arm arm1, Arm arm2)
    {
        assignSingleArm(tr, arm1, Side.LEFT, ArmAspect.NORMAL);
        assignSingleArm(tr, arm2, Side.RIGHT, ArmAspect.NORMAL);
    }

    public void assignArmsDebug(Transform tr, Arm arm1, Arm arm2, Arm arm3, Arm arm4)
    {
        assignSingleArm(tr, arm1, Side.UPPER_LEFT, ArmAspect.NORMAL);
        assignSingleArm(tr, arm2, Side.UPPER_RIGHT, ArmAspect.NORMAL);
        assignSingleArm(tr, arm3, Side.LOWER_LEFT, ArmAspect.NORMAL);
        assignSingleArm(tr, arm4, Side.LOWER_RIGHT, ArmAspect.NORMAL);
    }

    void assignSingleArm(Transform tr, Arm arm, Side side, ArmAspect armAspect)
    {
        GameObject armBase = getArmBase(arm);
        GameObject armObj = Instantiate(armBase);
        armObj.transform.SetParent(tr);
        Transform armTr = armObj.GetComponent<RectTransform>();
        Transform armBaseTr = armBase.GetComponent<RectTransform>();
        armTr.localScale = armBaseTr.localScale;
        armTr.localPosition = armBaseTr.localPosition;
        fixVerticalArmPosition(armTr, arm, side);

        applyArmAspect(armTr, armAspect, arm);

        // Left arms have default positions/rotations, so stop
        if (side == Side.LEFT || side == Side.UPPER_LEFT || side == Side.LOWER_LEFT) return;

        Model armModel = getModel(arm);
        fixRightArmPosition(armTr, armModel);
    }

    GameObject getArmBase(Arm arm)
    {
        switch (arm)
        {
            case Arm.NORMAL:
                return armNormal;
            case Arm.HOOK:
                return armHook;
            case Arm.POWER_GLOW:
                return armPowerGlow;
            case Arm.BLADE:
                return armBlade;
            case Arm.BUZZSAW:
                return armBuzzsaw;
            case Arm.SHIELD:
                return armShield;
            case Arm.FAN:
                return armFan;
            case Arm.GRAPNEL:
                return armGrapnel;
            case Arm.SALVE:
                return armSalve;
            case Arm.FORBIDDEN_RELIC:
                return armForbiddenRelic;
            case Arm.SIPHON:
                return armSiphon;
            case Arm.BATTERY:
                return armBattery;
            case Arm.MIASMA:
                return armMiasma;
            case Arm.AEGIS:
                return armAegis;
            case Arm.CARTILAGE:
                return armCartilage;
            case Arm.FLOE:
                return armFloe;
            case Arm.ILLUSIONIST:
                return armIllusionist;
        }
        Debug.Log("Check getArmBase");
        return null;
    }

    Model getModel(Arm arm)
    {
        switch (arm)
        {
            case Arm.NORMAL:
            case Arm.POWER_GLOW:
            case Arm.BUZZSAW:
            case Arm.SHIELD:
            case Arm.FAN:
            case Arm.GRAPNEL:
            case Arm.SALVE:
            case Arm.FORBIDDEN_RELIC:
            case Arm.SIPHON:
            case Arm.BATTERY:
            case Arm.MIASMA:
            case Arm.FLOE:
                return Model.STANDARD;
            case Arm.BLADE:
            case Arm.CARTILAGE:
            case Arm.ILLUSIONIST:
                return Model.NON_SYMMETRICAL;
            case Arm.AEGIS:
                return Model.THREE_SEGMENTS;
            case Arm.HOOK:
                return Model.NO_GEAR;
        }
        Debug.Log("Model not associated, check getModel");
        return Model.STANDARD;
    }

    public static string getArmDescription(Arm arm, ArmAspect armAspect)
    {
        string armAspectLetter = "";

        switch (armAspect)
        {
            case ArmAspect.NORMAL:
            default:
                armAspectLetter = "";
                break;
            case ArmAspect.SILVER:
                armAspectLetter = "_S";
                break;
            case ArmAspect.GOLD:
                armAspectLetter = "_G";
                break;
        }
        
        TextScript.Sentence armName = (TextScript.Sentence)
            System.Enum.Parse(typeof(TextScript.Sentence), "ARM" + (int)arm, true);

        TextScript.Sentence armDescription = (TextScript.Sentence)
                    System.Enum.Parse(typeof(TextScript.Sentence), "ARM" + (int)arm + "_DESC" + armAspectLetter, true);

        return "<b>" + TextScript.get(armName) + "</b>\n\n" + TextScript.get(armDescription);
    }

    void fixVerticalArmPosition(Transform armTr, Arm arm, Side side)
    {
        float verticalDisplacement = 0;
        switch (side)
        {
            case Side.UPPER_LEFT:
            case Side.UPPER_RIGHT:
                verticalDisplacement = 0;
                break;
            case Side.LOWER_LEFT:
            case Side.LOWER_RIGHT:
                verticalDisplacement = -100;
                break;
            default:
                return;
        }
        armTr.localPosition = new Vector2(armTr.localPosition.x, armTr.localPosition.y + verticalDisplacement);
        fixCanvasOrder(armTr.gameObject, arm, side);
    }

    void applyArmAspect(Transform armTr, ArmAspect armAspect, Arm arm)
    {
        Material aspectMaterial;
        GameObject aspectParticleSystem;

        // Choose material & particle system to apply (stop if normal)
        switch (armAspect)
        {
            case ArmAspect.NORMAL:
            default:
                return;
            case ArmAspect.SILVER:
                aspectMaterial = silverMaterial;
                aspectParticleSystem = particleSilver;
                break;
            case ArmAspect.GOLD:
                aspectMaterial = goldMaterial;
                aspectParticleSystem = particleGold;
                break;
        }

        // Apply particle system
        Transform handTr = findGrandchild(armTr, "Hand");
        Transform particleSystemTr = Instantiate(aspectParticleSystem).transform;
        particleSystemTr.SetParent(handTr);
        particleSystemTr.localPosition = Vector3.zero;
        particleSystemTr.localScale = Vector3.one;

        // Special cases
        if (armAspect == ArmAspect.SILVER && arm == Arm.BUZZSAW)
        {
            foreach (Image img in armTr.GetComponentsInChildren<Image>())
            {
                img.color = new Color(0.53f, 1, 1, 1);
            }
            return;
        }
        if (armAspect == ArmAspect.SILVER && arm == Arm.CARTILAGE)
        {
            foreach (Image img in armTr.GetComponentsInChildren<Image>())
            {
                img.color = new Color(0.66f, 0.85f, 0.95f, 1);
            }
            return;
        }

        // Apply chosen material
        foreach (Image img in armTr.GetComponentsInChildren<Image>())
        {
            img.material = aspectMaterial;
        }
    }

    void fixRightArmPosition(Transform armTr, Model armModel)
    {
        Transform middleArmTr; Transform lowerArmTr; Transform upperGearTr; Transform lowerGearTr; Transform handTr;
        float mirroredAngle;
        switch (armModel)
        {
            case Model.STANDARD:
                // Reposition upper arm
                armTr.localPosition = new Vector2(-armTr.localPosition.x, armTr.localPosition.y);
                mirroredAngle = getMirroredAngle(armTr.localEulerAngles.z);
                armTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition gear
                upperGearTr = findGrandchild(armTr, "Gear");
                mirroredAngle = getMirroredAngle(upperGearTr.localEulerAngles.z);
                upperGearTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition lower arm
                lowerArmTr = findGrandchild(armTr, "LowerArm");
                if (lowerArmTr.localPosition.x != 0) // If it's not centered on the parent, mirror it
                    lowerArmTr.localPosition = new Vector2(-lowerArmTr.localPosition.x, lowerArmTr.localPosition.y);
                mirroredAngle = getMirroredAngle(lowerArmTr.localEulerAngles.z);
                lowerArmTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition hand
                handTr = findGrandchild(armTr, "Hand");
                mirroredAngle = getMirroredAngle(handTr.localEulerAngles.z);
                handTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);
                break;
            case Model.NON_SYMMETRICAL:
                // Reposition upper arm
                armTr.localPosition = new Vector2(-armTr.localPosition.x, armTr.localPosition.y);
                mirroredAngle = getMirroredAngle(armTr.localEulerAngles.z);
                armTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);
                armTr.localScale = new Vector2(armTr.localScale.x, -armTr.localScale.y);
                break;
            case Model.THREE_SEGMENTS:
                // Reposition upper arm
                armTr.localPosition = new Vector2(-armTr.localPosition.x, armTr.localPosition.y);
                mirroredAngle = getMirroredAngle(armTr.localEulerAngles.z);
                armTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition upper gear
                upperGearTr = findGrandchild(armTr, "UpperGear");
                mirroredAngle = getMirroredAngle(upperGearTr.localEulerAngles.z);
                upperGearTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition middle arm
                middleArmTr = findGrandchild(armTr, "MiddleArm");
                if (middleArmTr.localPosition.x != 0) // If it's not centered on the parent, mirror it
                    middleArmTr.localPosition = new Vector2(-middleArmTr.localPosition.x, middleArmTr.localPosition.y);
                mirroredAngle = getMirroredAngle(middleArmTr.localEulerAngles.z);
                middleArmTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition lower gear
                lowerGearTr = findGrandchild(armTr, "LowerGear");
                mirroredAngle = getMirroredAngle(lowerGearTr.localEulerAngles.z);
                lowerGearTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition lower arm
                lowerArmTr = findGrandchild(armTr, "LowerArm");
                if (lowerArmTr.localPosition.x != 0) // If it's not centered on the parent, mirror it
                    lowerArmTr.localPosition = new Vector2(-lowerArmTr.localPosition.x, lowerArmTr.localPosition.y);
                mirroredAngle = getMirroredAngle(lowerArmTr.localEulerAngles.z);
                lowerArmTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition hand
                handTr = findGrandchild(armTr, "Hand");
                mirroredAngle = getMirroredAngle(handTr.localEulerAngles.z);
                handTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);
                break;
            case Model.NO_GEAR:
                // Reposition upper arm
                armTr.localPosition = new Vector2(-armTr.localPosition.x, armTr.localPosition.y);
                mirroredAngle = getMirroredAngle(armTr.localEulerAngles.z);
                armTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);

                // Reposition lower arm
                lowerArmTr = findGrandchild(armTr, "LowerArm");
                mirroredAngle = getMirroredAngle(lowerArmTr.localEulerAngles.z);
                lowerArmTr.localEulerAngles = new Vector3(0, 0, mirroredAngle);
                lowerArmTr.localScale = new Vector2(-lowerArmTr.localScale.x, lowerArmTr.localScale.y);
                break;
        }
    }

    void fixCanvasOrder(GameObject armObj, Arm arm, Side side)
    {
        Canvas[] canvases = armObj.GetComponentsInChildren<Canvas>();
        switch (side)
        {
            case Side.UPPER_LEFT:
            case Side.UPPER_RIGHT:
                canvases[0].sortingOrder = (arm != Arm.ILLUSIONIST) ? -4 : -5;
                canvases[1].sortingOrder = -4;
                break;
            case Side.LOWER_LEFT:
            case Side.LOWER_RIGHT:
                canvases[0].sortingOrder = -3;
                canvases[1].sortingOrder = -1;
                break;
        }
    }

    // Gets the mirrored horizontal angle
    float getMirroredAngle(float angle)
    {
        // Make the angle positive
        while (angle < 0) angle += 360;
        // Find quadrant
        int quadrant;
        if (angle >= 0 && angle <= 90) quadrant = 1;
        else if (angle >= 90 && angle <= 180) quadrant = 2;
        else if (angle >= 180 && angle <= 270) quadrant = 3;
        else quadrant = 4;
        // Get angle between this angle and the closest vertical wall (90º or 270º)
        float angleToVertical;
        switch (quadrant)
        {
            default:
            case 1:
            case 2:
                angleToVertical = Vector3.Angle(Vector3.up, Quaternion.Euler(0, 0, angle) * Vector3.right);
                break;
            case 3:
            case 4:
                angleToVertical = Vector3.Angle(Vector3.down, Quaternion.Euler(0, 0, angle) * Vector3.right);
                break;
        }
        
        // New arm must be placed at twice that angle (so that it's mirrored)
        float angleDisplacement = angleToVertical * 2f;
        int angleSign = (quadrant == 1 || quadrant == 3) ? 1 : -1;
        return angle + (angleDisplacement * angleSign);
    }

    Arm getRandomArm(Arm[] possibleArms)
    {
        int rnd = Random.Range(0, possibleArms.Length);
        return possibleArms[rnd];
    }

    Transform findGrandchild(Transform tr, string name)
    {
        foreach (Transform child in tr)
        {
            if (child.name == name)
                return child;
            Transform grandchild = findGrandchild(child, name);
            if (grandchild != null)
                return grandchild;
        }

        return null;
    }

}
