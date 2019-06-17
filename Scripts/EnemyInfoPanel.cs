using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPanel : MonoBehaviour {

    public GameObject emptyPanel;
    public GameObject emptyTextContainer;

    public Sprite coreNormal;

    Enemy enemy;

    Transform imageContainer;

    private void Awake()
    {
        imageContainer = transform.Find("ImageContainer");
        enemy = BattleManager.bmInstance.enemy;
    }

    public void hidePanel()
    {
        gameObject.SetActive(false);
        BattleManager.bmInstance.makeEnemyInteractable(true);
    }

    private void OnEnable()
    {
        deleteOldImagesAndTexts();
        addNewPanels();
        addNewTexts();
        addMaxRollText();
    }

    void addNewPanels()
    {
        int nArms = enemy.armList.Count;

        // Body panel
        addNewPanel(new Vector2(0, 70), ArmManager.ArmAspect.NORMAL);

        // Arm panels
        switch (nArms)
        {
            case 2:
                addNewPanel(new Vector2(-525, 70), enemy.armAspects[0]);
                addNewPanel(new Vector2(525, 70), enemy.armAspects[1]);
                break;
            case 4:
                addNewPanel(new Vector2(-525, 190), enemy.armAspects[0]);
                addNewPanel(new Vector2(525, 190), enemy.armAspects[1]);
                addNewPanel(new Vector2(-525, -190), enemy.armAspects[2]);
                addNewPanel(new Vector2(525, -190), enemy.armAspects[3]);
                break;
            case 6:
            case 8:
                addNewPanel(new Vector2(-525, 70), enemy.armAspects[0], nArms);
                break;
        }
    }

    void addNewPanel(Vector2 position, ArmManager.ArmAspect armAspect)
    {
        addNewPanel(position, armAspect, 4);
    }

    void addNewPanel(Vector2 position, ArmManager.ArmAspect armAspect, int nArms)
    {
        GameObject panelObj = Instantiate(emptyPanel);
        panelObj.transform.SetParent(imageContainer);
        RectTransform panelRT = panelObj.GetComponent<RectTransform>();
        panelRT.localScale = Vector3.one;
        panelRT.localPosition = position;


        if (nArms >= 6)
        {
            GameObject nArmsText = panelRT.GetChild(1).gameObject;
            nArmsText.SetActive(true);
            nArmsText.GetComponent<Text>().text = "x" + nArms;
            if (nArms == 8) nArmsText.GetComponent<Text>().color = new Color(0.2f, 0.5f, 0.8f, 1);
        }

        Color armAspectColor;
        string expBonusText;

        switch (armAspect)
        {
            case ArmManager.ArmAspect.NORMAL:
            default:
                return;
            case ArmManager.ArmAspect.SILVER:
                armAspectColor = new Color(0.68f, 0.95f, 0.9f, 0.39f);
                expBonusText = "+25%";
                break;
            case ArmManager.ArmAspect.GOLD:
                armAspectColor = new Color(0.96f, 1f, 0.18f, 0.39f);
                expBonusText = "+50%";
                break;
        }

        panelObj.GetComponent<Image>().color = armAspectColor;
        GameObject extraExpText = panelRT.GetChild(0).gameObject;
        extraExpText.SetActive(true);
        extraExpText.GetComponent<Text>().text = expBonusText + " " + TextScript.get(TextScript.Sentence.EXP);
    }

    void addNewTexts()
    {
        int nArms = enemy.armList.Count;

        string enemyName = "<b><size=35>" + enemy.enemyName + "</size></b>\n";
        addNewText(enemyName + CoreManager.getCoreDescription(enemy.currentCore), new Vector2(0, 70));

        switch (nArms)
        {
            case 2:
                addNewText(ArmManager.getArmDescription(enemy.armList[0], enemy.armAspects[0]), new Vector2(-525, 70));
                addNewText(ArmManager.getArmDescription(enemy.armList[1], enemy.armAspects[1]), new Vector2(525, 70));
                break;
            case 4:
                addNewText(ArmManager.getArmDescription(enemy.armList[0], enemy.armAspects[0]), new Vector2(-525, 190));
                addNewText(ArmManager.getArmDescription(enemy.armList[1], enemy.armAspects[1]), new Vector2(525, 190));
                addNewText(ArmManager.getArmDescription(enemy.armList[2], enemy.armAspects[2]), new Vector2(-525, -190));
                addNewText(ArmManager.getArmDescription(enemy.armList[3], enemy.armAspects[3]), new Vector2(525, -190));
                break;
            case 6:
            case 8:
                addNewText(ArmManager.getArmDescription(enemy.armList[0], enemy.armAspects[0]), new Vector2(-525, 70));
                break;
        }
        
    }

    void addMaxRollText()
    {
        addNewText("<b>" +  TextScript.get(TextScript.Sentence.MAX_ROLL) + "</b>: " + Player.maxRoll, new Vector2(0, 300));
    }

    void addNewText(string text, Vector2 position)
    {
        GameObject textObj = Instantiate(emptyTextContainer);
        textObj.GetComponent<Text>().text = text;
        textObj.transform.SetParent(imageContainer);
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.localScale = Vector2.one;
        textRT.localPosition = position;
    }

    void deleteOldImagesAndTexts()
    {
        foreach (Transform child in imageContainer)
            Destroy(child.gameObject);
    }

}
