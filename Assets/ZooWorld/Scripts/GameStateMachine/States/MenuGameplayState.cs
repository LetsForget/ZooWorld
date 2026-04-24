using System;
using Cysharp.Threading.Tasks;
using ZooWorld.Levels;
using ZooWorld.UI;

namespace ZooWorld.StateMachine
{
    public class MenuGameplayState : GameState
    {
        private readonly IUIManager<GameUIType> uiManager;
        private readonly ILevelsProvider levelsProvider;

        private IMenuFrame menuFrame;
        private UniTaskCompletionSource startClickedSource;

        public override GameStateType Type => GameStateType.Menu;

        public MenuGameplayState(IUIManager<GameUIType> uiManager, ILevelsProvider levelsProvider)
        {
            this.uiManager = uiManager;
            this.levelsProvider = levelsProvider;
        }

        public override async UniTask OnEnter()
        {
            var frame = await uiManager.ShowFrame(FrameType.Screen, GameUIType.Menu);
            menuFrame = frame as IMenuFrame ?? throw new InvalidCastException("Menu frame does not implement IMenuFrame.");

            startClickedSource = new UniTaskCompletionSource();
            menuFrame.StartClicked += OnStartClicked;

            await startClickedSource.Task;

            levelsProvider.SetCurrentLevel(0);

            await uiManager.HideFrame(FrameType.Screen);
            
            RaiseChangeState(GameStateType.LevelLoad);
        }

        private void OnStartClicked()
        {
            startClickedSource?.TrySetResult();
        }
        
        public override UniTask OnExit()
        {
            if (menuFrame != null)
            {
                menuFrame.StartClicked -= OnStartClicked;
                menuFrame = null;
            }

            startClickedSource = null;

            return UniTask.CompletedTask;
        }
    }
}