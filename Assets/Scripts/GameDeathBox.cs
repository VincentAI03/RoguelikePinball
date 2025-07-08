using System;
using UnityEngine;

/// <summary>
/// Zona “morte”: quando la palla entra in questo trigger, notifica il GameController
/// affinché gestisca la caduta (respawn o game over).
/// </summary>
public class GameDeathBox : MonoBehaviour
{
    // Riferimento al GameController (impostato via Inspector)
    [SerializeField] private GameController gameController;

    /// <summary>
    /// Viene chiamato quando un Collider 3D entra nel trigger.
    /// Se il tag è “Ball”, chiama GameController.BallFall().
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Verifica che l’oggetto entrato sia la palla
        if (other.gameObject.CompareTag("Ball"))
        {
            // Notifica al controller che la palla è caduta
            gameController.BallFall(other.gameObject);
        }
    }
}
