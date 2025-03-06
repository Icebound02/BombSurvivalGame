using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonScriptThing : MonoBehaviour
{
    private Button myButton;
    [SerializeField] int player = 0;
    [SerializeField] int button = 0;
    private void OnEnable() {
        myButton = gameObject.GetComponent<Button>();
        myButton.onClick.AddListener(() => FindChangeKey(player,button));
    }


    public void FindChangeKey(int player = 0, int button = 0) {
        GameObject.Find("KeybindManager").GetComponent<ChangeControls>().ChangeKey(gameObject, player, button);
    }
}
