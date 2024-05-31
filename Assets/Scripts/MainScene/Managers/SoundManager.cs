using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour 
{
    public static SoundManager Instance;
    public Noise[] Noises;
    public AudioSource MusicSource;
    public SFXType currentType;
    private string menuSceneName = "MainMenu";
    private string gameSceneName = "MainScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this);
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Unsubscribe from the event to avoid memory leaks
    }

    private void Start()
    {
        if (!gameObject.TryGetComponent<AudioSource>(out var audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        MusicSource = audioSource;
        MusicSource.volume = 0.5f;
        StartCoroutine(PlayMusicLoop());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuSceneName)
        {
            ChangeMusicType(SFXType.MenuMusic);
        }
        else if (scene.name == gameSceneName)
        {
            ChangeMusicType(SFXType.GameMusic);
        }
    }


    [CanBeNull]
    public AudioClip FindClip(string name)
    {
        var noise = Noises.FirstOrDefault(x => x.name == name);
        if (string.IsNullOrWhiteSpace(noise.name))
        {
            Debug.Log($"Could not locate sound: {name}");
            return null;
        }
        return noise.clip;
    }


    public AudioClip GetRandomOfType(SFXType type)
    {
        var allOfType = Noises.Where(o => o.type.Equals(type)).ToArray();
        var hits = allOfType.Count();
        if (hits == 0)
        {
            Debug.LogWarning($"No audio clips of type {type} found.");
            return null;
        }
        var noise = allOfType[Random.Range(0, hits)];
        if (string.IsNullOrWhiteSpace(noise.name))
        {
            Debug.Log($"Could not locate sound: {noise.name}");
            return null;
        }

        MusicSource.volume += noise.OverwriteVolumeOffset;
        return noise.clip;
    }
    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            var volumeBackup = MusicSource.volume;
            var clip = GetRandomOfType(currentType);
            if (clip != null)
            {
                MusicSource.clip = clip;
                MusicSource.Play();
                yield return new WaitForSeconds(clip.length);
                MusicSource.volume = volumeBackup;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void ChangeMusicType(SFXType type)
    {
        currentType = type;
        if (MusicSource == null)
        {
            return;
        }
        if (MusicSource.isPlaying)
        {
            MusicSource.Stop();
        }
        StartCoroutine(PlayMusicLoop());
    }
}



[Serializable]
public struct Noise
{
    public string name;
    public SFXType type;
    public AudioClip clip;
    public float OverwriteVolumeOffset;
}

public enum SFXType
{
    MenuMusic,
    GameMusic,
    SoundEffect,
    Misc,
}