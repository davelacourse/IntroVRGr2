using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Préférences du joueur : volume des groupes du mixer, rotation continue et vignette.
/// Les lit au démarrage, les applique, et les enregistre avec PlayerPrefs chaque fois
/// qu'elles changent. Ne connaît aucun élément d'interface.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    // Clés PlayerPrefs. Pour les volumes, ce sont aussi les noms des paramètres
    // exposés dans l'Audio Mixer.
    const string AmbienceKey = "AmbienceVolume";
    const string SfxKey = "SfxVolume";
    const string SmoothTurnKey = "SmoothTurn";
    const string VignetteKey = "Vignette";

    [SerializeField] AudioMixer _mixer;

    [Tooltip("Le ControllerInputActionManager du RightController.")]
    [SerializeField] ControllerInputActionManager _rightController;

    [Tooltip("Le Tunneling Vignette Controller de la Main Camera.")]
    [SerializeField] TunnelingVignetteController _vignette;

    public float AmbienceVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public bool SmoothTurn { get; private set; }
    public bool Vignette { get; private set; }

    // Awake : on lit les préférences tôt, pour que l'UIManager les trouve prêtes
    // dans son Start. Tous les Awake passent avant tous les Start.
    void Awake()
    {
        AmbienceVolume = PlayerPrefs.GetFloat(AmbienceKey, 1f);
        SfxVolume = PlayerPrefs.GetFloat(SfxKey, 1f);
        SmoothTurn = PlayerPrefs.GetInt(SmoothTurnKey, 0) == 1;
        Vignette = PlayerPrefs.GetInt(VignetteKey, 1) == 1;
    }

    // Start : on les applique. Le mixer n'accepte pas encore SetFloat pendant Awake.
    void Start()
    {
        SetAmbienceVolume(AmbienceVolume);
        SetSfxVolume(SfxVolume);
        SetSmoothTurn(SmoothTurn);
        SetVignette(Vignette);
    }

    public void SetAmbienceVolume(float value)
    {
        AmbienceVolume = ApplyVolume(AmbienceKey, value);
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = ApplyVolume(SfxKey, value);
    }

    public void SetSmoothTurn(bool on)
    {
        SmoothTurn = on;
        _rightController.smoothTurnEnabled = on;
        PlayerPrefs.SetInt(SmoothTurnKey, on ? 1 : 0);
    }

    public void SetVignette(bool on)
    {
        Vignette = on;
        _vignette.enabled = on;
        PlayerPrefs.SetInt(VignetteKey, on ? 1 : 0);
    }

    float ApplyVolume(string parameter, float value)
    {
        // L'oreille perçoit le volume de façon logarithmique :
        // 1 donne 0 dB, 0,0001 donne -80 dB, le silence pour le mixer.
        value = Mathf.Max(value, 0.0001f);
        _mixer.SetFloat(parameter, Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(parameter, value);
        return value;
    }
}
