using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceEffects;
    [SerializeField] private AudioSource _audioSourceMusic;

    private float _baseVolume;
    private float _pauseVolume = 0.05f;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        _baseVolume = _audioSourceMusic.volume;

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        _audioSourceEffects.PlayOneShot(clip);
    }

    public void Pause()
    {
        _audioSourceMusic.volume = _pauseVolume;
    }

    public void Unpause()
    {
        _audioSourceMusic.volume = _baseVolume;
    }
}