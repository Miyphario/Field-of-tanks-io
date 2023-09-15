using UnityEngine;

public class SoundRandomValues : MonoBehaviour
{
    [SerializeField] private float _minPitch = 0.9f;
    [SerializeField] private float _maxPitch = 1.1f;
    [SerializeField] private float _minVolume = 1f;
    [SerializeField] private float _maxVolume = 1f;
    [SerializeField] private bool _playOnAwake = true;
    [SerializeField] private AudioClip[] _audioClips;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.SetRandomPitchAndVolume(_minPitch, _maxPitch, _minVolume, _maxVolume);
        _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Length)];

        if (_playOnAwake)
            _audioSource.Play();
    }
}
