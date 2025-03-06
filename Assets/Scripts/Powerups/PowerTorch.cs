using UnityEngine;

public class PowerTorch : Powerup
{
    [SerializeField] private float digInterval = 0.5f;
    private float nextDig;

    protected override void GetPowerup()
    {
        useKey = KeyMaps.Use;
        use.AddListener(UsePowerup);
        stopUsing.AddListener(StopUsingPowerup);
        toRotate = true;

        base.GetPowerup();
    }

    protected override void UsePowerup()
    {
        if(Time.time < nextDig)
            return;

        player.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<Animator>().enabled = true;
        player.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).transform.localScale = new Vector3(0.5f, 0.5f, 1);
        player.TorchAnim.SetBool("IsActive", true);
        TerrainManager.singleton.Explode(player.powerup.torchRenderer.transform.position, Mathf.RoundToInt(0.55f * TerrainManager.singleton.PPU));

        nextDig = Time.time + digInterval;
        usageTime += 1f;
    }

    protected override void StopUsingPowerup() {
        player.TorchAnim.SetBool("IsActive", false);
    }
}
