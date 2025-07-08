using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Pulsante che, se premuto, invia al GameController il tipo di missione scelto.
/// </summary>
public class MissionSelectButton : MonoBehaviour
{
    // Tipo di missione associato (set in Inspector)
    [SerializeField] private GameData.MissionType missionType;

    private GameController gameController;
    private Button button;

    private void Awake()
    {
        // Trova il controller e il componente Button
        gameController = FindObjectOfType<GameController>();
        button = GetComponent<Button>();
        // Aggiunge listener all’onClick
        button.onClick.AddListener(SelectMission);
    }

    /// <summary>
    /// Callback per il click: notifica il GameController della missione scelta.
    /// </summary>
    private void SelectMission()
    {
        gameController.MissionSelect(missionType);
    }
}
