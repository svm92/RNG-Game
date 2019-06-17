using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FortuneId = Fortune.FortuneId;

public static class FortuneManager {

    public static List<Fortune> allFortunes = new List<Fortune>() {
        new Fortune(FortuneId.ENDURANCE), new Fortune(FortuneId.QUICK_START), new Fortune(FortuneId.SHIELD),
        new Fortune(FortuneId.REGEN), new Fortune(FortuneId.LOWER_CEIL), new Fortune(FortuneId.EXTRA_EXP),
        new Fortune(FortuneId.EXTRA_PRIZE_CARD)
    };

    public static void updateAvailableFortunes()
    {
        foreach (Fortune fortune in allFortunes)
        {
            if (!fortune.isAvailable && fortune.fulfillsAvailableCondition())
            {
                fortune.isAvailable = true;
            }
        }
    }

    public static int getNOfAvailableFortunes()
    {
        int nOfAvailableFortunes = 0;
        foreach (Fortune fortune in allFortunes)
        {
            if (fortune.isAvailable) nOfAvailableFortunes++;
        }
        return nOfAvailableFortunes;
    }

    public static void applyAllFortuneEffects()
    {
        foreach (Fortune fortune in allFortunes)
        {
            if (fortune.level > 0)
                fortune.applyEffect();
        }
    }

    public static void markAllFortunesAsSeen() // Turns 'isNew' to false in all fortunes
    {
        foreach (Fortune fortune in allFortunes)
        {
            if (fortune.isAvailable && fortune.isNew)
            {
                fortune.isNew = false;
            }
        }
    }

    public static bool checkIfAnyFortuneIsNew()
    {
        foreach (Fortune fortune in allFortunes)
        {
            if (fortune.isAvailable && fortune.isNew)
            {
                return true;
            }
        }
        // If no new fortune is found, return false
        return false;
    }

    public static void restartAllFortunes()
    {
        List<Fortune> newAllFortunes = new List<Fortune>();

        foreach (Fortune oldFortune in allFortunes)
        {
            newAllFortunes.Add(new Fortune(oldFortune.id));
        }

        allFortunes = newAllFortunes;

        // Reapply, even though they are Lv0 (applyAllFortuneEffects() doesn't work for Lv0)
        foreach (Fortune fortune in allFortunes)
        {
            fortune.applyEffect();
        }
    }

}
