using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextScript {

    public enum Language { ENGLISH, SPANISH };
    public static Language language = Language.ENGLISH;

    public enum Sentence { START, LOADING, RANK, ATTACK_ROLLS, ATTACK_ROLL, TUTORIAL, DECK_EDITOR, FORTUNES, QUIT, OPTIONS,
        RETURN, MUTE, CREDITS, CREDITS_CONTENT, CHANGE_TO_DECK_VIEW, CHANGE_TO_TRADE_VIEW, DECK, COLLECTION, TRADE_FOR_EXP,
        TRADE, EXP, YOUR_DECK_MUST_HAVE_15_CARDS, FORTUNES_UPPERCASE,

        F0_NAME, F1_NAME, F2_NAME, F3_NAME, F4_NAME, F5_NAME, F6_NAME, F0_DESC, F1_DESC, F2_DESC, F3_DESC, F4_DESC,
        F5_DESC, F6_DESC,

        ARM0, ARM1, ARM2, ARM3, ARM4, ARM5, ARM6, ARM7, ARM8, ARM9, ARM10, ARM11, ARM12, ARM13, ARM14, ARM15, ARM16,
        ARM0_DESC, ARM1_DESC, ARM2_DESC, ARM3_DESC, ARM4_DESC, ARM5_DESC, ARM6_DESC, ARM7_DESC, ARM8_DESC, ARM9_DESC,
        ARM10_DESC, ARM11_DESC, ARM12_DESC, ARM13_DESC, ARM14_DESC, ARM15_DESC, ARM16_DESC,

        ARM0_DESC_S, ARM1_DESC_S, ARM2_DESC_S, ARM3_DESC_S, ARM4_DESC_S, ARM5_DESC_S, ARM6_DESC_S, ARM7_DESC_S, ARM8_DESC_S,
        ARM9_DESC_S, ARM10_DESC_S, ARM11_DESC_S, ARM12_DESC_S, ARM13_DESC_S, ARM14_DESC_S, ARM15_DESC_S, ARM16_DESC_S,

        ARM0_DESC_G, ARM1_DESC_G, ARM2_DESC_G, ARM3_DESC_G, ARM4_DESC_G, ARM5_DESC_G, ARM6_DESC_G, ARM7_DESC_G, ARM8_DESC_G,
        ARM9_DESC_G,ARM10_DESC_G, ARM11_DESC_G, ARM12_DESC_G, ARM13_DESC_G, ARM14_DESC_G, ARM15_DESC_G, ARM16_DESC_G,

        CORE0, CORE1, CORE2, CORE3, CORE4, CORE5, CORE6, CORE7, CORE8, CORE9,
        CORE0_DESC, CORE1_DESC, CORE2_DESC, CORE3_DESC, CORE4_DESC, CORE5_DESC, CORE6_DESC, CORE7_DESC, CORE8_DESC,
        CORE9_DESC,

        LV, PRICE, CURRENT, NEXT, NONE, HEALTH, DICE, DMG, INSTANT, CONTINUOUS, COST,
        COND0, COND1, COND1_A, COND2, DICE_ABBR, DICE_UPPER, HEALTH_UPPER, THIS_TURN, NEXT_TURN, IN, TURNS, TEMPORARY,
        FIXED_DICE_A, FIXED_DICE_B, ADD_ATTACK_ROLL_A, ADD_ATTACK_ROLL_B, FORBID_NUMBER_A, FORBID_NUMBER_B, LOWER_MAX,
        IMPROVE, SWAP_CARDS,

        LOSE_HEALTH_A, LOSE_HEALTH_B, LOSE_DICE, SINGLE_DICE, YOUR_DECK_LOSES, CARD_SINGLE, CARD_PLURAL, DISCARD, RECOVER,
        DRAIN_A, DRAIN_B, ATTACK_PLUS, CURSE_HALF, CURSE_GENERIC, WEAKEN_CARDS, DISCARD_PLUS_DAMAGE_A, DISCARD_PLUS_DAMAGE_B,

        DESC_NOTHING, DESC_NORMAL, DESC_DICE, DESC_MILL, DESC_DISCARD, DESC_HEAL, DESC_RECOIL, DESC_DRAIN, DESC_CHARGE,
        DESC_CURSE, DESC_WEAKEN_CARDS, DESC_SWAP_CARDS, EXCL, AND, CAUSE_DAMAGE_A, CAUSE_DAMAGE_B, WIN, LOSE, LOSE_NO_CARDS,
        VICTORY, DEFEAT, MAX_ROLL, SAVING_TEXT,

        T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24,
        T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44, T45, T46, T47,
        T48, T49, T50, T51, T52, T53, T54, T55, T56, T57, T58, T59, T60, T61, T62, T63, T64, T65, T66, T67, T68, T69, T70,
        T71, T72, T73, T74, T75, T76, T77, T78, T79, T80, T81, T82, T83, T84, T85,

        D0, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, D12, D13, D14, D15, D16, D17, D18, D19, D20, D21,
        D22, D23, D24, D25, D26, D27, D28, D29, D30, D31, D32, D33, D34, D35, D36, D37, D38, D39, D40, D41, D42, D43, D44,
        D45, D46, D47, D48, D49, D50, D51, D52, D53, D54, D55, D56, D57, D58, D59, D60, D61, D62, D63, D64, D65, D66, D67,
        D68, D69, D70, D71, D72, D73, D74, D75, D76, D77, D78, D79, D80, D81, D82, D83, D84, D85, D86, D87, D88, D89, D90,
        D91, D92, D93, D94, D95, D96, D97, D98, D99, D100, D101, D102, D103, D104, D105, D106, D107, D108, D109, D110, D111,
        D112, D113, D114, D115, D116, D117, D118, D119, D120, D121, D122, D123, D124, D125, D126, D127, D128, D129, D130,
        D131, D132, D133, D134, D135, D136, D137, D138, D139, D140, D141, D142, D143, D144, D145, D146, D147, D148, D149,
        D150, D151, D152, D153, D154, D155, D156, D157, D158, D159, D160, D161, D162, D163, D164, D165, D166, D167, D168,
        D169, D170, D171, D172, D173, D174, D175, D176, D177, D178, D179, D180, D181, D182, D183, D184, D185, D186, D187,
        D188, D189, D190, D191, D192, D193, D194, D195, D196, D197, D198, D199, D200, D201, D202, D203, D204, D205, D206,
        D207, D208, D209, D210, D211, D212, D213, D214, D215, D216, D217, D218, D219, D220, D221, D222, D223, D224, D225,
        D226, D227, D228, D229, D230, D231, D232, D233, D234, D235, D236, D237, D238, D239, D240, D241, D242, D243, D244,
        D245, D246, D247, D248, D249, D250, D251, D252, D253, D254, D255, D256, D257, D258, D259, D260, D261, D262, D263,
        D264, D265, D266, D267, D268, D269, D270, D271, D272, D273, D274, D275, D276, D277, D278, D279, D280, D281, D282,
        D283, D284, D285, D286, D287, D288, D289, D290, D291, D292, D293, D294,

        SKIP_ANIM, DELETE_SAVE, DELETE_BUTTON, DELETE_WARNING_CLICK,
        DELETE_WARNING_TAP, REROLL, REROLL_UPPERCASE, REROLL_EXPLANATION_INTRO, REROLL_EXPLANATION_END_CLICK,
        REROLL_EXPLANATION_END_TAP, REROLL_CARDS, REROLL_EXP, REROLL_SKIP, SURRENDER, GIVE_UP, SKIP_ENEMY_DIALOGUE,
        NEW_VERSION_AVAILABLE, DOWNLOAD_LINK, CHECK_UPDATES_ON_START};

    /*static TextScript()
    {
        string qwerty = "";
        for (int i=100; i < 500; i++)
        {
            qwerty += "case Sentence.D" + i + ":\nreturn new string[] {\"\"};\n";
            //qwerty += "D" + i + ", ";
        }
        Debug.Log(qwerty);
    }*/

    public static string get(Sentence sentence)
    {
        string[] sentenceInAllLanguages = getForAllLanguages(sentence);
        return sentenceInAllLanguages[(int)language];
    }

    public static string[] getForAllLanguages(Sentence sentence)
    {
        switch (sentence)
        {
            //// Title screen
            case Sentence.START:
                return new string[] { "START", "EMPEZAR" };
            case Sentence.LOADING:
                return new string[] { "Loading", "Cargando" };

            //// Hub menu
            case Sentence.SAVING_TEXT:
                return new string[] { "Game Saved", "Juego guardado" };
            // Battle button
            case Sentence.RANK:
                return new string[] { "Rank", "Rango" };
            case Sentence.ATTACK_ROLLS:
                return new string[] { "Attack rolls", "Tiradas de ataque" };
            case Sentence.ATTACK_ROLL:
                return new string[] { "Attack roll", "Tirada de ataque" };
            // Hub buttons
            case Sentence.TUTORIAL:
                return new string[] { "Tutorial", "Tutorial" };
            case Sentence.DECK_EDITOR:
                return new string[] { "Deck Editor", "Editar Baraja" };
            case Sentence.FORTUNES:
                return new string[] { "Fortunes", "Fortunas" };
            case Sentence.QUIT:
                return new string[] { "Quit", "Abandonar" };
            case Sentence.OPTIONS:
                return new string[] { "Options", "Opciones" };
            case Sentence.REROLL:
                return new string[] { "Reroll", "Relanzar" };
            // Options text
            case Sentence.RETURN:
                return new string[] { "Return", "Volver" };
            case Sentence.MUTE:
                return new string[] { "Mute", "Silenciar" };
            case Sentence.SKIP_ANIM:
                return new string[] { "Quick animations", "Animación rápida" };
            case Sentence.SKIP_ENEMY_DIALOGUE:
                return new string[] { "No enemy dialogue", "Sin diálogo enemigo" };
            case Sentence.CHECK_UPDATES_ON_START:
                return new string[] { "Check for updates\non game start", "Buscar actualizaciones\nal iniciar juego" };
            case Sentence.DELETE_SAVE:
                return new string[] { "Delete save", "Borrar partida" };
            case Sentence.DELETE_BUTTON:
                return new string[] { "DELETE", "BORRAR" };
            case Sentence.DELETE_WARNING_CLICK:
                return new string[] { "Delete your savefile?\n" +
                    "<b>You cannot undo this action.</b>\n" +
                    "If you agree, click the \"DELETE\" button 7 times.",
                "¿Borrar la partida?\n" +
                "<b>Esta acción no puede deshacerse.</b>\n" +
                "Si estás de acuerdo, haz click en \"BORRAR\" 7 veces."};
            case Sentence.DELETE_WARNING_TAP:
                return new string[] { "Delete your savefile?\n" +
                    "<b>You cannot undo this action.</b>\n" +
                    "If you agree, tap the \"DELETE\" button 7 times.",
                "¿Borrar la partida?\n" +
                "<b>Esta acción no puede deshacerse.</b>\n" +
                "Si estás de acuerdo, toca \"BORRAR\" 7 veces."};
            case Sentence.CREDITS:
                return new string[] { "Credits", "Créditos" };
            case Sentence.CREDITS_CONTENT:
                return new string[] { "<size=70><b>Programming / Design / Graphics / Music / Everything else</b>\n" +
                    "</size>Samuel Vázquez",
                    "<size=70><b>Programación / Diseño / Gráficos / Música / Todo lo demás</b>\n" +
                    "</size>Samuel Vázquez" };

            //// Deck Editor
            case Sentence.CHANGE_TO_DECK_VIEW:
                return new string[] { "Change to Deck View", "Cambiar a Modo Baraja" };
            case Sentence.CHANGE_TO_TRADE_VIEW:
                return new string[] { "Change to Trade View", "Cambiar a Modo Intercambio" };
            case Sentence.DECK:
                return new string[] { "DECK", "BARAJA" };
            case Sentence.COLLECTION:
                return new string[] { "COLLECTION", "COLECCIÓN" };
            case Sentence.TRADE_FOR_EXP:
                return new string[] { "TRADE FOR EXP", "CAMBIAR POR EXP" };
            case Sentence.TRADE:
                return new string[] { "Trade", "Intercambiar" };
            case Sentence.EXP:
                return new string[] { "EXP", "EXP" };
            case Sentence.YOUR_DECK_MUST_HAVE_15_CARDS:
                return new string[] { "Your deck must have 15 cards", "Tu baraja debe tener 15 cartas" };

            //// Fortunes
            case Sentence.FORTUNES_UPPERCASE:
                return new string[] { "FORTUNES", "FORTUNAS" };
            case Sentence.F0_NAME:
                return new string[] { "Endurance", "Resistencia" };
            case Sentence.F0_DESC:
                return new string[] { "Increases your health at the beginning of a battle.",
                    "Aumenta la salud con la que inicias el combate." };
            case Sentence.F1_NAME:
                return new string[] { "Quick Start", "Inicio Rápido" };
            case Sentence.F1_DESC:
                return new string[] { "Roll additional dice on the first turn.",
                    "Lanza dados adicionales durante el primer turno." };
            case Sentence.F2_NAME:
                return new string[] { "Shield", "Escudo" };
            case Sentence.F2_DESC:
                return new string[] { "Lower all damage received.", "Reduce todo el daño recibido." };
            case Sentence.F3_NAME:
                return new string[] { "Regeneration", "Regeneración" };
            case Sentence.F3_DESC:
                return new string[] { "Recover some health every turn.", "Recupera algo de salud cada turno." };
            case Sentence.F4_NAME:
                return new string[] { "Lower Ceil", "Reducir Máx" };
            case Sentence.F4_DESC:
                return new string[] { "Lower the number of sides on the dice.", "Reduce el número de lados en los dados." };
            case Sentence.F5_NAME:
                return new string[] { "Extra Exp", "Exp Extra" };
            case Sentence.F5_DESC:
                return new string[] { "Gain additional exp from winning battles.", "Recibe exp adicional por ganar batallas." };
            case Sentence.F6_NAME:
                return new string[] { "Extra Prize Card", "Carta Premio Extra" };
            case Sentence.F6_DESC:
                return new string[] { "Chance to get an extra prize card on victory.",
                    "Posibilidad de recibir una carta extra al ganar." };
            case Sentence.LV:
                return new string[] { "Lv", "Nv" };
            case Sentence.PRICE:
                return new string[] { "Price", "Precio" };
            case Sentence.CURRENT:
                return new string[] { "Current", "Actual" };
            case Sentence.NEXT:
                return new string[] { "Next", "Siguiente" };
            case Sentence.NONE:
                return new string[] { "None", "Ninguno" };
            case Sentence.HEALTH:
                return new string[] { "health", "salud" };
            case Sentence.DICE:
                return new string[] { "dice", "dados" };
            case Sentence.DMG:
                return new string[] { "dmg", "daño" };

            //// Arms
            // Normal
            case Sentence.ARM0:
                return new string[] { "Normal arm", "Brazo Normal" };
            case Sentence.ARM0_DESC:
                return new string[] { "No added effect", "Sin efecto añadido" };

            case Sentence.ARM1:
                return new string[] { "Claw", "Garra" };
            case Sentence.ARM1_DESC:
                return new string[] { "Higher chance of knocking off your dice", "Mayor probabilidad de derribar tus dados" };

            case Sentence.ARM2:
                return new string[] { "Power Glow", "Fuerza Brillante" };
            case Sentence.ARM2_DESC:
                return new string[] { "+20% damage for standard attack", "+20% daño con ataque normal" };

            case Sentence.ARM3:
                return new string[] { "Blade", "Espada" };
            case Sentence.ARM3_DESC:
                return new string[] { "Higher chance of standard attack", "Mayor probabilidad de ataque normal" };

            case Sentence.ARM4:
                return new string[] { "Buzzsaw", "Sierra" };
            case Sentence.ARM4_DESC:
                return new string[] { "Can attack the deck directly", "Puede atacar la baraja directamente" };

            case Sentence.ARM5:
                return new string[] { "Shield", "Escudo" };
            case Sentence.ARM5_DESC:
                return new string[] { "+20% health", "+20% salud" };

            case Sentence.ARM6:
                return new string[] { "Fan", "Ventilador" };
            case Sentence.ARM6_DESC:
                return new string[] { "Can attack your hand directly", "Puede atacar tu mano directamente" };

            case Sentence.ARM7:
                return new string[] { "Grapnel", "Arpeo" };
            case Sentence.ARM7_DESC:
                return new string[] { "Can knock off more dice at once", "Puede derribar más dados a la vez" };

            case Sentence.ARM8:
                return new string[] { "Healing Salve", "Bálsamo Sanador" };
            case Sentence.ARM8_DESC:
                return new string[] { "Can heal itself", "Puede curarse" };

            case Sentence.ARM9:
                return new string[] { "Forbidden Relic", "Reliquia Prohibida" };
            case Sentence.ARM9_DESC:
                return new string[] { "Can sacrifice health to deal double damage", "Puede sacrificar salud para hacer doble daño" };

            case Sentence.ARM10:
                return new string[] { "Siphon", "Drenador" };
            case Sentence.ARM10_DESC:
                return new string[] { "Can drain health to heal itself", "Puede drenar salud para curarse" };

            case Sentence.ARM11:
                return new string[] { "Battery", "Batería" };
            case Sentence.ARM11_DESC:
                return new string[] { "Can charge up energy for future attacks (+20%)", "Puede cargar energía para ataques futuros (+20%)" };

            case Sentence.ARM12:
                return new string[] { "Miasma", "Miasma" };
            case Sentence.ARM12_DESC:
                return new string[] { "Halves all your healing", "Todas tus curaciones se reducen a la mitad" };

            case Sentence.ARM13:
                return new string[] { "Aegis", "Égida" };
            case Sentence.ARM13_DESC:
                return new string[] { "Lowers all received damage by 20%", "Reduce todo el daño recibido en un 20%" };

            case Sentence.ARM14:
                return new string[] { "Cartilage", "Cartílago" };
            case Sentence.ARM14_DESC:
                return new string[] { "Can temporarily halve your number of rolls", "Puede temporalmente reducir a la mitad tus dados" };

            case Sentence.ARM15:
                return new string[] { "Floe", "Témpano" };
            case Sentence.ARM15_DESC:
                return new string[] { "Can weaken the cards in your hand until the battle ends", "Puede debilitar las cartas de tu mano hasta que termine la batalla" };

            case Sentence.ARM16:
                return new string[] { "Illusionist", "Ilusionista" };
            case Sentence.ARM16_DESC:
                return new string[] { "Can swap the cards in your hand for random ones", "Puede cambiar las cartas de tu mano por otras aleatorias" };

            // Silver
            case Sentence.ARM0_DESC_S:
                return new string[] { "Still no added effect", "Todavía sin efecto añadido" };

            case Sentence.ARM1_DESC_S:
                return new string[] { "Can attack your health and dice at once", "Puede atacar tu salud y tus dados a la vez" };

            case Sentence.ARM2_DESC_S:
                return new string[] { "+50% damage for standard attack", "+50% daño con ataque normal" };

            case Sentence.ARM3_DESC_S:
                return new string[] { "Higher chance of standard attack, and +20% attack", "Mayor probabilidad de ataque normal, y +20% ataque" };

            case Sentence.ARM4_DESC_S:
                return new string[] { "Can attack your deck and health at once", "Puede atacar tu baraja y tu salud a la vez" };

            case Sentence.ARM5_DESC_S:
                return new string[] { "x2 health", "x2 salud" };

            case Sentence.ARM6_DESC_S:
                return new string[] { "Can attack your hand and health at once", "Puede atacar tu mano y tu salud a la vez" };

            case Sentence.ARM7_DESC_S:
                return new string[] { "Can knock off more dice at once", "Puede derribar más dados a la vez" };

            case Sentence.ARM8_DESC_S:
                return new string[] { "Can heal itself, even past its inital health", "Puede curarse, incluso más allá de su salud inicial" };

            case Sentence.ARM9_DESC_S:
                return new string[] { "Can sacrifice health to deal x4 damage", "Puede sacrificar salud para hacer daño x4" };

            case Sentence.ARM10_DESC_S:
                return new string[] { "Can drain health to heal itself", "Puede drenar salud para curarse" };

            case Sentence.ARM11_DESC_S:
                return new string[] { "Can charge up energy for future attacks (x2)", "Puede cargar energía para ataques futuros (x2)" };

            case Sentence.ARM12_DESC_S:
                return new string[] { "Divides all your healing by 4", "Todas tus curaciones se dividen por 4" };

            case Sentence.ARM13_DESC_S:
                return new string[] { "Lowers all received damage by 50%", "Reduce todo el daño recibido en un 50%" };

            case Sentence.ARM14_DESC_S:
                return new string[] { "Can temporarily divide your number of rolls by 4", "Puede temporalmente dividir tus dados por 4" };

            case Sentence.ARM15_DESC_S:
                return new string[] { "Can weaken the cards in your hand until the battle ends", "Puede debilitar las cartas de tu mano hasta que termine la batalla" };

            case Sentence.ARM16_DESC_S:
                return new string[] { "Can swap the cards in your hand for random weaker ones", "Puede cambiar las cartas de tu mano por cartas débiles aleatorias" };

            // Gold
            case Sentence.ARM0_DESC_G:
                return new string[] { "Does absolutely nothing", "No hace absolutamente nada" };

            case Sentence.ARM1_DESC_G:
                return new string[] { "Can attack your health and dice at once", "Puede atacar tu salud y tus dados a la vez" };

            case Sentence.ARM2_DESC_G:
                return new string[] { "+150% damage for standard attack", "+150% daño con ataque normal" };
                
            case Sentence.ARM3_DESC_G:
                return new string[] { "Higher chance of standard attack, and +50% attack", "Mayor probabilidad de ataque normal, y +50% ataque" };
                
            case Sentence.ARM4_DESC_G:
                return new string[] { "Can attack your deck (2 cards) and your health at once", "Puede atacar tu baraja (2 cartas) y tu salud a la vez" };
                
            case Sentence.ARM5_DESC_G:
                return new string[] { "x4 health", "x4 salud" };
                
            case Sentence.ARM6_DESC_G:
                return new string[] { "Can attack your hand and health at once", "Puede atacar tu mano y tu salud a la vez" };
                
            case Sentence.ARM7_DESC_G:
                return new string[] { "Can knock off more dice at once", "Puede derribar más dados a la vez" };
                
            case Sentence.ARM8_DESC_G:
                return new string[] { "Can heal itself, even past its inital health", "Puede curarse, incluso más allá de su salud inicial" };

            case Sentence.ARM9_DESC_G:
                return new string[] { "Can sacrifice health to deal x8 damage", "Puede sacrificar salud para hacer daño x8" };
                
            case Sentence.ARM10_DESC_G:
                return new string[] { "Can drain health to heal itself", "Puede drenar salud para curarse" };
                
            case Sentence.ARM11_DESC_G:
                return new string[] { "Can charge up energy for future attacks (x3.5)", "Puede cargar energía para ataques futuros (x3.5)" };
                
            case Sentence.ARM12_DESC_G:
                return new string[] { "Divides all your healing by 10", "Todas tus curaciones se dividen por 10" };
                
            case Sentence.ARM13_DESC_G:
                return new string[] { "Lowers all received damage by 75%", "Reduce todo el daño recibido en un 75%" };
                
            case Sentence.ARM14_DESC_G:
                return new string[] { "Can temporarily divide your number of rolls by 10", "Puede temporalmente dividir tus dados por 10" };

            case Sentence.ARM15_DESC_G:
                return new string[] { "Can weaken the cards in your hand until the battle ends", "Puede debilitar las cartas de tu mano hasta que termine la batalla" };

            case Sentence.ARM16_DESC_G:
                return new string[] { "Can swap the cards in your hand for random weaker ones", "Puede cambiar las cartas de tu mano por cartas débiles aleatorias" };

            //// Cores
            case Sentence.CORE0:
                return new string[] { "STANDARD CORE", "NÚCLEO ESTÁNDAR" };
            case Sentence.CORE0_DESC:
                return new string[] { "No effect", "Sin efecto" };
            case Sentence.CORE1:
                return new string[] { "BLUE CORE", "NÚCLEO AZUL" };
            case Sentence.CORE1_DESC:
                return new string[] { "Skip drawing a card every third turn", "No robas carta cada tres turnos" };
            case Sentence.CORE2:
                return new string[] { "RED CORE", "NÚCLEO ROJO" };
            case Sentence.CORE2_DESC:
                return new string[] { "+200 sides to dice. It's harder to roll low numbers",
                                      "+200 caras a los dados. Es más difícil sacar números bajos" };
            case Sentence.CORE3:
                return new string[] { "GREEN CORE", "NÚCLEO VERDE" };
            case Sentence.CORE3_DESC:
                return new string[] { "Start the battle with one less card in hand", "Empieza con una carta menos en tu mano" };

            case Sentence.CORE4:
                return new string[] { "AUROUS CORE", "NÚCLEO ÁUREO" };
            case Sentence.CORE4_DESC:
                return new string[] { "Can wield more arms or weapons at once", "Puede portar más brazos o armas a la vez" };

            case Sentence.CORE5:
                return new string[] { "CYAN CORE", "NÚCLEO CIAN" };
            case Sentence.CORE5_DESC:
                return new string[] { "Increases its health every turn", "Aumenta su salud cada turno" };

            case Sentence.CORE6:
                return new string[] { "MAGENTA CORE", "NÚCLEO MAGENTA" };
            case Sentence.CORE6_DESC:
                return new string[] { "Increases the max roll by a 25% every turn", "Aumenta el tiro máximo en un 25% cada turno"};

            case Sentence.CORE7:
                return new string[] { "ORANGE CORE", "NÚCLEO NARANJA" };
            case Sentence.CORE7_DESC:
                return new string[] { "You lose 10% of your dice every turn", "Pierdes el 10% de tus dados cada turno" };

            case Sentence.CORE8:
                return new string[] { "WHITE CORE", "NÚCLEO BLANCO" };
            case Sentence.CORE8_DESC:
                return new string[] { "???", "???" };

            case Sentence.CORE9:
                return new string[] { "BLACK CORE", "NÚCLEO NEGRO" };
            case Sentence.CORE9_DESC:
                return new string[] { "???", "???" };

            //// Card descriptions
            // Types
            case Sentence.INSTANT:
                return new string[] { "Instant", "Instantánea" };
            case Sentence.CONTINUOUS:
                return new string[] { "Continuous", "Continua" };
            // Conditions
            case Sentence.COST:
                return new string[] { "Cost", "Coste" };
            case Sentence.COND0:
                return new string[] { "During this turn, for rolls starting with",
                    "Durante este turno, para tiradas que empiecen por" };
            case Sentence.COND1:
                return new string[] { "Every turn, increase the modifier of rolls ending in",
                    "Cada turno, aumenta el modificador de tiradas que acaban en" };
            case Sentence.COND1_A:
                return new string[] { "by", "en" };
            case Sentence.COND2:
                return new string[] { "For rolls ending in", "Para tiradas que acaben en" };
            case Sentence.DICE_ABBR:
                return new string[] { "d", "d" };
            // Effect
            case Sentence.DICE_UPPER:
                return new string[] { "Dice", "Dados" };
            case Sentence.HEALTH_UPPER:
                return new string[] { "Health", "Salud" };
            case Sentence.THIS_TURN:
                return new string[] { "this turn", "este turno" };
            case Sentence.NEXT_TURN:
                return new string[] { "next turn", "en turno siguiente" };
            case Sentence.IN:
                return new string[] { "in", "en" };
            case Sentence.TURNS:
                return new string[] { "turns", "turnos" };
            case Sentence.TEMPORARY:
                return new string[] { "temporary", "temporal" };
            case Sentence.FIXED_DICE_A:
                return new string[] { "Ensures you will roll a", "Asegura que lanzarás un" };
            case Sentence.FIXED_DICE_B:
                return new string[] { "every turn", "cada turno" };
            case Sentence.FORBID_NUMBER_A:
                return new string[] { "Ensures you won't roll numbers ending in", "Asegura que tus tiradas no acabarán en" };
            case Sentence.FORBID_NUMBER_B:
                return new string[] { "from this turn on", "de ahora en adelante" };
            case Sentence.ADD_ATTACK_ROLL_A:
                return new string[] { "From this turn on,", "De este turno en adelante," };
            case Sentence.ADD_ATTACK_ROLL_B:
                return new string[] { "is considered an attack roll", "se considera tirada de ataque" };
            case Sentence.LOWER_MAX:
                return new string[] { "Lowers the number of sides of all dice by", "Reduce el número de caras de los dados en" };
            case Sentence.IMPROVE:
                return new string[] { "For this battle, improve all cards in your hand", "Para esta batalla, mejora todas las cartas en tu mano" };

            //// Battle
            // Enemy attacks
            case Sentence.EXCL:
                return new string[] { "", "¡" };
            case Sentence.DESC_NOTHING:
                return new string[] { "lazes about!", "no hace nada!" };
            case Sentence.DESC_NORMAL:
                return new string[] { "attacks!", "ataca!" };
            case Sentence.DESC_DICE:
                return new string[] { "knocks off your dice!", "derriba tus dados!" };
            case Sentence.DESC_MILL:
                return new string[] { "targets your deck!", "ataca tu baraja!" };
            case Sentence.DESC_DISCARD:
                return new string[] { "targets your hand!", "ataca tu mano!" };
            case Sentence.DESC_HEAL:
                return new string[] { "heals itself!", "se cura!" };
            case Sentence.DESC_RECOIL:
                return new string[] { "sacrifices health to attack!", "sacrifica salud para atacar!" };
            case Sentence.DESC_DRAIN:
                return new string[] { "drains your health!", "absorbe tu salud!" };
            case Sentence.DESC_CHARGE:
                return new string[] { "charges up energy!", "carga energía!" };
            case Sentence.DESC_CURSE:
                return new string[] { "curses you!", "te maldice!" };
            case Sentence.DESC_WEAKEN_CARDS:
                return new string[] { "sends a cold breeze towards your hand!", "envía una fría brisa hacia tu mano!" };
            case Sentence.DESC_SWAP_CARDS:
                return new string[] { "performs a fancy trick!", "realiza un curioso truco!" };
            case Sentence.LOSE_HEALTH_A:
                return new string[] { "You lose", "Recibes" };
            case Sentence.LOSE_HEALTH_B:
                return new string[] { "health", "de daño" };
            case Sentence.LOSE_DICE:
                return new string[] { "You lose", "Pierdes" };
            case Sentence.SINGLE_DICE:
                return new string[] { "die", "dado" };
            case Sentence.YOUR_DECK_LOSES:
                return new string[] { "Your deck loses", "Tu baraja pierde" };
            case Sentence.CARD_SINGLE:
                return new string[] { "card", "carta" };
            case Sentence.CARD_PLURAL:
                return new string[] { "cards", "cartas" };
            case Sentence.DISCARD:
                return new string[] { "You discard 1 card at random.", "Descartas 1 carta al azar." };
            case Sentence.RECOVER:
                return new string[] { "It recovers", "Recupera" };
            case Sentence.DRAIN_A:
                return new string[] { "It drains", "Absorbe" };
            case Sentence.DRAIN_B:
                return new string[] { "health from you", "de tu salud" };
            case Sentence.ATTACK_PLUS:
                return new string[] { "Its attack increases.", "Su ataque aumenta." };
            case Sentence.CURSE_HALF:
                return new string[] { "You will roll half as many dice this turn.", "Lanzarás la mitad de dados este turno." };
            case Sentence.CURSE_GENERIC:
                return new string[] { "You will roll less dice this turn.", "Lanzarás menos dados este turno." };
            case Sentence.WEAKEN_CARDS:
                return new string[] { "Your cards are weakened for this battle.", "Tus cartas se debilitan durante esta batalla." };
            case Sentence.SWAP_CARDS:
                return new string[] { "It swaps the cards in your hand.", "Cambia las cartas en tu mano." };
            case Sentence.AND:
                return new string[] { "and", "y" };
            case Sentence.DISCARD_PLUS_DAMAGE_A:
                return new string[] { "You discard 1 card", "Descartas 1 carta" };
            case Sentence.DISCARD_PLUS_DAMAGE_B:
                return new string[] { "and lose", "y recibes" };
            case Sentence.CAUSE_DAMAGE_A:
                return new string[] { "You dealt", "¡Causas" };
            case Sentence.CAUSE_DAMAGE_B:
                return new string[] { "damage!", "de daño!" };
            // Info
            case Sentence.GIVE_UP:
                return new string[] { "Give up", "Rendirse" };
            case Sentence.WIN:
                return new string[] { "You win!", "¡Has ganado!" };
            case Sentence.LOSE:
                return new string[] { "You lose...", "Has perdido..." };
            case Sentence.LOSE_NO_CARDS:
                return new string[] { "You ran out of cards.\nYou lose...", "Te quedaste sin cartas.\nHas perdido..." };
            case Sentence.SURRENDER:
                return new string[] { "You surrendered...", "Te has rendido..." };
            case Sentence.VICTORY:
                return new string[] { "Victory", "Victoria" };
            case Sentence.DEFEAT:
                return new string[] { "Defeat", "Derrota" };
            case Sentence.MAX_ROLL:
                return new string[] { "Max roll", "Tirada máxima" };

            //// Tutorial
            case Sentence.T0:
                return new string[] { "Tap anywhere to continue...", "Toca para continuar..." };
            case Sentence.T1:
                return new string[] { "Click anywhere to continue...", "Haz click para continuar..." };
            case Sentence.T2:
                return new string[] { "Welcome, hominid.", "Te doy la bienvenida, homínido." };
            case Sentence.T3:
                return new string[] { "My victory is assured, but I figured\n I might as well explain the rules,\nif only to give you a fighting chance.",
                "Mi victoria está asegurada, pero supongo\nque podría explicarte las reglas, aunque\nsolo sea para darte una oportunidad."};
            case Sentence.T4:
                return new string[] { "You will fight me by rolling dice.\nIf your die lands on any number\nbetween 1 and 5, you get to attack me.",
                "Te enfrentarás a mí lanzando\ndados. Si sacas un número entre\n1 y 5, puedes atacarme."};
            case Sentence.T5:
                return new string[] { "Care to try?", "¿Lo intentas?" };
            case Sentence.T6:
                return new string[] { "Did I somehow forget to mention\nthat this die has 1000 sides?\nHonest mistake.",
                "¿Acaso se me olvidó mencionar\nque este dado tiene 1000 lados?\nFallo mío."};
            case Sentence.T7:
                return new string[] { "Well, you only had a 0.5% chance\nof succeeding, so that was\nthe most likely outcome.",
                "Bueno, solo tenías una probabilidad\ndel 0.5% de lograrlo, así que ese era\nel resultado más probable."};
            case Sentence.T8:
                return new string[] { "But maybe you can overcome the odds!\nYou are human, after all!",
                "¡Pero quizás puedas lograr lo imposible!\n¡Eres un ser humano, después de todo!"};
            case Sentence.T9:
                return new string[] { "Go on! Try rolling that die again!\nShow the world that willpower can trump luck!",
                "¡Adelante! ¡Prueba a volver a lanzar ese dado!\n¡Demuéstrale al mundo que la fuerza de\nvoluntad vale más que la suerte!"};
            case Sentence.T10:
                return new string[] { "Well, it turns out it can't.", "Vaya, pues resulta que no." };
            case Sentence.T11:
                return new string[] { "I mean, 0.5% <i>really</i> is a low chance.",
                "Quiero decir, un 0.5% es una\nprobabilidad <i>muy</i> baja."};
            case Sentence.T12:
                return new string[] { "I know I wanted to win,\nbut not <i>like this</i>.",
                "Sé que quería ganar,\npero no <i>así</i>."};
            case Sentence.T13:
                return new string[] { "There is no fun in crushing\nyou if you can't fight back.",
                "No tiene gracia aplastarte\nsi no te defiendes."};
            case Sentence.T14:
                return new string[] { "Oh, I know! Let's give you some\nkind of advantage! You are only\nhuman, after all. You need it.",
                "¡Ah, ya sé! ¡Vamos a darte ventaja!\nQuiero decir, no eres más que un\nser humano. Te hará falta."};
            case Sentence.T15:
                return new string[] { "Let's give you a deck of cards\nto play with! You can play one\neach turn. Go on, draw five.",
                "Vamos a darte una baraja de cartas\ncon la que jugar. Puedes jugar una\ncada turno. Venga, roba cinco."};
            case Sentence.T16:
                return new string[] { "Let's give you a deck of cards\nto play with!",
                "Vamos a darte una baraja de\ncartas con la que jugar."};
            case Sentence.T17:
                return new string[] { "You can play one each turn,\nby double tapping or sliding.\nGo on, draw five.",
                "Puedes jugar una carta cada turno,\ntocándola dos veces o arrastrándola.\nVenga, roba cinco.."};
            case Sentence.T18:
                return new string[] { "Go on, choose ANY one you like!\nEnjoy this breath of free will in\nthis otherwise deterministic universe!",
                 "¡Adelante, elige CUALQUIERA de ellas!\n¡Disfruta de este soplo de libre albedrío\nen este universo determinista!"};
            case Sentence.T19:
                return new string[] { "Did you see that?\nYou got three more dice!",
                "¿Has visto eso? ¡Has\nconseguido tres dados más!"};
            case Sentence.T20:
                return new string[] { "Your chances of victory\njust skyrocketed from 0.5%...!",
                "¡Tu probabilidad de victoria\nha escalado desde un 0.5%...!"};
            case Sentence.T21:
                return new string[] { "...to 1.9%!", "...hasta un 1.9%!" };
            case Sentence.T22:
                return new string[] { "No, that's still not gonna cut it.\nWe have to do something about this.\nWe need more dice!",
                "No, con eso todavía no nos basta.\nTenemos que hacer algo al respecto.\n¡Necesitamos más dados!"};
            case Sentence.T23:
                return new string[] { "Try playing that fancy\nnew card you just drew.",
                "Prueba a jugar esa carta\nnueva que acabas de robar."};
            case Sentence.T24:
                return new string[] { "You are really bad at following instructions.", "Se te da fatal seguir instrucciones." };
            case Sentence.T25:
                return new string[] { "Did you see that modifier on the 9\nchange to a +2? Now what does that mean?",
                "¿Viste como el modificador del 9\ncambió a +2? ¿Y eso que significa?"};
            case Sentence.T26:
                return new string[] { "Simple. From this turn on,\nfor <b>each</b> die you roll that ends\non a 9, you will get +2 dice.",
                "Fácil. De este turno en adelante,\npor <b>cada</b> dado que tires que acabe\nen 9, recibirás +2 dados."};
            case Sentence.T27:
                return new string[] { "Shall we test it?", "¿Lo probamos?" };
            case Sentence.T28:
                return new string[] { "Oh, you got lucky this time.\nThat <i>349</i> ends on a 9, so\ngiven the effect of the previous\ncard, you will get +2 dice.",
                "Oh, esta vez has tenido suerte.\nEse <i>349</i> acaba en 9, así\nque dado el efecto de la carta\nanterior, recibes +2 dados."};
            case Sentence.T29:
                return new string[] { "Better, but not yet enough.", "Mejor, pero aún no es suficiente." };
            case Sentence.T30:
                return new string[] { "I know, let's get rid of your\ncurrent hand of cards and...",
                "Ya sé, vamos a librarnos de\ntu actual mano de cartas y..."};
            case Sentence.T31:
                return new string[] { "I'm sure you'll know what to do.", "Seguro que sabes que hacer." };
            case Sentence.T32:
                return new string[] { "Oh? That's some unexpected luck.\nYou have a total 5 dice ending\nin either 7 or 9. That means you get\n+2 dice five times, so +10 dice.",
                "¿Oh? Has tenido una suerte inesperada.\nHas sacado cinco números que acaban\nen 7 o 9. Eso significa que recibes\n+2 dados cinco veces, luego +10 dados."};
            case Sentence.T33:
                return new string[] { " dice... No, that's okay.\nThat's not dangerous yet.",
                " dados... No, no pasa nada.\nEso no es peligroso, todavía."};
            case Sentence.T34:
                return new string[] { "You rolled ten dice ending in 7, 8 or 9.\nSo that's +20 dice. Mmh, not bad.\nAlso...",
                "Tienes diez números que acaban en 7, 8 o 9.\nAsí que +20 dados. Mmh, no esta mal.\nY además..."};
            case Sentence.T35:
                return new string[] { "Wait.", "Espera." };
            case Sentence.T36:
                return new string[] { "WAIT.", "ESPERA." };
            case Sentence.T37:
                return new string[] { "You managed to roll a 4?\nThat's a number between 1 and 5!\nThat's considered an attack roll!",
                "¿Has sacado un 4? ¡Ese número\nestá entre 1 y 5! ¡Eso se\nconsidera una tirada de ataque!"};
            case Sentence.T38:
                return new string[] { "Which means you get to attack me.\nWith 16 dice, there was only a 7.7% chance\nof that happening... Beginner's luck, I would guess?",
                "Lo que significa que puedes atacarme. Con\n16 dados, solo había una probabilidad del 7.7% de\nque eso sucediera... La suerte del principiante, supongo."};
            case Sentence.T39:
                return new string[] { "I think I've given you enough advice.\nI won't make things easy for you anymore.",
                "Creo que te he dado suficientes consejos.\nYa no te lo pondré tan fácil."};
            case Sentence.T40:
                return new string[] { "Not like you have much of a choice anyway.\nYou <b>MUST</b> play a card every turn.",
                "Tampoco es que tengas mucha elección.\n<b>DEBES</b> jugar una carta cada turno."};
            case Sentence.T41:
                return new string[] { "By the way, the tutorial now forces\nme to tell you that you can click\non my body to get information about me.",
                "Por cierto, el tutorial me obliga a\ndecirte que puedes hacer click en mi\ncuerpo para recibir información sobre mí."};
            case Sentence.T42:
                return new string[] { "I'm as basic as they come since\nwe're at the tutorial, but in the future\nit may pay to click me whenever\nyou see unfamiliar arms or bodies.",
                "Soy muy básico por que estamos en\nel tutorial, pero en el futuro podría ser\nbuena idea hacer click si ves\nbrazos o cuerpos desconocidos."};
            case Sentence.T43:
                return new string[] { "By the way, the tutorial now forces\nme to tell you that you can tap\nmy body to get information about me.",
                "Por cierto, el tutorial me obliga a\ndecirte que puedes tocarme para\nrecibir información sobre mí."};
            case Sentence.T44:
                return new string[] { "I'm as basic as they come since\nwe're at the tutorial, but in the future\nit may pay to tap me whenever\nyou see unfamiliar arms or bodies.",
                    "Soy muy básico por que estamos en\nel tutorial, pero en el futuro podría ser\nbuena idea tocarme si ves\nbrazos o cuerpos desconocidos."};
            case Sentence.T45:
                return new string[] { "Because knowledge\nis power, yes...?", "Porque el conocimiento\nes poder, ¿no?" };
            case Sentence.T46:
                return new string[] { "Hold on, 188 dice? That's\na 61% chance of hitting me!\nI don't like this...",
                "¿Espera, 188 dados? Eso es una\nprobabilidad del 61% de atacarme!\nEsto no me gusta..."};
            case Sentence.T47:
                return new string[] { "Oh, but you have almost\nno health left, anyway!",
                "¡Oh, pero casi no\nte queda salud!"};
            case Sentence.T48:
                return new string[] { "You are about to lose\nin a tutorial battle!",
                "¡Estás a punto de perder\nen un tutorial!"};
            case Sentence.T49:
                return new string[] { "It's not like you could be\nso lucky as to just so happen\nto draw a healing card this turn!",
                "¡No creo que tengas tanta suerte\ncomo para robar una carta de\ncuración justo en este momento!"};
            case Sentence.T50:
                return new string[] { ".......", "......." };
            case Sentence.T51:
                return new string[] { "I don't know why I bother sometimes.", "A veces no sé ni por que me molesto." };
            case Sentence.T52:
                return new string[] { "That healing card works just like\nthe ones that give you extra dice.",
                "Esa carta curativa funciona igual\nque las que te dan dados extra."};
            case Sentence.T53:
                return new string[] { "But as you can see, this one\nchecks for dice that <b>start</b> with a\ncertain number, 1 in this case.",
                "Pero como puedes ver, esta carta busca\ndados que <b>empiecen</b> por un\ncierto número, 1 en este caso."};
            case Sentence.T54:
                return new string[] { "Also, unlike the ones that give you\ndice, healing cards last only one turn.\nSo don't get used to this.",
                "Además, al contrario que las que te dan dados,\nlas cartas curativas solo duran un turno.\nAsí que no te acostumbres a esto."};
            case Sentence.T55:
                return new string[] { "I'll count for you. You rolled a total\n30 dice starting with 1. You get +2 health\nfor each one, so... +60 health.",
                "Contaré por ti. Has sacado un total de\n30 dados que empiezan por 1. Cada uno te da\n+2 de salud, así que... en total, +60 de salud."};
            case Sentence.T56:
                return new string[] { "It just occured to me that teaching\nyou how to defeat me might not\nbe the most clever of ideas.",
                "Se me acaba de ocurrir que enseñarte\ncomo derrotarme igual no ha sido\nla más inteligente de las ideas."};
            case Sentence.T57:
                return new string[] { "500 dice... That's 91.8% chance\nof 1 hit, 71.3% chance of 2 hits...",
                "500 dados... Eso es una probabilidad\ndel 91.8% de 1 ataque, y del\n71.3% de 2 ataques..."};
            case Sentence.T58:
                return new string[] { "Hold on! You rolled 500 dice, right?\nBut try counting them! Count them!",
                "¡Un momento! Has lanzado 500 dados, ¿verdad?\n¡Pero prueba a contarlos! ¡Cuéntalos!"};
            case Sentence.T59:
                return new string[] { "You already counted them, right?", "Ya los has contado, ¿no?" };
            case Sentence.T60:
                return new string[] { "Oh, wait, you are an imperfect\nhuman, so you might need more\ntime. I'll patiently wait.",
                "Oh, espera, como eres un\nser humano imperfecto, necesitarás\nmás tiempo. Esperaré pacientemente."};
            case Sentence.T61:
                return new string[] { "Okay, that was more than enough time.\nI'll assume you finished counting them.",
                "Vale, eso ha sido tiempo más que suficiente.\nVoy a asumir que terminaste de contarlos." };
            case Sentence.T62:
                return new string[] { "So as you just checked yourself there\nare only 361 dice on the screen,\nbut you supposedly rolled 500!",
                "Como acabas de comprobar personalmente,\nsolo hay 361 dados en pantalla, ¡pero\nse suponía que habías lanzado 500!"};
            case Sentence.T63:
                return new string[] { "What is this nonsense? The creator\npromised us the genuine dice-rolling\nexperience! What's with this arbitrary limit?",
                "¿Pero qué está pasando aquí? ¡Se suponía\nque este juego iba de lanzar dados!\n¿A qué viene este límite arbitrario?"};
            case Sentence.T64:
                return new string[] { "Oh...? Wait, let me check the code...", "¿Oh...? Espera, déjame mirar el código..." };
            case Sentence.T65:
                return new string[] { "Um, it seems like the game still rolls\nthose dice and applies their effects, but\ndoesn't show them due to space limitations.",
                "Eh, parece que el juego calcula\nesos dados y aplica sus efectos, pero\nno los muestra por limitaciones de espacio."};
            case Sentence.T66:
                return new string[] { "Still a cop-out if you ask me,\nbut at least there is no limit to how\nmany dice you can gather and roll.",
                "A mí me parece más una excusa que\notra cosa, pero por lo menos no hay límite\nsobre cuantos dados puedes tener y lanzar."};
            case Sentence.T67:
                return new string[] { "You probably think you've already won.", "Probablemente pienses que ya has ganado." };
            case Sentence.T68:
                return new string[] { "You have a ridiculous amount of dice and health.\nRealistically, no matter what I do, I cannot\ndecrease your health to 0 before you defeat me.",
                "Tienes una cantidad absurda de dados y salud.\nSiendo realistas, haga lo que haga, no puedo\nreducir tu salud a 0 antes de que me derrotes."};
            case Sentence.T69:
                return new string[] { "But I sure hope you have been\npaying attention to this...",
                "Pero espero que le hayas estado\nprestando atención a esto..."};
            case Sentence.T70:
                return new string[] { "That's how many cards\nyour deck has left.",
                "Ese es el número de cartas\nque quedan en tu baraja."};
            case Sentence.T71:
                return new string[] { "Did I fail to mention that\nif you run out of cards in <b>BOTH</b>\nyour hand and your deck, you lose?",
                "¿Acaso se me olvidó mencionar\nque si te quedas sin cartas en\ntu mano <b>Y</b> en tu baraja, pierdes?"};
            case Sentence.T72:
                return new string[] { "You don't have that\nmany left, now do you?",
                "No es que te queden\ndemasiadas, ¿verdad?"};
            case Sentence.T73:
                return new string[] { "So all I have to do is survive\nfor a few more turns and\nyou will lose by default!",
                "¡Así que todo lo que tengo que hacer\nes sobrevivir unos pocos turnos más\ny perderás por defecto!"};
            case Sentence.T74:
                return new string[] { "...but that's not going to happen.\nIt's statistically impossible.",
                "...pero eso no va a suceder.\nEs estadísticamente imposible."};
            case Sentence.T75:
                return new string[] { "You just rolled 1500 dice.\nThat translates to a 94.1%\nchance of delivering 4 hits.",
                "Acabas de lanzar 1500 dados. Eso es\nuna probabilidad del 94.1%\nde atacarme 4 veces."};
            case Sentence.T76:
                return new string[] { "Once you gather a high enough amount of\ndice, winning is not a matter of chance,\nbut an statistical inevitability.",
                "Una vez has reunido un número lo\nsuficientemente alto de dados, ganar no es\ncuestión de suerte, sino una certeza estadística."};
            case Sentence.T77:
                return new string[] { "Should I start getting serious?", "¿Debería empezar a tomármelo en serio?" };
            case Sentence.T78:
                return new string[] { "Ouch.", "Ay." };
            case Sentence.T79:
                return new string[] { "My point exactly.", "Justo lo que te decía." };
            case Sentence.T80:
                return new string[] { "Well, I won't just sit still any longer.\nI'll be attacking from now on.",
                "Bueno, ya no me voy a quedar parado.\nEmpezaré a atacarte de ahora en adelante."};
            case Sentence.T81:
                return new string[] { "No luck this time, uh?\nBut just because you can't attack\ndoesn't mean I'll just sit and watch.",
                "¿Esta vez no ha habido suerte, eh?\nPero que tu no puedas atacar no significa\nque yo me vaya a quedar parado."};
            case Sentence.T82:
                return new string[] { "You know what\nelse is power?\nActual power.",
                "¿Y sabes que también es poder?\nEl poder."};
            case Sentence.T83:
                return new string[] { "That's... a lot of health.", "Eso... es mucha salud." };
            case Sentence.T84:
                return new string[] { "Not sure what's the point of\nattacking you right now.",
                "Ahora mismo no sé de\nque me sirve atacarte."};
            case Sentence.T85:
                return new string[] { "Fine, well done. But would you\nbe so succesful in a real battle?",
                    "De acuerdo, bien hecho, ¿pero lo\nharías igual en un combate real?" };

            //// Battle Dialogue
            case Sentence.D0:
                return new string[] { "Best of luck.", "Buena suerte." };
            case Sentence.D1:
                return new string[] { "Here we go again.", "Allá vamos otra vez." };
            case Sentence.D2:
                return new string[] { "Feeling lucky today?", "¿Te sientes con suerte?" };
            case Sentence.D3:
                return new string[] { "Time to roll again.", "Al lío." };
            case Sentence.D4:
                return new string[] { "Well, get rolling already.", "Bueno, lanza esos dados de una vez." };
            case Sentence.D5:
                return new string[] { "Well here we are again.", "Aquí estamos una vez más." };
            case Sentence.D6:
                return new string[] { "Time to roll...\nliterally.", "Manos a la obra.\nO a los dados, supongo." };
            case Sentence.D7:
                return new string[] { "This will be fun.\nBut mainly for me.", "Será divertido.\nPara mí sobre todo." };
            case Sentence.D8:
                return new string[] { "I did feel like thrashing\na human today, so...",
                "Pues hoy sí que me apetecía\naplastar a un ser humano, así que..."};
            case Sentence.D9:
                return new string[] { "Heads will roll. No, hold on.\nDice. I meant dice.",
                "Van a rodar cabezas. No, espera.\nDados. Quería decir dados."};
            case Sentence.D10:
                return new string[] { "May the best one win.\nThat's me, by the way.",
                "Que gane el mejor.\nQue soy yo, por cierto."};
            case Sentence.D11:
                return new string[] { "Woah, so much choice,\nam I right?", "Vaya, cuantas elecciones,\n¿verdad que sí?" };
            case Sentence.D12:
                return new string[] { "Sure, take your sweet time.\nIt's not like you have literally\none card or anything.",
                "Claro, tú tómate tu tiempo.\nNo es como si tuvieras\nliteralmente una carta ni nada."};
            case Sentence.D13:
                return new string[] { "C'mon, play that already.\nYou can't really do much else.",
                "Venga, juégala de una vez.\nTampoco puedes hacer otra cosa."};
            case Sentence.D14:
                return new string[] { "Not a fan of the illusion\nof choice, uh?",
                    "No te va la ilusión\nde la elección, ¿eh?" };
            case Sentence.D15:
                return new string[] { "Things not looking good\nfor either of us.",
                "Esto no tiene buena pinta.\nPara ninguno de nosotros."};
            case Sentence.D16:
                return new string[] { "This battle might end soon...", "Puede que la batalla acabe pronto..." };
            case Sentence.D17:
                return new string[] { "I wonder who will\nwin this one...", "Me pregunto quién\nganará esta..." };
            case Sentence.D18:
                return new string[] { "We are both up\nagainst the wall, uh?", "Estamos los dos\ncontra la pared, ¿eh?" };
            case Sentence.D19:
                return new string[] { "It won't be long before\nthe winner is decided.",
                "No puede quedar mucho para\nque se decida el ganador."};
            case Sentence.D20:
                return new string[] { "I haven't given up yet.", "Aún no me he rendido." };
            case Sentence.D21:
                return new string[] { "I'm finding it hard to\npredict the winner...",
                "Me está resultando difícil\npredecir el ganador..."};
            case Sentence.D22:
                return new string[] { "This might just be the\nmost critical moment...",
                "Este podría ser el\nmomento más crucial..."};
            case Sentence.D23:
                return new string[] { "It's all or nothing, hominid.", "Es todo o nada, homínido." };
            case Sentence.D24:
                return new string[] { "Keep at it!", "¡No te rindas!" };
            case Sentence.D25:
                return new string[] { "Is that all?", "¿Eso es todo?" };
            case Sentence.D26:
                return new string[] { "Perhaps you should\nconsider healing.", "Igual deberías\nconsiderar curarte." };
            case Sentence.D27:
                return new string[] { "Might I suggest healing\nso you don't have to suffer\na pitiful defeat?",
                "¿Me permites que te sugiera\ncurarte para evitar\nuna derrota lamentable?"};
            case Sentence.D28:
                return new string[] { "You haven't given up yet,\nhave you?",
                "¿Aún no te has rendido, verdad?"};
            case Sentence.D29:
                return new string[] { "Can you turn this\nsituation around?", "¿Puedes darle la\nvuelta a la situación?" };
            case Sentence.D30:
                return new string[] { "Another hit and you're down!", "¡Otro golpe y se acabó!" };
            case Sentence.D31:
                return new string[] { "Your health is sitting\ndangerously close to zero, no?",
                "Tu salud se está acercando\npeligrosamente a cero, ¿no?"};
            case Sentence.D32:
                return new string[] { "So what can you do now?", "¿Qué puedes hacer ahora?" };
            case Sentence.D33:
                return new string[] { "I think this is going to be\nvery fun for exactly one of us.",
                "Creo que esto va a ser muy divertido\npara exactamente uno de nosotros."};
            case Sentence.D34:
                return new string[] { "At least you tried.", "Por lo menos lo intentaste." };
            case Sentence.D35:
                return new string[] { "Nobody can say you didn't try.", "Nadie dirá que no lo has intentado." };
            case Sentence.D36:
                return new string[] { "So can I declare myself\nthe winner already or...?",
                "¿Me puedo declarar\nganador ya o...?"};
            case Sentence.D37:
                return new string[] { "It will be fun seeing you try\nto get yourself out of this one.",
                "Será divertido ver como\nintentas salir de esta."};
            case Sentence.D38:
                return new string[] { "I have this very nice\nred screen that says <i>Defeat</i>\non it. Want to take a look?",
                "Tengo una pantallita roja\nmuy bonita que dice <i>Derrota</i>.\n¿Quieres echarla un vistazo?"};
            case Sentence.D39:
                return new string[] { "I must say, it takes some merit\nto stay at exactly 1 health.",
                "Deja que te diga que tiene\nmérito quedarse con justo 1 de salud."};
            case Sentence.D40:
                return new string[] { "You like living on the edge,\ndon't you?", "¿Te gusta vivir al límite, eh?" };
            case Sentence.D41:
                return new string[] { "I think a soft breeze would\nbe enought to take you down now.",
                "Creo que ahora mismo una suave\nbrisa bastaría para derribarte."};
            case Sentence.D42:
                return new string[] { "We're not done yet.", "Aún no hemos acabado." };
            case Sentence.D43:
                return new string[] { "The battle is not over until\nmy health hits that zero.",
                "La batalla no termina hasta\nque mi salud toque ese cero."};
            case Sentence.D44:
                return new string[] { "Hey, you haven't won just yet.\nBut almost...",
                "Oye, todavía no has ganado.\nPero casi..."};
            case Sentence.D45:
                return new string[] { "<i>(enemy.health != 0)</i>\nOh, I'm still in\nthe fight, then!",
                "<i>(enemy.health != 0)</i>\n¡Oh, entonces aún\npuedo seguir luchando!"};
            case Sentence.D46:
                return new string[] { "Let's see if I can\nturn this situation around.",
                "A ver si puedo darle\nla vuelta a la situación."};
            case Sentence.D47:
                return new string[] { "I don't think I can\nendure much longer...",
                "No creo que pueda\nresistir mucho más..."};
            case Sentence.D48:
                return new string[] { "What can I do here...?", "¿Y ahora qué puedo hacer...?" };
            case Sentence.D49:
                return new string[] { "I don't quite like how\nthis situation is unfolding.",
                "No me termina de gustar\neste giro de los acontecimientos."};
            case Sentence.D50:
                return new string[] { "Not a fan of this turn of\nevents, to be honest.",
                "No soy fan de este giro de los\nacontecimientos, si te digo la verdad."};
            case Sentence.D51:
                return new string[] { "How about we call it a tie?", "¿Y si lo dejamos en empate?" };
            case Sentence.D52:
                return new string[] { "I almost can't see\nmy healthbar anymore...",
                    "Ya casi no puedo ver\nmi barra de salud..." };
            case Sentence.D53:
                return new string[] { "Now, this did not go\nentirely according to the plan.",
                "La verdad, esto no ha ido\nexactamente según el plan."};
            case Sentence.D54:
                return new string[] { "I fear that my estimations\nmight have been just\na little bit off.",
                "Me parece que mis estimaciones\nestaban desviadas muy\nligeramente de la realidad."};
            case Sentence.D55:
                return new string[] { "I suddenly feel like singing\nabout a daisy bell...",
                "De repente me apetece\ncantar sobre una margarita..."};
            case Sentence.D56:
                return new string[] { "Keep at it, hominid!", "¡Sigue así, homínido!" };
            case Sentence.D57:
                return new string[] { "Take your time.", "Tómate tu tiempo." };
            case Sentence.D58:
                return new string[] { "Ponder your choices thoroughly\nso that I can laugh when\nyou pick the less optimal one.",
                "Pondera bien tus decisiones\npara que me pueda reír\ncuando tomes la menos óptima."};
            case Sentence.D59:
                return new string[] { "I wonder what will happen?", "¿Me pregunto qué sucederá?" };
            case Sentence.D60:
                return new string[] { "Let's see what that deck can do.", "Veamos que puede hacer esa baraja." };
            case Sentence.D61:
                return new string[] { "C'mon, choose a card.", "Vamos, elige una carta." };
            case Sentence.D62:
                return new string[] { "Your move, hominid.", "Tu turno, homínido." };
            case Sentence.D63:
                return new string[] { "What next?", "¿Y ahora qué?" };
            case Sentence.D64:
                return new string[] { "Well, hominid?", "¿Y bien, homínido?" };
            case Sentence.D65:
                return new string[] { "What will you do?", "¿Qué vas a hacer?" };
            case Sentence.D66:
                return new string[] { "You won't be getting far\nif you don't get some more dice.",
                "No llegarás lejos si\nno consigues más dados."};
            case Sentence.D67:
                return new string[] { "Maybe you need more dice?\nJust a thought.",
                "¿Igual necesitas más dados?\nYo solo lo digo."};
            case Sentence.D68:
                return new string[] { "This is hardly a challenge.\nRoll more dice, hominid.",
                "Esto no es desafío alguno.\nLanza más dados, homínido."};
            case Sentence.D69:
                return new string[] { "My attacks will hit harder\nthanks to my Power Glow weapon.",
                "Mis ataques pegarán más fuerte\ngracias a mi Fuerza Brillante"};
            case Sentence.D70:
                return new string[] { "Who needs legs\nwith arms like these?",
                "¿Quién necesita piernas\ncon brazos como estos?" };
            case Sentence.D71:
                return new string[] { "I might decide to attack\nyour deck with my Buzzsaw\none of these turns.",
                "Puede que decida atacar tu\nbaraja con mi Sierra\nuno de estos turnos."};
            case Sentence.D72:
                return new string[] { "Should I attack you or your\ndeck? Decisions, decisions...",
                "¿Debería atacarte a ti o a\ntu baraja? Decisiones, decisiones..."};
            case Sentence.D73:
                return new string[] { "You won't take me down\nso easily thanks to my Shield.",
                "No me derrotarás tan\nfácilmente gracias a mi Escudo."};
            case Sentence.D74:
                return new string[] { "The extra health my Shield\nprovides really comes in handy.",
                "La salud adicional que me otorga\nmi Escudo resulta bastante útil."};
            case Sentence.D75:
                return new string[] { "Should I attack you or your\nhand? Decisions, decisions...",
                "¿Debería atacarte a ti o a\ntu mano? Decisiones, decisiones..."};
            case Sentence.D76:
                return new string[] { "You have way too many cards in\nthat hand. I might have to put\nmy Fan to good use...",
                "Tienes demasiadas cartas en\nesa mano. Quizá deba recurrir\na mi Ventilador..."};
            case Sentence.D77:
                return new string[] { "I might need to actually\nheal at some point...",
                "Quizá debería empezar a\nconsiderar curarme..."};
            case Sentence.D78:
                return new string[] { "Would this be a good time to use\nthe Forbidden Relic or not?",
                "¿Sería este buen momento para usar\nla Reliquia Prohibida o no?"};
            case Sentence.D79:
                return new string[] { "Maybe I should consider\ndraining your health...",
                "Quizá debería considerar\nabsorber tu salud..."};
            case Sentence.D80:
                return new string[] { "Thanks to my Miasma, your\nhealing won't be nearly as\neffective as usual!",
                "¡Gracias a mi Miasma, tus cartas\ncurativas serán menos\neficaces que de costumbre!"};
            case Sentence.D81:
                return new string[] { "Isn't my Miasma annoying?\nYou're gonna have a hard time\nhealing that damage back.",
                "¿A que mi Miasma es insoportable?\nTe va a costar recuperar\nese daño."};
            case Sentence.D82:
                return new string[] { "My Aegis will protect me\nfrom low damage!",
                "¡Mi Égida me protegerá\ndel daño leve!"};
            case Sentence.D83:
                return new string[] { "Thanks to my Aegis, a single\nsuccessful roll won't cut it!",
                "¡Gracias a mi Égida, una sola\ntirada exitosa no bastará!"};
            case Sentence.D84:
                return new string[] { "I'm wondering when should I\ncurse you. Halving your dice\neven for a turn is useful.",
                "Me pregunto cuando debería maldecirte.\nReducir tus dados a la mitad por\nun turno resulta muy útil."};
            case Sentence.D85:
                return new string[] { "You are supposed to keep\nsome cards in your hand.",
                "Se supone que tienes que intentar\nquedarte alguna carta en mano."};
            case Sentence.D86:
                return new string[] { "I'm not seeing any cards\nyou can play.",
                "No veo ninguna carta\nque puedas jugar."};
            case Sentence.D87:
                return new string[] { "Where did your cards go?", "¿A dónde se fueron tus cartas?" };
            case Sentence.D88:
                return new string[] { "You are supposed to bring\ncards you can play.",
                "Se supone que tienes que\ntraer cartas que puedas jugar."};
            case Sentence.D89:
                return new string[] { "Those cards are too costly.", "Esas cartas cuestan demasiado." };
            case Sentence.D90:
                return new string[] { "Drew into all the\ncostly cards, uh?", "¿Has robado todas las\ncartas costosas, eh?" };
            case Sentence.D91:
                return new string[] { "Not bad at all...", "No está nada mal..." };
            case Sentence.D92:
                return new string[] { "Ouch!", "¡Ay!" };
            case Sentence.D93:
                return new string[] { "What!?", "¿¡Qué!?" };
            case Sentence.D94:
                return new string[] { "Wait, did you just...?", "Espera, ¿acabas de...?" };
            case Sentence.D95:
                return new string[] { "No way I can survive that...", "Es imposible que sobreviva eso..." };
            case Sentence.D96:
                return new string[] { "?!?", "¿¡¿?!?" };
            case Sentence.D97:
                return new string[] { "What in Turing's name...?", "En nombre de Turing,\n¿que está...?" };
            case Sentence.D98:
                return new string[] { "C-Chill out now!", "¡C-Cálmate un poquito!" };
            case Sentence.D99:
                return new string[] { "What in the world...?", "¿Pero qué...?" };
            case Sentence.D100:
                return new string[] { "Thanks to the Aegis,\nI barely felt that.",
                "Gracias a la Égida,\nni lo he notado."};
            case Sentence.D101:
                return new string[] { "That's not really\ngonna be enough." , "Con eso no va a bastar."};
            case Sentence.D102:
                return new string[] { "That's not gonna cut it\nwhile I have this shield.",
                "Eso no será suficiente\nmientras tenga este escudo."};
            case Sentence.D103:
                return new string[] { "You'll need to do\nmore damage if you want\nto pierce the Aegis.",
                "Tendrás que hacer más\ndaño si quieres\natravesar la Égida."};
            case Sentence.D104:
                return new string[] { "0 damage!\nThat's the Aegis for you.",
                "¡0 de daño!\nAsí es la Égida."};
            case Sentence.D105:
                return new string[] { "Literally not even a dent.", "Literalmente ni un rasguño." };
            case Sentence.D106:
                return new string[] { "This but a scratch!", "¡Apenas un rasguño!" };
            case Sentence.D107:
                return new string[] { "Wow, 1 damage.\nYou must be proud.", "Vaya, 1 de daño. Como\npara estar orgulloso." };
            case Sentence.D108:
                return new string[] { "I barely felt that.", "Casi ni lo he sentido." };
            case Sentence.D109:
                return new string[] { "That's not really\ngonna be enough.", "Eso no bastará." };
            case Sentence.D110:
                return new string[] { "Hey, at least you\ngave it your best.", "Oye, por lo menos\nte has esforzado." };
            case Sentence.D111:
                return new string[] { "Next time try\nrolling more dice.", "La próxima vez prueba\na lanzar más dados." };
            case Sentence.D112:
                return new string[] { "You can stop fooling\naround now.", "Cuando quieras puedes\nempezar a jugar en serio." };
            case Sentence.D113:
                return new string[] { "You were holding\nback, right...?", "Te estabas conteniendo, ¿verdad...?" };
            case Sentence.D114:
                return new string[] { "Next time try attacking\nlike you actually mean it.",
                "La próxima vez prueba a atacar\ncomo si lo hicieras a drede."};
            case Sentence.D115:
                return new string[] { "Not the most\ncromulent of attacks.", "No es que haya sido\nel ataque más cromulento." };
            case Sentence.D116:
                return new string[] { "Would you feel better if\nI pretended like that hurt?",
                "¿Te sentirías mejor si\npretendiera que me ha dolido?"};
            case Sentence.D117:
                return new string[] { "Not bad, I guess...", "No está mal, supongo..." };
            case Sentence.D118:
                return new string[] { "I'm a robot,\nI can't even feel pain.", "Soy un robot,\nni siquiera siento dolor." };
            case Sentence.D119:
                return new string[] { "You got lucky this time.", "Esta vez tuviste suerte." };
            case Sentence.D120:
                return new string[] { "Not too shabby.", "Nada mal." };
            case Sentence.D121:
                return new string[] { "I've dealt with worse.", "He pasado por cosas peores." };
            case Sentence.D122:
                return new string[] { "That's not gonna be enough.", "Con eso no va a bastar." };
            case Sentence.D123:
                return new string[] { "The only health point\nthat matters is the last one.",
                "El único punto de salud\nque importa es el último."};
            case Sentence.D124:
                return new string[] { "Can you not attack?\nThat's not very conducive\nto me winning.",
                "¿Puedes no atacar?\nEso no contribuye en\nnada a mi victoria."};
            case Sentence.D125:
                return new string[] { "Like, maybe chill out?", "¿Y si te calmas o algo?" };
            case Sentence.D126:
                return new string[] { "Quick checksum...\nYes, I'm fine.", "Checksum rápido...\nSí, estoy bien." };
            case Sentence.D127:
                return new string[] { "I'm not even going to\ndo anything this turn.",
                "Ni siquiera voy a\nhacer nada este turno."};
            case Sentence.D128:
                return new string[] { "I feel like resting\nfor a second.", "Me apetece descansar\nun segundo." };
            case Sentence.D129:
                return new string[] { "I'll give you a free turn.\nI'm that nice.",
                "Te doy un turno gratis.\nSoy así de majo."};
            case Sentence.D130:
                return new string[] { "I'll wait a bit until the battle\nstarts getting interesting...",
                "Voy a esperar un poco\na que la batalla\nse ponga interesante..."};
            case Sentence.D131:
                return new string[] { "Are you actually going to\ndo anything or...?",
                "¿Entonces vas a\nhacer algo o...?"};
            case Sentence.D132:
                return new string[] { "I can afford to give you\na free turn.",
                "Me puedo permitir\ndarte un turno gratis."};
            case Sentence.D133:
                return new string[] { "Kinda getting bored here.", "Me estoy empezando a aburrir." };
            case Sentence.D134:
                return new string[] { "Okay, now try giving me\nsome trouble for a change, yes?",
                "Vale, y ahora intenta darme\nproblemas para variar, ¿quieres?"};
            case Sentence.D135:
                return new string[] { "I need a moment to catch my breath...",
                    "Necesito un momento para\nrecuperar el aliento..." };
            case Sentence.D136:
                return new string[] { "Not giving me an easy time, are you?", "No te gusta ponérmelo fácil, ¿eh?" };
            case Sentence.D137:
                return new string[] { "I'm getting a little tired...", "Me estoy empezando a cansar..." };
            case Sentence.D138:
                return new string[] { "Doing what I can here...", "Estoy haciendo lo que puedo..." };
            case Sentence.D139:
                return new string[] { "I need to cool down my kernel...", "Necesito enfriar mi núcleo..." };
            case Sentence.D140:
                return new string[] { "I think you might have\ngone a little overboard with\nwith all those shields...",
                "Igual te has pasado\ncon tanto escudo..."};
            case Sentence.D141:
                return new string[] { "I'm feeling a little\nridiculous right now.",
                "Ahora mismo me siento\nun poco ridículo." };
            case Sentence.D142:
                return new string[] { "Not my best hit, I'll admit.", "No ha sido mi\nmejor golpe, la verdad." };
            case Sentence.D143:
                return new string[] { "I'm the one supposed to have\nsteel skin here, hominid.",
                "Se supone que el que tiene\nla piel de hierro\nsoy yo, homínido."};
            case Sentence.D144:
                return new string[] { "I might need to recalibrate...", "Quizá necesite recalibrar..." };
            case Sentence.D145:
                return new string[] { "You being actually\ninvincible was a possibilty\nI failed to anticipate.",
                "Que fueras literalmente\ninvencible es una posibilidad\nque no llegué a considerar."};
            case Sentence.D146:
                return new string[] { "Sure, make this literally\nimpossible for me. That's fun.",
                "Claro, haz que sea\nliteralmente imposible\npara mi. Muy divertido." };
            case Sentence.D147:
                return new string[] { "Here comes the finishing blow!", "¡Aquí llega el golpe de gracia!" };
            case Sentence.D148:
                return new string[] { "It's over!", "¡Se acabó!" };
            case Sentence.D149:
                return new string[] { "Here comes sweet victory!", "¡Dulce victoria!" };
            case Sentence.D150:
                return new string[] { "Time to say goodbye!", "¡Hora de decir adiós!" };
            case Sentence.D151:
                return new string[] { "Time for the curtain to fall!", "¡Es hora de que caiga el telón!" };
            case Sentence.D152:
                return new string[] { "The double damage is\ntoo much for you, uh?",
                "El doble daño es\ndemasiado para ti, ¿eh?"};
            case Sentence.D153:
                return new string[] { "You cannot possibly survive\nthis reinforced hit!",
                "¡Es imposible que sobrevivas\na este golpe reforzado!"};
            case Sentence.D154:
                return new string[] { "I'm not going anywhere\nlike this...", "Así no voy\na ninguna parte..." };
            case Sentence.D155:
                return new string[] { "C'mon, it's not fair!\nYou have too much health!",
                "¡Venga, no es justo!\n¡Tienes demasiada salud!"};
            case Sentence.D156:
                return new string[] { "It's gonna take me forever\nto take you down like this...",
                "Me va a llevar una\neternidad vencerte así..."};
            case Sentence.D157:
                return new string[] { "I would win too,\nif I had that much health.",
                "Yo también ganaría\nsi tuviera tanta salud."};
            case Sentence.D158:
                return new string[] { "Do you even need\nthat much health?",
                "¿De verdad te hace\nfalta tanta salud?"};
            case Sentence.D159:
                return new string[] { "Oh, c'mon, who\nbalanced this game?",
                "¿Pero quién se ha encargado\ndel balance de este juego?"};
            case Sentence.D160:
                return new string[] { "Slow and steady wins the race.\nBut maybe not this slow...",
                "Sin prisa pero sin pausa.\nPero algo más de prisa\ntampoco me haría daño..." };
            case Sentence.D161:
                return new string[] { "That can't even\nbe called damage...",
                "A eso ni se lo\npuede llamar daño."};
            case Sentence.D162:
                return new string[] { "You can't say I'm not trying.", "No puedes decir que\nno lo estoy intentando." };
            case Sentence.D163:
                return new string[] { "I might have gone too\far with that one...",
                "Igual he ido demasiado\nlejos con eso..."};
            case Sentence.D164:
                return new string[] { "That left me dangerously\nclose to defeat...",
                "Eso me ha dejado\npeligrosamente cerca\nde la derrota..."};
            case Sentence.D165:
                return new string[] { "Not too sure if that\nwas a good idea...", "No sé yo si eso\nha sido buena idea..." };
            case Sentence.D166:
                return new string[] { "Who needs health, anyway?\nMe, I guess...",
                "¿Y quién necesita salud?\nYo, supongo..."};
            case Sentence.D167:
                return new string[] { "All is fine as long as\nI have at least one point\nof health left.",
                "Todo está bien mientras\nme quede al menos\nun punto de salud."};
            case Sentence.D168:
                return new string[] { "Who needs health, anyway?", "¿Y quién necesita salud?" };
            case Sentence.D169:
                return new string[] { "Health is overrated,\nlet me tell you.",
                    "La salud está sobrevalorada,\ndeja que te diga." };
            case Sentence.D170:
                return new string[] { "Dealing double damage is\nstupid fun, let me tell you.",
                "Deja que te diga, hacer\nel doble de daño es\ndivertido que no veas."};
            case Sentence.D171:
                return new string[] { "I could get used to\nthis kind of power.",
                "Me podría acostumbrar a\neste tipo de poder."};
            case Sentence.D172:
                return new string[] { "You will have to\ndo a lot better if\nyou want to survive.",
                "Vas a tener que\nhacerlo mucho mejor si\nquieres sobrevivir."};
            case Sentence.D173:
                return new string[] { "Take that!", "¡Toma esa!" };
            case Sentence.D174:
                return new string[] { "Look at that health go down!", "¡Mira como baja esa salud!" };
            case Sentence.D175:
                return new string[] { "If only there was\na dodge button, right?",
                "Si tan solo hubiera un\nbotón de esquivar, ¿verdad?"};
            case Sentence.D176:
                return new string[] { "Makes me wonder if\nyou're even trying.",
                "Me pregunto si siquiera\nlo estás intentando."};
            case Sentence.D177:
                return new string[] { "Here we go!", "¡Allá vamos!" };
            case Sentence.D178:
                return new string[] { "Not so lucky now, are we?", "Ya no tenemos\ntanta suerte, ¿eh?" };
            case Sentence.D179:
                return new string[] { "One more step towards victory!",
                "¡Un paso más hacia la victoria!"};
            case Sentence.D180:
                return new string[] { "I've seen malfunctioning\ntoasters with better\nreflexes than you.",
                "He visto tostadoras\naveriadas con mejores\nreflejos que tú."};
            case Sentence.D181:
                return new string[] { "That must have hurt.", "Eso tiene que haber dolido." };
            case Sentence.D182:
                return new string[] { "It's like you don't\neven want to win.", "Es como si no quisieras ganar." };
            case Sentence.D183:
                return new string[] { "You didn't need\nthat health anyway.",
                "Tampoco es que\nnecesitaras esa salud."};
            case Sentence.D184:
                return new string[] { "I'm sure you can\nlive through that.",
                "Seguro que eso\nlo puedes resistir."};
            case Sentence.D185:
                return new string[] { "It's you or me, buddy.", "O tú o yo, camarada." };
            case Sentence.D186:
                return new string[] { "Neither of us is looking\nall that good, right now...",
                "Ahora mismo ninguno de nosotros\tiene el mejor de los aspectos..."};
            case Sentence.D187:
                return new string[] { "This might end soon...\nfor either of us...",
                "Puede que esto acabe pronto...\npara cualquiera de nosotros..."};
            case Sentence.D188:
                return new string[] { "You have too many dice.", "Tienes demasiados dados." };
            case Sentence.D189:
                return new string[] { "Do you really need\nthat many dice?", "¿De verdad necesitas\ntantos dados?" };
            case Sentence.D190:
                return new string[] { "Rolling too many dice would\nmake it hard for me to win,\nso let's not do that.",
                "Lanzar demasiados dados haría\nque me fuera más difícil ganar,\nasí que vamos a evitar eso."};
            case Sentence.D191:
                return new string[] { "See? I can make\nthe dice roll too.", "¿Has visto? Yo también\nsé tirar dados." };
            case Sentence.D192:
                return new string[] { "What kind of combat resource\nis dice, anyway?",
                "¿Pero que clase de recurso\nde combate es 'dados',\nsi puede saberse?"};
            case Sentence.D193:
                return new string[] { "Look at all the little dice go!", "¡Mira como se van\ntodos esos dados!" };
            case Sentence.D194:
                return new string[] { "And that was the last one!", "¡Y esa era la última!" };
            case Sentence.D195:
                return new string[] { "No more cards for me\nto worry about!",
                    "¡Ya no quedan más cartas\nde las que preocuparme!" };
            case Sentence.D196:
                return new string[] { "Because who needs a deck\nwith actual cards?",
                "¿Porque quién necesita una\nbaraja que tenga cartas?"};
            case Sentence.D197:
                return new string[] { "You probably didn't need\nthat card anyway.",
                "Probablemente esa carta\nni te hacía falta."};
            case Sentence.D198:
                return new string[] { "Oops! Here goes your best card!", "¡Huy! ¡Ahí va tu mejor carta!" };
            case Sentence.D199:
                return new string[] { "That's one less turn\nI need to survive to win.",
                "Un turno menos que tengo\nque sobrevivir para ganar."};
            case Sentence.D200:
                return new string[] { "Don't complain. I'm playing\nwithout any cards myself.",
                "No te quejes. Yo estoy\njugando sin cartas."};
            case Sentence.D201:
                return new string[] { "I'm helping you out.\nThat deck seemed unwieldy.",
                "Te estoy ayudando. Esa\nbaraja tan grande debía\nser difícil de manejar."};
            case Sentence.D202:
                return new string[] { "Who needs that many cards?",
                "¿Pero quién necesita tanta carta?"};
            case Sentence.D203:
                return new string[] { "Left you handless!", "¡Te dejé sin cartas!" };
            case Sentence.D204:
                return new string[] { "That was the last one\nin your hand!",
                "¡Esa era la última\nde tu mano!"};
            case Sentence.D205:
                return new string[] { "Now you have an empty hand!", "¡Ahora tu mano está vacía!" };
            case Sentence.D206:
                return new string[] { "Because choice is overrated.", "Tenías demasiadas elecciones." };
            case Sentence.D207:
                return new string[] { "Now it will be easier\nto hold them!",
                "¡Ahora te será más\nfácil sujetarlas!"};
            case Sentence.D208:
                return new string[] { "I'm saving you effort of\nhaving to play that one.",
                "Te voy a evitar el esfuerzo\nde tener que jugar esa."};
            case Sentence.D209:
                return new string[] { "I didn't like the way\nthat card was looking at me.",
                "No me gustaba la forma con\nla que me miraba esa carta."};
            case Sentence.D210:
                return new string[] { "I can't really heal\nif you don't cooperate.",
                "No me puedo curar\nsi no cooperas."};
            case Sentence.D211:
                return new string[] { "Did I just win with\nthe drain attack?",
                "¿Acabo de ganar con\nel ataque de drenaje?"};
            case Sentence.D212:
                return new string[] { "I just defeated you with\nliterally my weakest move.",
                "Te acabo de vencer con\nel que es literalmente\nmi movimiento más débil."};
            case Sentence.D213:
                return new string[] { "Looks like I won't be\nneeding that health after all.",
                "Parece que no necesitaré esa\nsalud extra después de todo."};
            case Sentence.D214:
                return new string[] { "Back to full health!", "¡Salud al máximo!" };
            case Sentence.D215:
                return new string[] { "It's as if nothing happened!", "¡Es como si no\nhubiera pasado nada!" };
            case Sentence.D216:
                return new string[] { "You attacked me for naught.", "Me has atacado para nada." };
            case Sentence.D217:
                return new string[] { "Let's pretend you haven't\nattacked me yet.",
                "Vamos a hacer como si aún\nno me hubieras atacado."};
            case Sentence.D218:
                return new string[] { "If anyone asks, you never\nattacked me, okay?",
                "Si alguien pregunta, no me\nhas llegado a atacar, ¿vale?"};
            case Sentence.D219:
                return new string[] { "Look at that healthbar go\nall the way to the end!",
                "¡Mira como esa barra de\nsalud se llena al máximo!"};
            case Sentence.D220:
                return new string[] { "I'm gonna have to buy myself\na bigger healthbar.",
                "Me voy a tener que comprar\nuna barra de salud más grande."};
            case Sentence.D221:
                return new string[] { "I can easily outheal\nthat little damage!",
                "¡Me resulta fácil curarme\nese daño tan bajo!"};
            case Sentence.D222:
                return new string[] { "It's not fair that you can\nheal past your initial health\nbut I can't...",
                "No es justo que tu puedas\ncurarte más allá de tu\nsalud inicial y yo no..."};
            case Sentence.D223:
                return new string[] { "Aren't I an impish devil?", "¿A que estoy hecho un diablillo?" };
            case Sentence.D224:
                return new string[] { "You're not the only one\nwith annoying healing powers.",
                "Por si acaso pensabas que\nsolo tu tenías molestos\npoderes curativos."};
            case Sentence.D225:
                return new string[] { "Watch my healthbar grow!", "¡Observa como sube mi salud!" };
            case Sentence.D226:
                return new string[] { "This should hold you back\nfor a little while...",
                "Esto te retendrá\ndurante un tiempo..."};
            case Sentence.D227:
                return new string[] { "Let's hope this is enough.", "Esperemos que sea suficiente." };
            case Sentence.D228:
                return new string[] { "Some more health\nwould do me good.", "Más salud me vendría bien." };
            case Sentence.D229:
                return new string[] { "My next attacks\nwill hit harder!", "¡Mis próximos ataques\npegarán más fuerte!" };
            case Sentence.D230:
                return new string[] { "Slow and steady\nwins the race..." , "Sin prisa pero sin pausa..." };
            case Sentence.D231:
                return new string[] { "It's not like I even need\nthis much power, but you know.",
                "Ni siquiera necesito\ntanto poder, pero en fin."};
            case Sentence.D232:
                return new string[] { "Now that's quite\nthe electron rush!", "¡Menudo empujón de electrones!" };
            case Sentence.D233:
                return new string[] { "A very literal power surge!", "¡Una muy literal\nsubida de potencia!" };
            case Sentence.D234:
                return new string[] { "Stat buffs!\nWho doesn't love them?",
                "¡Aumentos de estadísticas!\n¿Y a quién no le gustan?"};
            case Sentence.D235:
                return new string[] { "You know, I don't think\nthis is going to make much\nof a difference...",
                "Sabes, no creo que\nesto vaya a significar\nuna gran diferencia..."};
            case Sentence.D236:
                return new string[] { "Given your ridiculous health\nI don't think this is\ngonna be enough...",
                "Dada tu absurda salud, no\ncreo que esto vaya a bastar..."};
            case Sentence.D237:
                return new string[] { "Nope, I still can't outpower\nyour absurd healing.",
                "Nada, aún no puedo\nhacerte daño más rápido\nde lo que te curas."};
            case Sentence.D238:
                return new string[] { "This will make it harder\nfor you to gather dice.",
                "Esto hará que te sea\nmás difícil reunir dados."};
            case Sentence.D239:
                return new string[] { "It's easier to stop you\nwhen you don't have\nas many dice.",
                "Es más fácil detenerte\nsi no tienes tantos dados."};
            case Sentence.D240:
                return new string[] { "Let's see you try to increase\nyour number of dice like this.",
                "A ver como aumentas\ntu número de dados así."};
            case Sentence.D241:
                return new string[] { "Nothing worse than a slow start!",
                "¡Nada peor que un inicio lento!"};
            case Sentence.D242:
                return new string[] { "Let's see if you can\novercome this hurdle.",
                "Veamos si puedes\nsuperar este obstáculo."};
            case Sentence.D243:
                return new string[] { "You have way too many dice.", "Tienes demasiados dados." };
            case Sentence.D244:
                return new string[] { "Let's keep that dice number\nreasonable, shall we?",
                "Tengamos un número\nde dados razonable, ¿sí?"};
            case Sentence.D245:
                return new string[] { "If you roll that many dice\nyou might win,\nso let's avoid that.",
                "Si lanzas tantos dados\npodrías ganar, así\nque evitemos eso."};
            case Sentence.D246:
                return new string[] { "You probably don't even care\nthat I'm cursing you\nat this point.",
                "Probablemente ni te\nimporta que te maldiga,\nllegados a este punto."};
            case Sentence.D247:
                return new string[] { "You're lucky this only\nlasts one turn.",
                "Tienes suerte que esto\ndure solo un turno."};
            case Sentence.D248:
                return new string[] { "Halving your dice might\nhelp me last longer.",
                "Puede que reducir\ntus dados me ayude a\nsobrevivir más tiempo."};
            case Sentence.D249:
                return new string[] { "Because who needs to roll\nthat many dice?",
                "¿Porque quién necesita\nlanzar tantos dados?"};
            case Sentence.D250:
                return new string[] { "I'm gonna curse you.\nCurse you, mortal!\nDone.",
                "Voy a maldecir.\n¡Maldita sea!\nHecho."};
            case Sentence.D251:
                return new string[] { "If only this curse lasted\nmore than a turn...",
                "Si tan solo esta maldición\ndurara más de un turno..."};
            case Sentence.D252:
                return new string[] { "I was so close...", "Estaba tan cerca..." };
            case Sentence.D253:
                return new string[] { "Ugh, that was a\nvery even match.", "Ugh, ha estado\nmuy igualado." };
            case Sentence.D254:
                return new string[] { "Fine. Fair and square.", "De acuerdo.\nUna victoria justa." };
            case Sentence.D255:
                return new string[] { "You did well, I'll admit.", "Admito que lo\nhas hecho bien." };
            case Sentence.D256:
                return new string[] { "Just one more turn\nand I could have won...",
                "Solo un turno más\ny podría haber ganado..."};
            case Sentence.D257:
                return new string[] { "You fought well.", "Has luchado bien." };
            case Sentence.D258:
                return new string[] { "You got lucky this time...", "Esta vez has tenido suerte..." };
            case Sentence.D259:
                return new string[] { "Don't get used to\nthis kind of result.",
                "No te acostumbres a\neste tipo de resultado."};
            case Sentence.D260:
                return new string[] { "Won't happen again...", "No volverá a suceder..." };
            case Sentence.D261:
                return new string[] { "Fine, you win this one...", "Vale, tú ganas esta..." };
            case Sentence.D262:
                return new string[] { "I'm not even mad.", "No estoy ni enfadado." };
            case Sentence.D263:
                return new string[] { "I don't even care.", "Ni me importa." };
            case Sentence.D264:
                return new string[] { "Okay, but don't get\ntoo cocky now...", "Vale, pero tampoco te emociones." };
            case Sentence.D265:
                return new string[] { "At least I had some fun...", "Al menos ha\nsido entretenido..." };
            case Sentence.D266:
                return new string[] { "Okay, fine.\nHere's your prize.", "Bueno, vale.\nAquí tu premio." };
            case Sentence.D267:
                return new string[] { "That did not quite go\naccording to the plan.",
                "Eso no ha salido\nexactamente según el plan."};
            case Sentence.D268:
                return new string[] { "The only weak point in my\nstrategy was that I actually\nforgot to follow it at all.",
                "El único punto débil\nen mi estrategia es que\nse me olvidó seguirla."};
            case Sentence.D269:
                return new string[] { "That could have\ngone either way.", "Podría haber ganado cualquiera." };
            case Sentence.D270:
                return new string[] { "For a moment there I thought\nI was going to lose.",
                "Por un momento pensé\nque iba a perder."};
            case Sentence.D271:
                return new string[] { "That was really close...", "Por muy poco..." };
            case Sentence.D272:
                return new string[] { "Thank goodness...\nI wasn't sure I could\nendure another turn.",
                "Menos mal...\nNo sabía si iba a\npoder aguantar otro turno."};
            case Sentence.D273:
                return new string[] { "Ugh, another turn and victory\nmight have been yours.",
                "Ugh, otro turno y la victoria\npodría haber sido tuya."};
            case Sentence.D274:
                return new string[] { "Hey, at least you tried.", "Oye, por lo menos\nlo has intentado." };
            case Sentence.D275:
                return new string[] { "Could have gone worse.", "Podría haber sido peor." };
            case Sentence.D276:
                return new string[] { "I got a little lucky, I guess.", "He tenido un poco\nde suerte, supongo." };
            case Sentence.D277:
                return new string[] { "That was improvable.", "Eso ha sido mejorable." };
            case Sentence.D278:
                return new string[] { "As far as attempts go,\nthat was average.",
                "Como intento, ha sido pasable."};
            case Sentence.D279:
                return new string[] { "That worked out well.", "Me ha salido bien." };
            case Sentence.D280:
                return new string[] { "That was mediocre at best.", "Eso como mucho\nha sido mediocre." };
            case Sentence.D281:
                return new string[] { "Next time try playing\nas if you wanted to win.",
                "La próxima vez prueba a\njugar como si quisieras ganar."};
            case Sentence.D282:
                return new string[] { "That wasn't your best, was it?", "No ha sido tu\nmejor intento, ¿verdad?" };
            case Sentence.D283:
                return new string[] { "Did you have that\nmuch bad luck?", "¿Tan mala suerte has tenido?" };
            case Sentence.D284:
                return new string[] { "I can't even tell what\nyou were trying to do.",
                "Ni siquiera entiendo\nque intentabas hacer."};
            case Sentence.D285:
                return new string[] { "My healthbar isn't full,\nso I guess that wasn't\ncompletely horrible.",
                "Mi salud no está al máximo,\nasí que supongo que no ha\nsido completamente horrible."};
            case Sentence.D286:
                return new string[] { "Had me worried for a second...", "Me había preocupado\npor un momento..." };
            case Sentence.D287:
                return new string[] { "Subpar, but not the worst.",
                    "Insuficiente, pero podría\nhaber sido peor" };
            case Sentence.D288:
                return new string[] { "Decked out much?", "¿Se te acabó la baraja?" };
            case Sentence.D289:
                return new string[] { "Can't really play if\nyou have no cards.",
                "Sin cartas poco\nvas a poder jugar."};
            case Sentence.D290:
                return new string[] { "Now where did your deck go?", "¿A dónde a ido tu baraja?" };
            case Sentence.D291:
                return new string[] { "Those cards are too good.\nLet's solve that.", "Esas cartas son demasiado buenas.\nVamos a resolver eso." };
            case Sentence.D292:
                return new string[] { "I'm not liking those cards.\nThese are better, trust me.", "No me gustan esas cartas.\nEstas son mejores, créeme." };
            case Sentence.D293:
                return new string[] { "Wanna trade cards?", "¿Quieres intercambiar cartas?" };
            case Sentence.D294:
                return new string[] { "I would win, too,\nwith cards that good.", "Yo también ganaría,\ncon cartas así de buenas." };

            //// Reroll
            case Sentence.REROLL_UPPERCASE:
                return new string[] { "REROLL", "RELANZAR" };
            case Sentence.REROLL_EXPLANATION_INTRO:
                return new string[] { "Having trouble progressing? If you ever do, you might consider rerolling the die of fate to start anew.\n\n" +
                    "You will go back to <b>square one</b>, leaving behind your <b>cards</b>, your <b>fortunes</b> and your <b>experience</b>. In exchange, Lady Luck will smile on you. The higher your rank, the greater your reward. You will receive:\n\n",

                    "¿Te cuesta progresar? Si es así, quizá quieras considerar volver a lanzar el dado del destino para empezar de cero.\n\n" +
                    "Volverás a la <b>casilla de inicio</b>, dejando atrás tus <b>cartas</b>, tus <b>fortunas</b> y tu <b>experiencia</b>. A cambio, la suerte te sonreirá. Cuanto mayor sea to rango, mayor tu recompensa. Recibirás:\n\n"};
            case Sentence.REROLL_EXPLANATION_END_CLICK:
                return new string[] { "<i>(If you agree, click the \"Reroll\" button 5 times)</i>",
                    "<i>(Si estás de acuerdo, haz click en el botón \"Relanzar\" 5 veces)</i>"};
            case Sentence.REROLL_EXPLANATION_END_TAP:
                return new string[] { "<i>(If you agree, tap the \"Reroll\" button 5 times)</i>" ,
                    "<i>(Si estás de acuerdo, toca en el botón \"Relanzar\" 5 veces)</i>"};
            case Sentence.REROLL_CARDS:
                return new string[] { "Card level", "Nivel de cartas" };
            case Sentence.REROLL_EXP:
                return new string[] { "Global EXP", "EXP global" };
            case Sentence.REROLL_SKIP:
                return new string[] { "Enemy skip", "Saltar enemigos" };

            //// Update
            case Sentence.NEW_VERSION_AVAILABLE:
                return new string[] { "There is a new version available", "Hay una nueva versión disponible" };
            case Sentence.DOWNLOAD_LINK:
                return new string[] { "Download link", "Enlace de descarga" };

            default:
                return new string[] { "???", "???" };
        }
    }

}
