using UnityEngine;

public class PlayerToggleLight : MonoBehaviour
{
    [SerializeField] private float darknessAltitude = default;
    [SerializeField] private GameObject lights = default;

    private bool lightToggledOn;

    private void Update()
    {
        if(!lightToggledOn && transform.position.y < darknessAltitude)
        {
            lightToggledOn = true;
            StartCoroutine(LightFlicker.EnableLightFlicker(true, 1.5f, lights));
        }
    }
}
