using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZooWorld.UI
{
    public class MenuFrame : Frame<GameUIType>, IMenuFrame
    {
        [SerializeField] private Button startButton;

        public event Action StartClicked;

        public override void Initialize()
        {
            base.Initialize();

            if (!startButton)
            {
                return;
            }

            startButton.onClick.RemoveListener(OnStartButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnDestroy()
        {
            if (startButton)
            {
                startButton.onClick.RemoveListener(OnStartButtonClicked);
            }
        }

        private void OnStartButtonClicked()
        {
            StartClicked?.Invoke();
        }
    }
}
