using System;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Script che rileva quando una palla attraversa un trigger,
/// per notificare il GameController (es. quando lascia una zona di lancio)
/// e riprodurre un effetto sonoro.
/// </summary>
public class BallPassCheck : MonoBehaviour
{
    private void Start()
    {
        // Cerca il GameController nella scena all’avvio
        gameController = FindObjectOfType<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l’oggetto entrato nel trigger è la palla
        if (other.CompareTag("Ball"))
        {
            // Suono di passaggio
            RuntimeManager.PlayOneShot(ballPassSound, transform.position);

            // Notifica al GameController che la palla ha lasciato la zona
            Ball ballComponent = other.GetComponent<Ball>();
            gameController.BallLeftStriker(ballComponent);
        }
    }

    private GameController gameController;

    [SerializeField] private EventReference ballPassSound; // Suono da riprodurre quando la palla passa
}
