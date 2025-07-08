using System;
using UnityEngine;

/// <summary>
/// Ruota costantemente il GameObject su un asse, modulando la velocità con un moltiplicatore.
/// Utilizzato per elementi decorativi o feedback visivo (es. disco che gira).
/// </summary>
public class RotatingObject : MonoBehaviour
{
    // Gradi al secondo di base
    [SerializeField] private float baseDegreePerSec = 5f;
    // Moltiplicatore dinamico della rotazione (es. per variare in base allo stato di gioco)
    [SerializeField] private float rotationMultiplier = 1f;

    /// <summary>
    /// Imposta il moltiplicatore di rotazione da codice esterno.
    /// </summary>
    public void SetRotationMultiplier(float multiplier)
    {
        rotationMultiplier = multiplier;
    }

    /// <summary>
    /// In FixedUpdate per garantire rotazione fluida indipendente dal framerate.
    /// </summary>
    private void FixedUpdate()
    {
        // Rotazione attorno all’asse Z (Vector3.forward)
        float angle = baseDegreePerSec * Time.fixedDeltaTime * rotationMultiplier;
        transform.Rotate(Vector3.forward * angle);
    }
}
