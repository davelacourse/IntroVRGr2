using UnityEngine;

/// <summary>
/// Allume et éteint un voyant en changeant son matériau.
///
/// Deux méthodes distinctes plutôt qu'une bascule : avec une bascule branchée à la
/// fois sur Activated et sur Deactivated, un seul événement manqué suffit pour que
/// le voyant reste inversé jusqu'à la fin de la partie.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class IndicatorLight : MonoBehaviour
{
    [Tooltip("Matériau du voyant allumé.")]
    [SerializeField] Material _onMaterial;

    Renderer _renderer;
    Material _offMaterial;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();

        // sharedMaterial lit le matériau tel quel, sans en créer de copie.
        // (Lire .material créerait une copie propre à cet objet à chaque fois.)
        _offMaterial = _renderer.sharedMaterial;
    }

    public void TurnOn()
    {
        if (_onMaterial != null)
            _renderer.sharedMaterial = _onMaterial;
    }

    public void TurnOff()
    {
        _renderer.sharedMaterial = _offMaterial;
    }
}
