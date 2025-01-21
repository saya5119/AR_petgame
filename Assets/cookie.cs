using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class cookie : MonoBehaviour
{
    // Start is called before the first frame update
    public Button cookiebutton;
    private HandAnimator HandAnimatorscript;
    [SerializeField] public GameObject hand4;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Cookiebutton(){
        HandAnimatorscript = hand4.GetComponent<HandAnimator>();
        HandAnimatorscript.SetHandItem(4);
    }
}
