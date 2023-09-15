using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip[] _hitObject;
    public AudioClip HitObj => _hitObject[Random.Range(0, _hitObject.Length)];

    [SerializeField] private AudioClip[] _destroyObject;
    public AudioClip DestroyObj => _destroyObject[Random.Range(0, _destroyObject.Length)];

    [SerializeField] private AudioClip[] _destroyTank;
    public AudioClip DestroyTank => _destroyTank[Random.Range(0, _destroyTank.Length)];

    public void Initialize()
    {
        Instance = this;
    }
}
