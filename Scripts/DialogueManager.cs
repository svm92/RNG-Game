using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attack = Enemy.Attack;
using Arm = ArmManager.Arm;
using T = TextScript;
using S = TextScript.Sentence;

public static class DialogueManager {

	public enum DialogueTrigger { TURN_START, NO_PLAYABLE_CARDS, HIT_AGAINST_ENEMY, ENEMY_ATTACK, VICTORY,
        LOSE_NO_HEALTH, LOSE_NO_CARDS};

    // Max text length:
    // "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    public static string getDialogue(DialogueTrigger dialogueTrigger)
    {
        Enemy enemy = BattleManager.bmInstance.enemy;
        switch (dialogueTrigger)
        {
            // LOSE_NO_CARDS takes preference over NO_PLAYABLE_CARDS, which in turn takes preference over TURN_START
            // ENEMY_ATTACK and LOST_NO_HEALTH happen sequentially
            case DialogueTrigger.TURN_START:
                if (Player.currentTurn == 1) // First turn
                    return getFirstTurnDialogue();
                else if (Hand.hand.Count == 1) // Exactly 1 card in hand
                    return getOneCardInHandDialogue();
                // Both player and enemy weakened
                else if (Player.health <= enemy.damageAgainstHealth && enemy.health < enemy.maxHealth * 0.5f)
                    return getBothWeakTurnDialogue();
                else if (Player.health <= enemy.damageAgainstHealth) // Player weak
                    return getPlayerWeakTurnDialogue();
                else if (enemy.health < enemy.maxHealth * 0.5f) // Enemy weak
                    return getEnemyWeakTurnDialogue();
                else
                    return getGenericTurnDialogue();
            case DialogueTrigger.NO_PLAYABLE_CARDS:
                if (BattleManager.bmInstance.noCardsLeftInHand())
                    return getNoCardsInHandDialogue();
                else
                    return getNoPlayableCardsDialogue();
            case DialogueTrigger.HIT_AGAINST_ENEMY:
                if (enemy.health == 0) // Finishing blow
                    return getEnemyFinishingHitDialogue();
                else if (BattleManager.damageDealtToEnemy == 0) // 0 damage (shields)
                    return getEnemyHitNoDamageDialogue();
                else if (BattleManager.damageDealtToEnemy == 1) // Exactly 1 damage
                    return getEnemyHitOneDamageDialogue();
                else
                    return getGenericEnemyHitDialogue();
            case DialogueTrigger.ENEMY_ATTACK:
                return getEnemyAttackDialogue();
            case DialogueTrigger.VICTORY:
                return getVictoryDialogue();
            case DialogueTrigger.LOSE_NO_HEALTH:
                return getLoseNoHealthDialogue();
            case DialogueTrigger.LOSE_NO_CARDS:
                return getLoseNoCardsDialogue();
            default:
                return "Something went wrong";
        }
    }

    static string getFirstTurnDialogue()
    {
        List<string> texts = new List<string>
        {
            T.get(S.D0),
            T.get(S.D1),
            T.get(S.D2),
            T.get(S.D3),
            T.get(S.D4),
            T.get(S.D5),
            T.get(S.D6),
            T.get(S.D7),
            T.get(S.D8),
            T.get(S.D9),
            T.get(S.D10)
        };
        return getRandomChoiceFrom(texts);
    }

    static string getOneCardInHandDialogue()
    {
        List<string> texts = new List<string>
        {
            T.get(S.D11),
            T.get(S.D12),
            T.get(S.D13),
            T.get(S.D14)
        };
        return getRandomChoiceFrom(texts);
    }

    static string getBothWeakTurnDialogue()
    {
        List<string> texts = new List<string>
        {
            T.get(S.D15),
            T.get(S.D16),
            T.get(S.D17),
            T.get(S.D18),
            T.get(S.D19),
            T.get(S.D20),
            T.get(S.D21),
            T.get(S.D22),
            T.get(S.D23)
        };
        return getRandomChoiceFrom(texts);
    }

    static string getPlayerWeakTurnDialogue()
    {
        List<string> texts = new List<string>
        {
            T.get(S.D24),
            T.get(S.D25),
            T.get(S.D26),
            T.get(S.D27),
            T.get(S.D28),
            T.get(S.D29),
            T.get(S.D30),
            T.get(S.D31),
            T.get(S.D32),
            T.get(S.D33),
            T.get(S.D34),
            T.get(S.D35),
            T.get(S.D36),
            T.get(S.D37),
            T.get(S.D38)
        };
        if (Player.health == 1)
        {
            for (int i=0; i < 3; i++)
                texts.AddRange(new List<string> {
                    T.get(S.D39),
                    T.get(S.D40),
                    T.get(S.D41)
                });
        }
        return getRandomChoiceFrom(texts);
    }

    static string getEnemyWeakTurnDialogue()
    {
        List<string> texts = new List<string>
        {
            T.get(S.D20),
            T.get(S.D42),
            T.get(S.D43),
            T.get(S.D44),
            T.get(S.D45),
            T.get(S.D46),
            T.get(S.D47),
            T.get(S.D48),
            T.get(S.D49),
            T.get(S.D50),
            T.get(S.D51),
            T.get(S.D52),
            T.get(S.D53),
            T.get(S.D54),
            T.get(S.D55)
        };
        return getRandomChoiceFrom(texts);
    }

    static string getGenericTurnDialogue()
    {
        List<string> texts = new List<string>
        {
            T.get(S.D56),
            T.get(S.D57),
            T.get(S.D58),
            T.get(S.D59),
            T.get(S.D60),
            T.get(S.D61),
            T.get(S.D62),
            T.get(S.D63),
            T.get(S.D64),
            T.get(S.D65)
        };
        if (Player.nDice <= 9)
        {
            texts.AddRange(new List<string>
            {
                T.get(S.D66),
                T.get(S.D67),
                T.get(S.D68)
            });
        }
        Enemy enemy = BattleManager.bmInstance.enemy;
        if (enemy.armList.Contains(Arm.POWER_GLOW))
        {
            for (int i=0; i < 2; i++)
            {
                texts.Add(T.get(S.D69));
                texts.Add(T.get(S.D70));
            }
        }
        if (enemy.armList.Contains(Arm.BUZZSAW) && !BattleManager.bmInstance.noCardsLeftInDeck())
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D71));
                texts.Add(T.get(S.D72));
            }
        }
        if (enemy.armList.Contains(Arm.SHIELD))
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D73));
                texts.Add(T.get(S.D74));
            }
        }
        if (enemy.armList.Contains(Arm.FAN) && Hand.hand.Count > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D75));
                if (Hand.hand.Count >= 4)                                
                    texts.Add(T.get(S.D76));
            }
        }
        if (enemy.armList.Contains(Arm.SALVE) && enemy.health <= enemy.maxHealth * 0.6f)
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D77));
            }
        }
        if (enemy.armList.Contains(Arm.FORBIDDEN_RELIC) && enemy.health > enemy.maxHealth * 0.2f)
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D78));
            }
        }
        if (enemy.armList.Contains(Arm.SIPHON) && enemy.health <= enemy.maxHealth * 0.7f)
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D79));
            }
        }
        if (enemy.armList.Contains(Arm.MIASMA) && Player.health < Player.initialHealth)
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D80));
                texts.Add(T.get(S.D81));
            }
        }
        if (enemy.armList.Contains(Arm.AEGIS) && enemy.health >= enemy.maxHealth * 0.7f)
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D82));
                texts.Add(T.get(S.D83));
            }
        }
        if (enemy.armList.Contains(Arm.CARTILAGE))
        {
            for (int i = 0; i < 2; i++)
            {
                texts.Add(T.get(S.D84));
            }
        }
        return getRandomChoiceFrom(texts);
    }

    static string getNoCardsInHandDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D85),
            T.get(S.D86),
            T.get(S.D87)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getNoPlayableCardsDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D88),
            T.get(S.D89),
            T.get(S.D90)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getEnemyFinishingHitDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D91),
            T.get(S.D92),
            T.get(S.D93),
            T.get(S.D94),
            T.get(S.D95),
            T.get(S.D96),
            T.get(S.D97),
            T.get(S.D98),
            T.get(S.D99)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getEnemyHitNoDamageDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D100),
            T.get(S.D101),
            T.get(S.D102),
            T.get(S.D103),
            T.get(S.D104),
            T.get(S.D105)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getEnemyHitOneDamageDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D106),
            T.get(S.D107),
            T.get(S.D108),
            T.get(S.D109),
            T.get(S.D110),
            T.get(S.D111),
            T.get(S.D112),
            T.get(S.D113),
            T.get(S.D114),
            T.get(S.D115),
            T.get(S.D116)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getGenericEnemyHitDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D92),
            T.get(S.D93),
            T.get(S.D106),
            T.get(S.D117),
            T.get(S.D118),
            T.get(S.D119),
            T.get(S.D120),
            T.get(S.D121),
            T.get(S.D122),
            T.get(S.D123),
            T.get(S.D124),
            T.get(S.D125),
            T.get(S.D126)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getEnemyAttackDialogue()
    {
        List<string> texts;
        Enemy enemy = BattleManager.bmInstance.enemy;
        switch (enemy.lastUsedAttack)
        {
            case Attack.NOTHING:
                texts = new List<string>
                 {
                    T.get(S.D127),
                    T.get(S.D128),
                    T.get(S.D129)
                 };
                if (enemy.health == enemy.maxHealth) // If at max health
                {
                    texts.AddRange(new List<string> {
                        T.get(S.D130),
                        T.get(S.D131),
                        T.get(S.D132),
                        T.get(S.D133),
                        T.get(S.D134)
                    });
                }
                if (enemy.health <= enemy.maxHealth * 0.5f) // If at 50% health or less
                {
                    texts.AddRange(new List<string> {
                        T.get(S.D135),
                        T.get(S.D136),
                        T.get(S.D137),
                        T.get(S.D138),
                        T.get(S.D139)
                    });
                }
                break;
            case Attack.NORMAL_ATTACK:
            case Attack.RECOIL_ATTACK:
                if (enemy.damageFromLastAttack >= 0 && enemy.damageFromLastAttack <= 2) // 0~2 damage
                {
                    texts = new List<string>
                    {
                        T.get(S.D140),
                        T.get(S.D141),
                        T.get(S.D142),
                        T.get(S.D143),
                        T.get(S.D144)
                    };
                    if (enemy.damageFromLastAttack == 0)
                    {
                        texts.AddRange(new List<string> {
                            T.get(S.D145),
                            T.get(S.D146)
                        });
                    }
                }
                else if (Player.health <= 0) // Lethal damage
                {
                    texts = new List<string>
                    {
                        T.get(S.D147),
                        T.get(S.D148),
                        T.get(S.D149),
                        T.get(S.D150),
                        T.get(S.D151)
                    };
                    if (enemy.lastUsedAttack == Attack.RECOIL_ATTACK) // Additional specific lines for recoil attack
                    {
                        texts.AddRange(new List<string>
                        {
                            T.get(S.D152),
                            T.get(S.D153)
                        });
                    }
                }
                else if (enemy.damageAgainstHealth * 10 < Player.health &&
                    Player.health > Player.initialHealth * 1.3f) // Low damage (player has too much health)
                {
                    texts = new List<string>
                    {
                        T.get(S.D154),
                        T.get(S.D155),
                        T.get(S.D156),
                        T.get(S.D157),
                        T.get(S.D158),
                        T.get(S.D159),
                        T.get(S.D160),
                        T.get(S.D161),
                        T.get(S.D162)
                    };
                }
                else if (enemy.lastUsedAttack == Attack.RECOIL_ATTACK && 
                    enemy.health <= enemy.maxHealth * 0.4f) // Leaving the enemy at low health with recoil attack
                {
                    texts = new List<string>
                    {
                        T.get(S.D163),
                        T.get(S.D164),
                        T.get(S.D165),
                        T.get(S.D166),
                        T.get(S.D167)
                    };
                }
                else if (enemy.lastUsedAttack == Attack.RECOIL_ATTACK) // Recoil attack generic
                {
                    texts = new List<string>
                    {
                        T.get(S.D168),
                        T.get(S.D169),
                        T.get(S.D170),
                        T.get(S.D171),
                        T.get(S.D172)
                    };
                }
                else // Normal attack generic
                {
                    texts = new List<string>
                    {
                        T.get(S.D173),
                        T.get(S.D174),
                        T.get(S.D175),
                        T.get(S.D176),
                        T.get(S.D177),
                        T.get(S.D178),
                        T.get(S.D179),
                        T.get(S.D180),
                        T.get(S.D181),
                        T.get(S.D182),
                        T.get(S.D183),
                        T.get(S.D184)
                    };
                    if (enemy.health <= enemy.maxHealth * 0.5f) // If enemy is weakened (50% health or less)
                    {
                        texts.AddRange(new List<string>
                        {
                            T.get(S.D185),
                            T.get(S.D186),
                            T.get(S.D187)
                        });
                    }
                }
                break;
            case Attack.DICE_ATTACK:
                texts = new List<string>
                {
                    T.get(S.D188),
                    T.get(S.D189),
                    T.get(S.D190),
                    T.get(S.D191),
                    T.get(S.D192),
                    T.get(S.D193)
                };
                break;
            case Attack.MILL:
                if (BattleManager.bmInstance.noCardsLeftInDeck()) // Milled last card
                {
                    texts = new List<string>
                    {
                        T.get(S.D194),
                        T.get(S.D195),
                        T.get(S.D196)
                    };
                } else
                {
                    texts = new List<string>
                    {
                        T.get(S.D197),
                        T.get(S.D198),
                        T.get(S.D199),
                        T.get(S.D200),
                        T.get(S.D201),
                        T.get(S.D202)
                    };
                }
                break;
            case Attack.DISCARD:
                if (BattleManager.bmInstance.noCardsLeftInHand()) // Discarded last card in hand
                {
                    texts = new List<string>
                    {
                        T.get(S.D203),
                        T.get(S.D204),
                        T.get(S.D205)
                    };
                }
                else
                {
                    texts = new List<string>
                    {
                        T.get(S.D197),
                        T.get(S.D198),
                        T.get(S.D200),
                        T.get(S.D206),
                        T.get(S.D207),
                        T.get(S.D208),
                        T.get(S.D209)
                    };
                }
                break;
            case Attack.HEAL:
            case Attack.DRAIN:
                if (enemy.lastUsedAttack == Attack.DRAIN && enemy.damageFromLastAttack == 0) // No damage (drain only)
                {
                    texts = new List<string>
                    {
                        T.get(S.D140),
                        T.get(S.D141),
                        T.get(S.D143),
                        T.get(S.D145),
                        T.get(S.D146),
                        T.get(S.D210)
                    };
                }
                else if (Player.health <= 0) // Lethal damage (drain only)
                {
                    texts = new List<string>
                    {
                        T.get(S.D211),
                        T.get(S.D212),
                        T.get(S.D213)
                    };
                }
                else if (enemy.health == enemy.maxHealth) // Healed 100% health
                {
                    texts = new List<string>
                    {
                        T.get(S.D214),
                        T.get(S.D215),
                        T.get(S.D216),
                        T.get(S.D217),
                        T.get(S.D218),
                        T.get(S.D219),
                        T.get(S.D220),
                        T.get(S.D221),
                        T.get(S.D222),
                        T.get(S.D223),
                        T.get(S.D224)
                    };
                }
                else
                {
                    texts = new List<string>
                    {
                        T.get(S.D223),
                        T.get(S.D224),
                        T.get(S.D225),
                        T.get(S.D226),
                        T.get(S.D227),
                        T.get(S.D228)
                    };
                }
                break;
            case Attack.CHARGE:
                texts = new List<string>
                {
                    T.get(S.D229),
                    T.get(S.D230),
                    T.get(S.D231),
                    T.get(S.D232),
                    T.get(S.D233),
                    T.get(S.D234)
                };
                if (enemy.damageAgainstHealth * 10 < Player.health &&
                    Player.health > Player.initialHealth * 1.3f) // Player has too much health
                {
                    for (int i=0; i < 3; i++) // Higher chance of picking one of these
                    {
                        texts.AddRange(new List<string>
                        {
                            T.get(S.D235),
                            T.get(S.D236),
                            T.get(S.D237)
                        });
                    }
                }
                break;
            case Attack.CURSE:
                if (Player.nDice <= 50) // 50 dice or less
                {
                    texts = new List<string>
                    {
                        T.get(S.D238),
                        T.get(S.D239),
                        T.get(S.D240),
                        T.get(S.D241),
                        T.get(S.D242)
                    };
                }
                else if (Player.nDice <= 1000) // 1000 dice or less
                {
                    texts = new List<string> {};
                }
                else // Above 1000 dice
                {
                    texts = new List<string>
                    {
                        T.get(S.D243),
                        T.get(S.D244),
                        T.get(S.D245),
                        T.get(S.D246)
                    };
                }
                texts.AddRange(new List<string> // Common to all
                {
                    T.get(S.D247),
                    T.get(S.D248),
                    T.get(S.D249),
                    T.get(S.D250),
                    T.get(S.D251)
                });
                break;
            case Attack.WEAKEN_CARDS:
            case Attack.SWAP_CARDS:
                texts = new List<string>
                    {
                        T.get(S.D291),
                        T.get(S.D292),
                        T.get(S.D293),
                        T.get(S.D294)
                    };
                break;

            default:
                return "What attack is this?";
        }
        return getRandomChoiceFrom(texts);
    }

    static string getVictoryDialogue()
    {
        List<string> texts;
        if (Player.health <= BattleManager.bmInstance.enemy.damageAgainstHealth)
        {
            texts = new List<string>
            {
                T.get(S.D252),
                T.get(S.D253),
                T.get(S.D254),
                T.get(S.D255),
                T.get(S.D256),
                T.get(S.D257)
            };
        } else
        {
            texts = new List<string>
            {
                T.get(S.D254),
                T.get(S.D255),
                T.get(S.D258),
                T.get(S.D259),
                T.get(S.D260),
                T.get(S.D261),
                T.get(S.D262),
                T.get(S.D263),
                T.get(S.D264),
                T.get(S.D265),
                T.get(S.D266),
                T.get(S.D267),
                T.get(S.D268)
            };
        }
        return getRandomChoiceFrom(texts);
    }

    static string getLoseNoHealthDialogue()
    {
        List<string> texts;
        Enemy enemy = BattleManager.bmInstance.enemy;
        if (enemy.health <= enemy.maxHealth * 0.4f)
        {
            texts = new List<string>
            {
                T.get(S.D253),
                T.get(S.D255),
                T.get(S.D269),
                T.get(S.D270),
                T.get(S.D271),
                T.get(S.D272),
                T.get(S.D273)
            };
        } else
        {
            texts = new List<string>
            {
                T.get(S.D274),
                T.get(S.D275),
                T.get(S.D276),
                T.get(S.D277),
                T.get(S.D278),
                T.get(S.D279)
            };
            if (enemy.health == enemy.maxHealth)
            {
                texts.AddRange(new List<string> {
                    T.get(S.D280),
                    T.get(S.D281),
                    T.get(S.D282),
                    T.get(S.D283),
                    T.get(S.D284)
                });
            }
            if (enemy.health < enemy.maxHealth)
            {
                texts.AddRange(new List<string> {
                    T.get(S.D285),
                    T.get(S.D286),
                    T.get(S.D287)
                });
            }
        }
        return getRandomChoiceFrom(texts);
    }

    static string getLoseNoCardsDialogue()
    {
        List<string> texts = new List<string>
         {
            T.get(S.D288),
            T.get(S.D289),
            T.get(S.D290)
         };
        return getRandomChoiceFrom(texts);
    }

    static string getRandomChoiceFrom(List<string> texts)
    {
        int randomIndex = Random.Range(0, texts.Count);
        return texts[randomIndex];
    }
}
