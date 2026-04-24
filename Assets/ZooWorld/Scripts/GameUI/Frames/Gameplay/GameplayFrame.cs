using TMPro;
using UnityEngine;

namespace ZooWorld.UI
{
    public class GameplayFrame : Frame<GameUIType>, IGameplayFrame
    {
        [SerializeField] private TMP_Text firstCounterText;
        [SerializeField] private TMP_Text secondCounterText;
        [SerializeField] private WorldSpaceFloatingTextPresenter floatingTextPresenter;

        public int FirstCounter { get; private set; }
        public int SecondCounter { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            
            RefreshFirstCounter();
            RefreshSecondCounter();
        }

        public void SetFirstCounter(int value)
        {
            FirstCounter = value;
            RefreshFirstCounter();
        }

        public void SetSecondCounter(int value)
        {
            SecondCounter = value;
            RefreshSecondCounter();
        }

        public void IncreaseFirstCounter()
        {
            SetFirstCounter(FirstCounter + 1);
        }

        public void IncreaseSecondCounter()
        {
            SetSecondCounter(SecondCounter + 1);
        }

        public void ShowFloatingText(string text, Vector3 worldPosition, Camera worldCamera = null)
        {
            EnsureFloatingTextPresenter();
            floatingTextPresenter?.ShowFloatingText(text, worldPosition, worldCamera);
        }

        private void RefreshFirstCounter() => RefreshCounter(firstCounterText, FirstCounter);
        private void RefreshSecondCounter() => RefreshCounter(secondCounterText, SecondCounter);

        private void RefreshCounter(TMP_Text counterText, int value)
        {
            if (!counterText)
            {
                return;
            }
            
            counterText.text = value.ToString();
        }

        private void EnsureFloatingTextPresenter()
        {
            if (floatingTextPresenter == null)
            {
                floatingTextPresenter = GetComponentInChildren<WorldSpaceFloatingTextPresenter>(true);
            }
        }
    }
}
