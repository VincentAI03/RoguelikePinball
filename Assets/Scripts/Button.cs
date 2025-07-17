using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Estensione della classe Bumper che rappresenta un pulsante.
/// Al primo colpo da parte della palla si disattiva, cambia colore, attiva animazioni,
/// e può notificare altri oggetti tramite eventi Unity.
/// </summary>
public class Button : Bumper
{
    protected override void Awake()
    {
        base.Awake();
        baseColor = spriteRenderer.color; // Salva il colore iniziale del pulsante
    }

    /// <summary>
    /// Disattiva il pulsante: cambia colore, attiva animazione e invoca evento.
    /// </summary>
    public virtual void Deactive()
    {
        OnDeactive.Invoke(); // Evento Unity (può notificare ButtonGroup)
        buttonCollider.enabled = false;
        isActive = false;
        spriteRenderer.color = baseColor * 0.3f; // Colore più scuro per indicare disattivazione
        animator.SetTrigger("DeActivated");
    }

    /// <summary>
    /// Riattiva il pulsante: ripristina colore, animazione e collider.
    /// </summary>
    public virtual void Active()
    {
        OnActive.Invoke(); // Evento Unity (facoltativo)
        buttonCollider.enabled = true;
        isActive = true;
        spriteRenderer.color = baseColor;
        animator.SetTrigger("ReActivated");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se la palla colpisce il pulsante mentre è attivo
        if (collision.gameObject.CompareTag("Ball") && isActive)
        {
            BumpResolve(collision.collider); // Suono + punteggio
            Deactive(); // Disattiva il pulsante
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Supporta anche trigger (utile per altri tipi di contatto)
        if (collision.gameObject.CompareTag("Ball") && isActive)
        {
            BumpResolve(collision);
            Deactive();
        }
    }

    [SerializeField] private Collider buttonCollider;         // Collider 3D del pulsante
    [SerializeField] private SpriteRenderer spriteRenderer;    // Per cambiare colore
    [SerializeField] private Animator animator;                // Per animazioni
    public UnityEvent OnDeactive;                              // Evento invocato quando si disattiva
    public UnityEvent OnActive;                                // Evento invocato quando si riattiva

    private bool isActive = true;                              // Stato attuale del pulsante
    private Color baseColor;                                   // Colore iniziale
}
