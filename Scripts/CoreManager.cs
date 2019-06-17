using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sentence = TextScript.Sentence;

public class CoreManager : MonoBehaviour {

    public static CoreManager cmInstance;

    public Sprite coreNormal;
    public Sprite coreBlue;
    public Sprite coreRed;
    public Sprite coreGreen;
    public Sprite coreGold;
    public Sprite coreCyan;
    public Sprite coreMagenta;
    public Sprite coreOrange;
    public Sprite coreWhite;
    public Sprite coreBlack;

    public enum Core { NORMAL, BLUE, RED, GREEN, GOLD, CYAN, MAGENTA, ORANGE, WHITE, BLACK };

    private void Awake()
    {
        cmInstance = this;
    }

    public Sprite getSpriteFromCore(Core core)
    {
        switch (core)
        {
            case Core.NORMAL:
                return coreNormal;
            case Core.BLUE:
                return coreBlue;
            case Core.RED:
                return coreRed;
            case Core.GREEN:
                return coreGreen;
            case Core.GOLD:
                return coreGold;
            case Core.CYAN:
                return coreCyan;
            case Core.MAGENTA:
                return coreMagenta;
            case Core.ORANGE:
                return coreOrange;
            case Core.WHITE:
                return coreWhite;
            case Core.BLACK:
                return coreBlack;
        }
        Debug.Log("Check getSpriteFromCore");
        return null;
    }

    public static Color getEyeColor(Core core)
    {
        switch (core)
        {
            case Core.NORMAL:
            default:
                return new Color(0, 0, 0);
            case Core.RED:
                return new Color(0.2f, 0, 0);
            case Core.GREEN:
                return new Color(0, 0.2f, 0);
            case Core.BLUE:
                return new Color(0, 0, 0.2f);
            case Core.GOLD:
                return new Color(0.2f, 0.2f, 0);
            case Core.CYAN:
                return new Color(0, 0.2f, 0.2f);
            case Core.MAGENTA:
                return new Color(0.2f, 0, 0.2f);
            case Core.ORANGE:
                return new Color(0.3f, 0.1f, 0);
            case Core.WHITE:
                return new Color(0.5f, 0.5f, 0.5f);
            case Core.BLACK:
                return new Color(0.25f, 0.25f, 0.25f);
        }
    }

    public static string getCoreDescription(Core core)
    {
        Sentence coreSentence = (Sentence)System.Enum.Parse(typeof(Sentence), "CORE" + (int)core, true);
        Sentence coreDescSentence = (Sentence)System.Enum.Parse(typeof(Sentence), "CORE" + (int)core + "_DESC", true);

        return "<b>" + TextScript.get(coreSentence) + "</b>\n" + TextScript.get(coreDescSentence);
    }

}
