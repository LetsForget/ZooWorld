using UnityEngine;

namespace ZooWorld.UI
{
    public interface IGameplayFrame
    {
        int FirstCounter { get; }
        int SecondCounter { get; }

        void SetFirstCounter(int value);
        void SetSecondCounter(int value);
        void IncreaseFirstCounter();
        void IncreaseSecondCounter();
        void ShowFloatingText(string text, Vector3 worldPosition, Camera worldCamera = null);
    }
}
