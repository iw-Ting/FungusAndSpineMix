using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEngine.UI;
//using Unity.





namespace Fungus
{
    [Serializable]
    public class ExportData
    {
        public static BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        public FlowChartSaveData flowChartSaveData;
        public StageSaveData stageSaveData;


        public ExportData(Flowchart flowchart)
        {
            stageSaveData = new StageSaveData(flowchart.mStage);
            flowChartSaveData = new FlowChartSaveData(flowchart);

        }

        public ExportData() { }

        public IEnumerator SetDataInfoToGame(Flowchart flowchart)
        {
            stageSaveData.SetDataToClass(flowchart.mStage);
            yield return flowChartSaveData.DataSetToClass(flowchart);

        }


    }


    [Serializable]
    public class FlowChartSaveData {


        public List<BlockSaveData> blockSaveDataList = new List<BlockSaveData>();

        public List<VariableData> variables = new List<VariableData>();

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
            saveSelection = flowchart.SaveSelection;
            localizationId = flowchart.LocalizationId;
            showLineNumbers = flowchart.ShowLineNumbers;
            hideCommands = new List<string>(flowchart.HideCommands);
            luaBindingName = flowchart.LuaBindingName;

            variables.Clear();
            blockSaveDataList.Clear();

            foreach (var variable in flowchart.Variables) {
                variables.Add(new VariableData(variable));
            }

            foreach (var data in flowchart.GetComponents<Block>()) {
                blockSaveDataList.Add(new BlockSaveData(data));
            }



        }
        
        public IEnumerator DataSetToClass(Flowchart flowchart)
        {
            flowchart.SetOverrideData(this);

            foreach (var blockData in blockSaveDataList) {
                var block = flowchart.gameObject.AddComponent<Block>();
               blockData.SetDataToBlock(block);
            }

            int finish = 0;

            foreach (var blockData in blockSaveDataList)
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(blockData.SetBlockCommandData(() => { finish++; } ));
            }
            yield return new WaitUntil( () => finish >= blockSaveDataList.Count );
            //�L�k��� �]�������set value����k
        }


    }
    [Serializable]
    public class BlockSaveData
    {

        public string blockName = "New Block";
        public List<CommandSaveData> commandSaveDataList;

        public bool eventIsNull = false;

        public EventHandleSaveData eventSaveData = null;

        public string description = "";

        public ExecutionState executionState;

        public bool useCustomTint = false;

        public Color tint = Color.white;

        public Rect nodeRect=new Rect();



        public BlockSaveData(Block block)
        {
            Debug.Log("execute serializable block name=>"+block.BlockName);
            blockName = block.BlockName;
            description = block.Description;
            executionState = block.State;
            useCustomTint = block.UseCustomTint;
            nodeRect = block._NodeRect;
            tint = block.Tint;

            bool isHave=false;
            if (block._EventHandler) {
                if (!block._EventHandler.Equals(null)&& block._EventHandler!=null)
                {
                    isHave = true;
                    eventSaveData = new EventHandleSaveData(block._EventHandler);
                }
            }

          if(!isHave)
            {
                eventSaveData = null;
                eventIsNull = true;
            }
            commandSaveDataList = new List<CommandSaveData>();

            foreach (var com in block.CommandList)
            {

                CommandSaveData sData = new CommandSaveData(com);

                if (sData != null)
                {
                    commandSaveDataList.Add(sData);
                }
            }
        }

        public void SetDataToBlock(Block block)
        {
            block.BlockName   = blockName;
            block.Description = description;

            block._NodeRect = nodeRect;
            block.State = executionState;
            block.UseCustomTint = useCustomTint;
            block.Tint = tint;


            if (!eventIsNull)
            {
                SetEventDataToBlock(block);
            }
            else
            {
                eventSaveData = null;
                block._EventHandler = null;
            }
            tempBlock = block;

        }

        private void SetEventDataToBlock(Block block)//����Block�ƾ�
        {
            EventHandler newHandler = block.gameObject.AddComponent(Type.GetType(eventSaveData.typeName)) as EventHandler;

            Debug.Log("����block�W�r=>" + block.BlockName);

            Debug.Log("��������=>" + eventSaveData.typeName);

            newHandler.ParentBlock = block;


            for (int i = 0; i < eventSaveData.propertyValues.Count; i++)
            {

                var property = Type.GetType(eventSaveData.typeName).GetFields()[i];

                EditorCoroutineUtility.StartCoroutine(eventSaveData.propertyValues[i].GetValueData(
                    block.GetComponent<Flowchart>(),
                    res => {
                        property.SetValue(newHandler, res);
                    }),
                    this
                    );
            }

            block._EventHandler = newHandler;
        }

        public Block tempBlock=null;//�ηN�O���F��command�}�C���Ҧ�block��l�Ƨ�����b����
        
        public IEnumerator SetBlockCommandData(Action cb=null)
        {
            if (tempBlock==null) {
                Debug.Log("�o�Ϳ��~,���[���Ҧ�block�K�[��command");
            }
            foreach (var comSaveData in commandSaveDataList)
            {
                yield return SetSaveData(comSaveData, tempBlock);
            }
            if (cb!=null) {
                cb();
            }
        }

        private IEnumerator SetSaveData(CommandSaveData saveData, Block block)//�]�m�x�s���
        {
            Flowchart flowchart = block.gameObject.GetComponent<Flowchart>();
            Debug.Log("command�����W��=>"+saveData.commandType);
            var type = Type.GetType(saveData.commandType);
            var component = block.gameObject.AddComponent(type) as Command;

            block.CommandList.Add(component);
            component.ParentBlock = block;
            List<string> strlist = new List<string>();

            for (int i = 0; i < type.GetFields(ExportData.DefaultBindingFlags).Length; i++)
            {

                var field = type.GetFields(ExportData.DefaultBindingFlags)[i];

                Debug.Log("�ثe�b�����field�ȦW��==========>" + field.Name);

                if (field.FieldType.IsGenericType)
                {
                    if (field.FieldType == typeof(List<string>))//�x���L�k��J�r��type list<>�u�౵�����x�������O �]�������^��ilist��icollect  �ҥH�u��ܥ�è���@�@ù�C
                    {

                        List<string> list = new List<string>();
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as string[]).ToList();

                        });
                        field.SetValue(component, list);

                    }
                    else if (field.FieldType == typeof(List<List<string>>))
                    {
                        List<List<string>> list = new List<List<string>>();

                        list = null;
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as List<string>[]).ToList();

                        });
                        field.SetValue(component, list);

                    }
                    else if (field.FieldType == typeof(List<int>))
                    {
                        List<int> list = new List<int>();
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as int[]).ToList();

                        });
                        field.SetValue(component, list);

                    }
                    else if (field.FieldType == typeof(List<float>))
                    {
                        List<float> list = new List<float>();
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as float[]).ToList();

                        });
                        field.SetValue(component, list);

                    }
                    else if (field.FieldType == typeof(List<bool>))
                    {
                        List<bool> list = new List<bool>();
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as bool[]).ToList();

                        });
                        field.SetValue(component, list);

                    }
                    else if (field.FieldType == typeof(List<object>))
                    {
                        List<object> list = new List<object>();
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as object[]).ToList();

                        });
                        field.SetValue(component, list);

                    }
                    else if (field.FieldType == typeof(List<Variable>))
                    {
                        List<Variable> list = new List<Variable>();
                        yield return saveData.fieldDataList[i].GetValueData(flowchart, obj =>
                        {

                            list = (obj as Variable[]).ToList();

                        });
                        field.SetValue(component, list);
                    }
                    else
                    {
                        Debug.Log("����������=>" + field.FieldType);
                    }
                }
                else if (field.FieldType.IsEnum)
                {
                    yield return saveData.fieldDataList[i].GetValueData(flowchart, res =>
                    {

                        field.SetValue(component, Enum.Parse(field.FieldType, (string)res));

                    });
                }
                else if (field.FieldType.IsValueType) {

                    yield return saveData.fieldDataList[i].GetValueData(flowchart, res => {
                        field.SetValue(component, res);
                    });

                }
                else // �D list
                {

                    if (field.FieldType == typeof(Camera)) {
                        field.SetValue(component, Camera.main);
                        yield break;
                    }

                   yield return  saveData.fieldDataList[i].GetValueData(flowchart, res => {

                        if (field.FieldType == typeof(Block))
                       {
                           field.SetValue(component, (res as Block));
                       }

                       else if (field.FieldType == typeof(Stage))
                       {
                           field.SetValue(component, (res as Stage));
                       }
                       else if (field.FieldType == typeof(RectTransform))
                       {
                           field.SetValue(component, (res as RectTransform));
                       }
                       else if (field.FieldType == typeof(Character))
                       {
                           field.SetValue(component, (res as Character));
                       }
                       else if (field.FieldType == typeof(View))
                       {
                           field.SetValue(component, (res as View));
                       }
                       else if (field.FieldType == typeof(SpriteRenderer))
                       {
                           field.SetValue(component, (res as SpriteRenderer));
                       }
                       else if (field.FieldType == typeof(Image))
                       {
                           field.SetValue(component, (res as Image));
                       }
                       else if (field.FieldType == typeof(Sprite)) //say
                       {
                           field.SetValue(component, (res as Sprite));
                          // component.SetSaveDataToValue(field.Name, res);
                           //sprite�]���C��command������ؼФ��P,�G�ݭn�h��command���U���g����᪺�����k
                           //�ھڭȪ��W�h���α� ex component.setValue(field.Name,res);
                       }
                       else if (field.FieldType == typeof(AudioClip))//say show audio
                       {
                           field.SetValue(component, (res as AudioClip));
                       }
                       else if (field.FieldType == typeof(Texture))//say show audio
                       {
                           field.SetValue(component, (res as Texture));
                       }
                       else if (field.FieldType == typeof(Texture2D))//say show audio
                       {
                           field.SetValue(component, (res as Texture2D));
                       }
                       else  // sturct����
                       {
                           Debug.Log("��o����=>" + res);
                           Debug.Log("�i��w�]�����=>" + field.FieldType.Name);
                           field.SetValue(component, res);
                       }

                   });
  
                }

            }

        }
    }

    [Serializable]
    public class EventHandleSaveData
    {
        public string typeName;
        public List<DataObjectValue> propertyValues = new List<DataObjectValue>();//�ھڤ��P���O���x�s�ƾ�,�������P���}�C


        public EventHandleSaveData(EventHandler eventHandler)
        {
            if (eventHandler == null) {
                return;
            }

            typeName = eventHandler.GetType().FullName;

            propertyValues.Clear();
            foreach (var value in eventHandler.GetType().GetFields()) {

                DataObjectValue valueData = new DataObjectValue();
                valueData.SetDataToValue(value.GetValue(eventHandler));
                propertyValues.Add(valueData);
            }
        }
    }


    [Serializable]
    public class CommandSaveData//Command�x�s�ɮ׳������~��
    {
        public string commandType;

        public List<DataObjectValue> fieldDataList = new List<DataObjectValue>();

        public CommandSaveData(Command data)
        {

            commandType = data.GetType().FullName;
            
            DataObjectValue.JudgeValueType(data, dataObj => { fieldDataList.Add(dataObj); });

        }

    }

    [Serializable]
    public class StageSaveData
    {

        public bool dimPortraits;

        public Color dimColor;
        public float fadeDuration;
        public float moveDuration;
        public LeanTweenType fadeEaseType;
        public Vector2 shiftOffset;

        public string defaultPosition;//recttransform
        public List<RectPositionsInfo> createAreaPositions = new List<RectPositionsInfo>();
        public List<ViewPositionsInfo> createViewPositions = new List<ViewPositionsInfo>();
        public List<SpriteRenderInfo> spriteRenderers = new List<SpriteRenderInfo>();
        public List<AudioInfo> audios = new List<AudioInfo>();

        public StageSaveData(Stage stage)
        {
            dimPortraits = stage.DimPortraits;
            dimColor = stage.DimColor;
            fadeDuration = stage.FadeDuration;
            moveDuration = stage.MoveDuration;
            shiftOffset = stage.ShiftOffset;
            fadeEaseType = stage.FadeEaseType;
            if (stage.DefaultPosition != null) {
                defaultPosition = stage.DefaultPosition.name;
            }
            else
            {
                defaultPosition = stage.Positions[0].name;
            }


            foreach (var rect in stage.Positions)
            {
                RectPositionsInfo info = new RectPositionsInfo(rect);
                createAreaPositions.Add(info);

            }

            for (int i=0;i<stage.ViewParent.childCount;i++) {
                var child=stage.ViewParent.GetChild(i);
                ViewPositionsInfo info = new ViewPositionsInfo(child.GetComponent<View>());
                createViewPositions.Add(info);
               }

            for (int i = 0; i < stage.ImageParent.childCount; i++)
            {
                var child = stage.ImageParent.GetChild(i);
                SpriteRenderInfo info = new SpriteRenderInfo(child.GetComponent<SpriteRenderer>());
                spriteRenderers.Add(info);
            }
            
            for (int i=0;i<stage.AudiosParent.childCount;i++) 
            {
                var child=stage.AudiosParent.GetChild(i);
                AudioInfo info = new AudioInfo(child.GetComponent<AudioSource>());
                audios.Add(info);
             }


        }

        public void SetDataToClass(Stage stage)
        {
            stage.ClearData();

            stage.DimPortraits = dimPortraits;
            stage.DimColor = dimColor;
            stage.FadeDuration = fadeDuration;
            stage.FadeEaseType = fadeEaseType;
            stage.MoveDuration = moveDuration;
            stage.ShiftOffset = shiftOffset;

            foreach (var rectInfo in createAreaPositions)
            {
                GameObject sp = new GameObject(rectInfo.rectName, typeof(RectTransform));
                sp.transform.SetParent(stage.PositionsParent, false);
                RectTransform spRect = sp.GetComponent<RectTransform>();
                stage.Positions.Add(spRect);
                rectInfo.SetDataToRectTransform(spRect);
                if (rectInfo.rectName == defaultPosition) {
                    stage.DefaultPosition = spRect;
                }
            }
            foreach (var viewInfo in createViewPositions)
            {
                
                GameObject sp = new GameObject(viewInfo.rectName, typeof(RectTransform),typeof(View));
                sp.transform.SetParent(stage.ViewParent, false);
                View spRect = sp.GetComponent<View>();
                viewInfo.SetViewDataToRectTransform(spRect);
                
            }
            foreach (var sprite in spriteRenderers)
            {

                GameObject sp = new GameObject(sprite.transName, typeof(SpriteRenderer));
                sp.transform.SetParent(stage.ImageParent, false);
                SpriteRenderer spRect = sp.GetComponent<SpriteRenderer>();
                sprite.SetImageDataToRectTransform(spRect);

            }
            Debug.Log("�x�s�ƶq=>"+audios.Count);
            foreach (var audio in audios)
            {

                GameObject sp = new GameObject(audio.transName,  typeof(AudioSource));

                sp.transform.SetParent(stage.AudiosParent, false);
                AudioSource spRect = sp.GetComponent<AudioSource>();
                audio.SetAudioSourceDataToRectTransform(spRect);

            }



        }


    }


    [Serializable]
    public class VariableData
    {

        public VariableScope variableScope;

        public string key;

        public object value;

        public VariableData(Variable var)
        {
            variableScope = var.Scope;
            key = var.Key;
            value = var.GetValue();


            //     type=var.

        }

        public Variable DataSetToClass(Flowchart flowchart)
        {
            Variable var = flowchart.gameObject.AddComponent<Variable>();
            var.Scope = variableScope;
            var.Key = key;
            //�ݭn��� value  �ثe�|�L�k�����value����m

            return var;
        }

    }
    [Serializable]
    public class RectPositionsInfo
    {

        public string rectName;

        public Vector2 position;

        public Vector2 size;

        public Vector2 pivot;

        public Vector2 anchorMin;

        public Vector2 anchorMax;

        public RectPositionsInfo(string _rectName, Vector2 _pos, Vector2 _size, Vector2 _pivot, Vector2 _anchorMin, Vector2 _anchorMax) {
            rectName = _rectName;
            position = _pos;
            size = _size;
            pivot = _pivot;
            anchorMin = _anchorMin;
            anchorMax = _anchorMax;

        }

        public RectPositionsInfo(RectTransform rect) {
            rectName = rect.name;
            position = rect.position;
            size = rect.sizeDelta;
            pivot = rect.pivot;
            anchorMin = rect.anchorMin;
            anchorMax = rect.anchorMax;
        }

        public void SetDataToRectTransform(RectTransform rect)
        {
            rect.name = rectName;
            rect.position = position;
            rect.sizeDelta = size;
            rect.pivot = pivot;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

        }


    }


    [Serializable]

    public class PositionsInfo
    {
        public string transName;

        public Vector2 position;

        public Vector2 Scale;

        public PositionsInfo(Transform _transform)
        {
            transName = _transform.name;
            position = _transform.position;
            Scale = _transform.localScale;
        }
        public void SetDataToTransform(Transform _transform)
        {
            _transform.name = transName;
            _transform.position= position;
            _transform.localScale = Scale;

        }


    }
    [Serializable]
    public class SpriteRenderInfo : PositionsInfo//��ܪ��Ϥ�
    {

        public string assetSpritePath = "";
        public Color color;

        public string sortOrderName = "";
        public int order = 0;

        public SpriteRenderInfo(SpriteRenderer sprite) : base(sprite.transform)
        {
            if (sprite.sprite)
            {
                assetSpritePath = AssetDatabase.GetAssetPath(sprite.sprite);
            }

            color = sprite.color;


                order =sprite.sortingOrder;
                sortOrderName = sprite.sortingLayerName;
            

        }
        public void SetImageDataToRectTransform(SpriteRenderer sprite)
        {
           SetDataToTransform(sprite.transform);

            if (!assetSpritePath.Equals("") && !assetSpritePath.Equals(null))
            {
                sprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetSpritePath);//�Ϥ��n�令��resources �ثe�u���beditor�W�ॿ�`����
            }
            sprite.color = color;

            if (!sortOrderName.Equals("") || sortOrderName.Equals(null))
            {
                sprite.sortingLayerName = sortOrderName;
                sprite.sortingOrder = order;
            }
          }
         }

        [Serializable]
    public class ViewPositionsInfo:RectPositionsInfo{

        public float viewSize = 0.5f;


       public Vector2 primaryAspectRatio = new Vector2(4, 3);


        public Vector2 secondaryAspectRatio = new Vector2(2, 1);

        public ViewPositionsInfo(View view) :base(view.GetComponent<RectTransform>())
        {
            viewSize = view.ViewSize;
            primaryAspectRatio = view.PrimaryAspectRatio;
            secondaryAspectRatio = view.SecondaryAspectRatio;

        }

        public  void SetViewDataToRectTransform(View view)
        {
            SetDataToRectTransform(view.GetComponent<RectTransform>());
            view.ViewSize = viewSize;
            view.PrimaryAspectRatio = primaryAspectRatio;
            view.SecondaryAspectRatio = secondaryAspectRatio;
        }

    }
    [Serializable]
    public class AudioInfo:PositionsInfo//��ܪ�����
    {

        public string ClipAssetsPath = "";

        public bool mute;
        public bool playOnAwake;
        public bool loop;

        public int priority;
        public float volume;
        public float pitch;

        public AudioInfo(AudioSource audio) : base(audio.transform)
        {
            if (audio.clip) {
                ClipAssetsPath = AssetDatabase.GetAssetPath(audio.clip);
            }
            mute=audio.mute;
            playOnAwake=audio.playOnAwake;
            loop=audio.loop;
            priority=audio.priority;
            volume=audio.volume;
            pitch=audio.pitch;


        }

        public void SetAudioSourceDataToRectTransform(AudioSource audio)
        {
            SetDataToTransform(audio.transform);
            if (!ClipAssetsPath.Equals("") && !ClipAssetsPath.Equals(null))
            {
                audio.clip = AssetDatabase.LoadAssetAtPath<AudioClip>( ClipAssetsPath);
            }

            audio.mute = mute;
            audio.playOnAwake = playOnAwake;
            audio.loop = loop;
            audio.priority = priority;
            audio.volume = volume;
            audio.pitch = pitch;
        }

    }
    [Serializable]
    public class ImageInfo:RectPositionsInfo//��ܪ��Ϥ�
    {

        public string assetSpritePath = "";
        public Color color;


        public bool raycastTarget = false;

        public bool PreserveAspect = true;

        public string sortOrderName = "";
        public int order=0;

        public ImageInfo(Image image) : base(image.GetComponent<RectTransform>())
        {
            if (image.sprite) {
                assetSpritePath = AssetDatabase.GetAssetPath(image.sprite);
            }

            color = image.color;
            raycastTarget = image.raycastTarget;
            PreserveAspect = image.preserveAspect;
            Canvas canvas = null;
            if (image.TryGetComponent<Canvas>(out canvas)) {
                order = canvas.sortingOrder;
                sortOrderName = canvas.sortingLayerName;
            }

        }

        public void SetImageDataToRectTransform(Image image)
        {
            SetDataToRectTransform(image.GetComponent<RectTransform>());

            if (!assetSpritePath.Equals("")&& !assetSpritePath.Equals(null)) {
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetSpritePath);
            }
            image.color = color;
            image.raycastTarget = raycastTarget;
            image.preserveAspect = PreserveAspect;

            if (!sortOrderName.Equals("")|| sortOrderName.Equals(null)) {
                Canvas canvas = image.GetComponent<Canvas>();
                canvas.sortingLayerName = sortOrderName;
                 canvas.sortingOrder = order;
            }
        }
        //sprite��T  �ϼh��T  ����rect transform

    }

    [Serializable]
    public class DataObjectValue
    {
        //class list is dataobjectValue  string is class type
        //enum  int is enum index  string is enum class name
        public string typeName="";

        public List<DataObjectValue> _class ;  //class   list array �]�O��
        public string _string="";
        public float _floatValue=0;
        public int _intValue=0;
        public bool _bool;
        public Vector2 _vec2;
        public Vector3 _vec3;
        public Vector4 _vec4;
        public Color _color;
        public TweenTime tweenTime;
        public BooleanData _boolData;
        public StringData _stringData;
        public FloatData _floatData;
        public IntegerData _intData;
        public ColorData _colorData;
        public Vector2Data _vector2Data;
        public Vector3Data _vector3Data;
        public Vector4Data _vector4Data;
        public AudioSourceData _audioSourceData;

        public bool isNull=false;
        public string _path = "";

        
        public static void JudgeValueType(object _data,Action<DataObjectValue> _cb=null)//serializable parse class 
        {


                 Debug.Log("��ܭ�=>"+_data);
           
                 foreach (var field in Type.GetType(_data.GetType().FullName).GetFields(ExportData.DefaultBindingFlags))
                 {
                     DataObjectValue value = new DataObjectValue();
                     Debug.Log("��쪺�W��=>"+field.Name);

                    if (field.FieldType.IsGenericType)
                     {
                      value.SetDataToValue(field.FieldType.GetField("_items", ExportData.DefaultBindingFlags).GetValue(field.GetValue(_data)));
                     }
                     else
                     {
                         value.SetDataToValue(field.GetValue(_data));
                     }
                if (_cb != null)
                     {

                         _cb(value);

                      }
                 }


        }

        public void SetDataToValue( object value)
        {
            Debug.Log("��ڪ���=>" + value);

            if (value == null)//�s���������S
            {
                isNull = true;
                return;
            }
            var mType = value.GetType();
            typeName = mType.FullName;
            _class = new List<DataObjectValue>();

            foreach (var field in this.GetType().GetFields(ExportData.DefaultBindingFlags))
            {

                if (typeName == field.FieldType.FullName&&field.Name!="_class"&& field.Name != "typeName")
                {
                    Debug.Log("���ӭȦW?=>"+field.Name);

                    field.SetValue(this,value);

                    Debug.Log("������J����=>" + field.GetValue(this));
                    return;
                }
            }

                bool isDefaultExecuteClass = false;
                _string = typeName;

                if (value.Equals(null))//�����O��
                {
                    isNull = true;
                    return;
                }
                switch (typeName)//���wprefab
                {
                    case "UnityEngine.RectTransform":
                        _string = (value as RectTransform).name;
                        isDefaultExecuteClass = true;
                        break;
                case "UnityEngine.AudioSource"://�n���stage�W��audio �����ͦ��ê��W��������T
                        _string = (value as AudioSource).name;
                         isDefaultExecuteClass = true;
                      break;
                case "UnityEngine.Camera"://�n���stage�W��audio �����ͦ��ê��W��������T
                      _string = (value as Camera).name;
                        isDefaultExecuteClass = true;
                    break;
                case "Fungus.Stage":
                        _string = (value as Stage).name;
                        isDefaultExecuteClass = true;
                        break;
                    case "Fungus.View":
                        _string = (value as View).name;
                        isDefaultExecuteClass = true;
                        break;
                case "UnityEngine.UI.Image":
                    _string = (value as Image).name;
                    isDefaultExecuteClass = true;
                    break;
                case "UnityEngine.SpriteRenderer":
                    _string = (value as SpriteRenderer).name;
                    isDefaultExecuteClass = true;
                    break;
                case "Fungus.Character":
                        _string = (value as Character).name;
                        isDefaultExecuteClass = true;
                        break;
                 case "Fungus.Block":
                        _string = (value as Block).BlockName;//menu ��target
                         isDefaultExecuteClass=true;
                        break;
                    case "UnityEngine.Sprite"://�ݭn�Ϥ����|  �i��|��hierarchy�W��
                    _string = (value as Sprite).name;
                    _path = AssetDatabase.GetAssetPath( (value as UnityEngine.Object));
                    isDefaultExecuteClass = true;
                    break;
                case "UnityEngine.Texture":
                    _string = (value as Texture).name;
                    _path = AssetDatabase.GetAssetPath((value as UnityEngine.Object));
                    isDefaultExecuteClass = true;
                    break;
                    case "UnityEngine.Texture2D":
                    _string = (value as Texture2D).name;
                    _path = AssetDatabase.GetAssetPath((value as UnityEngine.Object));
                    isDefaultExecuteClass = true;
                    break;
                case "UnityEngine.AudioClip":
                    _string = (value as AudioClip).name;
                    _path = AssetDatabase.GetAssetPath((value as UnityEngine.Object));
                    isDefaultExecuteClass = true;
                    break;
            }

            if (isDefaultExecuteClass) {
                    return;
                }
               
                if (mType.IsGenericType) {
                    Debug.Log("�O�M��=>" + typeName);
                    Debug.Log("�ȦW��=>" + value);
                    DataObjectValue newValue = new DataObjectValue();
                    SetDataToValue(   mType.GetField("_items", ExportData.DefaultBindingFlags).GetValue(value)  );
                   // this = newValue;

                }
                else if(mType.IsArray){

                    Debug.Log("�O�}�C=>" + typeName+"����=>"+(value as Array).Length);
                    Debug.Log("�ȦW��=>" + value);
                    ICollection collection = value as ICollection;

                    
                    foreach (var col in collection)
                    {
                        if (col==null)
                        {
                            continue;
                        }
                        if (col.GetType().IsGenericType) {
                            Debug.Log("�٬O�M��=>"+col);
                            DataObjectValue newValue = new DataObjectValue();

                            newValue.SetDataToValue(col.GetType().GetField("_items", ExportData.DefaultBindingFlags).GetValue(col));
                            _class.Add(newValue);
                        // this = newValue;
                    }
                        else
                        {
                            Debug.Log("��J�}�C��=>" + col);
                            DataObjectValue newValue = new DataObjectValue();
                            newValue.SetDataToValue(col);
                            _class.Add(newValue);
                        }
                    }

                }else if (mType.IsEnum)
                 {
                _string = value.ToString();
                 }
                  else if(mType.IsClass|| mType.IsValueType)
                {
                
                    Debug.Log("�O���O=>" + typeName+ "�ȦW��=>" + value);

                    _class = new List<DataObjectValue>();

                    List<DataObjectValue> temp = new List<DataObjectValue>();
                    JudgeValueType(value, valueObj => { temp.Add(valueObj); });
                    _class.AddRange(temp);
            }
            else
            {
                Debug.LogError("�w�Ƥ��~���|������==>" + value);
            }



        }



            public IEnumerator GetValueData(Flowchart parentObj=null,Action<object> cbValue=null)
             {
            if (isNull) {
                yield break;
            }
            var type = Type.GetType(typeName);

            foreach (var field in this.GetType().GetFields(ExportData.DefaultBindingFlags))
            {

                if (typeName == field.FieldType.FullName && field.Name != "_class" && field.Name != "typeName")
                {
                    Debug.Log("��^�ƭ�=>" + _string);
                    cbValue( field.GetValue(this));
                    yield break;
                }
            }


            switch (typeName)
            {
                case "UnityEngine.RectTransform":
                    cbValue(parentObj.mStage.GetPosition(_string));
                    yield break;
                case "Fungus.Stage":
                    cbValue(parentObj.mStage);
                    yield break;
                case "Fungus.View":
                    cbValue(parentObj.mStage.GetView(_string));
                    yield break;
                case "UnityEngine.UI.Image":
                    cbValue(parentObj.mStage.GetImage(_string));
                    yield break;
                case "UnityEngine.SpriteRenderer":
                    cbValue(parentObj.mStage.GetSpriteRenderer(_string));
                    yield break;
                case "Fungus.Block":
                    cbValue(parentObj.FindBlock(_string));
                    yield break;
                case "Fungus.Character":
                    Character resChara = null;
                    yield return FungusResources.GetCharacter(_string, _res => { resChara = _res; });
                    cbValue(resChara);

                    yield break;


                case "UnityEngine.Sprite"://�ݭn�Ϥ����|  �i��|��hierarchy�W��

                    cbValue(AssetDatabase.LoadAssetAtPath<Sprite>(_path));
                   yield  break;
                case "UnityEngine.AudioClip":
                    cbValue(AssetDatabase.LoadAssetAtPath<AudioClip>(_path));
                    yield break;
                case "UnityEngine.Texture":
                    cbValue(AssetDatabase.LoadAssetAtPath<Texture>(_path));
                    yield break;
                case "UnityEngine.Texture2D":
                    cbValue(AssetDatabase.LoadAssetAtPath<Texture2D>(_path));
                    yield break;
            }

            Debug.Log("��������������=====>" + typeName);
            Debug.Log("������=====>" + type);

            if (type.IsArray) {

                Debug.Log("�}�C������type=>"+ type.GetElementType());

                Array arr = Array.CreateInstance(type.GetElementType(), _class.Count);
                
                //�����O�M��  ���w�]��^�����Oarray
                
                // list<list<string>>
                for (int i=0;i<arr.Length;i++) {
                    object resVal = null;
                    yield return _class[i].GetValueData(parentObj, val => { resVal = val; });

                    Debug.Log("�^�Ǫ��ƭ�=>" + resVal);
                   // Debug.Log("�^�Ǫ��ƭ�����=>"+resVal.GetType());
                   // Debug.Log("�}�C������type=>" + type.GetElementType());
                   if ( type.GetElementType().IsGenericType)
                    {
                        if (type.GetElementType()==typeof(List<string>)) {
                            List<string> strList = new List<string>(resVal as string[]);
                            arr.SetValue(strList, i);

                        }

                    }
                    else
                    {
                        arr.SetValue(resVal, i);
                    }
                }
                
                cbValue(arr);
                yield break;
            }
            else if (type.IsEnum)
            {
                //  Debug.Log("enum��type=>" + _enum.GetType());
                Debug.Log("���մ���");
                cbValue(_string);

            }
            else if (type.IsClass||type.IsValueType)
            {
                Debug.Log("�]class=>"+Type.GetType(typeName));   

                cbValue(_class);
                yield break;

            }
            else
            {
                Debug.LogWarning("�N�~�����~,�����O�W��=>" + typeName);
               cbValue( null);
                yield break;

            }

        }



    }


}
