using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RerollManager : MonoBehaviour {

    public static bool alreadyVisitedSinceLoading = false;

    Button returnButton;
    Button rerollButton;

    int nTimesPressedReroll;

    private void Awake()
    {
        returnButton = GameObject.Find("ReturnButton").GetComponent<Button>();
        rerollButton = GameObject.Find("RerollButton").GetComponent<Button>();

        returnButton.GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.RETURN);
        rerollButton.GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.REROLL);
        GameObject.Find("Label").GetComponent<Text>().text = TextScript.get(TextScript.Sentence.REROLL_UPPERCASE);
        GameObject.Find("ExplanationText").GetComponent<Text>().text = getExplanationText();
    }

    private void Start()
    {
        alreadyVisitedSinceLoading = true;
        if (Player.keyboardModeOn) selectFirstButton();
    }

    private void Update()
    {
        // If left/right/down/up pressed, activate keyboard mode
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !Player.keyboardModeOn)
        {
            Player.keyboardModeOn = true;
            selectFirstButton();
        }

    }

    string getExplanationText()
    {
        double newRerollPoints = getRerollPoints(HubControl.maxUnlockedDifficultyId);

        string explanationText = TextScript.get(TextScript.Sentence.REROLL_EXPLANATION_INTRO) +

            "<b>" + TextScript.get(TextScript.Sentence.REROLL_CARDS) + " +" 
            + NumberStringConverter.convert(newRerollPoints + Player.rerollPoints) 
            + "</b> (" + TextScript.get(TextScript.Sentence.CURRENT) + ": +" + 
            NumberStringConverter.convert(Player.rerollPoints) + ")\n" +

             "<b>" + TextScript.get(TextScript.Sentence.REROLL_EXP) + " x" 
            + NumberStringConverter.convert(getRerollExpMult(newRerollPoints + Player.rerollPoints)) 
            + "</b> (" + TextScript.get(TextScript.Sentence.CURRENT) + ": x" + 
            NumberStringConverter.convert(getRerollExpMult(Player.rerollPoints)) + ")\n" +

             "<b>" + TextScript.get(TextScript.Sentence.REROLL_SKIP) + " "
            + NumberStringConverter.convert(getRerollSkip(newRerollPoints + Player.rerollPoints))
            + "</b> (" + TextScript.get(TextScript.Sentence.CURRENT) + ": " +
            NumberStringConverter.convert(getRerollSkip(Player.rerollPoints)) + ")\n\n" +

            ((Player.isAndroid()) ? TextScript.get(TextScript.Sentence.REROLL_EXPLANATION_END_TAP) :
                                    TextScript.get(TextScript.Sentence.REROLL_EXPLANATION_END_CLICK));
       return explanationText;
    }

    public void checkIfReroll()
    {
        nTimesPressedReroll++;
        rerollButton.GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.REROLL) 
            + "(" + nTimesPressedReroll + ")";
        if (nTimesPressedReroll >= 5) reroll();
    }

    void reroll()
    {
        Player.rerollPoints += getRerollPoints(HubControl.maxUnlockedDifficultyId);

        resetPlayerValues(); // Doesn't reset reroll points
        Player.restoreValuesToDefault();

        alreadyVisitedSinceLoading = false;
        returnToHub();
    }

    void resetPlayerValues()
    {
        Player.deck = null;
        Player.collection = null;
        HubControl.maxUnlockedDifficultyId = 0;
        HubControl.currentDifficultyId = 0;
        Player.experience = 0;
        FortuneManager.restartAllFortunes();
    }

    public static double getRerollPoints(double maxUnlockedId)
    {
        return System.Math.Floor(System.Math.Pow(maxUnlockedId * 0.1d, 1.5d) / 2d);
    }

    public static double getRerollExpMult(double rerollPoints)
    {
        return 1d + (rerollPoints / 8d);
    }

    public static int getRerollSkip(double rerollPoints)
    {
        if (rerollPoints == 0) return 0;
        return (int)System.Math.Floor(System.Math.Log(rerollPoints, 2));
    }

    void selectFirstButton()
    {
        returnButton.Select();
    }

    public void returnToHub()
    {
        Savegame.save();
        SceneManager.LoadScene("Hub");
    }

}
