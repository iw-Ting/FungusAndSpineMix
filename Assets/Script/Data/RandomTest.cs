using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class RandomizeAttribute : PropertyAttribute
{

    public readonly float minValue;
    public readonly float maxValue;

    public RandomizeAttribute(float min, float max)
    {
        
        minValue = min;
        maxValue = max;
        

    }

}