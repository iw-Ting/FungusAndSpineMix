using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;





namespace Fungus
{

    [Serializable]
    public class FlowChartSaveData {


        public List<BlockSaveData> blockSaveDataList=new List<BlockSaveData>();

        public List<Variable>variables=new List<Variable>();

        public string description = "";

        public string dataName = "";

        public bool colorCommands = true;

        public bool hideComponents = true;

        public float stepPause = 0f;

        public bool saveSelection = true;

        public string localizationId = "";

        public bool showLineNumbers = false;

        public List<string> hideCommands = new List<string>();

        //public LuaEnvironment luaEnvironment;

        public string luaBindingName = "flowchart";

        public FlowChartSaveData(Flowchart flowchart)
        {
            description = flowchart.Description;
            dataName = flowchart.DataName;
            colorCommands = flowchart.ColorCommands;
            hideComponents = flowchart.HideComponents;
            stepPause = flowchart.StepPause;
            saveSelection=flowchart.SaveSelection;
            localizationId = flowchart.LocalizationId;
            showLineNumbers = flowchart.ShowLineNumbers;
            hideCommands = new List<string>(flowchart.HideCommands);
            //    luaEnvironment=flowchart.lua
            luaBindingName = flowchart.LuaBindingName;

        }

    }
    [Serializable]
    public class BlockSaveData {

        public List<CommandSaveData> commandSaveDataList;

       public string blockName = "New Block";

       public string description = "";

        public ExecutionState executionState;

        public bool useCustomTint = false;

        public Color tint = Color.white;


        public BlockSaveData(Block data)
        {
            blockName = data.BlockName;
            description = data.Description;
            executionState = data.State;
            useCustomTint = data.UseCustomTint;
            tint= data.Tint;

            foreach (var com in data.CommandList) {
                commandSaveDataList.Add(com.GetSaveData<CommandSaveData>());
            }

        }

    }
    [Serializable]
    public class CommandSaveData//儲存檔案都必須繼承
    {
        public Type commandType;



    }


    public class VariableData
    {

        public VariableScope variableScope;

        public string key;

    }



}
