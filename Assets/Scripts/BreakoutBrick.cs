using System;
using UnityEngine;

/// <summary>
/// Rappresenta un mattone stile "Breakout" che può essere colpito.
/// Ogni urto riduce gli HP del mattone. Se gli HP arrivano a 0, viene disattivato.
/// </summary>
public class BreakoutBrick : Bumper
{
    /// <summary>
    /// Gestisce cosa succede quando la palla collide col mattone.
    /// Riduce gli HP di 1 per ogni collisione.
    /// Se gli HP scendono a 0, disattiva l’oggetto (simulando la distruzione).
    /// </summary>
    protected override void BumpResolve(Collider collision)
    {
        hp -= 1f;
        if (hp <= 0f)
        {
            gameObject.SetActive(false); // Simula la distruzione
        }
    }

    [SerializeField] private float hp = 100f; // Vita del mattone
}
