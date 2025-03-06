using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeControls : MonoBehaviour {
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public TMP_Text jump, right, left, use,
                    jump2, right2, left2, use2,
                    jump3, right3, left3, use3,
                    jump4, right4, left4, use4;

    private GameObject currentKey;

    Player player;

    private Color32 normal = new Color32(255, 255, 255, 255);
    private Color32 selected = new Color32(97, 162, 255, 255);

    private int playerNum;
    private int buttonNum;
    private bool sameKeyCode = false;
    public GameObject keyAlreadyTaken;

    int numOfPlayers = joinedPlayers.playablePlayers;

    // Start is called before the first frame update
    void Start() {
        keys.Add("jump", Controls.defaultKeys[0][0]);
        keys.Add("left", Controls.defaultKeys[0][1]);
        keys.Add("right", Controls.defaultKeys[0][2]);  
        keys.Add("use", Controls.defaultKeys[0][3]);

        keys.Add("jump2", Controls.defaultKeys[1][0]);
        keys.Add("right2", Controls.defaultKeys[1][1]);
        keys.Add("left2", Controls.defaultKeys[1][2]);
        keys.Add("use2", Controls.defaultKeys[1][3]);

        keys.Add("jump3", Controls.defaultKeys[2][0]);
        keys.Add("right3", Controls.defaultKeys[2][1]);
        keys.Add("left3", Controls.defaultKeys[2][2]);
        keys.Add("use3", Controls.defaultKeys[2][3]);

        keys.Add("jump4", Controls.defaultKeys[3][0]);
        keys.Add("right4", Controls.defaultKeys[3][1]);
        keys.Add("left4", Controls.defaultKeys[3][2]);
        keys.Add("use4", Controls.defaultKeys[3][3]);

        jump.text = keys["jump"].ToString();
        left.text = keys["left"].ToString();
        right.text = keys["right"].ToString();
        use.text = keys["use"].ToString();

        jump2.text = keys["jump2"].ToString();
        left2.text = keys["left2"].ToString();
        right2.text = keys["right2"].ToString();
        use2.text = keys["use2"].ToString();

        jump3.text = keys["jump3"].ToString();
        left3.text = keys["left3"].ToString();
        right3.text = keys["right3"].ToString();
        use3.text = keys["use3"].ToString();

        jump4.text = keys["jump4"].ToString();
        left4.text = keys["left4"].ToString();
        right4.text = keys["right4"].ToString();
        use4.text = keys["use4"].ToString();
    }

    public void OnGUI() {
        Event e = Event.current;
        if (currentKey != null) {
            if (e.isKey) {

                Controls.defaultKeys[playerNum][buttonNum] = e.keyCode;

                //Check hvis 2 controls er de samme
                numOfPlayers = 4;
                for (int i = 0; i < numOfPlayers; i++) {
                    for (int j = 0; j < 4; j++) {
                        if (Controls.defaultKeys[i][j] != Controls.defaultKeys[playerNum][buttonNum]) {
                            if (Controls.defaultKeys[i][j] == e.keyCode) {
                                Debug.Log("True");
                                sameKeyCode = true;
                            }
                        } else {
                            Debug.Log(false);
                        }
                    }
                }
                
                if (!sameKeyCode) {
                    currentKey.GetComponent<Image>().color = normal;
                    keys[currentKey.name] = e.keyCode;
                    currentKey.transform.GetChild(1).GetComponent<TMP_Text>().text = e.keyCode.ToString();
                    currentKey = null;
                    Controls.UpdateControls(Player.players[playerNum]);
                    //keyAlreadyTaken.SetActive(false);
                } else {
                    sameKeyCode = false;
                    currentKey.GetComponent<Image>().color = normal;
                    keys[currentKey.name] = e.keyCode;
                    currentKey = null;
                    //keyAlreadyTaken.SetActive(true);

                }
            }
        }
    }

    public void ChangeKey(GameObject clicked, int player, int button) {
        playerNum = player;
        buttonNum = button;

        if (currentKey != null) {
                currentKey.GetComponent<Image>().color = normal;
            }
            currentKey = clicked;
            currentKey.GetComponent<Image>().color = selected;
    }
}