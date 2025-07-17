using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UIButton = UnityEngine.UI.Button;

/// <summary>
/// Script collegato a un pulsante per acquistare potenziamenti.
/// Quando il pulsante è cliccato, scala il costo del potenziamento in base al livello attuale.
/// </summary>
public class BuffBuyButton : MonoBehaviour
{
    private void Awake()
    {
        // Trova il GameController e il GameData nella scena
        gameController = FindObjectOfType<GameController>();
        gameData = gameController.gameData;

        // Recupera il componente Button e collega la funzione BuyBuff all’evento onClick
        button = GetComponent<UIButton>();
        button.onClick.AddListener(new UnityAction(BuyBuff));
    }

    private void OnEnable()
    {
        // Calcola il costo del potenziamento in base al tipo
        cost = 0;
        switch (buffType)
        {
            case GameData.BuffType.HyperBall:
                cost = gameData.BallMassUpgradePrice(gameController.upgrade_ballmass);
                break;
            case GameData.BuffType.MoreTries:
                cost = gameData.BallUpgradePrice(gameController.upgrade_ballcount);
                break;
            case GameData.BuffType.MultiLaunch:
                cost = gameData.ActiveBallUpgradePrice(gameController.upgrade_activeBall);
                break;
        }

        // Disattiva il pulsante se il giocatore non ha abbastanza monete
        button.interactable = gameController.coin >= cost;
    }

    /// <summary>
    /// Riduce le monete del giocatore e attiva il potenziamento selezionato.
    /// </summary>
    private void BuyBuff()
    {
        gameController.coin -= cost;
        gameController.Upgrades(buffType);
    }

    [SerializeField] private GameData.BuffType buffType; // Tipo di potenziamento da acquistare
    private GameController gameController; // Riferimento al controller principale
    private GameData gameData; // Contiene le logiche di calcolo per i prezzi
    private UIButton button; // Componente UI corretto
    private int cost; // Costo calcolato per il potenziamento attuale
}
