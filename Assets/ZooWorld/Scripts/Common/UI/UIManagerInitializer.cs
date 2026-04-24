using System;
using System.Collections.Generic;
using ContentLoading;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace ZooWorld.UI
{
    public class UIManagerInitializer<FrameKey> where FrameKey : Enum
    {
        private UIData currentFrameData;
        

        public async UniTask Initialize(UIConfig config, Dictionary<FrameType, FrameHolder<FrameKey>> frameHolders, IContentLoader loader)
        {
            foreach (var frameHolderData in config.FrameHolderDatas)
            {
                var currentFrameHolder = frameHolders[frameHolderData.type];

                foreach (var uiData in frameHolderData.frames)
                {
                    currentFrameData = uiData;

                    var frameOriginal = await loader.LoadComponent<Frame<FrameKey>>(currentFrameData.data);
                    var frame = Object.Instantiate(frameOriginal);
                    frame.Initialize();
            
                    currentFrameHolder.AddFrame(frame);
                }
            }
        }
    }
}