using UnityEngine;

public class PowerJetpack : Powerup
{
    [SerializeField] private int jetpackFallingPower = 60;
    [SerializeField] private int jetpackPower = 20;

    protected override void GetPowerup()
    {
        useKey = KeyMaps.Jump;
        use.AddListener(UsePowerup);
        stopUsing.AddListener(StopUsingPowerup);
        isFixedUpdate = true;

        base.GetPowerup();
    }

    protected override void UsePowerup()
    {
        player.movement.rb.mass = 0.6f;
        if(player.movement.rb.velocity.y < 0)
            player.movement.rb.AddForce(player.transform.up * jetpackFallingPower);
        else
            player.movement.rb.AddForce(player.transform.up * jetpackPower);

        player.powerup.jetpackParticles.Play();

        usageTime += Time.fixedDeltaTime;
    }

    protected override void StopUsingPowerup()
    {
        player.movement.rb.mass = 1f;

        player.powerup.jetpackParticles.Stop();
    }
}
