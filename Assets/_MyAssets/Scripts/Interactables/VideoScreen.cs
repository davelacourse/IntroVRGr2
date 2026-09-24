using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Lit une ou plusieurs vidéos sur un écran.
///
/// Réglages attendus sur le Video Player, dans l'inspecteur :
///   Play On Awake décoché, Render Mode = Material Override,
///   Renderer = cet écran, Material Property = _BaseMap.
/// Le son de la vidéo est relié à l'Audio Source de l'écran par le script.
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Renderer))]
public class VideoScreen : MonoBehaviour
{
    [Tooltip("Vidéos disponibles, lues dans l'ordre de la liste.")]
    [SerializeField] List<VideoClip> _clips = new List<VideoClip>();

    [Tooltip("Commencer la lecture dès le lancement de la scène.")]
    [SerializeField] bool _playOnStart;

    VideoPlayer _player;
    AudioSource _audio;
    Renderer _renderer;
    Material _offMaterial;
    Material _onInstance;
    int _index;
    bool _paused;

    void Awake()
    {
        _player = GetComponent<VideoPlayer>();
        _audio = GetComponent<AudioSource>();
        _renderer = GetComponent<Renderer>();

        // Le son de la vidéo passe par l'Audio Source de l'écran : il vient du
        // téléviseur. On le relie ici, parce que l'inspecteur du Video Player
        // n'affiche ce champ que si un clip est déjà placé dans le Video Player.
        _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _player.EnableAudioTrack(0, true);
        _player.SetTargetAudioSource(0, _audio);

        // Matériau de l'écran éteint, et sa copie blanche pour la lecture.
        _offMaterial = _renderer.sharedMaterial;
        _onInstance = new Material(_offMaterial);
        _onInstance.SetColor("_BaseColor", Color.white);
    }

    void Start()
    {
        if (_playOnStart)
            Play();
        else
            Stop();
    }

    /// <summary>Lit la vidéo courante depuis le début.</summary>
    public void Play()
    {
        if (_clips.Count == 0)
            return;

        _player.clip = _clips[_index];
        _renderer.sharedMaterial = _onInstance;

        _player.Play();
        _paused = false;
    }

    public void Stop()
    {
        _player.Stop();
        _renderer.sharedMaterial = _offMaterial;
        _paused = false;
    }

    /// <summary>Met en pause, reprend, ou démarre si rien ne jouait.</summary>
    public void TogglePlayPause()
    {
        if (_player.isPlaying)
        {
            _player.Pause();
            _paused = true;
        }
        else if (_paused)
        {
            _player.Play();
            _paused = false;
        }
        else
        {
            Play();
        }
    }

    public void NextClip()
    {
        if (_clips.Count == 0)
            return;

        _index = (_index + 1) % _clips.Count;
        Play();
    }

    public void PreviousClip()
    {
        if (_clips.Count == 0)
            return;

        // On ajoute Count avant le modulo : en C#, -1 % n vaut -1, pas n - 1.
        _index = (_index - 1 + _clips.Count) % _clips.Count;
        Play();
    }

    void OnDestroy()
    {
        // Le matériau copié dans Awake n'appartient qu'à cet écran : on le libère.
        Destroy(_onInstance);
    }
}
