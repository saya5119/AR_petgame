using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Call : MonoBehaviour
{
    // Start is called before the first frame update
    Character Characterscript;
    public Button button;  // ボタンをInspectorで設定
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Callbutton(){
        GameObject obj = GameObject.FindWithTag("character");
        Characterscript = obj.GetComponent<Character>();    
        Characterscript.tocameramove = true;
    }
}
