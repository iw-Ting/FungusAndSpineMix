using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TestControlSpine : MonoBehaviour
{
    public GameObject ControlRole = null;


    public void Start()
    {
        ControlRole.GetComponent<SkeletonAnimation>().AnimationName = "delight";
        
        ControlRole.GetComponent<SkeletonAnimation>().loop = false;
        // ControlRole.GetComponent<SkeletonAnimation>().enabled = false;

    }


}


