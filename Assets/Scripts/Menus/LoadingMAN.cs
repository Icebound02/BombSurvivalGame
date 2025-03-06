using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingMAN : MonoBehaviour {

    //Huh?
    public RawImage ri;
    private int count = 0;

    //Just call update as soon as it awakes
    void awake() {
        Update();
    }

    //I have absolutely no idea what I am doing
    void Update() {

        //Frame counter
        count++;
        if(count == 8)
            count = 0;
        
        //I don't know why, but this works
        ri.uvRect = new Rect((float)count/8, 0f, 0.125f, 1f);
    }
}
