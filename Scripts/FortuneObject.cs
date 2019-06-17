using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortuneObject : MonoBehaviour {

    public Fortune fortune;

    public void buy()
    {
        Player.stats[Player.Stat.FORTUNES_BOUGHT]++;
        Player.stats[Player.Stat.TOTAL_EXP_SPENT] += fortune.priceToNextLevel;
        AudioManager.amInstance.playSellSound();

        Player.experience -= fortune.priceToNextLevel;
        fortune.levelUpAndApplyEffect();
        fortune.isNew = false;
        Shop.shopInstance.updateFortunes();

        bool isReBuyable = (Player.experience >= fortune.priceToNextLevel);
        if (!isReBuyable)
        {
            Shop.shopInstance.selectFirstButton();
        }
    }

}
