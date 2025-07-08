using System;
using UnityEngine;

/// <summary>
/// In editor o runtime, questo script disegna una linea tramite LineRenderer
/// a partire da una lista di punti predefiniti (rappresentativi di un bordo).
/// Versione 3D — non usa EdgeCollider2D ma richiede punti manuali.
/// </summary>
[ExecuteInEditMode]
public class DrawLineFromEdge : MonoBehaviour
{
    private void Awake()
    {
        // Ottiene il LineRenderer attaccato all'oggetto
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // In editor o runtime, disegna la linea aggiornata
        DrawLineAlongEdge();
    }

    /// <summary>
    /// Aggiorna il LineRenderer usando i punti assegnati.
    /// Utile per debug o rappresentazioni visive.
    /// </summary>
    private void DrawLineAlongEdge()
    {
        if (edgePoints == null || edgePoints.Length == 0 || lineRenderer == null)
            return;

        lineRenderer.positionCount = edgePoints.Length;
        lineRenderer.SetPositions(edgePoints);
    }

    [SerializeField] private LineRenderer lineRenderer;

    [Tooltip("Lista di punti 3D da usare per disegnare la linea (modifica manuale o editor)")]
    [SerializeField] private Vector3[] edgePoints;
}
