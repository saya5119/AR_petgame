using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class eat : MonoBehaviour
{
    // Start is called before the first frame update
    public Button eatbutton;  // ボタンをInspectorで設定
    private HandAnimator HandAnimatorscript;
    [SerializeField] public GameObject hand;
    public GameObject food;
    void Start()
    {
        food.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Eatbutton(){
        if(food.activeSelf){
            HandAnimatorscript = hand.GetComponent<HandAnimator>();
            HandAnimatorscript.SetHandItem(0);
        }
        food.SetActive(!food.activeSelf);
    }
}
