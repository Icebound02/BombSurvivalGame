using UnityEngine;

public class PlayerPowerup : MonoBehaviour
{
    [SerializeField] private Player player = default;

    public SpriteRenderer torchRenderer;
    public SpriteRenderer powerupRenderer;
    [SerializeField] private Transform powerupSlider = default;
    [SerializeField] private GameObject powerupUI = default;

    [SerializeField] private AudioClip audioPickup = default;

    public ParticleSystem jetpackParticles;

    public Powerup powerup { get; private set; }

    private void Update()
    {
        if(!powerup)
            return;
        if(powerup.isFixedUpdate)
            return;
        HandleInput();
    }

    private void FixedUpdate()
    {
        if(!powerup)
            return;
        if(!powerup.isFixedUpdate)
            return;
        HandleInput();
    }

    private void HandleInput()
    {
        if((powerup.useOnce && player.GetKeyDown(powerup.useKey)) || (!powerup.useOnce && player.GetKey(powerup.useKey)))
        {
            powerup.use.Invoke();
            powerupSlider.localScale = new Vector3(1f, (powerup.maxUsageTime - powerup.usageTime) / powerup.maxUsageTime, 1f);
            if(powerup.usageTime > powerup.maxUsageTime)
            {
                powerup.stopUsing.Invoke();
                Destroy(powerup.gameObject);
                SetPowerup(null);
            }
        }
        else
            powerup.stopUsing.Invoke();
    }

    public void SetPowerup(Powerup powerup)
    {
        if(powerup)
        {
            if(powerup.toRotate)
                torchRenderer.sprite = powerup.spriteRenderer.sprite;
            else
                powerupRenderer.sprite = powerup.spriteRenderer.sprite;
            powerupRenderer.sortingOrder = powerup.sortingOrder;
            powerupSlider.localScale = Vector3.one;
            powerupUI.SetActive(true);
            player.audioSource.PlayOneShot(audioPickup);
        }
        else
        {
            if(this.powerup.toRotate)
                torchRenderer.sprite = null;
            else
                powerupRenderer.sprite = null;
            powerupUI.SetActive(false);
            jetpackParticles.Stop();
            player.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<Animator>().enabled = false;
        }
        this.powerup = powerup;
    }
}
