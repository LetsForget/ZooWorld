using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZooWorld.UI
{
    [CreateAssetMenu(fileName="UIConfig", menuName="Common/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [field: SerializeField] public float ShowGroupTime {get; private set;}
        [field: SerializeField] public float HideGroupTime { get; private set; }
        
        [field: SerializeField] public FrameHolderData[] FrameHolderDatas { get; private set; }
    }

    [Serializable]
    public struct FrameHolderData
    {
        public FrameType type;
        public UIData[] frames;
    }
    
    [Serializable]
    public struct UIData
    {
        public AssetReference data;
    }
}