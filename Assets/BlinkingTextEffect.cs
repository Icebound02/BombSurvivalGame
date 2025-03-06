using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BlinkingTextEffect : MonoBehaviour
{

    public float timeActive = 1.6f;
    public float timeDeactived = 0.6f;

    public void Awake()
    {
        StartCoroutine(BlinkingEffect());
    }

    IEnumerator BlinkingEffect()
    {
        while(true)
        {
            gameObject.GetComponent<TMP_Text>().enabled = true;
            yield return new WaitForSeconds(timeActive);
            gameObject.GetComponent<TMP_Text>().enabled = false;
            yield return new WaitForSeconds(timeDeactived);
        }
    }

}
