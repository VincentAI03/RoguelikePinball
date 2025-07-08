using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlla le animazioni e i pulsanti dello schermo “Level Complete”:
/// compra potenziamenti, seleziona missione successiva e avvia il livello.
/// </summary>
public class LevelCompleteScreen : MonoBehaviour
{
    private GameController gameController;

    // Pulsanti per acquistare buff (collegati in Inspector)
    [SerializeField] private Button[] buyButtons;
    // Pulsanti per selezionare la missione successiva
    [SerializeField] private Button[] missionButtons;
    // Animator che gestisce le transizioni di UI
    [SerializeField] private Animator animator;

    private void Awake()
    {
        // Trova il GameController nella scena
        gameController = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        // Quando il pannello appare, abilita tutti i pulsanti
        foreach (var b in buyButtons)
            b.interactable = true;

        foreach (var m in missionButtons)
            m.interactable = true;
    }

    /// <summary>
    /// Chiamato da un pulsante “Buy Buff” con indice.
    /// Disabilita tutti i buyButtons, fa flip di quelli non scelti,
    /// quindi passa alla selezione missione dopo 1s.
    /// </summary>
    public void BuffBought(int index)
    {
        for (int i = 0; i < buyButtons.Length; i++)
        {
            buyButtons[i].interactable = false;
            if (i != index)
                buyButtons[i].GetComponent<Animator>().SetTrigger("flip");
        }
        StartCoroutine(ToMissionSelect());
    }

    /// <summary>
    /// Chiamato da un pulsante missione.
    /// Disabilita tutti missionButtons, fa flip di quelli non scelti,
    /// quindi chiude il menu e avvia il livello dopo 2s.
    /// </summary>
    public void MissionSelected(int index)
    {
        for (int i = 0; i < missionButtons.Length; i++)
        {
            missionButtons[i].interactable = false;
            if (i != index)
                missionButtons[i].GetComponent<Animator>().SetTrigger("flip");
        }
        StartCoroutine(CloseMenu());
    }

    /// <summary>
    /// Attende 1 secondo, poi innesca animazione di transizione a Mission Select.
    /// </summary>
    private IEnumerator ToMissionSelect()
    {
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("mission");
    }

    /// <summary>
    /// Attende 2 secondi, innesca chiusura, poi avvia StartLevel().
    /// </summary>
    private IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("close");
        yield return new WaitForSeconds(1f);
        gameController.StartLevel();
    }
}
