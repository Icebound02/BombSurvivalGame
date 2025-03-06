using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bomb : MonoBehaviour
{
    //Color variables
    [SerializeField] Color PrimaryColor = default;
    [SerializeField] Color SecondaryColor = default;

    //The different bomb states
    public enum BombPhases {
        Ticking,
        FlashingPrimary,
        FlashingSecondary,
        Exploding
    }
    
    [Header("Bombe lys blink ting")]
    public Light2D[] bombLights;
    [SerializeField] float minIntensity;
    [SerializeField] float maxIntensity;

    //Bomb phase structure
    [System.Serializable]
    public struct BombPhase {
        public double timestamp;
        public BombPhases phaseType;
    }

    //Bomb fields (not dangerous :))
    [Header("Bomb behaviour")]
    public List<BombPhase> phases;
    public bool explodeOnImpact = false;

    //Hidden public variables
    [HideInInspector]
    public double bombTimer = 0;
    [HideInInspector]
    public bool hasExploded = false;

    //Normal variables
    private double flashTimer = 0;
    private int phaseCount = 0;
    private BombPhase phaseCurrent;
    private bool postSinePeak = false;
    private float lastPos = 0;

    //Components
    [Header("Components")]
    public SpriteRenderer sprite = null;
    public Explosion explosionScript = null;
    public AudioSource tickSound = null;
    public Rigidbody2D rb = null;
    
    
    //Start is called before the first frame update
    void Start() {

        //Are all components good?
        if(sprite == null || tickSound == null || rb == null)
            Debug.Log("A component wasn't found :(");

        //Make sure at least one phase exists
        if(phases.Count == 0) {
            Debug.LogError("No bomb phases specified. This is invalid behaviour.");
        }
        else {
            //Set current phase to phase 0
            phaseCurrent = phases[0];
        }
    }

    //Update is called once per frame
    void Update() {

        //Tick timer
        bombTimer += Time.deltaTime;

        //This
        while(bombTimer >= phaseCurrent.timestamp && phaseCount < phases.Count) {
            
            //Move on to next phase
            phaseCount += 1;
            bombTimer -= phaseCurrent.timestamp;
            phaseCurrent = phases[Min(phaseCount, phases.Count-1)];
            flashTimer = 0;
            lastPos = 1;
        }

        //Execute current phase
        switch(phaseCurrent.phaseType) {
            case BombPhases.Ticking: PhaseTick(); break;
            case BombPhases.FlashingPrimary: PhaseFlashPrimary(); break;
            case BombPhases.FlashingSecondary: PhaseFlashSecondary(); break;
            case BombPhases.Exploding: PhaseExplode(); break;
        }
    }

    //My own min function, because I can't be bothered to find out what namespace or class the regular one is in. Plus, it's a one-line function, just let it be. Great weather we're having today huh? According to all known laws of aviation, 
    int Min(int a, int b) {
        return a < b ? a : b;
    }

    //Set everything to normal
    void PhaseTick() {
        sprite.transform.localScale = Vector3.one;
        sprite.color = Color.white;
    }

    //Primary flash phase
    void PhaseFlashPrimary() {

        //Increment timer and get animation step
        flashTimer += Time.deltaTime;
        float animstep = Mathf.Sin((float)flashTimer * 15) / 2f + 0.5f;

        //Play ticking sound
        if(postSinePeak != animstep > 0.5f) {
            postSinePeak = !postSinePeak;
            if(postSinePeak)
                tickSound.Play();
        }

        //Set bomb size
        float scale = animstep / 3f + 0.85f;
        sprite.transform.localScale = new Vector3(scale, scale, scale);
        for(int i = 0; i < bombLights.Length; ++i)
        {
            bombLights[i].intensity = animstep / 2;
            bombLights[i].color = PrimaryColor;
        }

        //Set bomb color
        sprite.color = Color.Lerp(Color.white, PrimaryColor, animstep);
    }

    //Secondary flashing phase
    void PhaseFlashSecondary() {

        //Increment timer and get animation step
        flashTimer += Time.deltaTime * 6;
        float animstep = (float)flashTimer % 1f;

        //Play ticking sound
        if(animstep < lastPos)
            tickSound.Play();
        lastPos = animstep;

        //Set bomb size
        float scale = animstep / 2f + 1f;
        sprite.transform.localScale = new Vector3(scale, scale, scale);
        for(int i = 0; i < bombLights.Length; ++i)
        {
            bombLights[i].intensity = animstep;
            bombLights[i].color = SecondaryColor;
        }

        //Set bomb color
        sprite.color = Color.Lerp(Color.Lerp(Color.white, SecondaryColor, 0.5f), SecondaryColor, animstep);
    }

    //Exploding phase
    void PhaseExplode() {

        //Default explosion script
        if(!hasExploded) {
            hasExploded = true;
            explosionScript.Explode();
        }
    }

    //Template explode function, override when making new bombs
    private void Explode(GameObject gmobj) {
        Debug.Log("No explosion script specified, just destroying instance");
        Destroy(gmobj);
    }

    //Destroy on impact behaviour
    private void OnCollisionEnter2D(Collision2D other) {
        if(explodeOnImpact)
            PhaseExplode();
    }
}
