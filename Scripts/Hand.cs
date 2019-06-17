using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public static List<GameObject> hand;

    public static void removeCard(GameObject cardObj)
    {
        hand.Remove(cardObj);
        Destroy(cardObj);
        BattleManager.repositionCardsInHand();
    }

}
