using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Applica uno shake continuo al GameObject usando Perlin Noise,
/// regolabile in ampiezza e frequenza. Fornisce anche l’offset corrente.
/// </summary>
public class ShakingObject : MonoBehaviour
{
    // Se true, sposta la posizione dell’oggetto; altrimenti solo offsetVector è aggiornato
    [SerializeField] private bool shakeObject = true;
    // Abilita shake sull’asse X
    [SerializeField] private bool X = true;
    // Abilita shake sull’asse Y
    [SerializeField] private bool Y = true;
    // Ampiezza massima dello shake (in unità di world-space)
    [SerializeField] private float amplitude = 0.1f;
    // Velocità con cui “scorre” il Perlin Noise
    [SerializeField] private float frequency = 0.1f;

    // Offset corrente calcolato (utile per leggere lo shake senza muovere)
    private Vector2 offsetVector;

    /// <summary>
    /// Esponibile per leggere il vettore di offset corrente.
    /// </summary>
    public Vector2 OffsetVector => offsetVector;

    private void Start()
    {
        // Avvia la coroutine che genera lo shake
        StartCoroutine(ShakeObjectWithPerlinNoise());
    }

    /// <summary>
    /// Modifica l’ampiezza dello shake a runtime.
    /// </summary>
    public void SetShakeIntensity(float intensity)
    {
        amplitude = intensity;
    }

    /// <summary>
    /// Modifica la frequenza dello shake a runtime.
    /// </summary>
    public void SetShakeFrequency(float freq)
    {
        frequency = freq;
    }

    /// <summary>
    /// Coroutine infinita che:
    /// 1) Calcola due valori Perlin per X e Y
    /// 2) Mappa da [0,1] → [-0.5,0.5] e moltiplica per ampiezza
    /// 3) Sposta il transform.localPosition e aggiorna offsetVector
    /// </summary>
    private IEnumerator ShakeObjectWithPerlinNoise()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0f;

        while (true)
        {
            // Genera noise su due assi
            float noiseX = (Mathf.PerlinNoise(elapsedTime, 0f) - 0.5f) * amplitude;
            float noiseY = (Mathf.PerlinNoise(0f, elapsedTime) - 0.5f) * amplitude;

            // Calcola nuove coordinate
            float newX = originalPos.x + noiseX;
            float newY = originalPos.y + noiseY;

            // Se abilitato, applica la trasformazione
            if (shakeObject)
                transform.localPosition = new Vector3(
                    X ? newX : originalPos.x,
                    Y ? newY : originalPos.y,
                    originalPos.z);

            // Aggiorna offsetVector per lettura esterna
            offsetVector = new Vector2(X ? noiseX : 0f, Y ? noiseY : 0f);

            // Incrementa il tempo di campionamento
            elapsedTime += Time.deltaTime * frequency;

            yield return null;
        }
    }
}
