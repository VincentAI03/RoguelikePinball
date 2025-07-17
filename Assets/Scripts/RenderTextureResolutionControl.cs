using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

/// <summary>
/// Assicura che la RenderTexture usata dalla camera e dall’effetto “CRT”
/// abbia sempre la risoluzione dello schermo corrente.
/// </summary>
[ExecuteAlways]
public class RenderTextureResolutionControl : MonoBehaviour
{
    // La RenderTexture usata come target della camera
    public RenderTexture renderTexture;

    // Immagine UI o quad con materiale CRT dove mostrare il buffer
    [SerializeField] private Image CRTImageRender;
    // La camera che renderizza nella RenderTexture
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        // Al primo avvio, crea o aggiorna la RT
        UpdateRenderTexture();
    }

    private void Update()
    {
        // Se risoluzione schermo è cambiata, ricrea la RT
        if (renderTexture.width != Screen.width
         || renderTexture.height != Screen.height)
        {
            UpdateRenderTexture();
        }
    }

    /// <summary>
    /// Rilascia (se esiste) e ricrea la RenderTexture con i nuovi parametri.
    /// Imposta il formato ad alta precisione e aggiorna materiali e aspect.
    /// </summary>
    private void UpdateRenderTexture()
    {
        // Se esiste già, libera la memoria
        if (renderTexture != null)
            renderTexture.Release();
        else
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        // Ridefinisci dimensioni e formato
        renderTexture.width  = Screen.width;
        renderTexture.height = Screen.height;
        renderTexture.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

        // Imposta la RenderTexture come target della camera
        targetCamera.targetTexture = renderTexture;

        // Passa la RT e lo scale al materiale CRT
        CRTImageRender.material.SetFloat("_CRTScale", Screen.width / 3f);
        CRTImageRender.material.SetTexture("_RenderTexture", renderTexture);

        // Aggiorna il rapporto d’aspetto della camera
        targetCamera.aspect = (float)Screen.width / Screen.height;
    }
}
