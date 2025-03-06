using UnityEngine;
using UnityEngine.Events;

public abstract class Powerup : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int sortingOrder;
    [SerializeField] private int playerLayer = default;

    public float maxUsageTime = 1f;
    [System.NonSerialized] public float usageTime;

    protected Player player;

    [System.NonSerialized] public KeyMaps useKey;
    [System.NonSerialized] public bool isFixedUpdate;
    [System.NonSerialized] public bool useOnce;
    [System.NonSerialized] public bool toRotate;
    [System.NonSerialized] public UnityEvent use = new UnityEvent();
    [System.NonSerialized] public UnityEvent stopUsing = new UnityEvent();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == playerLayer)
        {
            player = collider.gameObject.GetComponent<Player>();
            if(!player.powerup.powerup)
                GetPowerup();
        }
    }

    protected virtual void GetPowerup()
    {
        player.powerup.SetPowerup(this);
        gameObject.SetActive(false);
    }

    protected abstract void UsePowerup();

    protected virtual void StopUsingPowerup()
    {
    }
}
