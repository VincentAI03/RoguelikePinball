using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script che controlla il comportamento di un flipper 3D.
/// Utilizza un HingeJoint con motore e limiti angolari, e risponde agli input tramite il nuovo Input System.
/// </summary>
public class Flipper : MonoBehaviour
{
    private void Start()
    {
        // Salva i limiti angolari originali del joint
        fullLimit = new Vector2(joint.limits.min, joint.limits.max);
        halfLimit = fullLimit * 0.5f;

        // Configura la forza massima del motore
        motor.force = 100000f;

        // Determina tipo di flipper e azioni input
        switch (flipperType)
        {
            case FlipperType.Left:
                flipperMotorSpeed *= -1f; // Inverte direzione per flipper sinistro
                flipperAction = InputSystem.actions.FindAction("Flipper_L_Full", false);
                flipperHalfAction = InputSystem.actions.FindAction("Flipper_L_Half", false);
                break;

            case FlipperType.Right:
                flipperAction = InputSystem.actions.FindAction("Flipper_R_Full", false);
                flipperHalfAction = InputSystem.actions.FindAction("Flipper_R_Half", false);
                break;
        }
    }

    private void Update()
    {
        if (flipperLocked) return;

        // Controllo half-press
        if (flipperHalfAction != null)
        {
            if (flipperHalfAction.WasPressedThisFrame())
            {
                SetJointLimit(halfLimit);
                motorActivated_half = true;
            }
            if (flipperHalfAction.WasReleasedThisFrame())
            {
                SetJointLimit(fullLimit);
                motorActivated_half = false;
            }
        }

        // Controllo full-press
        if (flipperAction != null)
        {
            if (flipperAction.WasPressedThisFrame())
            {
                RuntimeManager.PlayOneShot(flipSound, transform.position);
                SetJointLimit(fullLimit);
                motorActivated_full = true;
            }
            if (flipperAction.WasReleasedThisFrame())
            {
                SetJointLimit(halfLimit);
                motorActivated_full = false;
            }
        }

        MotorControl(); // Applica motore se attivo
    }

    /// <summary>
    /// Applica i nuovi limiti angolari al flipper.
    /// </summary>
    private void SetJointLimit(Vector2 limit)
    {
        JointLimits limits = joint.limits;
        limits.min = limit.x;
        limits.max = limit.y;
        joint.limits = limits;
    }

    /// <summary>
    /// Attiva il motore del flipper con la velocità impostata.
    /// </summary>
    private void ActivateMotor()
    {
        motor.targetVelocity = flipperMotorSpeed;
        joint.motor = motor;
        joint.useMotor = true;
    }

    /// <summary>
    /// Inverte il motore (ritorno alla posizione di riposo).
    /// </summary>
    private void DeactivateMotor()
    {
        motor.targetVelocity = -flipperMotorSpeed;
        joint.motor = motor;
        joint.useMotor = true;
    }

    /// <summary>
    /// Blocca o sblocca il flipper.
    /// </summary>
    public void SetLock(bool locked)
    {
        flipperLocked = locked;
        if (flipperLocked)
            DeactivateMotor();
    }

    /// <summary>
    /// Gestisce quale direzione deve avere il motore in base allo stato attuale.
    /// </summary>
    private void MotorControl()
    {
        if (motorActivated_full || motorActivated_half)
            ActivateMotor();
        else
            DeactivateMotor();
    }

    [SerializeField] private FlipperType flipperType;           // Sinistra o destra
    [SerializeField] private HingeJoint joint;                  // Joint fisico del flipper (3D)
    [SerializeField] private float flipperMotorSpeed = 1000f;   // Velocità del motore
    [SerializeField] private EventReference flipSound;          // Suono quando viene attivato

    private InputAction flipperAction;                          // Input completo (press)
    private InputAction flipperHalfAction;                      // Input parziale (half-press)

    private Vector2 fullLimit;                                  // Limiti di angolo completi
    private Vector2 halfLimit;                                  // Limiti a metà flipper
    private JointMotor motor;                                   // Motor 3D
    private bool motorActivated_full = false;
    private bool motorActivated_half = false;
    private bool flipperLocked = false;

    public enum FlipperType { Left, Right }
}
