using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fornisce una funzione pubblica per ricaricare la scena corrente.
/// Utile per il pulsante “Restart” o debug.
/// </summary>
public class ResetGame : MonoBehaviour
{
    /// <summary>
    /// Ricarica la scena attiva usando il suo build index.
    /// </summary>
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
