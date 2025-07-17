using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlla un gruppo di Target: 
/// ne attiva uno a random alla volta,  
/// gestisce il conteggio distruzioni e notifica GameController.
/// </summary>
public class TargetController : MonoBehaviour
{
    private GameController gameController;
    // Lista di bersagli currently inactive
    [SerializeField] private List<Target> deactiveTargets = new List<Target>();

    private void Awake()
    {
        // Trova GameController
        gameController = FindObjectOfType<GameController>();

        // Popola la lista con i Target figli e disattivali
        deactiveTargets = new List<Target>(GetComponentsInChildren<Target>());
        foreach (var t in deactiveTargets.ToArray())
            t.DeactiveByController();

        // Attiva il primo bersaglio a caso
        ActiveRandomTarget();
    }

    /// <summary>
    /// Viene chiamato da Target.Deactive():  
    /// 1) Aggiorna punteggio bersagli  
    /// 2) Attiva un nuovo bersaglio a caso  
    /// 3) Aggiunge il target rimosso in lista per riusarlo in futuro  
    /// </summary>
    public void DeactiveTarget(Target target)
    {
        gameController.TargetStrucked();
        ActiveRandomTarget();
        deactiveTargets.Add(target);
    }

    /// <summary>
    /// Sceglie un bersaglio a caso dalla lista, lo attiva e lo rimuove dalla lista.
    /// </summary>
    private void ActiveRandomTarget()
{
    if (deactiveTargets == null || deactiveTargets.Count == 0)
        return;

    int idx = UnityEngine.Random.Range(0, deactiveTargets.Count); // Namespace esplicito
    var t = deactiveTargets[idx];
    if (t != null)
        t.Active();

    deactiveTargets.RemoveAt(idx);
}

}
