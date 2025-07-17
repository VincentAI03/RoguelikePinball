using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Il cuore del gioco: controlla il flusso di ogni livello,
/// gestisce punteggi, monete, tempo, missioni, spawn delle palle,
/// interazione con flipper, UI e suoni.
/// </summary>
public class GameController : MonoBehaviour
{
    // -------------------- Proprietà ESPOSTE / Accessori --------------------

    /// <summary>
    /// Monete correnti del giocatore.
    /// Se aggiunte, suona <see cref="coinSound"/>; aggiorna UI.
    /// </summary>
    public int coin
    {
        get => currentCoin;
        set
        {
            // Se il nuovo valore è maggiore, suona il feedback audio
            if (value > currentCoin)
                RuntimeManager.PlayOneShot(coinSound, transform.position);

            currentCoin = value;

            // Aggiorna i testi UI relativi alle monete e al "bank"
            coinText.text = "<color=#FFFF00>$" + currentCoin;
            bankText.text = $"BANK: <color=#FFFF00>${currentCoin}";
        }
    }

    /// <summary>
    /// Punteggio accumulato nel livello corrente.
    /// Aggiorna il contatore UI.
    /// </summary>
    private int score
    {
        get => levelScore;
        set
        {
            levelScore = value;
            scoreText.text = levelScore.ToString();
        }
    }

    /// <summary>
    /// Conta quanti bersagli sono stati colpiti.
    /// Aggiorna l’indicatore UI.
    /// </summary>
    private int targetHit
    {
        get => targetStrucked;
        set
        {
            targetStrucked = value;
            targetHitText.text = targetStrucked.ToString();
        }
    }

    /// <summary>
    /// Palline rimaste di riserva.
    /// Rappresentate in UI con tanti punti (“.”) quanti sono i pallini.
    /// </summary>
    private int ballLeft
    {
        get => levelBallLeft;
        set
        {
            levelBallLeft = value;
            // Ricrea stringa di pallini
            var dots = new System.Text.StringBuilder();
            for (int i = 0; i < levelBallLeft; i++)
                dots.Append('.');
            ballText.text = dots.ToString();
        }
    }

    /// <summary>
    /// Tempo rimanente nel livello (in secondi).
    /// Quando scende sotto 10s, suona <see cref="tenSecSound"/>.
    /// Se arriva a 0 blocca i flipper.
    /// </summary>
    private float timeLeft
    {
        get => levelTimeLeft;
        set
        {
            // Feedback sonoro a 10 secondi
            if (levelTimeLeft > 10f && value <= 10f)
                RuntimeManager.PlayOneShot(tenSecSound, transform.position);

            levelTimeLeft = value;

            // Formatta mm:ss
            int minutes = Mathf.FloorToInt(levelTimeLeft / 60f);
            int seconds = Mathf.FloorToInt(levelTimeLeft % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";

            // Se tempo finito, blocca flipper
            if (levelTimeLeft <= 0f)
                SetFlipperLock(true);
        }
    }

    // -------------------- Unity Lifecycle --------------------

    private void Awake()
    {
        // Recupera l’azione di input per caricare il Striker
        strikerChargeAction = inputActions.FindAction("StrikerCharge", true);

    }

    private void Start()
    {
        // Aggiorna il testo dei potenziamenti iniziali (LIVES / MULTI / BOUNCE)
        updateBuffText();
    }

    private void Update()
    {
        // 1) Gestione del timer di gioco
        if (isLevelRunning && timeLeft > 0f)
        {
            timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
        }

        // 2) Calcolo del punteggio istantaneo per secondo (SPS)
        int sps = 0;
        foreach (var log in scoreLogs)
            if (Time.time - log.logTime <= 1f)
                sps += log.score;

        // 3) Aggiorna un oggetto rotante (grafica “likeARecordBaby”) in base allo sps
        float multiplier = (1f + sps / 1000f) * ((currentLevel % 2 == 0) ? -1f : 1f);
        likeARecordBaby.SetRotationMultiplier(multiplier);

        // 4) Se la missione è “ScorePerSecond” e raggiungo l’obiettivo, completo il livello
        if (currentMission == GameData.MissionType.ScorePerSecond
            && sps >= gameData.SPSRequirementForLevel(currentLevel))
        {
            CompleteLevel();
        }

        scorePerSecondText.text = sps.ToString();
        // Rimuovi vecchi log oltre 1 secondo
        scoreLogs.RemoveAll(x => Time.time - x.logTime > 1f);

        // 5) Gestione input striker (meccanismo di carica e rilascio)
        if (strikerChargeAction.WasPressedThisFrame())
        {
            strikerJoint.useMotor = false;
            Debug.Log("Striker charging...");
        }
        if (strikerChargeAction.WasReleasedThisFrame())
        {
            RuntimeManager.PlayOneShot(strikerSound, transform.position);
            strikerJoint.useMotor = true;
            Debug.Log("Striker released!");
        }
    }

    // -------------------- Metodi di interazione da altri script --------------------

    /// <summary>
    /// Aggiunge monete (chiamato da ButtonGroup o obiettivi vari).
    /// </summary>
    public void AddCoin(int amount)
    {
        coin += amount;
    }

    /// <summary>
    /// Abilita/disabilita i flipper.
    /// </summary>
    private void SetFlipperLock(bool locked)
    {
        foreach (var f in flippers)
            f.SetLock(locked);
    }

    /// <summary>
    /// Viene chiamato da BallPassCheck quando una palla lascia il kicker.
    /// Aggiunge la palla alla lista di quelle “in gioco”.
    /// </summary>
    public void BallLeftStriker(Ball ball)
    {
        if (!ballsAtPlay.Contains(ball))
            ballsAtPlay.Add(ball);
    }

    /// <summary>
    /// Viene chiamato quando una palla cade oltre il bordo.
    /// Se ci sono palline di riserva, ne rilancia una; altrimenti game over.
    /// </summary>
    public void BallFall(GameObject ballGO)
    {
        levelBallFell++;
        ballsAtPlay.Remove(ballGO.GetComponent<Ball>());

        if (ballLeft > 0)
        {
            ballLeft--;
            ballGO.GetComponent<Ball>().RespawnAt(ballspawnPoint.position);
        }

        if (levelBallFell >= upgradedMaxBallLive)
        {
            GameOver();
        }
    }

    // -------------------- Setup Iniziale del Livello --------------------

    /// <summary>
    /// Instanzia le palle in base al potenziamento “activeBall”
    /// e le posiziona al kicker.
    /// </summary>
    private void SetupLevelsBallAtStriker()
    {
        // Finché non ho il numero giusto di palle, le creo
        while (InstantiatedBalls.Count < upgradedMaxActiveBall)
        {
            var ball = Instantiate(ballPrefab, ballspawnPoint.position, Quaternion.identity, ballParent)
                        .GetComponent<Ball>();
            InstantiatedBalls.Add(ball);
        }

        // Per ciascuna: imposta massa e respawn
        foreach (var ball in InstantiatedBalls)
        {
            ball.SetMass(upgradedBallMass);
            ball.RespawnAt(ballspawnPoint.position);
        }
    }

    // -------------------- Gestione Missioni e Punteggi --------------------

    /// <summary>
    /// Aumenta il contatore bersagli e, se la missione è “HitTarget” e raggiunto l’obiettivo, completa livello.
    /// </summary>
    public void TargetStrucked()
    {
        targetHit++;
        if (currentMission == GameData.MissionType.HitTarget &&
            targetHit >= gameData.HitTargetForLevel(currentLevel))
        {
            CompleteLevel();
        }
    }

    /// <summary>
    /// Aggiunge punteggio da un urto / evento.
    /// Se supera soglie, premia con monete e verifica missione “Score”.
    /// Registra il log per SPS.
    /// </summary>
    public void AddScore(float rawScore)
    {
        score += (int)rawScore;

        // Se supero soglia, assegna monete di bonus
        if (score >= levelCoinThreshold)
        {
            int bonus = Mathf.FloorToInt(score / levelCoinThreshold);
            coin += bonus;
            levelCoinThreshold += gameData.BonusCoinPerScore * bonus;
        }

        // Registra log per calcolo SPS
        scoreLogs.Add(new ScoreLog((int)rawScore, Time.time));

        // Verifica missione “Score”
        if (currentMission == GameData.MissionType.Score &&
            score >= gameData.ScoreRequirementForLevel(currentLevel))
        {
            CompleteLevel();
        }
    }

    // -------------------- Inizio/Fine/Completamento Livello --------------------

    /// <summary>
    /// Inizializza i pannelli, riproduce suono, aggiorna UI testi missione
    /// e resetta tutte le variabili di stato. Alla fine lancia SetupLevelsBallAtStriker().
    /// </summary>
    public void StartLevel()
    {
        levelCompletePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        RuntimeManager.PlayOneShot(gameStartSound, transform.position);
        levelText.text = $"LEVEL-{currentLevel:00}";

        // Prepara descrizione missione in base al tipo
        string missionStr = currentMission switch
        {
            GameData.MissionType.Score         => $"SCORE {gameData.ScoreRequirementForLevel(currentLevel)}",
            GameData.MissionType.ScorePerSecond=> $"SCORE {gameData.SPSRequirementForLevel(currentLevel)} IN 1 SEC",
            GameData.MissionType.HitTarget     => $"HIT {gameData.HitTargetForLevel(currentLevel)} TARGETS",
            _ => ""
        };
        missionText.text = missionStr;

        // Sblocca flipper e inizializza parametri da GameData + upgrade
        SetFlipperLock(false);
        upgradedMaxBallLive   = gameData.BallPerLevel(upgrade_ballcount);
        upgradedMaxActiveBall = gameData.ActiveBallLimit(upgrade_activeBall);
        upgradedBallMass      = gameData.BallMass(upgrade_ballmass);

        isLevelComplete = false;
        score           = 0;
        ballLeft        = upgradedMaxBallLive - upgradedMaxActiveBall;
        levelBallFell   = 0;
        timeLeft        = gameData.TimeLimitForLevel(currentLevel);
        ballsAtPlay.Clear();
        targetHit       = 0;
        levelCoinThreshold = gameData.BonusCoinPerScore;

        SetupLevelsBallAtStriker();
        isLevelRunning = true;
    }

    public void EndLevel()
    {
        isLevelRunning = false;
        SetFlipperLock(true);
    }

    /// <summary>
    /// Chiama EndLevel(), assegna ricompense, aggiorna pannello di completamento
    /// e prepara i testi dei prossimo upgrade e missioni.
    /// </summary>
    public void CompleteLevel()
    {
        if (!isLevelRunning) return;

        isLevelComplete = true;
        EndLevel();

        // Ricompensa monete
        coin += gameData.CoinAwardedPerLevel(currentLevel);

        // Aggiorna UI pannello “Level Complete”
        coinAwardText .text = $"AWARDED <color=#FFFF00>${gameData.CoinAwardedPerLevel(currentLevel)}";
        bounceTitleText.text = $"BOUNCE\nV{upgrade_ballmass + 1}";
        bounceDescText .text = $"ball mass\n{gameData.BallMass(upgrade_ballmass):0.00}>{gameData.BallMass(upgrade_ballmass + 1):0.00}";
        bounceCostText .text = $"<color=#FFFF00>${gameData.BallMassUpgradePrice(upgrade_ballmass)}";

        livesTitleText.text = $"LIVES\nV{upgrade_ballcount + 1}";
        livesDescText .text = $"ball count\n{gameData.BallPerLevel(upgrade_ballcount)}>{gameData.BallPerLevel(upgrade_ballcount + 1)}";
        livesCostText .text = $"<color=#FFFF00>${gameData.BallUpgradePrice(upgrade_ballcount)}";

        multiTitleText.text = $"MULTI\nV{upgrade_activeBall + 1}";
        multiDescText .text = $"active ball\n{gameData.ActiveBallLimit(upgrade_activeBall)}>{gameData.ActiveBallLimit(upgrade_activeBall + 1)}";
        multiCostText .text = $"<color=#FFFF00>${gameData.ActiveBallUpgradePrice(upgrade_activeBall)}";

        currentLevel++;

        // Aggiorna testi missione per il livello successivo
        missionScoreText.text = $"score\n{gameData.ScoreRequirementForLevel(currentLevel)}";
        missionSPS      .text = $"score\n{gameData.SPSRequirementForLevel(currentLevel)}\nwithin 1 second";
        missionTarget   .text = $"hit {gameData.HitTargetForLevel(currentLevel)} targets";

        levelCompletePanel.SetActive(true);
    }

    // -------------------- Selezione Missione e Upgrade --------------------

    public void MissionSelect(GameData.MissionType m)
    {
        currentMission = m;
        updateBuffText();
    }

    public void Upgrades(GameData.BuffType t)
    {
        switch (t)
        {
            case GameData.BuffType.HyperBall: upgrade_ballmass++;  break;
            case GameData.BuffType.MoreTries: upgrade_ballcount++;break;
            case GameData.BuffType.MultiLaunch:upgrade_activeBall++;break;
        }
        updateBuffText();
    }

    private void updateBuffText()
    {
        buffText.text = $"LIVES V{upgrade_ballcount}\nMULTI V{upgrade_activeBall}\nBOUNCE V{upgrade_ballmass}\n";
    }

    public void GameOver()
    {
        if (isLevelComplete) return;
        EndLevel();
        gameOverPanel.SetActive(true);
    }

    // -------------------- TimeScale Lerp Smoothing --------------------

    public void SetTimeScale(float target, float smoothDuration)
    {
        if (routineScaleSet != null) StopCoroutine(routineScaleSet);
        if (smoothDuration <= 0f)
            Time.timeScale = target;
        else
            routineScaleSet = StartCoroutine(Routine_SetTimeScale(target, smoothDuration));
    }

    private IEnumerator Routine_SetTimeScale(float target, float smoothDuration)
    {
        float velocity = 0f;
        Time.timeScale = Mathf.SmoothDamp(Time.timeScale, target, ref velocity, smoothDuration,
                                           float.PositiveInfinity, Time.unscaledDeltaTime);
        yield return null;
    }

    // -------------------- Campi SerializeField e Variabili --------------------
    [Header("Board Setup")]
    [SerializeField] private Transform      ballspawnPoint;
    [SerializeField] private SliderJoint2D  strikerJoint;
    [SerializeField] private GameObject     ballPrefab;
    [SerializeField] private Transform      ballParent;
    [SerializeField] private Flipper[]      flippers;
    [SerializeField] private RotatingObject likeARecordBaby;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scorePerSecondText;
    [SerializeField] private TextMeshProUGUI ballText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI keepTimeText;
    [SerializeField] private TextMeshProUGUI targetHitText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI buffText;
    [SerializeField] private GameObject     gameOverPanel;
    [SerializeField] private GameObject     levelCompletePanel;
    [SerializeField] private TextMeshProUGUI coinAwardText;
    [SerializeField] private TextMeshProUGUI bankText;
    [SerializeField] private TextMeshProUGUI bounceTitleText;
    [SerializeField] private TextMeshProUGUI bounceDescText;
    [SerializeField] private TextMeshProUGUI bounceCostText;
    [SerializeField] private TextMeshProUGUI livesTitleText;
    [SerializeField] private TextMeshProUGUI livesDescText;
    [SerializeField] private TextMeshProUGUI livesCostText;
    [SerializeField] private TextMeshProUGUI multiTitleText;
    [SerializeField] private TextMeshProUGUI multiDescText;
    [SerializeField] private TextMeshProUGUI multiCostText;
    [SerializeField] private TextMeshProUGUI missionScoreText;
    [SerializeField] private TextMeshProUGUI missionSPS;
    [SerializeField] private TextMeshProUGUI missionTarget;

    [Header("Sound")]
    [SerializeField] private EventReference coinSound;
    [SerializeField] private EventReference strikerSound;
    [SerializeField] private EventReference gameStartSound;
    [SerializeField] private EventReference tenSecSound;
    [SerializeField] private InputActionAsset inputActions;


    [Header("Game Parameters")]
    public GameData gameData;

    // Variabili di stato interne
    private int     currentLevel      = 1;
    private int     currentCoin;
    public  int     upgrade_ballcount = 1;
    public  int     upgrade_activeBall= 1;
    public  int     upgrade_ballmass  = 1;
    private int     upgradedMaxBallLive;
    private int     upgradedMaxActiveBall;
    private float   upgradedBallMass;
    private bool    isLevelRunning;
    private bool    isLevelComplete;
    private List<Ball> InstantiatedBalls = new List<Ball>();
    private int     levelScore;
    private int     levelCoinThreshold;
    private int     levelBallLeft;
    private int     levelBallFell;
    private int     targetStrucked;
    private GameData.MissionType currentMission;
    private List<Ball> ballsAtPlay = new List<Ball>();
    private float   levelTimeLeft;
    private List<ScoreLog> scoreLogs = new List<ScoreLog>();
    private Coroutine routineScaleSet;
    private InputAction strikerChargeAction;

    // Struttura per loggare punteggi e calcolare SPS
    private struct ScoreLog
    {
        public int   score;
        public float logTime;
        public ScoreLog(int s, float t) { score = s; logTime = t; }
    }
}
