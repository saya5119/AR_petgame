using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class ice : MonoBehaviour
{
    // Start is called before the first frame update
    public Button icebutton;  // ボタンをInspectorで設定
    private HandAnimator HandAnimatorscript;
    [SerializeField] public GameObject hand1;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Icebutton(){
        HandAnimatorscript = hand1.GetComponent<HandAnimator>();
        HandAnimatorscript.SetHandItem(1);
    }
}
