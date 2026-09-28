using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface du panneau d'options : place les contrôles selon les préférences
/// actuelles, puis transmet chaque action au bon manager.
/// Ne s'occupe que de l'affichage : aucune logique de jeu ici.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] SettingsManager _settings;
    [SerializeField] Slider _ambienceSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Toggle _smoothTurnToggle;
    [SerializeField] Toggle _vignetteToggle;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _resetButton;

    // S'abonner dans OnEnable et se désabonner dans OnDisable, par paire,
    // comme pour les événements des interactables au cours 6.
    void OnEnable()
    {
        _ambienceSlider.onValueChanged.AddListener(_settings.SetAmbienceVolume);
        _sfxSlider.onValueChanged.AddListener(_settings.SetSfxVolume);
        _smoothTurnToggle.onValueChanged.AddListener(_settings.SetSmoothTurn);
        _vignetteToggle.onValueChanged.AddListener(_settings.SetVignette);
        _quitButton.onClick.AddListener(OnQuitClicked);
        _resetButton.onClick.AddListener(OnResetClicked);
    }

    void OnDisable()
    {
        _ambienceSlider.onValueChanged.RemoveListener(_settings.SetAmbienceVolume);
        _sfxSlider.onValueChanged.RemoveListener(_settings.SetSfxVolume);
        _smoothTurnToggle.onValueChanged.RemoveListener(_settings.SetSmoothTurn);
        _vignetteToggle.onValueChanged.RemoveListener(_settings.SetVignette);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
        _resetButton.onClick.RemoveListener(OnResetClicked);
    }

    // Le SettingsManager a lu les préférences dans son Awake : elles sont prêtes.
    void Start()
    {
        // WithoutNotify : on place le contrôle sans déclencher son événement.
        _ambienceSlider.SetValueWithoutNotify(_settings.AmbienceVolume);
        _sfxSlider.SetValueWithoutNotify(_settings.SfxVolume);
        _smoothTurnToggle.SetIsOnWithoutNotify(_settings.SmoothTurn);
        _vignetteToggle.SetIsOnWithoutNotify(_settings.Vignette);
    }

    // L'instance est lue au moment du clic, pas dans OnEnable : l'OnEnable de
    // l'UIManager peut passer avant l'Awake du GameManager, où Instance est encore null.
    void OnQuitClicked()
    {
        GameManager.Instance.Quit();
    }

    // L'instance est lue au moment du clic, pas dans OnEnable : l'OnEnable de
    // l'UIManager peut passer avant l'Awake du GameManager, où Instance est encore null.
    void OnResetClicked()
    {
        GameManager.Instance.Reset();
    }
}
