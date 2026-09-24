using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Fait vibrer la main qui interagit avec CET objet, et seulement lui.
///
/// Le Simple Haptic Feedback, posé sur l'interactor, vibre pour tous les objets.
/// Ce script, posé sur l'objet, donne à chaque objet sa propre vibration,
/// envoyée à la main qui a déclenché l'événement.
/// </summary>
[RequireComponent(typeof(XRBaseInteractable))]
public class ObjectHaptics : MonoBehaviour
{
    [Serializable]
    public struct HapticPulse
    {
        [Tooltip("Envoyer une vibration pour cet événement.")]
        public bool enabled;

        [Tooltip("Force de la vibration, de 0 à 1.")]
        [Range(0f, 1f)] public float amplitude;

        [Tooltip("Durée de la vibration, en secondes.")]
        public float duration;
    }

    [SerializeField] HapticPulse _onHover = Pulse(true, 0.1f, 0.1f);
    [SerializeField] HapticPulse _onSelect = Pulse(true, 0.5f, 0.25f);
    [SerializeField] HapticPulse _onActivate = Pulse(false, 0.7f, 0.15f);

    static HapticPulse Pulse(bool enabled, float amplitude, float duration)
    {
        return new HapticPulse
        {
            enabled = enabled,
            amplitude = amplitude,
            duration = duration,
        };
    }

    XRBaseInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();
    }

    void OnEnable()
    {
        _interactable.hoverEntered.AddListener(OnHoverEntered);
        _interactable.selectEntered.AddListener(OnSelectEntered);
        _interactable.activated.AddListener(OnActivated);
    }

    void OnDisable()
    {
        _interactable.hoverEntered.RemoveListener(OnHoverEntered);
        _interactable.selectEntered.RemoveListener(OnSelectEntered);
        _interactable.activated.RemoveListener(OnActivated);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        Send(args.interactorObject.transform, _onHover);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        Send(args.interactorObject.transform, _onSelect);
    }

    void OnActivated(ActivateEventArgs args)
    {
        Send(args.interactorObject.transform, _onActivate);
    }

    // L'interactor est un enfant du contrôleur, et c'est le contrôleur qui porte le
    // Haptic Impulse Player : on remonte donc la hiérarchie à partir de l'interactor.
    static void Send(Transform interactor, HapticPulse pulse)
    {
        if (!pulse.enabled)
            return;

        var player = interactor.GetComponentInParent<HapticImpulsePlayer>();
        if (player != null)
            player.SendHapticImpulse(pulse.amplitude, pulse.duration);
    }
}
