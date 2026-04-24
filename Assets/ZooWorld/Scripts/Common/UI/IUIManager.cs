using System;
using Cysharp.Threading.Tasks;

namespace ZooWorld.UI
{
    public interface IUIManager<FrameKey> where FrameKey : Enum
    {
        UniTask Initialize();

        void ShowFrameGroup(FrameType frameType);

        void HideFrameGroup(FrameType frameType);

        UniTask<Frame<FrameKey>> ShowFrame(FrameType frameType, FrameKey frameKey);

        UniTask HideFrame(FrameType frameType);
    }
}