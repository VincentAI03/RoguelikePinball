using System;
using UnityEngine;
using UnityEngine.UI;
using UIButton = UnityEngine.UI.Button;

/// <summary>
/// Pulsante che, se premuto, invia al GameController il tipo di missione scelto.
/// </summary>
[RequireComponent(typeof(UIButton))]
public class MissionSelectButton : MonoBehaviour
{
    [SerializeField] private GameData.MissionType missionType;

    private GameController gameController;
    private UIButton button;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("MissionSelectButton: GameController non trovato.");
            enabled = false;
            return;
        }

        button = GetComponent<UIButton>();
        if (button == null)
        {
            Debug.LogError("MissionSelectButton: Componente UIButton mancante.");
            enabled = false;
            return;
        }

        button.onClick.AddListener(SelectMission);
    }

    private void SelectMission()
    {
        gameController.MissionSelect(missionType);
    }
}
