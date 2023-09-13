using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class testGetMoth : MonoBehaviour
{
    

    

    public void Start()
    {

       
        Type f=typeof(func);
        

        foreach (var m in f.GetEvents()) { 
        
        Debug.Log("事件=>"+m.Name);
        
        
        }

        foreach (var m in f.GetMethods())
        {

            Debug.Log("方法=>"+m.Name);


        }
    }


}


public class func
{
    public event Action OneAct;

    public event Func<bool> TwoFunc;
    public void One()
    {
    }

    public void Two() 
    { 
    }
}
