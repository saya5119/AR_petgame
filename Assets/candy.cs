using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MediaPipe.HandPose;

public class candy : MonoBehaviour
{
    // Start is called before the first frame update
    public Button candybutton;
    private HandAnimator HandAnimatorscript;
    [SerializeField] public GameObject hand5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Candybutton(){
        HandAnimatorscript = hand5.GetComponent<HandAnimator>();
        HandAnimatorscript.SetHandItem(5);
    }
}
