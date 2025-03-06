using UnityEngine;

public class Carryable : MonoBehaviour
{
    [SerializeField] private int playerLayer = default;

    [SerializeField] private GameObject lights = default;
    [SerializeField] private SpriteRenderer spriteRenderer = default;

    [SerializeField] private Collider2D trigger = default;
    public Rigidbody2D rb;

    private Player carriedBy;

    [SerializeField] private Vector3[] localOffsets = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != playerLayer)
            return;

        
        Player player = collision.gameObject.GetComponent<Player>();
        SetCarrier(player);
    }

    private void SetCarrier(Player player)
    {
        trigger.enabled = false;
        transform.SetParent(player.transform);
        spriteRenderer.transform.localScale = Vector3.one / 2f;
        spriteRenderer.sortingOrder = -3 - player.carriedAliens.Count;
        transform.localPosition = localOffsets[player.carriedAliens.Count];
        if(player.carriedAliens.Count >= localOffsets.Length)
            transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(80f, 100f));
        rb.isKinematic = true;
        lights.SetActive(false);

        ScoreManager.singleton.AddRescueScore(player, 1);
        player.carriedAliens.Add(this);
        carriedBy = player;
    }

    public void Drop()
    {
        trigger.enabled = true;
        transform.SetParent(null);
        spriteRenderer.transform.localScale = Vector3.one;
        spriteRenderer.sortingOrder = -3;
        carriedBy.carriedAliens.Remove(this);
        rb.isKinematic = false;
        lights.SetActive(true);

        transform.position = carriedBy.transform.position;
        ScoreManager.singleton.AddRescueScore(carriedBy, -1);
        carriedBy = null;
    }
}
