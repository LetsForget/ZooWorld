using ContentLoading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZooWorld.Levels;

namespace ZooWorld.StateMachine
{
    public class LevelLoadState : GameState
    {
        private readonly ILevelsProvider levelsProvider;
        private readonly IContentLoader contentLoader;
        private readonly ICurrentLevelRuntime currentLevelRuntime;
        private readonly ILevelSceneResolver levelSceneResolver;

        public override GameStateType Type => GameStateType.LevelLoad;

        public LevelLoadState(ILevelsProvider levelsProvider, IContentLoader contentLoader,
            ICurrentLevelRuntime currentLevelRuntime, ILevelSceneResolver levelSceneResolver)
        {
            this.levelsProvider = levelsProvider;
            this.contentLoader = contentLoader;
            this.currentLevelRuntime = currentLevelRuntime;
            this.levelSceneResolver = levelSceneResolver;
        }

        public override async UniTask OnEnter()
        {
            if (!levelsProvider.TryGetLevel(out var level))
            {
                Debug.LogError("No current level selected.");
                return;
            }

            if (level.levelSceneRef == null)
            {
                Debug.LogError("Current level has no scene reference.");
                return;
            }

            var loadedScene = await contentLoader.LoadScene(level.levelSceneRef, LoadSceneMode.Additive);

            if (!levelSceneResolver.TryResolve(loadedScene, out var levelContainer))
            {
                Debug.LogError("Loaded scene has no LevelContainer.");
                return;
            }

            currentLevelRuntime.SetLevel(loadedScene, levelContainer);
            RaiseChangeState(GameStateType.Gameplay);
        }
    }
}