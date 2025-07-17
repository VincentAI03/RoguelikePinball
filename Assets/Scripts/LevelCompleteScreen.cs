using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UIButton = UnityEngine.UI.Button; // Alias per evitare conflitto con classe custom "Button"

/// <summary>
/// Controlla le animazioni e i pulsanti dello schermo “Level Complete”:
/// compra potenziamenti, seleziona missione successiva e avvia il livello.
/// </summary>
public class LevelCompleteScreen : MonoBehaviour
{
    private GameController gameController;

    // Pulsanti per acquistare buff (collegati in Inspector)
    [SerializeField] private UIButton[] buyButtons;
    // Pulsanti per selezionare la missione successiva
    [SerializeField] private UIButton[] missionButtons;
    // Animator che gestisce le transizioni di UI
    [SerializeField] private Animator animator;

    // Tempi parametrizzabili (opzionali)
    [SerializeField] private float buffToMissionDelay = 1f;
    [SerializeField] private float missionToStartDelay = 2f;
    [SerializeField] private float closeAnimationDelay = 1f;

    private void Awake()
    {
        // Trova il GameController nella scena
        gameController = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        // Quando il pannello appare, abilita tutti i pulsanti
        foreach (var b in buyButtons)
            if (b != null) b.interactable = true;

        foreach (var m in missionButtons)
            if (m != null) m.interactable = true;
    }

    /// <summary>
    /// Chiamato da un pulsante “Buy Buff” con indice.
    /// Disabilita tutti i buyButtons, fa flip di quelli non scelti,
    /// quindi passa alla selezione missione dopo un ritardo.
    /// </summary>
    public void BuffBought(int index)
    {
        DisableAndFlipOthers(buyButtons, index);
        StartCoroutine(ToMissionSelect());
    }

    /// <summary>
    /// Chiamato da un pulsante missione.
    /// Disabilita tutti missionButtons, fa flip di quelli non scelti,
    /// quindi chiude il menu e avvia il livello dopo un ritardo.
    /// </summary>
    public void MissionSelected(int index)
    {
        DisableAndFlipOthers(missionButtons, index);
        StartCoroutine(CloseMenu());
    }

    /// <summary>
    /// Disattiva tutti i pulsanti e fa animare solo quelli non selezionati.
    /// </summary>
    private void DisableAndFlipOthers(UIButton[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].interactable = false;

            if (i != selectedIndex)
            {
                Animator anim = buttons[i].GetComponent<Animator>();
                if (anim != null)
                    anim.SetTrigger("flip");
            }
        }
    }

    /// <summary>
    /// Attende un ritardo, poi innesca animazione di transizione a Mission Select.
    /// </summary>
    private IEnumerator ToMissionSelect()
    {
        yield return new WaitForSeconds(buffToMissionDelay);
        animator.SetTrigger("mission");
    }

    /// <summary>
    /// Attende i ritardi previsti, innesca chiusura, poi avvia StartLevel().
    /// </summary>
    private IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(missionToStartDelay);
        animator.SetTrigger("close");
        yield return new WaitForSeconds(closeAnimationDelay);
        gameController.StartLevel();
    }
}
