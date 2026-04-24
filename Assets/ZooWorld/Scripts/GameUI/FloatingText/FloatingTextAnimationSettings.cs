using System;
using DG.Tweening;
using UnityEngine;

namespace ZooWorld.UI
{
    [Serializable]
    public struct FloatingTextAnimationSettings
    {
        [Min(0f)] public float duration;
        public float moveOffsetY;
        [Min(0f)] public float startScale;
        [Min(0f)] public float endScale;
        [Min(0f)] public float fadeDelay;
        public Ease moveEase;
        public Ease scaleEase;
        public Ease fadeEase;
    }
}
