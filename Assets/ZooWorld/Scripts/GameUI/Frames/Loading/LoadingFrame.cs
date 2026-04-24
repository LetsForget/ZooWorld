using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZooWorld.UI
{
    public class LoadingFrame : Frame<GameUIType>, ILoadingFrame
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text progressText;
        
        public void SetProgress(float progress)
        {
            var clampedProgress = Mathf.Clamp01(progress);

            if (progressBar)
            {
                progressBar.value = clampedProgress;
            }

            if (progressText)
            {
                var progressPercents = Mathf.RoundToInt(clampedProgress * 100f);
                progressText.text = progressPercents.ToString();
            }
        }
    }
}