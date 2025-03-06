using UnityEngine;

public class CranePositioner : MonoBehaviour
{
    [SerializeField] private float deadzone = 0.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float height = 0.5f;

    [SerializeField] private float clearInterval = 1f;

    private float altitude;
    private float nextClearAltitude;

    private void Awake()
    {
        altitude = transform.position.y;
        nextClearAltitude = transform.position.y - clearInterval;
    }

    private void LateUpdate()
    {
        if(Player.players.Count == 0)
            return;

        float highestAltitude = float.NegativeInfinity;
        for(int i = 0; i < Player.players.Count; ++i)
        {
            if(Player.players[i].transform.position.y > highestAltitude)
                highestAltitude = Player.players[i].transform.position.y;
        }

        float desiredAltitude = highestAltitude + CameraController.singleton.cinemachine.m_Lens.OrthographicSize - height;
        if(desiredAltitude < altitude - deadzone || desiredAltitude > altitude + deadzone)
            altitude = desiredAltitude;
        if(transform.position.y < nextClearAltitude)
        {
            TerrainManager.singleton.ClearCrane(nextClearAltitude);
            nextClearAltitude -= clearInterval;
        }

        //Debug.Log("Movement: " + ((transform.position.y + height) - (CameraController.singleton.transform.position.y + CameraController.singleton.cinemachine.m_Lens.OrthographicSize)));
        transform.position = new Vector3(transform.position.x, Mathf.SmoothStep(transform.position.y, altitude, Time.deltaTime * moveSpeed), transform.position.z);
    }
}
