using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ZooWorld.UI
{
    public class FrameHolder<FrameKey> where FrameKey : Enum
    {
        private readonly UIConfig config;
        private readonly CanvasGroup group;
        
        private readonly Dictionary<FrameKey, Frame<FrameKey>> frames;

        private Frame<FrameKey> currentFrame;

        public FrameHolder(UIConfig config, CanvasGroup group)
        {
            this.config = config;
            this.group = group;
            
            frames = new Dictionary<FrameKey, Frame<FrameKey>>();
        }
        
        public void Show()
        {
            group.DOFade(1, config.ShowGroupTime);
        }

        public void Hide()
        {
            group.DOFade(0, config.HideGroupTime);
        }
        
        public void AddFrame(Frame<FrameKey> frame)
        {
            if (!frames.TryAdd(frame.Key, frame))
            {
                return;
            }

            frame.transform.SetParent(group.transform, false);
            frame.Hide();
        }

        public bool TryRemoveFrame(FrameKey key, out Frame<FrameKey> frame)
        {
            var result = frames.TryGetValue(key, out frame);

            if (result)
            {
                frames.Remove(key);
            }
            
            return result;
        }

        public async UniTask<Frame<FrameKey>> ShowFrame(FrameKey key)
        {
            if (!frames.TryGetValue(key, out var frame))
            {
                throw new UINotFondException();
            }
            
            await frame.Show();
            currentFrame = frame;
            
            return currentFrame;
        }

        public async UniTask HideFrame()
        {
            if (!currentFrame)
            {
                return;
            }
            
            await currentFrame.Hide();
        }
    }
}