using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour

{
    private AudioSource _audioSource;

    [SerializeField] private AudioClip _zapSoundEffect;
    [SerializeField] private AudioClip _cackle;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.LogError("AudioSource missing!");
    }
    public void PlayZap()
    {
        PlaySound(_zapSoundEffect);
    }
    public void PlayDeath()
    {
        PlaySound(_cackle);
    }
    private void PlaySound(AudioClip clip)
    {
        if (_audioSource == null || clip == null) return;
        _audioSource.PlayOneShot(clip);
    }
}
