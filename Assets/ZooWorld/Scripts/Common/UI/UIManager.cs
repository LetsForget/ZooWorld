using System;
using System.Collections.Generic;
using ContentLoading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZooWorld.UI
{
    public class UIManager<FrameKey> : IUIManager<FrameKey> where FrameKey : Enum
    {
        private readonly Dictionary<FrameType, FrameHolder<FrameKey>> frameHolders;

        private readonly UIManagerInitializer<FrameKey> uiManagerInitializer;
        private readonly UIContainer container;
        private readonly UIConfig config;
        
        private readonly IContentLoader loader;

        public UIManager(UIContainer container, UIConfig config, IContentLoader loader)
        {
            this.container = Object.Instantiate(container);
            Object.DontDestroyOnLoad(this.container);
            
            this.config = config;
            this.loader = loader;
 
            frameHolders = new Dictionary<FrameType, FrameHolder<FrameKey>>(4)
            {
                { FrameType.Screen, new FrameHolder<FrameKey>(config, this.container.ScreenGroup) },
                { FrameType.Window, new FrameHolder<FrameKey>(config, this.container.WindowGroup) },
                { FrameType.Popup, new FrameHolder<FrameKey>(config, this.container.PopupGroup) },
                { FrameType.Overlay, new FrameHolder<FrameKey>(config, this.container.OverlayGroup) }
            };

            uiManagerInitializer = new UIManagerInitializer<FrameKey>();
        }

        public async UniTask Initialize()
        {
            await uiManagerInitializer.Initialize(config, frameHolders, loader);
        }
        
        public void ShowFrameGroup(FrameType frameType)
        {
            if (!frameHolders.TryGetValue(frameType, out var frameHolder))
            {
                throw new UINotFondException();
            }

            frameHolder.Show();
        }

        public void HideFrameGroup(FrameType frameType)
        {
            if (!frameHolders.TryGetValue(frameType, out var frameHolder))
            {
                throw new UINotFondException();
            }
            
            frameHolder.Hide();
        }

        public async UniTask<Frame<FrameKey>> ShowFrame(FrameType frameType, FrameKey frameKey)
        {
            if (!frameHolders.TryGetValue(frameType, out var frameHolder))
            {
                throw new UINotFondException();
            }
            
            var result = await frameHolder.ShowFrame(frameKey);
            return result;  
        }

        public async UniTask HideFrame(FrameType frameType)
        {
            if (!frameHolders.TryGetValue(frameType, out var frameHolder))
            {
                throw new UINotFondException();
            }
            
            await frameHolder.HideFrame();
        }
    }
}