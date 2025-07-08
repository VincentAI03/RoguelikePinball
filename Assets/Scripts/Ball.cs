using System;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Questo script controlla la palla nel gioco.
/// Tiene traccia della sua energia in base alla velocità, aggiorna effetti visivi (trail e particelle),
/// gestisce il suono al respawn, la massa della palla, e la carica energetica.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    /// <summary>
    /// Valore pubblico di sola lettura dell'energia corrente della palla.
    /// L'energia aumenta con la velocità (curva personalizzabile) e si mantiene almeno a 1.
    /// </summary>
    public float energy => m_energy;

    /// <summary>
    /// Imposta la massa del Rigidbody della palla.
    /// Utile per potenziamenti o modificatori.
    /// </summary>
    public void SetMass(float mass)
    {
        rb.mass = mass;
    }

    private void Awake()
    {
        // Otteniamo riferimenti ai moduli interni del sistema di particelle.
        rb = GetComponent<Rigidbody>();
        ps_main = ps.main;
        ps_emission = ps.emission;
    }

    private void Update()
    {
        // Aggiorna l'energia in base alla velocità attuale.
        DecayEnergy();
    }

    /// <summary>
    /// Riporta la palla a una determinata posizione (es. per respawn).
    /// Resetta l'energia, disattiva e riattiva il trail per evitare artefatti grafici,
    /// e riproduce un suono.
    /// </summary>
    public void RespawnAt(Vector3 pos)
    {
        RuntimeManager.PlayOneShot(ballLoadSound, transform.position); // Suono di caricamento
        trail.emitting = false;  // Disattiva la scia per evitare artefatti
        transform.position = pos; // Sposta la palla
        rb.velocity = Vector3.zero; // Ferma completamente la palla
        m_energy = 1f; // Reset energia
        trail.emitting = true; // Riattiva la scia
    }

    /// <summary>
    /// Aggiorna l’energia della palla in base alla velocità corrente,
    /// usando una curva personalizzata. Evita che l’energia scenda sotto 1.
    /// </summary>
    private void DecayEnergy()
    {
        float speed = rb.velocity.magnitude; // Calcola la velocità
        float gain = energyGainPerSpeed.Evaluate(speed - minSpeed); // Guarda quanto guadagnare (curva)
        m_energy += gain * Time.deltaTime; // Incrementa l’energia nel tempo
        m_energy = Mathf.Max(1f, m_energy); // Clamp minimo: 1
    }

    /// <summary>
    /// Aumenta direttamente l'energia della palla (es. come bonus).
    /// Emana particelle proporzionali alla velocità attuale.
    /// </summary>
    public void ChargeEnergy(float gain)
    {
        m_energy += gain;
        ps_main.startSpeed = rb.velocity.magnitude; // Velocità iniziale delle particelle
        ps.Emit((int)(rb.velocity.magnitude / 20)); // Emissione in base alla velocità
    }

    [SerializeField] private TrailRenderer trail; // Trail grafico dietro la palla
    [SerializeField] private Rigidbody rb; // Componente fisico 3D
    [SerializeField] private float minSpeed = 60f; // Velocità minima per iniziare a guadagnare energia
    [SerializeField] private AnimationCurve energyGainPerSpeed; // Curva che regola quanto energia guadagnare in base alla velocità
    [SerializeField] public float m_energy = 1f; // Energia interna attuale

    [Header("Sound")]
    [SerializeField] private EventReference ballLoadSound; // Suono per il respawn della palla

    [SerializeField] private ParticleSystem ps; // Effetto particelle visivo
    private ParticleSystem.MainModule ps_main; // Modulo delle impostazioni principali
    private ParticleSystem.EmissionModule ps_emission; // Modulo emissione (non usato direttamente)
}
