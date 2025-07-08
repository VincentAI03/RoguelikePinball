using System;
using UnityEngine;

/// <summary>
/// Bersaglio che estende Button: quando colpito,
/// notifica il TargetController e poi si disattiva.
/// </summary>
public class Target : Button
{
    private TargetController targetController;

    /// <summary>
    /// Al Awake prende il riferimento al TargetController nel parent.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        targetController = GetComponentInParent<TargetController>();
    }

    /// <summary>
    /// Chiamato externamente per disattivare il bersaglio senza notificare.
    /// Utile all’inizio per inizializzare lo stato.
    /// </summary>
    public void DeactiveByController()
    {
        base.Deactive();
    }

    /// <summary>
    /// Disattiva il bersaglio quando viene colpito:
    /// 1) Notifica TargetController  
    /// 2) Procede con la logica di Button.Deactive()
    /// </summary>
    public override void Deactive()
    {
        targetController.DeactiveTarget(this);
        base.Deactive();
    }
}
