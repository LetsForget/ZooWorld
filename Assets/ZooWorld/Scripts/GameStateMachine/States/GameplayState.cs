using System.Collections.Generic;
using ContentLoading;
using Cysharp.Threading.Tasks;
using Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZooWorld.Animals;
using ZooWorld.Levels;
using ZooWorld.UI;

namespace ZooWorld.StateMachine
{
    public class GameplayState : GameState
    {
        private readonly ILevelsProvider levelsProvider;
        private readonly ICurrentLevelRuntime currentLevelRuntime;
        private readonly IContentLoader contentLoader;
        private readonly ILogsWriter logsWriter;
        private readonly IUIManager<GameUIType> uiManager;
        
        private readonly List<AnimalRuntime> animals;
        private IAnimalsSpawner spawner;
        private IGameplayFrame gameplayFrame;
        
        public override GameStateType Type => GameStateType.Gameplay;

        public GameplayState(ILevelsProvider levelsProvider, ICurrentLevelRuntime currentLevelRuntime, 
            IContentLoader contentLoader, ILogsWriter logsWriter, IUIManager<GameUIType> uiManager)
        {
            this.levelsProvider = levelsProvider;
            this.currentLevelRuntime = currentLevelRuntime;
            this.contentLoader = contentLoader;
            this.logsWriter = logsWriter;
            this.uiManager = uiManager;
            
            animals = new List<AnimalRuntime>(128);
        }

        public override async UniTask OnEnter()
        {
            gameplayFrame = await uiManager.ShowFrame(FrameType.Screen, GameUIType.Gameplay) as IGameplayFrame;

            if (gameplayFrame == null)
            {
                logsWriter.LogError("Gameplay frame could not be loaded");
                return;
            }
            
            if (!levelsProvider.TryGetLevel(out var level))
            {
                logsWriter.LogError("No current level selected.");
                return;
            }

            var levelContainer = currentLevelRuntime.LevelContainer;
            
            spawner = levelContainer.SpawnerFactory.Create(level.spawnerType, level.spawnerConfig);

            if (spawner == null)
            {
                logsWriter.LogError($"Failed to create spawner for '{level.spawnerType}'.");
                return;
            }

            spawner.Initialize(levelContainer.SpawnPoint, level.animalDTOs, animals);
        }

        public override void UpdateSelf(float deltaTime)
        {
            spawner.UpdateSelf(deltaTime);

            foreach (var runtime in animals)
            {
                if (runtime?.Animal == null || runtime.Animal.IsDead)
                {
                    continue;
                }

                runtime.Animal.UpdateSelf(deltaTime);
            }

            for (var i = animals.Count - 1; i >= 0; i--)
            {
                var runtime = animals[i];

                if (runtime == null || runtime.Animal == null)
                {
                    animals.RemoveAt(i);
                    continue;
                }

                if (!runtime.Animal.IsDead)
                {
                    continue;
                }
                
                switch (runtime.Animal.Type)
                {
                    case AnimalType.Prey:
                    {
                        gameplayFrame.IncreaseFirstCounter();
                        break;
                    }
                    case AnimalType.Predator:
                    {
                        gameplayFrame.IncreaseSecondCounter();
                        break;
                    }
                }

                var position = runtime.Container.transform.position;
                var camera = currentLevelRuntime.LevelContainer.Camera;
                gameplayFrame.ShowFloatingText("Tasty!", position, camera);
                
                animals.RemoveAt(i);
                Object.Destroy(runtime.Container.gameObject);
            }
        }

        public override async UniTask OnExit()
        {
            if (levelsProvider.TryGetLevel(out var level))
            {
                foreach (var animalDto in level.animalDTOs)
                {
                    if (animalDto.animalContainerRef == null)
                    {
                        continue;
                    }
                    
                    contentLoader.Release(animalDto.animalContainerRef);
                }
            }

            if (currentLevelRuntime.TryGetLoadedScene(out var scene))
            {
                await contentLoader.ReleaseScene(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }

            currentLevelRuntime.Clear();
            animals.Clear();
        }
    }
}