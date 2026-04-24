using System.Collections.Generic;
using Common;
using Common.StateMachine;

namespace ZooWorld.StateMachine
{
    public class GameStateMachine : StateMachine<GameStateType, GameState>
    {
        public GameStateMachine(ICommonFactory commonFactory)
        {
            var initializingState = commonFactory.FromNew<InitializingState>();
            var menu = commonFactory.FromNew<MenuGameplayState>();
            var gameplayInitializing = commonFactory.FromNew<LevelLoadState>();
            var gameplay = commonFactory.FromNew<GameplayState>();
            
            states = new Dictionary<GameStateType, GameState>
            {
                { GameStateType.Initializing, initializingState },
                { GameStateType.Menu, menu },
                { GameStateType.LevelLoad, gameplayInitializing },
                { GameStateType.Gameplay, gameplay }
            };
            
            InitializeStates();
        }
    }
}
