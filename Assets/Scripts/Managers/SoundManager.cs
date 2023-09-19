using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixer _mixer;
    private const string MASTER_VOLUME = "MasterVolume";

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

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        _mixer.SetFloat(MASTER_VOLUME, Mathf.Log10(volume) * 20f);
    }

    public float GetMasterVolume()
    {
        _mixer.GetFloat(MASTER_VOLUME, out float volume);
        if (volume == 0f) return 1f;

        volume = Mathf.Pow(10, volume / 20f);
        return volume;
    }
}
