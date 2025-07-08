using System;
using UnityEngine;

/// <summary>
/// ScriptableObject che contiene tutti i parametri di progressione del gioco:
/// prezzi upgrade, requisiti missioni, tempi, masse, ecc.
/// Permette di bilanciare facilmente il gameplay senza toccare il codice.
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    // ----- Calcolo premi e requisiti per livello -----

    /// <summary>
    /// Monete premiate al completamento di un livello.
    /// Aumentano di <see cref="coinAwardIncreasePerLevel"/> a livello.
    /// </summary>
    public int CoinAwardedPerLevel(int level)
    {
        return Mathf.FloorToInt(baseCoinAwardedPerLevel
            + coinAwardIncreasePerLevel * (level - 1));
    }

    /// <summary>
    /// Punteggio richiesto per completare la missione “Score”.
    /// Cresce di un moltiplicatore per ogni livello.
    /// </summary>
    public int ScoreRequirementForLevel(int level)
    {
        return Mathf.FloorToInt(BaseScoreRequirement
            + BaseScoreRequirement * (level - 1) * ScoreMissionMultiplierAddPerlevel);
    }

    /// <summary>
    /// Punteggio per secondo richiesto (SPS).
    /// </summary>
    public int SPSRequirementForLevel(int level)
    {
        return Mathf.FloorToInt(BaseSPSRequirement
            + BaseSPSRequirement * (level - 1) * SPSMissionMultiplierAddPerlevel);
    }

    /// <summary>
    /// Numero di bersagli da colpire per la missione “HitTarget”.
    /// </summary>
    public int HitTargetForLevel(int level)
    {
        return baseHitTarget + Mathf.FloorToInt(HitTargetAddPerLevel * (level - 1));
    }

    /// <summary>
    /// Tempo da mantenere in gioco per la missione “KeepTime” (non usata ora).
    /// </summary>
    public float KeepTimeForLevel(int level)
    {
        return baseKeepTime + KeepTimeAddPerLevel * (level - 1);
    }

    /// <summary>
    /// Limite massimo di tempo per il livello (si riduce a ogni livello).
    /// </summary>
    public float TimeLimitForLevel(int level)
    {
        return TimeLimit - TimeLimitRemovedPerLevel * (level - 1);
    }

    // ----- Calcolo parametri palline e upgrade -----

    /// <summary>
    /// Massa della palla in base al livello di upgrade “HyperBall”.
    /// Viene scalata esponenzialmente.
    /// </summary>
    public float BallMass(int upgradeLevel)
    {
        return baseBallMass
             * Mathf.Pow(BallMassMultiplierPerUpgrade, upgradeLevel - 1);
    }

    /// <summary>
    /// Prezzo per il prossimo upgrade massa palla.
    /// </summary>
    public int BallMassUpgradePrice(int currentLevel)
    {
        return Mathf.FloorToInt(ballmassUpgradePriceBase
            + ballmassUpgradePriceIncreasesPerLevel * (currentLevel - 1));
    }

    /// <summary>
    /// Numero di palline totali (lives) per livello.
    /// </summary>
    public int BallPerLevel(int upgradeLevel)
    {
        return StartingBallPerLevel + BallAddPerUpgrade * (upgradeLevel - 1);
    }

    /// <summary>
    /// Prezzo per aumentare il numero di palline di riserva.
    /// </summary>
    public int BallUpgradePrice(int currentLevel)
    {
        return Mathf.FloorToInt(ballUpgradePriceBase
            + ballUpgradePriceIncreasesPerLevel * (currentLevel - 1));
    }

    /// <summary>
    /// Numero di palline attive contemporaneamente (multi-launch).
    /// </summary>
    public int ActiveBallLimit(int upgradeLevel)
    {
        return startingActiveBallPerLevel + ActiveBallAddPerUpgrade * (upgradeLevel - 1);
    }

    /// <summary>
    /// Prezzo per aumentare il numero di palline attive.
    /// </summary>
    public int ActiveBallUpgradePrice(int currentLevel)
    {
        return Mathf.FloorToInt(activeBallUpgradePriceBase
            + activeBallUpgradePriceIncreasesPerLevel * (currentLevel - 1));
    }

    // ----- Parametri serializzati (bilanciamento) -----
    [Header("General")]
    public int    BonusCoinPerScore               = 50000;
    public float  baseCoinAwardedPerLevel         = 1f;
    public float  coinAwardIncreasePerLevel       = 0.8f;
    public float  BaseScoreRequirement            = 100000f;
    public float  ScoreMissionMultiplierAddPerlevel = 1f;
    public float  BaseSPSRequirement              = 3000f;
    public float  SPSMissionMultiplierAddPerlevel  = 1f;
    public int    baseHitTarget                   = 3;
    public float  HitTargetAddPerLevel            = 0.7f;
    public float  baseKeepTime                    = 15f;
    public float  KeepTimeAddPerLevel             = 5f;
    public float  TimeLimit                       = 300f;
    public float  TimeLimitRemovedPerLevel        = 60f;
    public int    maxTimeLimitLevel               = 4;

    [Header("Ball Parameters")]
    public float baseBallMass                     = 0.4f;
    public float BallMassMultiplierPerUpgrade     = 0.9f;
    public float ballmassUpgradePriceBase         = 1f;
    public float ballmassUpgradePriceIncreasesPerLevel = 1f;
    public int   StartingBallPerLevel             = 3;
    public int   BallAddPerUpgrade                = 1;
    public float ballUpgradePriceBase             = 3f;
    public float ballUpgradePriceIncreasesPerLevel = 3f;
    public int   startingActiveBallPerLevel       = 1;
    public int   ActiveBallAddPerUpgrade          = 1;
    public float activeBallUpgradePriceBase       = 5f;
    public float activeBallUpgradePriceIncreasesPerLevel = 5f;

    // Tipologie di missione e di buff, usate per selezione dinamica
    public enum MissionType { Score, ScorePerSecond, HitTarget }
    public enum BuffType    { HyperBall, MoreTries, MultiLaunch }
    public enum DebuffType  { TimeLimit, AddBlock, Glitch }
}
