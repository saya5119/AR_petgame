using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class chocolate : MonoBehaviour
{
    // Start is called before the first frame update
    public Button chocolatebutton;
    private HandAnimator HandAnimatorscript;
    [SerializeField] public GameObject hand2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Chocolatebutton(){
        HandAnimatorscript = hand2.GetComponent<HandAnimator>();
        HandAnimatorscript.SetHandItem(2);
    }
}
