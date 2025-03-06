using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BombtestObject : MonoBehaviour {

    //Bomb and key struct
    [System.Serializable]
    private struct BKCombo {
        public KeyCode key;
        public Bomb bombType;
    }

    //Bomb and key combos
    [Header("What bomb prefab to spawn per key")]
    [SerializeField]
    private List<BKCombo> combos;

    //Components this needs to work
    [Header("Components")]
    [SerializeField]
    private Camera cam;

    //Update is called once per frame
    void Update() {   

        //Restart scene maybe?
        if(Input.GetKeyDown(KeyCode.Return))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Spawn bombs maybe
        foreach(BKCombo i in combos) {
            if(Input.GetKeyDown(i.key)) {
                Bomb inst = Instantiate(i.bombType, cam.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                inst.transform.position = new Vector3(inst.transform.position.x, inst.transform.position.y, 0);
            }
        }
    }
}
