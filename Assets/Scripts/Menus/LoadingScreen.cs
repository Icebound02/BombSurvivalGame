using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen singleton;
    public GameObject uhm;
    public GameObject uhh;
    public GameObject umm;


    void Awake() {
        singleton = this;
    }

    public void Show() {
        uhm.SetActive(true);
        uhh.SetActive(true);
        umm.SetActive(true);
    }
}
