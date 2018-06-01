using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textManager : MonoBehaviour {

    public RL player;
    public MinotaurController mino;
    public Text eatenT;
    public Text exitedT;
	// Update is called once per frame
	void Update () {
        exitedT.text = player.exited.ToString();
        eatenT.text = mino.eaten.ToString();
	}
}
