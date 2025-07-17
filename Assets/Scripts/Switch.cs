using System;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

[RequireComponent(typeof(Collider))]
public class Switch : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] private bool startOn = false;
    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.red;
    [SerializeField] private EventReference switchSound;

    [Header("Events")]
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    private bool isOn;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        isOn = startOn;
        UpdateVisual();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateVisual();

        // Controllo corretto se l'EventReference è valido
        if (!switchSound.IsNull)
        {
            RuntimeManager.PlayOneShot(switchSound, transform.position);
        }

        if (isOn)
            OnActivate?.Invoke();
        else
            OnDeactivate?.Invoke();
    }

    private void UpdateVisual()
    {
        if (rend != null)
            rend.material.color = isOn ? onColor : offColor;
    }
}
