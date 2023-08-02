using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomPropertyDrawer(typeof(RandomizeAttribute))]//類似於一種用類別定義,會成為小類別去反應在檢查器上
public class EditorTest : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 48f;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Float)
        {
            // label = EditorGUI.BeginProperty(position, new GUIContent("測試看看數值名稱"), property);

            Rect aa = new Rect(position.x, position.y, position.width, 16);

            Rect bb = new Rect(position.x, position.y + aa.height, position.width, 16);

            Rect cc = new Rect(position.x, position.y + (aa.height*2), position.width, 16);

            var lg= EditorGUI.BeginProperty(position,label,property);
            
           


            // EditorGUI.LabelField(aa, label, lg);//顯示字串

            EditorGUI.LabelField(aa, label, new GUIContent(property.floatValue.ToString()));//顯示字串

            EditorGUI.Slider(cc,property.floatValue,0,10);//滑動塊

            // if (GUI.Button(bb, "fff"))
            // {

            //     RandomizeAttribute ran = (RandomizeAttribute)attribute;

            //     property.floatValue = Random.Range(ran.minValue, ran.maxValue);

            // }

            // EditorGUI.EndProperty();

        }
        else
        {

            EditorGUI.LabelField(position, "abc");
            
        }
    }

}
