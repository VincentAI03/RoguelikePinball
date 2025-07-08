using System;
using UnityEngine;

/// <summary>
/// Bumper che spinge la palla nella direzione opposta alla normale del contatto.
/// Estende Bumper base e ridefinisce la direzione d’urto.
/// </summary>
public class SurfaceBumper : Bumper
{
    /// <summary>
    /// Calcola la direzione del bump usando la normale del primo punto di contatto.
    /// </summary>
    protected override Vector3 BumpDirection(Collision collision)
    {
        // -normal per spingere la palla fuori dalla superficie
        return -collision.GetContact(0).normal;
    }
}
