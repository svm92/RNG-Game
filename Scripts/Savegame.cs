using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Savegame {

    public static void save()
    {
        HubControl.isAboutToSave = true;

        BinaryFormatter bf = new BinaryFormatter();
        string savegamePath = Application.persistentDataPath + "/RandomNumberGodSavefile.dat";
        FileStream file = File.Create(savegamePath);
        
        SavegameData save = new SavegameData(Player.saveVersion, Player.keyboardModeOn, Player.deck, Player.collection, 
            HubControl.maxUnlockedDifficultyId, HubControl.currentDifficultyId, Player.experience,
            FortuneManager.allFortunes, Player.stats, AudioManager.globlalMute, Dice.skipAnim, BattleManager.skipEnemyDialogue,
            Player.checkForUpdatesOnStart, TextScript.language, Player.rerollPoints);

        bf.Serialize(file, save);
        file.Close();
    }

    public static void load()
    {
        string savegamePath = Application.persistentDataPath + "/RandomNumberGodSavefile.dat";
        if (File.Exists(savegamePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savegamePath, FileMode.Open);
            SavegameData save = (SavegameData)bf.Deserialize(file);
            file.Close();

            applySavegameDataToPlayer(save);
        }
    }

    static void applySavegameDataToPlayer(SavegameData save)
    {
        //Player.saveVersion = save.saveVersion; // Don't overwrite save version!
        Player.keyboardModeOn = save.keyboardModeOn;
        Player.deck = new Deck(save.deckCards);
        Player.collection = save.collection;
        HubControl.maxUnlockedDifficultyId = save.maxUnlockedDifficultyId;
        HubControl.currentDifficultyId = save.currentDifficultyId;
        Player.experience = save.playerExpDouble;
        FortuneManager.allFortunes = save.allFortunes;
        FortuneManager.applyAllFortuneEffects();
        Player.stats = save.statsDouble;
        AudioManager.globlalMute = save.globalMute;
        Dice.skipAnim = save.skipAnim;
        BattleManager.skipEnemyDialogue = save.skipEnemyDialogue;
        Player.checkForUpdatesOnStart = save.checkForUpdatesOnStart;
        TextScript.language = save.language;
        Player.rerollPoints = save.rerollPoints;
        
        if (Player.isOlderSaveThan(save.saveVersion, "1.1.0"))
        {
            // After v1.0.0, Player.stats went from Dictionary<Stat, float> to Dictionary<Stat, double>
            Player.stats = new Dictionary<Player.Stat, double>();
            foreach (Player.Stat stat in save.stats.Keys)
            {
                Player.stats.Add(stat, save.stats[stat]);
            }

            // After v1.0.0, the "Greatest Rank" stat was added
            Player.stats.Add(Player.Stat.GREATEST_RANK, HubControl.maxUnlockedDifficultyId);

            // After v1.0.0, experience went from int to double
            Player.experience = save.playerExp;

            // Reset fortunes, and give the player their EXP back
            FortuneManager.restartAllFortunes();
            Player.experience += Player.stats[Player.Stat.TOTAL_EXP_SPENT];
        }

        if (Player.isOlderSaveThan(save.saveVersion, "1.1.3"))
        {
            // v1.1.3 added option to check for updates. Default to true
            Player.checkForUpdatesOnStart = true;
        }

        if (Player.isOlderSaveThan(save.saveVersion, "1.1.4"))
        {
            // v1.1.4 added stats for "Silver defeated" and "Gold defeated"
            Player.stats.Add(Player.Stat.SILVER_DEFEATED, 0);
            Player.stats.Add(Player.Stat.GOLD_DEFEATED, 0);
        }

    }

    public static bool checkIfSaveExists()
    {
        string savegamePath = Application.persistentDataPath + "/RandomNumberGodSavefile.dat";
        return File.Exists(savegamePath);
    }

    public static void deleteSaveFile()
    {
        string savegamePath = Application.persistentDataPath + "/RandomNumberGodSavefile.dat";
        if (File.Exists(savegamePath))
        {
            File.Delete(savegamePath);

            Player.deck = null;
            Player.collection = null;
            HubControl.maxUnlockedDifficultyId = 0;
            HubControl.currentDifficultyId = 0;
            Player.experience = 0;
            Player.stats = null;
            Player.rerollPoints = 0;
            FortuneManager.restartAllFortunes();

            SceneManager.LoadScene("TitleScreen");
        }
    }

}

[Serializable]
class SavegameData
{
    public string saveVersion;
    public bool keyboardModeOn;
    public List<Card> deckCards;
    public List<Card> collection;
    public int maxUnlockedDifficultyId;
    public int currentDifficultyId;
    public double playerExpDouble;
    public List<Fortune> allFortunes;
    public Dictionary<Player.Stat, double> statsDouble;
    public bool globalMute;
    public bool skipAnim;
    public bool skipEnemyDialogue;
    public bool checkForUpdatesOnStart;
    public TextScript.Language language;
    public double rerollPoints;

    // Old variables. No longer used, but kept for compatibility with old savefiles
    public Dictionary<Player.Stat, float> stats;
    public int playerExp;

    public SavegameData(string saveVersion, bool keyboardModeOn, Deck deck, List<Card> collection, int maxUnlockedDifficultyId, 
        int currentDifficultyId, double playerExpDouble, List<Fortune> allFortunes, Dictionary<Player.Stat, double> statsDouble, 
        bool globalMute, bool skipAnim, bool skipEnemyDialogue, bool checkForUpdatesOnStart, TextScript.Language language, 
        double rerollPoints)
    {
        this.saveVersion = saveVersion;
        this.keyboardModeOn = keyboardModeOn;
        this.deckCards = deck.cards;
        this.collection = collection;
        this.maxUnlockedDifficultyId = maxUnlockedDifficultyId;
        this.currentDifficultyId = currentDifficultyId;
        this.playerExpDouble = playerExpDouble;
        this.allFortunes = allFortunes;
        this.statsDouble = statsDouble;
        this.globalMute = globalMute;
        this.skipAnim = skipAnim;
        this.skipEnemyDialogue = skipEnemyDialogue;
        this.checkForUpdatesOnStart = checkForUpdatesOnStart;
        this.language = language;
        this.rerollPoints = rerollPoints;
    }

}
