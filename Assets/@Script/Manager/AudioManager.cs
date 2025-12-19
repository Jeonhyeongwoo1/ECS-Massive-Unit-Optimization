using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MewVivor;
using MewVivor.Enum;
using MewVivor.Key;
using UnityEngine;
using Object = UnityEngine.Object;

public class AudioManager
{
    private List<AudioSource> _audioSourceList = new((int)Sound.Max);
    private AudioSource _bgmAudioSource;
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private GameObject _audio;
    private bool _isInitialize;

    public void Initialize()
    {
        if (_isInitialize)
        {
            return;
        }

        _isInitialize = true;
        _audio = new GameObject("@AudioRoot");
        Object.DontDestroyOnLoad(_audio);

        GameObject newAudio = new GameObject(Sound.BGM.ToString());
        AudioSource audioSource = newAudio.AddComponent<AudioSource>();
        _bgmAudioSource = audioSource;
        _bgmAudioSource.loop = true;
        _bgmAudioSource.transform.SetParent(_audio.transform);

        // int length = Enum.GetNames(typeof(Sound)).Length;        
        // for (int i = 0; i < length; i++)
        // {
        //     Sound sound = (Sound)i;
        //     GameObject newAudio = new GameObject(sound.ToString());
        //     AudioSource audioSource = newAudio.AddComponent<AudioSource>();
        //     _audioSourceList.Add(audioSource);
        //     newAudio.transform.SetParent(_audio.transform);
        //
        //     if (sound == Sound.BGM)
        //     {
        //         audioSource.loop = true;
        //     }
        //     else
        //     {
        //         audioSource.loop = false;
        //     }
        // }
    }

    public async UniTask<AudioSource> Play(Sound sound, string key, float pitch = 0.5f, float volume = 0.8f)
    {
        if (!_isInitialize)
        {
            Initialize();
        }

        AudioClip audioClip = null;
        switch (sound)
        {
            case Sound.BGM:
                if (!Manager.I.IsOnBGM)
                {
                    return null;
                }

                AudioSource audioSource = _bgmAudioSource;
                audioClip = LoadAudioClip(key);
                await ChangeBGMSound(audioSource, audioClip, 0.5f, volume);
                return audioSource;
            case Sound.SFX:
                if (!Manager.I.IsOnSfx)
                {
                    return null;
                }
                
                audioSource = _audioSourceList.Find(v=> !v.isPlaying);
                if (audioSource == null)
                {
                    //최대 30개까지만
                    const int max = 30;
                    if (_audioSourceList.Count > max)
                    {
                        return null;
                    }
                    
                    GameObject newAudio = new GameObject(sound.ToString());
                    audioSource = newAudio.AddComponent<AudioSource>();
                    _audioSourceList.Add(audioSource);
                    newAudio.transform.SetParent(_audio.transform);
                }

                audioClip = LoadAudioClip(key);
                audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                audioSource.PlayOneShot(audioClip);
                return audioSource;
            case Sound.Max:
                break;
        }
        
        return null;
    }
    
    public async void Play(Sound sound, AudioClip audioClip, float pitch = 1.0f, float volume = 0.5f)
    {
        if (!_isInitialize)
        {
            Initialize();
        }
        
        AudioSource audioSource = null;
        switch (sound)
        {
            case Sound.BGM:
                if (!Manager.I.IsOnBGM)
                {
                    return;
                }

                audioSource = _bgmAudioSource;
                await ChangeBGMSound(audioSource, audioClip, 0.5f, volume);
                break;
            case Sound.SFX:
                if (!Manager.I.IsOnSfx)
                {
                    return;
                }
                
                audioSource = _audioSourceList.Find(v=> !v.isPlaying);
                if (audioSource == null)
                {
                    GameObject newAudio = new GameObject(sound.ToString());
                    audioSource = newAudio.AddComponent<AudioSource>();
                    _audioSourceList.Add(audioSource);
                    newAudio.transform.SetParent(_audio.transform);
                }

                // audioSource = _audioSourceList[(int)Sound.SFX];
                audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.PlayOneShot(audioClip);
                break;
            case Sound.Max:
                break;
        }
    }

    public void Stop(Sound sound)
    {
        if (!_isInitialize)
        {
            Initialize();
        }

        switch (sound)
        {
            case Sound.BGM:
                AudioSource audioSource = _bgmAudioSource;
                audioSource.Stop();
                break;
            case Sound.SFX:
                // audioSource = _audioSourceList[(int)Sound.SFX];
                // audioSource.Stop();
                break;
            case Sound.Max:
                break;
        }
    }

    public void StopFadeBGM()
    {
        _bgmAudioSource.DOFade(0, 0.5f);
    }

    public void AllSfxStop()
    {
        _audioSourceList.ForEach(v=> v?.Stop());
    }

    private async UniTask ChangeBGMSound(AudioSource audioSource, AudioClip audioClip, float fadeDuration, float volume = 1f)
    {
        audioSource.DOFade(0, fadeDuration);
        await UniTask.WaitForSeconds(fadeDuration);

        audioSource.Stop();

        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.DOFade(volume, fadeDuration);
        await UniTask.WaitForSeconds(fadeDuration);
    }

    private AudioClip LoadAudioClip(string key)
    {
        AudioClip audioClip = null;
        if (_audioClips.TryGetValue(key, out audioClip))
        {
            return audioClip;
        }

        audioClip = Manager.I.Resource.Load<AudioClip>(key);

        if (!_audioClips.ContainsKey(key))
        {
            _audioClips.Add(key, audioClip);
        }

        return audioClip;
    }
}