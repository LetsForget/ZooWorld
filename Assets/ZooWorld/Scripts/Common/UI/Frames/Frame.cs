using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZooWorld.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class Frame<FrameKey> : MonoBehaviour where FrameKey : Enum
    {
        [field: SerializeField] public FrameKey Key { get; private set; }
        [field: SerializeField] public Canvas Canvas { get; private set; }

        public virtual void Initialize() { }
        
        public virtual async UniTask Show()
        {
            await UniTask.SwitchToMainThread();
            Canvas.gameObject.SetActive(true);
        }

        public virtual async UniTask Hide()
        {
            await UniTask.SwitchToMainThread();
            Canvas.gameObject.SetActive(false);
        }
    }
}