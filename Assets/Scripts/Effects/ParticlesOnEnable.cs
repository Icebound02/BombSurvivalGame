using UnityEngine;

public class ParticlesOnEnable : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles = default;

    private void OnEnable()
    {
        particles.Play();
    }
}
