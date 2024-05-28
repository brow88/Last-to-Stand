using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public static class AudioExt
{
    public static void PlaySound(this GameObject gameObject, [CanBeNull] AudioClip audioClip, float? volumeOverwrite = null)
    {
        if (audioClip == null)
        {
            Debug.Log("AudioClip is null! Not playing effect");
            return;
        }
        Debug.Log($"PlaySound - {audioClip.name}");
        if (!gameObject.TryGetComponent<AudioSource>(out var audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        var volume = volumeOverwrite ?? 0.6f; //todo we can make this a setting if we have time
        audioSource.volume = volume;
        audioSource.PlayOneShot(audioClip);
    }

}