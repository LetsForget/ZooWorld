using Cysharp.Threading.Tasks;
using ZooWorld.UI;

namespace ZooWorld.StateMachine
{
    public class InitializingState : GameState
    {
        private readonly IUIManager<GameUIType> uiManager;
        
        public override GameStateType Type => GameStateType.Initializing;

        public InitializingState(IUIManager<GameUIType> uiManager)
        {
            this.uiManager = uiManager;
        }
        
        public override async UniTask OnEnter()
        {
            await uiManager.Initialize();
            
            RaiseChangeState(GameStateType.Menu);
        }
    }
}
