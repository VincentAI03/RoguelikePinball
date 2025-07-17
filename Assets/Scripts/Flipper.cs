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
    private void Awake()
    {
        // Ottieni le azioni dagli InputActionReference
        if (flipperActionReference != null)
            flipperAction = flipperActionReference.action;
        if (flipperHalfActionReference != null)
            flipperHalfAction = flipperHalfActionReference.action;

        // Salva i limiti angolari originali del joint
        fullLimit = new Vector2(joint.limits.min, joint.limits.max);
        halfLimit = fullLimit * 0.5f;

        // Configura la forza massima del motore
        motor.force = 100000f;

        // Inverte la direzione per il flipper sinistro
        if (flipperType == FlipperType.Left)
            flipperMotorSpeed *= -1f;

        // Abilita le azioni
        flipperAction?.Enable();
        flipperHalfAction?.Enable();
    }

    private void OnDestroy()
    {
        // Disabilita le azioni quando il GameObject viene distrutto
        flipperAction?.Disable();
        flipperHalfAction?.Disable();
    }

    private void Update()
    {
        if (flipperLocked) return;

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

        MotorControl();
    }

    private void SetJointLimit(Vector2 limit)
    {
        JointLimits limits = joint.limits;
        limits.min = limit.x;
        limits.max = limit.y;
        joint.limits = limits;
    }

    private void ActivateMotor()
    {
        motor.targetVelocity = flipperMotorSpeed;
        joint.motor = motor;
        joint.useMotor = true;
    }

    private void DeactivateMotor()
    {
        motor.targetVelocity = -flipperMotorSpeed;
        joint.motor = motor;
        joint.useMotor = true;
    }

    public void SetLock(bool locked)
    {
        flipperLocked = locked;
        if (flipperLocked)
            DeactivateMotor();
    }

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

    [Header("Input Actions")]
    [SerializeField] private InputActionReference flipperActionReference;      // Pressione completa
    [SerializeField] private InputActionReference flipperHalfActionReference;  // Pressione parziale (half-press)

    private InputAction flipperAction;                          
    private InputAction flipperHalfAction;                      

    private Vector2 fullLimit;                                  
    private Vector2 halfLimit;                                  
    private JointMotor motor;                                   
    private bool motorActivated_full = false;
    private bool motorActivated_half = false;
    private bool flipperLocked = false;

    public enum FlipperType { Left, Right }
}
