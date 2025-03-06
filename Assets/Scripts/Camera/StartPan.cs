using UnityEngine;
using System.Collections;

public class StartPan : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float duration = default;
    [SerializeField] private float panToY = default;
    [Header("References")]
    [SerializeField] private MultiplayerCameraFitter camMover = default;
    [SerializeField] private CranePositioner cranePositioner = default;

    private void Start()
    {
        StartCoroutine(PanCamera());
    }

    private IEnumerator PanCamera()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.1f);

        while(Time.unscaledDeltaTime > 0.5f)
            yield return null;

        yield return new WaitForSecondsRealtime(0.25f);

        float startY = transform.position.y;
        float time = 0f;
        while(time < duration)
        {
            if(Time.unscaledDeltaTime > 1f) // Don't move if lagging
                yield return null;

            time += Time.unscaledDeltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.SmoothStep(startY, panToY, time / duration), transform.position.z);
            yield return null;
        }

        cranePositioner.enabled = true;
        camMover.enabled = true;
        Time.timeScale = 1f;
    }
}
