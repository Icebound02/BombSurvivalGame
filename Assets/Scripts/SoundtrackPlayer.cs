using UnityEngine;

public class SoundtrackPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = default;
    [SerializeField] private AudioClip loopClip = default;

    private void LateUpdate()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.clip = loopClip;
            audioSource.Play();
        }
    }
}
