using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controlla un gruppo di pulsanti.
/// Quando tutti i pulsanti del gruppo sono stati premuti, attiva un effetto premio,
/// riproduce un suono, e riattiva i pulsanti dopo un certo tempo.
/// </summary>
public class ButtonGroup : MonoBehaviour
{
    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        buttons = GetComponentsInChildren<Button>();

        // Ascolta l'evento "OnDeactive" di ogni pulsante del gruppo
        foreach (Button button in buttons)
        {
            button.OnDeactive.AddListener(new UnityAction(OnButtonHit));
        }
    }

    /// <summary>
    /// Chiamato ogni volta che un pulsante viene premuto.
    /// Quando tutti sono stati premuti, chiama OnGroupClear().
    /// </summary>
    private void OnButtonHit()
    {
        hits++;
        if (hits >= buttons.Length)
        {
            OnGroupClear();
        }
    }

    /// <summary>
    /// Evento chiamato quando tutti i pulsanti del gruppo sono premuti.
    /// Premia il giocatore, riproduce suono e resetta i pulsanti.
    /// </summary>
    private void OnGroupClear()
    {
        hits = 0;
        RuntimeManager.PlayOneShot(clearSound, transform.position); // Suono premio
        gameController.AddCoin(1); // Premio: 1 moneta
        StartCoroutine(ReActivateAllButtons());
    }

    /// <summary>
    /// Dopo mezzo secondo riattiva tutti i pulsanti del gruppo.
    /// </summary>
    private IEnumerator ReActivateAllButtons()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Button button in buttons)
        {
            button.Active();
        }
    }

    private GameController gameController;

    [SerializeField] private Button[] buttons;       // Pulsanti del gruppo
    [SerializeField] private int hits;               // Conta quanti pulsanti sono stati premuti
    [SerializeField] private EventReference clearSound; // Suono quando il gruppo è completo
}
