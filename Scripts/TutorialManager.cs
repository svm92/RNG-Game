using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using T = TextScript;
using S = TextScript.Sentence;

public static class TutorialManager {

    public static bool skipEnemyTurn = false;
    public static bool playedCardInTutorial = false;
    public static bool showingDiceRolledText = false;
    public static bool isCombatLine = false;

    public static IEnumerator doRoutine(int id)
    {
        switch(id)
        {
            case 1: // Begin first turn, Introduction
                BattleManager.bmInstance.enemy.enemyName = "Random Number Generator";

                GameObject tutorialText = BattleManager.bmInstance.tutorialGizmos.GetChild(4).gameObject;
                tutorialText.SetActive(true);
                if (Player.isAndroid())
                    tutorialText.GetComponent<Text>().text = T.get(S.T0);
                else
                    tutorialText.GetComponent<Text>().text = T.get(S.T1);
                yield return forceEnemyDialogue(T.get(S.T2));
                tutorialText.SetActive(false);

                yield return forceEnemyDialogue(T.get(S.T3));
                yield return forceEnemyDialogue(T.get(S.T4));
                yield return forceEnemyDialogue(T.get(S.T5));
                yield return new WaitForSeconds(0.5f);

                Dice.minPreRollTime = 2f;
                Dice.maxPreRollValue = 6;
                DiceCluster.forcedRolls = new List<int> { Random.Range(100, 1000)};
                skipEnemyTurn = true;
                
                BattleManager.bmInstance.initiateBattleLogicFlow(null);
                break;
            case 2:
                yield return forceEnemyDialogue(T.get(S.T6));
                yield return forceEnemyDialogue(T.get(S.T7));
                yield return forceEnemyDialogue(T.get(S.T8));
                yield return forceEnemyDialogue(T.get(S.T9));
                yield return new WaitForSeconds(0.25f);

                Dice.restoreValuesToDefault();
                Dice.minPreRollTime = 1.5f;
                DiceCluster.forcedRolls = new List<int> { Random.Range(10, 1000) };

                BattleManager.bmInstance.initiateBattleLogicFlow(null);
                break;
            case 3:
                yield return forceEnemyDialogue(T.get(S.T10));
                yield return forceEnemyDialogue(T.get(S.T11));
                yield return forceEnemyDialogue(T.get(S.T12));
                yield return forceEnemyDialogue(T.get(S.T13));
                yield return new WaitForSeconds(1.25f);
                yield return forceEnemyDialogue(T.get(S.T14));
                if (!Player.isAndroid())
                    yield return forceEnemyDialogue(T.get(S.T15));
                else
                {
                    yield return forceEnemyDialogue(T.get(S.T16));
                    yield return forceEnemyDialogue(T.get(S.T17));
                }

                // Draw 5 (+3 dice) cards
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                List<Card> cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
                forceDrawTheseCards(cardsToDraw);
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                yield return new WaitForSeconds(0.5f);
                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T18), 8f);
                
                // Wait for player to play card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                // Highlight dice area
                GameObject diceHighlighter = BattleManager.bmInstance.tutorialGizmos.GetChild(0).gameObject;
                diceHighlighter.SetActive(true);

                // Show updated HUD
                yield return new WaitForSeconds(1f);
                BattleManager.bmInstance.updateHUD();
                yield return new WaitForSeconds(1.5f);

                // De-highlight dice area
                diceHighlighter.SetActive(false);
                yield return new WaitForSeconds(0.25f);

                yield return forceEnemyDialogue(T.get(S.T19));
                yield return forceEnemyDialogue(T.get(S.T20));
                
                Dice.restoreValuesToDefault();
                DiceCluster.forcedRolls = new List<int> { Random.Range(10, 1000), Random.Range(10, 1000), Random.Range(10, 1000), Random.Range(10, 1000) };

                BattleManager.bmInstance.initiateBattleLogicFlow(null);
                break;
            case 4:
                BattleManager.bmInstance.hideHand();
                yield return forceEnemyDialogue(T.get(S.T21));
                yield return forceEnemyDialogue(T.get(S.T22));

                // Draw (LAST.9.DICE.2)
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T23), 4f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                // Highlight right panel
                GameObject rightPanelHighlighter = BattleManager.bmInstance.tutorialGizmos.GetChild(1).gameObject;
                rightPanelHighlighter.SetActive(true);

                yield return new WaitForSeconds(1.25f);
                BattleManager.bmInstance.updateHUD();
                yield return new WaitForSeconds(0.5f);

                yield return forceEnemyDialogue(T.get(S.T25));
                yield return forceEnemyDialogue(T.get(S.T26));
                yield return forceEnemyDialogue(T.get(S.T27));

                // De-highlight right panel
                rightPanelHighlighter.SetActive(false);
                yield return new WaitForSeconds(0.25f);

                DiceCluster.forcedRolls = new List<int> { Random.Range(1, 100) * 10 + Random.Range(0, 9), Random.Range(1, 100) * 10 + Random.Range(0, 9),
                    349, Random.Range(1, 100) * 10 + Random.Range(0, 9) };
                BattleManager.bmInstance.initiateBattleLogicFlow(null);

                // Wait until rolls end, freeze with 349 on screen
                yield return new WaitUntil(() => showingDiceRolledText);
                yield return forceEnemyDialogue(T.get(S.T28));
                showingDiceRolledText = false;

                yield return new WaitForSeconds(1.1f);
                // Highlight dice area briefly
                BattleManager.bmInstance.hud.gameObject.SetActive(true);
                BattleManager.bmInstance.flashImageAnimator.SetTrigger("fadeFromBlack");

                diceHighlighter = BattleManager.bmInstance.tutorialGizmos.GetChild(0).gameObject;
                diceHighlighter.SetActive(true);
                yield return new WaitForSeconds(1f);
                BattleManager.bmInstance.updateHUD();
                yield return new WaitForSeconds(1.5f);
                diceHighlighter.SetActive(false);
                BattleManager.bmInstance.hideInfoTextPanel();
                BattleManager.bmInstance.startPlayerTurn();
                break;
            case 5:
                BattleManager.bmInstance.hideHand();
                yield return forceEnemyDialogue(T.get(S.T29));
                BattleManager.bmInstance.showHand();
                yield return forceEnemyDialogue(T.get(S.T30), 4f);
                yield return new WaitForSeconds(0.2f);

                // Discard hand, then draw (LAST.7.DICE.2)
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                BattleManager.bmInstance.discard();
                BattleManager.bmInstance.discard();
                BattleManager.bmInstance.discard();
                BattleManager.bmInstance.discard();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T31), 4f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();
                
                DiceCluster.forcedRolls = new List<int> { 537, 767, 39, 579, 124, 227};
                BattleManager.bmInstance.initiateBattleLogicFlow(null);

                // Wait until rolls end, freeze with numbers on screen
                yield return new WaitUntil(() => showingDiceRolledText);
                yield return forceEnemyDialogue(T.get(S.T32));
                showingDiceRolledText = false;

                yield return new WaitForSeconds(1.1f);
                // Highlight dice area briefly
                BattleManager.bmInstance.hud.gameObject.SetActive(true);
                BattleManager.bmInstance.flashImageAnimator.SetTrigger("fadeFromBlack");

                diceHighlighter = BattleManager.bmInstance.tutorialGizmos.GetChild(0).gameObject;
                diceHighlighter.SetActive(true);
                yield return new WaitForSeconds(1f);
                BattleManager.bmInstance.updateHUD();
                yield return new WaitForSeconds(1.5f);
                diceHighlighter.SetActive(false);
                BattleManager.bmInstance.hideInfoTextPanel();
                BattleManager.bmInstance.startPlayerTurn();
                break;
            case 6:
                // Draw (LAST.8.DICE.2)
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(Player.nDice + T.get(S.T33), 4f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                DiceCluster.forcedRolls = new List<int> { 618, 937, 71, 167, 359, 620, 667, 269,
                                                            4, 729, 777, 683, 765, 129, 270, 279};
                BattleManager.bmInstance.enemy.possibleAttacks = new List<Enemy.Attack>() { Enemy.Attack.NORMAL_ATTACK };
                skipEnemyTurn = false;
                BattleManager.bmInstance.initiateBattleLogicFlow(null);

                // Wait until rolls end, freeze with numbers on screen
                yield return new WaitUntil(() => showingDiceRolledText);
                yield return forceEnemyDialogue(T.get(S.T34));
                yield return forceEnemyDialogue(T.get(S.T35), 2f);
                yield return forceEnemyDialogue(T.get(S.T36), 1.5f);
                yield return forceEnemyDialogue(T.get(S.T37));
                yield return forceEnemyDialogue(T.get(S.T38));
                showingDiceRolledText = false;
                break;
            case 7:
                yield return forceEnemyDialogue(T.get(S.T39));

                // Draw
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T40), 4f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                DiceCluster.forcedRolls = new List<int> { 769, 320, 377, 626, 445, 29,
                                                        154, 118, 811, 53, 688, 228,
                                                        77, 977, 24, 296, 495, 169,
                                                        998, 300, 604, 461, 543, 964,
                                                        16, 729, 567, 808, 494, 149,
                                                        350, 88, 599, 909, 274, 67 };
                BattleManager.bmInstance.initiateBattleLogicFlow(null);
                break;
            case 8:
                if (!Player.isAndroid())
                {
                    yield return forceEnemyDialogue(T.get(S.T41));
                    yield return forceEnemyDialogue(T.get(S.T42));
                }
                else
                {
                    yield return forceEnemyDialogue(T.get(S.T43));
                    yield return forceEnemyDialogue(T.get(S.T44));
                }
                

                // Draw
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T45), 4f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                DiceCluster.forcedRolls = new List<int> { 639, 262, 417, 69, 596, 908, 177, 256, 931, 686, 35, 129, 210, 455, 278, 285, 101, 912, 660, 917, 541, 825, 363, 30, 890, 700, 165, 692, 8, 958, 14, 616, 432, 769, 630, 829, 329, 434, 627, 318, 125, 709, 567, 155, 17, 37, 146, 170, 789, 266, 627, 429, 169, 689, 746, 593, 875, 823, 443, 990, 105, 605, 20, 767, 890, 102, 321, 416, 928, 458, 867, 678, 923, 565, 593, 630, 781, 805, 261, 915, 777, 653, 330, 943, 898, 394 };
                BattleManager.bmInstance.initiateBattleLogicFlow(null);
                break;
            case 9:
                yield return forceEnemyDialogue(T.get(S.T46));

                // Highlight health
                GameObject healthHighlighter = BattleManager.bmInstance.tutorialGizmos.GetChild(2).gameObject;
                healthHighlighter.SetActive(true);
                yield return forceEnemyDialogue(T.get(S.T47));
                yield return forceEnemyDialogue(T.get(S.T48));
                healthHighlighter.SetActive(false);

                // Draw
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T49), 4.5f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                DiceCluster.forcedRolls = new List<int> { 198, 989, 817, 254, 548, 301, 803, 48, 481, 455, 38, 114, 410, 842, 95, 533, 187, 932, 896, 868, 939, 819, 179, 77, 336, 80, 65, 956, 937, 552, 695, 19, 448, 998, 238, 148, 109, 586, 10, 158, 487, 148, 107, 960, 230, 794, 431, 62, 910, 739, 65, 284, 429, 790, 579, 689, 218, 78, 818, 598, 188, 189, 58, 129, 659, 387, 738, 164, 568, 114, 588, 442, 86, 280, 175, 971, 825, 93, 89, 883, 786, 746, 870, 823, 146, 234, 765, 634, 799, 491, 209, 549, 708, 119, 743, 611, 127, 811, 161, 928, 672, 489, 395, 537, 161, 405, 617, 861, 61, 371, 838, 110, 726, 315, 534, 938, 845, 99, 968, 898, 333, 615, 960, 176, 796, 203, 930, 472, 153, 124, 968, 273, 868, 62, 421, 939, 510, 209, 880, 505, 534, 447, 288, 719, 30, 560, 487, 327, 999, 808, 538, 941, 607, 808, 437, 161, 259, 479, 39, 88, 875, 688, 35, 814, 215, 495, 539, 744, 766, 524, 12, 468, 264, 746, 538, 38, 478, 592, 165, 119, 808, 507, 594, 935, 772, 991, 208, 219 };
                BattleManager.bmInstance.initiateBattleLogicFlow(null);

                // Wait until rolls end, freeze with numbers on screen
                yield return new WaitUntil(() => showingDiceRolledText);
                yield return forceEnemyDialogue(T.get(S.T50));
                yield return forceEnemyDialogue(T.get(S.T51));
                yield return forceEnemyDialogue(T.get(S.T52));
                yield return forceEnemyDialogue(T.get(S.T53));
                yield return forceEnemyDialogue(T.get(S.T54));
                yield return forceEnemyDialogue(T.get(S.T55));
                showingDiceRolledText = false;

                yield return new WaitForSeconds(1.1f);
                BattleManager.bmInstance.hud.gameObject.SetActive(true);
                BattleManager.bmInstance.flashImageAnimator.SetTrigger("fadeFromBlack");
                BattleManager.bmInstance.updateHUD();
                break;
            case 10:
                yield return forceEnemyDialogue(T.get(S.T56));

                // Draw
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.FIRST, 3, CardEffect.Effect.HEALTH, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T57), 5f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                DiceCluster.forcedRolls = new List<int> { 910, 679, 500, 149, 140, 148, 959, 718, 176, 74, 18, 969, 524, 108, 161, 188, 168, 189, 181, 673, 628, 299, 114, 665, 640, 218, 455, 204, 189, 494, 264, 67, 437, 842, 363, 331, 329, 991, 192, 877, 891, 564, 85, 776, 317, 654, 919, 503, 692, 88, 287, 359, 9, 601, 313, 43, 244, 207, 463, 250, 320, 928, 836, 960, 980, 145, 49, 374, 808, 383, 172, 963, 894, 370, 705, 973, 453, 83, 256, 461, 398, 481, 626, 945, 923, 427, 255, 680, 93, 617, 212, 631, 719, 235, 602, 205, 308, 500, 864, 96, 291, 611, 500, 892, 390, 634, 276, 874, 358, 516, 969, 48, 59, 888, 829, 859, 774, 33, 953, 912, 758, 832, 614, 114, 618, 619, 167, 722, 109, 376, 88, 981, 549, 368, 58, 441, 421, 826, 352, 959, 372, 299, 168, 190, 483, 253, 207, 476, 404, 83, 371, 975, 251, 351, 683, 172, 509, 259, 437, 603, 816, 488, 398, 673, 419, 269, 964, 6, 658, 619, 171, 968, 738, 909, 639, 472, 700, 566, 987, 104, 89, 27, 375, 588, 686, 305, 13, 428, 30, 38, 684, 152, 161, 584, 535, 592, 261, 214, 672, 148, 563, 114, 992, 701, 436, 586, 848, 128, 539, 745, 339, 256, 433, 431, 40, 499, 953, 462, 150, 874, 878, 769, 308, 747, 563, 568, 952, 229, 415, 188, 448, 134, 526, 540, 952, 549, 491, 778, 60, 196, 321, 337, 99, 224, 660, 969, 816, 995, 901, 837, 104, 173, 956, 715, 398, 914, 968, 247, 649, 448, 434, 154, 379, 146, 679, 745, 194, 857, 604, 166, 191, 205, 427, 314, 546, 993, 58, 394, 293, 697, 519, 167, 865, 950, 469, 351, 648, 605, 186, 340, 749, 617, 70, 907, 347, 680, 281, 29, 696, 725, 148, 827, 888, 89, 43, 753, 476, 932, 756, 421, 228, 424, 818, 843, 578, 178, 939, 715, 832, 909, 17, 765, 695, 568, 25, 502, 456, 53, 968, 216, 108, 758, 559, 739, 308, 909, 8, 159, 9, 68, 193, 522, 619, 985, 759, 418, 616, 239, 703, 518, 189, 411, 158, 849, 208, 802, 849, 993, 728, 12, 848, 679, 638, 103, 138, 583, 429, 801, 499, 708, 909, 439, 89, 749, 848, 199, 618, 779, 726, 78, 859, 488, 209, 268, 567, 379, 479, 809, 539, 899, 788, 277, 228, 889, 2, 959, 888, 799, 189, 878, 529, 488, 869, 88, 879, 58, 269, 228, 129, 908, 169, 268, 469, 359, 109, 138, 188, 139, 449, 99, 718, 769, 818, 509, 341, 519, 88, 919, 649, 549, 3, 969, 789, 139, 639, 698, 73, 95, 481, 532, 280, 607, 719, 360, 134, 356, 71, 940, 405, 109, 487, 228, 845, 621, 348, 849, 488, 919, 49, 190, 618, 668, 729, 139, 478, 19, 888, 339, 299, 769, 569, 869, 858, 109, 249, 746, 698, 738, 38, 119, 609, 39, 318, 918, 669, 329, 539, 538, 279, 139, 69, 689, 129, 859, 419, 109, 699, 289, 638, 819 };
                BattleManager.bmInstance.initiateBattleLogicFlow(null);

                // Wait until rolls end, freeze with numbers on screen
                yield return new WaitUntil(() => showingDiceRolledText);
                yield return forceEnemyDialogue(T.get(S.T58));
                yield return new WaitForSeconds(0.5f);
                yield return forceEnemyDialogue(T.get(S.T59));
                yield return forceEnemyDialogue(T.get(S.T60));
                yield return new WaitForSeconds(1.5f);
                yield return forceEnemyDialogue(T.get(S.T61));
                yield return forceEnemyDialogue(T.get(S.T62));
                yield return forceEnemyDialogue(T.get(S.T63));
                yield return forceEnemyDialogue(T.get(S.T64));
                yield return new WaitForSeconds(0.75f);
                yield return forceEnemyDialogue(T.get(S.T65));
                yield return forceEnemyDialogue(T.get(S.T66));
                showingDiceRolledText = false;

                yield return new WaitForSeconds(2.1f);
                BattleManager.bmInstance.hud.gameObject.SetActive(true);
                BattleManager.bmInstance.updateHUD();
                break;
            case 11:
                yield return forceEnemyDialogue(T.get(S.T67));
                yield return forceEnemyDialogue(T.get(S.T68));
                yield return forceEnemyDialogue(T.get(S.T69));

                // Highlight deck
                GameObject deckHighlighter = BattleManager.bmInstance.tutorialGizmos.GetChild(3).gameObject;
                deckHighlighter.SetActive(true);
                yield return new WaitForSeconds(1f);
                yield return forceEnemyDialogue(T.get(S.T70));
                yield return forceEnemyDialogue(T.get(S.T71));
                deckHighlighter.SetActive(false);

                yield return forceEnemyDialogue(T.get(S.T72));
                yield return forceEnemyDialogue(T.get(S.T73));

                // Draw
                BattleManager.bmInstance.battleDeck.cards.RemoveAt(0);
                BattleManager.bmInstance.updateHUD();
                cardsToDraw = new List<Card>();
                cardsToDraw.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 2)));
                forceDrawTheseCards(cardsToDraw);
                BattleManager.bmInstance.showHand();
                if (Player.keyboardModeOn) BattleManager.bmInstance.selectFirstCard();

                playedCardInTutorial = false;
                BattleManager.playerActionAllowed = true;
                yield return forceEnemyDialogue(T.get(S.T74), 5f);

                // Wait until player plays that card
                yield return new WaitUntil(() => playedCardInTutorial);
                BattleManager.playerActionAllowed = false;
                BattleManager.bmInstance.hideDialoguePanel();

                DiceCluster.forcedRolls = new List<int> { 412, 26, 78, 223, 778, 829, 751, 524, 119, 894, 640, 349, 88, 982, 646, 74, 340, 110, 848, 981, 867, 719, 342, 897, 675, 629, 611, 181, 409, 994, 954, 594, 112, 400, 940, 618, 223, 142, 245, 971, 581, 209, 911, 345, 606, 759, 882, 258, 286, 421, 232, 534, 555, 795, 209, 670, 634, 670, 457, 276, 394, 926, 159, 535, 450, 521, 745, 287, 951, 374, 939, 492, 728, 62, 822, 379, 143, 430, 692, 405, 425, 283, 789, 555, 378, 178, 274, 28, 315, 174, 258, 35, 359, 870, 445, 406, 363, 693, 516, 570, 404, 219, 455, 986, 951, 90, 552, 851, 771, 781, 283, 10, 336, 258, 543, 542, 460, 761, 640, 869, 273, 674, 372, 950, 39, 926, 177, 882, 158, 504, 335, 539, 626, 883, 298, 623, 756, 247, 458, 255, 705, 547, 64, 609, 919, 299, 382, 3, 304, 110, 692, 682, 479, 726, 760, 231, 619, 771, 561, 625, 942, 527, 774, 566, 966, 549, 864, 999, 98, 985, 338, 692, 236, 37, 768, 409, 914, 297, 274, 608, 42, 985, 556, 202, 429, 357, 773, 891, 508, 955, 238, 194, 284, 419, 562, 190, 827, 472, 528, 862, 622, 347, 326, 534, 184, 476, 194, 599, 560, 83, 629, 778, 304, 928, 88, 205, 925, 537, 384, 920, 796, 928, 57, 816, 266, 185, 177, 613, 598, 330, 41, 58, 271, 305, 914, 235, 460, 267, 156, 41, 301, 923, 370, 338, 716, 309, 439, 781, 88, 939, 537, 506, 482, 936, 647, 267, 126, 978, 768, 878, 578, 660, 953, 973, 891, 523, 246, 925, 813, 627, 878, 639, 253, 550, 438, 376, 826, 840, 516, 202, 764, 385, 475, 630, 45, 376, 602, 219, 444, 765, 935, 846, 453, 57, 141, 39, 586, 498, 130, 187, 928, 911, 909, 601, 138, 534, 130, 74, 417, 829, 209, 611, 497, 576, 968, 633, 311, 287, 908, 991, 419, 416, 202, 297, 96, 354, 577, 169, 360, 249, 331, 469, 697, 449, 631, 261, 142, 752, 474, 269, 358, 292, 542, 234, 678, 536, 805, 958, 148, 319, 978, 962, 178, 213, 421, 398, 895, 723, 81, 498, 779, 244, 746, 317, 311, 780, 57, 579, 268, 546, 533, 129, 757, 676, 592, 462, 523, 211, 876, 157, 284, 773, 594, 76, 829, 790, 447, 73, 593, 466, 143, 656, 525, 887, 53, 344, 748, 156, 247, 809, 324, 981, 31, 56, 530, 688, 532, 987, 595, 740, 86, 909, 929, 242, 239, 426, 81, 459, 213, 750, 507, 830, 396, 215, 925, 973, 249, 360, 679, 167, 510, 919, 867, 795, 459, 202, 559, 398, 470, 529, 39, 965, 959, 789, 793, 776, 250, 180, 334, 671, 227, 187, 662, 959, 177, 151, 26, 631, 170, 380, 467, 598, 313, 834, 732, 173, 661, 408, 503, 845, 54, 663, 271, 846, 878, 147, 339, 473, 896, 992, 297, 528, 139, 793, 446, 129, 376, 860, 477, 30, 936, 576, 383, 796, 207, 494, 956, 582, 361, 34, 910, 319, 216, 323, 958, 486, 558, 654, 855, 95, 759, 595, 634, 704, 759, 178, 104, 863, 924, 768, 724, 19, 977, 540, 924, 877, 522, 222, 244, 991, 96, 201, 729, 683, 626, 83, 297, 606, 44, 534, 73, 585, 757, 251, 427, 133, 343, 708, 988, 171, 991, 303, 892, 425, 235, 517, 495, 556, 146, 546, 976, 147, 422, 641, 788, 517, 196, 859, 153, 928, 858, 639, 844, 381, 710, 45, 358, 791, 965, 194, 396, 609, 522, 217, 260, 681, 949, 395, 984, 772, 486, 969, 87, 910, 76, 798, 42, 54, 219, 589, 711, 698, 405, 247, 514, 835, 281, 607, 816, 166, 567, 402, 791, 982, 181, 205, 988, 61, 929, 488, 987, 199, 503, 501, 479, 72, 52, 256, 909, 170, 195, 784, 443, 854, 927, 83, 606, 493, 320, 830, 15, 364, 386, 371, 614, 976, 717, 651, 488, 743, 695, 400, 381, 195, 353, 251, 630, 636, 493, 693, 843, 313, 7, 486, 171, 799, 967, 215, 449, 529, 636, 937, 861, 715, 700, 878, 434, 119, 24, 403, 888, 967, 739, 589, 59, 511, 674, 190, 407, 148, 742, 280, 291, 590, 120, 675, 954, 507, 649, 973, 707, 167, 38, 930, 332, 581, 41, 969, 272, 700, 129, 656, 722, 634, 293, 689, 820, 109, 746, 474, 901, 965, 899, 784, 997, 517, 142, 400, 938, 434, 920, 812, 246, 860, 256, 834, 135, 566, 543, 962, 213, 469, 409, 700, 476, 291, 785, 332, 335, 647, 919, 424, 118, 518, 706, 155, 503, 854, 591, 317, 794, 29, 587, 491, 384, 403, 58, 848, 678, 629, 61, 328, 259, 320, 527, 686, 547, 897, 125, 355, 630, 332, 863, 548, 745, 668, 941, 146, 177, 222, 368, 355, 106, 312, 683, 856, 356, 352, 363, 711, 512, 237, 12, 263, 482, 752, 924, 136, 238, 144, 794, 244, 904, 554, 381, 309, 41, 606, 770, 348, 296, 663, 332, 594, 109, 822, 606, 845, 546, 1, 381, 964, 68, 18, 813, 482, 170, 780, 351, 151, 6, 868, 700, 105, 71, 675, 862, 715, 329, 724, 371, 121, 425, 475, 255, 418, 378, 14, 324, 325, 823, 14, 860, 241, 780, 796, 778, 999, 322, 763, 257, 557, 207, 649, 649, 293, 149, 821, 807, 656, 488, 446, 217, 561, 497, 351, 981, 513, 87, 548, 936, 694, 614, 707, 885, 591, 267, 191, 576, 42, 272, 606, 976, 478, 646, 758, 69, 638, 251, 536, 73, 838, 443, 61, 164, 111, 135, 320, 683, 232, 640, 680, 610, 560, 56, 481, 457, 446, 306, 591, 705, 528, 754, 416, 681, 140, 161, 544, 613, 689, 585, 68, 276, 901, 539, 825, 38, 37, 926, 651, 525, 453, 610, 700, 414, 1, 615, 805, 650, 106, 73, 276, 953, 980, 69, 361, 476, 320, 174, 792, 455, 173, 215, 944, 790, 687, 236, 256, 608, 818, 177, 509, 288, 961, 245, 376, 135, 723, 581, 473, 25, 129, 440, 379, 568, 351, 597, 788, 441, 804, 580, 879, 930, 691, 450, 224, 4, 429, 401, 167, 344, 763, 416, 209, 425, 919, 715, 131, 718, 67, 829, 298, 770, 683, 189, 167, 447, 897, 987, 364, 798, 332, 384, 412, 187, 133, 675, 524, 598, 259, 212, 693, 966, 518, 172, 328, 189, 461, 881, 622, 861, 257, 625, 437, 89, 575, 465, 598, 870, 887, 455, 599, 684, 722, 391, 550, 112, 261, 713, 862, 790, 45, 807, 234, 846, 147, 723, 417, 823, 232, 609, 973, 314, 7, 907, 757, 295, 295, 411, 422, 247, 820, 473, 930, 740, 685, 908, 546, 669, 360, 264, 199, 132, 836, 737, 507, 556, 608, 954, 768, 519, 333, 712, 115, 843, 53, 235, 237, 119, 891, 725, 101, 46, 176, 495, 708, 76, 420, 97, 477, 175, 416, 10, 944, 295, 793, 330, 280, 327, 89, 148, 714, 283, 578, 960, 814, 515, 882, 943, 738, 678, 959, 986, 804, 397, 11, 123, 839, 629, 526, 987, 411, 788, 29, 983, 30, 495, 649, 953, 187, 943, 958, 571, 526, 122, 652, 563, 237, 374, 658, 286, 673, 796, 943, 882, 293, 26, 120, 691, 59, 349, 164, 542, 363, 506, 220, 830, 724, 829, 156, 86, 992, 911, 15, 449, 753, 184, 194, 64, 800, 176, 511, 951, 309, 797, 617, 862, 758, 529, 112, 742, 900, 809, 827, 972, 476, 611, 499, 308, 846, 447, 687, 168, 917, 929, 812, 706, 307, 643, 567, 430, 141, 488, 125, 769, 637, 469, 556, 442, 425, 512, 75, 624, 390, 119, 596, 34, 449, 690, 638, 960, 68, 116, 522, 672, 767, 768, 957, 771, 532, 457, 329, 175, 598, 653, 29, 710, 552, 181, 458, 169, 976, 261, 421, 832, 801, 511, 608, 646, 719, 431, 471, 753, 415, 126, 230, 755, 640, 937, 905, 235, 138, 852, 214, 586, 92, 60, 356, 824, 812, 659, 387, 521, 600, 27, 117, 711, 771, 255, 212, 779, 681, 85, 811, 929, 248, 920, 77, 995, 218, 820, 533, 603, 202, 542, 888, 271, 756, 684, 224, 926, 360, 121, 760, 784, 923, 400, 10, 765, 464, 604, 279, 215, 889, 827, 882, 648, 646, 292, 583, 834, 728, 424, 591, 604, 642, 135, 317, 956, 443, 555, 253, 154, 663, 655, 508, 166, 548, 152, 607, 60, 744, 327, 537, 127, 304, 303, 652, 50, 744, 980, 187, 410, 702, 445, 365, 138, 758, 76, 447, 71, 149, 470, 399, 757, 567, 980, 871, 120, 655, 377, 980, 203, 508, 974, 580, 206, 684, 765, 645, 919, 840, 286, 835, 975, 559, 179, 553, 339, 269, 202, 291, 529, 203, 705, 158, 779, 115, 134, 575, 697, 433, 57, 296, 209, 955, 874, 273, 70, 243, 968, 110, 243, 497, 134, 137, 225, 505, 300, 632, 228, 885, 784, 55, 966, 725, 907, 813, 880, 457, 389, 521, 842, 541, 575, 588, 898, 465, 862, 742, 463, 812, 274, 625, 289, 230, 890, 578, 832, 397, 691, 922, 904, 713, 319, 244, 156, 1000, 751, 232 };
                BattleManager.bmInstance.initiateBattleLogicFlow(null);

                // Wait until rolls end, freeze with numbers on screen
                yield return new WaitUntil(() => showingDiceRolledText);
                yield return forceEnemyDialogue(T.get(S.T75));
                yield return forceEnemyDialogue(T.get(S.T76));
                showingDiceRolledText = false;

                yield return new WaitForSeconds(2.1f);
                BattleManager.bmInstance.hud.gameObject.SetActive(true);
                BattleManager.bmInstance.updateHUD();
                break;
        }
    }

    public static IEnumerator forceEnemyDialogue(string dialogue, float timeUntilForcedSkip)
    {
        BattleManager.bmInstance.showEnemyDialogue(dialogue);
        BattleManager.bmInstance.prepareWaitForInfoTextPanel(timeUntilForcedSkip);
        yield return new WaitForSeconds(0.85f);
        yield return new WaitWhile(() => BattleManager.bmInstance.showingInfoTextPanel);
        BattleManager.bmInstance.hideDialoguePanelSlow();
        yield return new WaitForSeconds(.35f);
    }

    public static IEnumerator forceEnemyDialogue(string dialogue)
    {
        yield return forceEnemyDialogue(dialogue, 0);
    }

    public static bool freezeScreenAfterRoll()
    {
        return (new int[] { 4, 5, 6, 9, 10, 11 }.Contains(Player.currentTurn));
    }

    public static bool freezeScreenAfterRollWithoutAttacking()
    {
        return (new int[] { 4, 5, 9 }.Contains(Player.currentTurn));
    }

    public static bool freezeScreenAfterRollWithoutEnemyAttack()
    {
        return (new int[] { 4, 5 }.Contains(Player.currentTurn));
    }

    public static string getEnemyHurtLine()
    {
        switch (Player.currentTurn)
        {
            case 6:
                return T.get(S.T77);
            case 10:
                return T.get(S.T78);
            case 11:
                return T.get(S.T79);
            default:
                return "???";
        }
    }

    public static string getEnemyAttackLine()
    {
        switch (Player.currentTurn)
        {
            case 6:
                return T.get(S.T80);
            case 7:
                return T.get(S.T81);
            case 8:
                return T.get(S.T82);
            case 9:
                return T.get(S.T83);
            case 10:
                return T.get(S.T84);
            default:
                return "???";
        }
    }

    static void forceDrawTheseCards(List<Card> cardsToDraw)
    {
        foreach (Card c in cardsToDraw)
            BattleManager.bmInstance.forceDrawSpecificCard(c);
        BattleManager.repositionCardsInHand();
    }

}
