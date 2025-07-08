/// <summary>
/// Switch interattivo a tema pinball: quando la palla lo colpisce,
/// alterna il suo stato On/Off, cambia colore e invia eventi personalizzabili.
/// Può ad esempio aprire una porta, attivare un bonus tempo o illuminare un percorso.
/// </summary>
using System;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

[RequireComponent(typeof(Collider))]
public class Switch : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] private bool startOn = false;      // Stato iniziale della switch
    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.red;
    [SerializeField] private EventReference switchSound; // Suono FMOD da riprodurre

    [Header("Events")]
    public UnityEvent OnActivate;    // Invocato quando la switch si attiva
    public UnityEvent OnDeactivate;  // Invocato quando la switch si disattiva

    private bool isOn;
    private Renderer rend;

    private void Awake()
    {
        // Ottiene il renderer per cambiare colore
        rend = GetComponent<Renderer>();
        // Imposta lo stato iniziale e aggiorna la visual
        isOn = startOn;
        UpdateVisual();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se la palla colpisce la switch, alterna lo stato
        if (collision.gameObject.CompareTag("Ball"))
        {
            Toggle();
        }
    }

    /// <summary>
    /// Alterna lo stato della switch e invoca gli eventi.
    /// </summary>
    public void Toggle()
    {
        isOn = !isOn;
        UpdateVisual();

        // Riproduce suono se valido
        if (switchSound.IsValid())
            RuntimeManager.PlayOneShot(switchSound, transform.position);

        // Invoca evento corrispondente
        if (isOn)
            OnActivate?.Invoke();
        else
            OnDeactivate?.Invoke();
    }

    /// <summary>
    /// Aggiorna il colore basato sullo stato.
    /// </summary>
    private void UpdateVisual()
    {
        if (rend != null)
            rend.material.color = isOn ? onColor : offColor;
    }
}
