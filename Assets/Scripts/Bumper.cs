using System;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Classe base per tutti i "bumper", ovvero oggetti che reagiscono quando la palla li colpisce.
/// Applica una forza alla palla e incrementa il punteggio.
/// Può essere estesa da altri oggetti interattivi (es. pulsanti, blocchi...).
/// </summary>
public class Bumper : MonoBehaviour
{
    protected virtual void Awake()
    {
        // Ottiene un riferimento al GameController per aggiornare punteggio o eventi globali.
        gameController = FindObjectOfType<GameController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se l'oggetto che ha colpito è la palla...
        if (collision.gameObject.CompareTag("Ball"))
        {
            BumpToDirection(collision); // Applica forza alla palla
            BumpResolve(collision.collider); // Gestisce effetti secondari come suono e punteggio
        }
    }

    /// <summary>
    /// Calcola la direzione dell'urto tra il bumper e la palla.
    /// Di default, punta dal centro del bumper verso la palla.
    /// </summary>
    protected virtual Vector3 BumpDirection(Collision collision)
    {
        return collision.transform.position - transform.position;
    }

    /// <summary>
    /// Applica una forza impulsiva alla palla nella direzione calcolata.
    /// </summary>
    protected void BumpToDirection(Collision collision)
    {
        Vector3 direction = BumpDirection(collision);
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            rb.AddForce(direction.normalized * bumperForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Effetti accessori del bump: suono, punteggio e carica energia.
    /// </summary>
    protected virtual void BumpResolve(Collider collision)
    {
        RuntimeManager.PlayOneShot(bumperSound, transform.position);

        Ball ball = collision.GetComponent<Ball>();
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        float score = baseScore * ball.energy * rb.velocity.magnitude;

        ball.ChargeEnergy(baseScore);
        gameController.AddScore(score / 100f); // Divide per 100 per bilanciare lo scaling
    }

    protected GameController gameController;

    [SerializeField] protected float bumperForce = 10f;      // Forza applicata alla palla
    [SerializeField] private float baseScore = 10f;          // Base score per urto
    [SerializeField] private EventReference bumperSound;     // Suono da riprodurre all'urto
}
