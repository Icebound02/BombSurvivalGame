using UnityEngine;

public class MultiplayerCameraFitter : MonoBehaviour
{
    [Tooltip("Y axis")]
    [SerializeField] private float followOffset = 0f;

    [SerializeField] private float zoomSpeed = 1f;

    private float minZoom;

    private void Start()
    {
        minZoom = CameraController.singleton.cinemachine.m_Lens.OrthographicSize;
    }

    private void LateUpdate()
    {
        if(Player.players.Count == 1) // One player
        {
            transform.position = new Vector3(transform.position.x, Player.players[0].transform.position.y + followOffset, transform.position.z);
        }
        else if(Player.players.Count > 1) // Multiple players
        {
            float topAltitude = float.NegativeInfinity;
            float bottomAltitude = float.PositiveInfinity;
            for(int i = 0; i < Player.players.Count; ++i)
            {
                if(Player.players[i].transform.position.y > topAltitude)
                    topAltitude = Player.players[i].transform.position.y;
                if(Player.players[i].transform.position.y < bottomAltitude)
                    bottomAltitude = Player.players[i].transform.position.y;
            }

            transform.position = new Vector3(transform.position.x, GetMiddleAltitude(topAltitude, bottomAltitude) - followOffset, transform.position.z);
            float desiredZoom = (topAltitude - bottomAltitude) / 2f + 5f;
            CameraController.singleton.cinemachine.m_Lens.OrthographicSize = Mathf.Max(Mathf.Lerp(CameraController.singleton.cinemachine.m_Lens.OrthographicSize, desiredZoom, Time.deltaTime * zoomSpeed), minZoom);
        }
    }

    private static float GetMiddleAltitude(float topAltitude, float bottomAltitude)
    {
        return Mathf.Lerp(bottomAltitude, topAltitude, 0.5f);
    }
}
