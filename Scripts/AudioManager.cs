using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    // Soundfont: EAWPats

    public static AudioManager amInstance;

    public AudioClip sample0;
    public AudioClip sample1;
    public AudioClip sample2;
    public AudioClip sample3;
    public AudioClip pisano;
    public AudioClip goldenRatio;
    AudioClip[] battleThemes;

    public AudioClip victoryTune;
    public AudioClip deckTune;

    public AudioClip diceRolling;
    public AudioClip diceRollEnd;
    public AudioClip positiveChime;
    public AudioClip preAttackChime;
    public AudioClip attackChime;
    public AudioClip enemyAttackChime;
    public AudioClip diceAttackChime;
    public AudioClip windAttackChime;
    public AudioClip enemyBuffChime;

    public static bool globlalMute;

    AudioSource musicSource;
    AudioSource soundSource;

    AudioEchoFilter echoFilter;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        musicSource = audioSources[0];
        soundSource = audioSources[1];

        echoFilter = GetComponent<AudioEchoFilter>();

        battleThemes = new AudioClip[] { sample0, sample1, sample2, sample3, pisano, goldenRatio };

        amInstance = this;
    }

    private void Start()
    {
        applyGlobalMuteSetting();
    }

    public void applyGlobalMuteSetting()
    {
        musicSource.mute = globlalMute;
        soundSource.mute = globlalMute;
    }

    // Music
    void resetMusicValues()
    {
        echoFilter.enabled = false;
        musicSource.pitch = 1;
    }

    public void playMainMenuMusic()
    {
        resetMusicValues();
        echoFilter.enabled = true;
        musicSource.pitch = 1.2f;
        musicSource.clip = victoryTune;
        musicSource.Play();
    }

    public void playDeckMusic()
    {
        resetMusicValues();
        musicSource.clip = deckTune;
        musicSource.Play();
    }

    public void playShopMusic()
    {
        resetMusicValues();
        musicSource.pitch = 1.5f;
        musicSource.clip = deckTune;
        musicSource.Play();
    }

    public void playBattleMusic()
    {
        resetMusicValues();
        AudioClip battleClip;
        if (BattleManager.isTutorial)
            battleClip = sample0;
        else
        {
            int rnd = Random.Range(0, battleThemes.Length);
            battleClip = battleThemes[rnd];
        }
        musicSource.clip = battleClip;
        musicSource.Play();
    }

    public void playVictoryTune()
    {
        resetMusicValues();
        musicSource.clip = victoryTune;
        musicSource.Play();
    }

    public void playDefeatTune()
    {
        resetMusicValues();
        musicSource.clip = victoryTune;
        musicSource.Play();
        musicSource.pitch = -2; // Negative pitch only works if audio is alreadly playing
    }

    // Sounds

    void resetSoundValues()
    {
        soundSource.pitch = 1;
    }

    public void playRollingDiceSound()
    {
        resetSoundValues();
        soundSource.clip = diceRolling;
        soundSource.pitch = 3;
        soundSource.Play();
    }

    public void playRollEndSound()
    {
        resetSoundValues();
        soundSource.PlayOneShot(diceRollEnd);
    }

    public void playPositiveChime()
    {
        resetSoundValues();
        soundSource.PlayOneShot(positiveChime);
    }

    public void playBuzz()
    {
        resetSoundValues();
        soundSource.pitch = 1.75f;
        soundSource.PlayOneShot(diceRolling);
    }

    public void playPreAttackChime()
    {
        resetSoundValues();
        soundSource.PlayOneShot(preAttackChime);
    }

    public void playAttackChime()
    {
        resetSoundValues();
        soundSource.PlayOneShot(attackChime);
    }

    public void playEnemyAttackChime()
    {
        resetSoundValues();
        soundSource.PlayOneShot(enemyAttackChime);
    }

    public void playDiceAttackChime()
    {
        soundSource.pitch = .95f;
        soundSource.PlayOneShot(diceAttackChime);
    }

    public void playWindAttackChime()
    {
        soundSource.pitch = 1.45f;
        soundSource.PlayOneShot(windAttackChime);
    }

    public void playEnemyHealChime()
    {
        soundSource.pitch = 1.2f;
        soundSource.PlayOneShot(enemyBuffChime);
    }

    public void playEnemyBuffChime()
    {
        soundSource.pitch = .75f;
        soundSource.PlayOneShot(enemyBuffChime);
    }

    public void playSellSound()
    {
        soundSource.pitch = 1.02f;
        soundSource.PlayOneShot(enemyBuffChime);
    }

    public void stopSound()
    {
        soundSource.Stop();
    }

}
