using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace Fungus
{

    public enum DisplayEffect
    {
        None,
        Fade,
        Move
    }
    [Serializable]
    public class MenuImageOption: PropertyAttribute
    {
        public Block targetBlock;
        public Sprite _image=null;
        public Vector2 imageSize = Vector2.zero;
        [FormerlySerializedAs("targetSequence")]
        public InputCallBack.InputOptions _options;


    }
    [CommandInfo("Narrative",
             "Menu Image",
             "Displays a button in a multiple Image choice menu")]
    [AddComponentMenu("")]

    public class MenuImage : Command
    {

        //private static List<MenuImageOption>

        [SerializeField ] private MenuImageOption Options;

        public override void GetConnectedBlocks (ref List<Block> connectedBlocks)
        {
            if (Options.targetBlock != null)
            {
                connectedBlocks.Add(Options.targetBlock);
            }
        }


        public override void OnEnter()//Cammand°õ¦æÅÞ¿è
        {

            if (Options._options.parentPos == null)
            {
                return;
            }


            if(Options._options.touchSize == Vector2.zero)
            {
                Options._options.touchSize = Options._options.parentPos.sizeDelta;
            }

            InputCallBack.GetInputCallBack().CreateMenuImage(Options);

           Continue();
        
        }


     }



}
