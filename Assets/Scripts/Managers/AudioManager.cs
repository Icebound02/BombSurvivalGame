using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager singleton;

    [SerializeField] private AudioSource[] audioSources = null;

    private void Awake()
    {
        singleton = this;
    }

    public static void PlayAudioAt(AudioClip clip, Vector3 position, float pitch = 1f)
    {
        AudioSource audioSource = singleton.GetFreeAudioSource();
        if(!audioSource)
        {
            Debug.LogWarning("[AudioManager]: No sources available.");
            return;
        }
         audioSource.pitch = pitch;
        audioSource.transform.position = position;
        audioSource.PlayOneShot(clip);
    }

    private AudioSource GetFreeAudioSource()
    {
        for(int i = 0; i < audioSources.Length; ++i)
        {
            if(!audioSources[i].isPlaying)
                return audioSources[i];
        }
        return null;
    }
}
