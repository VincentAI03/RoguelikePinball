using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Gestisce la sequenza di animazione del titolo all’avvio:
/// imposta delay, intensità di emissione e disattiva il GameObject dopo 5s,
/// poi avvia il livello.
/// </summary>
public class Title : MonoBehaviour
{
    // Intensità di emissione da applicare al materiale del font
    [SerializeField] private float EmissionIntensity = 1f;
    // Mesh TMP da aggiornare ogni frame
    [SerializeField] private TextMeshProUGUI[] AffectedMesh;
    // Animator per ogni parte del titolo
    [SerializeField] private Animator[] titleAnimators;
    // Step di delay per le animazioni successive
    [SerializeField] private float delayStep = 0.1f;

    private GameController gameController;

    private void Awake()
    {
        // Trova GameController per avviare il livello dopo il titolo
        gameController = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        // Scatena le animazioni a catena con delay crescente
        for (int i = 0; i < titleAnimators.Length; i++)
        {
            if (i == 0)
                titleAnimators[i].SetTrigger("play");
            else
                titleAnimators[i].SetFloat("delay", 1f / (i * delayStep));
        }
        // Dopo 5 secondi disattiva questo GameObject e avvia il livello
        StartCoroutine(DisableSelf());
    }

    private void Update()
    {
        // Ogni frame aggiorna il colore di emissione del materiale TMP
        foreach (var text in AffectedMesh)
        {
            text.fontSharedMaterial.SetColor(
                ShaderUtilities.ID_FaceColor,
                Color.white * EmissionIntensity);
        }
    }

    /// <summary>
    /// Coroutine che, dopo 5 secondi:
    /// 1) Disattiva il GameObject del titolo  
    /// 2) Chiama GameController.StartLevel()
    /// </summary>
    private IEnumerator DisableSelf()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
        gameController.StartLevel();
    }
}
