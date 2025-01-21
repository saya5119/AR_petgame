using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class strewberry : MonoBehaviour
{
    // Start is called before the first frame update
    public Button strewberrybutton;
    private HandAnimator HandAnimatorscript;
    [SerializeField] public GameObject hand3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Strewberrybutton(){
        HandAnimatorscript = hand3.GetComponent<HandAnimator>();
        HandAnimatorscript.SetHandItem(3);
    }
}
