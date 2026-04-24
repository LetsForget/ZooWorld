# GameplayState Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement `GameplayState` so it creates runtime animal state on enter, updates spawner and animals every frame, and unloads animal refs plus scene on exit.

**Architecture:** Keep `GameplayState` thin and orchestration-only. Add a small spawner factory interface so the state can be tested in isolation, then drive implementation with focused editor tests using fake level/runtime/content-loader collaborators.

**Tech Stack:** Unity C#, NUnit editor tests, Zenject DI, Addressables content loader, `dotnet test` / `dotnet build`.

---

## File Structure

- Create `Assets/ZooWorld/Tests/Editor/GameStateMachine/GameplayStateTests.cs`: editor tests with local fakes for levels runtime, spawner factory, spawner, and content loader.
- Create `Assets/ZooWorld/Scripts/Animals/Spawner/Base/ISpawnerFactory.cs`: abstraction for creating `IAnimalsSpawner` by `SpawnerType`.
- Modify `Assets/ZooWorld/Scripts/Animals/Spawner/SpawnerFactory.cs`: implement `ISpawnerFactory`.
- Modify `Assets/ZooWorld/Scripts/Animals/AnimalsInstaller.cs`: bind `SpawnerFactory` through interfaces and self.
- Modify `Assets/ZooWorld/Scripts/GameStateMachine/States/GameplayState.cs`: implement enter/update/exit lifecycle.

No git commits. User asked for implementation, not commit workflow.

### Task 1: Add Failing GameplayState Tests

**Files:**
- Create: `Assets/ZooWorld/Tests/Editor/GameStateMachine/GameplayStateTests.cs`

- [ ] **Step 1: Write the failing tests**

Create this test file:

```csharp
using System.Collections.Generic;
using ContentLoading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using ZooWorld.Animals;
using ZooWorld.Animals.Movement;
using ZooWorld.Levels;
using ZooWorld.StateMachine;

namespace ZooWorld.Tests.Editor.GameStateMachine
{
    [TestFixture]
    public class GameplayStateTests
    {
        [Test]
        public async UniTask OnEnter_CreatesAnimalsList_AndInitializesSpawnerFromCurrentLevel()
        {
            var level = CreateLevel();
            var levelsProvider = new FakeLevelsProvider(level, hasLevel: true);
            var runtime = new FakeCurrentLevelRuntime(new LevelContainerStub(CreateSpawnPoints(2)), hasContainer: true);
            var spawner = new FakeAnimalsSpawner();
            var spawnerFactory = new FakeSpawnerFactory(spawner);
            var contentLoader = new FakeContentLoader();
            var state = new GameplayState(levelsProvider, runtime, spawnerFactory, contentLoader);

            await state.OnEnter();

            Assert.That(spawner.InitializeCallCount, Is.EqualTo(1));
            Assert.That(spawner.LastSpawnPoints.Length, Is.EqualTo(2));
            Assert.That(spawner.LastVariants, Is.SameAs(level.animalDTOs));
            Assert.That(spawner.LastTargetList, Is.Not.Null);
            Assert.That(spawner.LastTargetList.Count, Is.EqualTo(0));
        }

        [Test]
        public async UniTask UpdateSelf_UpdatesSpawner_AndAllNonNullAnimals()
        {
            var level = CreateLevel();
            var levelsProvider = new FakeLevelsProvider(level, hasLevel: true);
            var runtime = new FakeCurrentLevelRuntime(new LevelContainerStub(CreateSpawnPoints(1)), hasContainer: true);
            var spawner = new FakeAnimalsSpawner();
            var spawnerFactory = new FakeSpawnerFactory(spawner);
            var contentLoader = new FakeContentLoader();
            var state = new GameplayState(levelsProvider, runtime, spawnerFactory, contentLoader);
            await state.OnEnter();

            var firstDirection = new TrackingDirectionSelector(Vector3.right);
            var firstLocomotion = new TrackingLocomotion();
            var secondDirection = new TrackingDirectionSelector(Vector3.forward);
            var secondLocomotion = new TrackingLocomotion();

            spawner.LastTargetList.Add(new Animal(AnimalType.Elephant, firstDirection, firstLocomotion));
            spawner.LastTargetList.Add(null);
            spawner.LastTargetList.Add(new Animal(AnimalType.Lion, secondDirection, secondLocomotion));

            state.UpdateSelf(0.5f);

            Assert.That(spawner.UpdateCallCount, Is.EqualTo(1));
            Assert.That(firstDirection.UpdateCallCount, Is.EqualTo(1));
            Assert.That(firstLocomotion.MoveCallCount, Is.EqualTo(1));
            Assert.That(secondDirection.UpdateCallCount, Is.EqualTo(1));
            Assert.That(secondLocomotion.MoveCallCount, Is.EqualTo(1));
        }

        [Test]
        public async UniTask OnExit_ReleasesAnimalRefs_UnloadsScene_ClearsRuntime_AndLocalState()
        {
            var level = CreateLevel();
            var scene = default(SceneInstance);
            var levelsProvider = new FakeLevelsProvider(level, hasLevel: true);
            var runtime = new FakeCurrentLevelRuntime(new LevelContainerStub(CreateSpawnPoints(1)), scene, hasContainer: true, hasScene: true);
            var spawner = new FakeAnimalsSpawner();
            var spawnerFactory = new FakeSpawnerFactory(spawner);
            var contentLoader = new FakeContentLoader();
            var state = new GameplayState(levelsProvider, runtime, spawnerFactory, contentLoader);
            await state.OnEnter();

            await state.OnExit();

            Assert.That(contentLoader.ReleasedRefs, Has.Count.EqualTo(level.animalDTOs.Length));
            Assert.That(contentLoader.ReleasedSceneCount, Is.EqualTo(1));
            Assert.That(runtime.ClearCallCount, Is.EqualTo(1));
            Assert.That(spawner.LastTargetListReferenceStillAlive, Is.True);
            state.UpdateSelf(0.25f);
            Assert.That(spawner.UpdateCallCount, Is.EqualTo(0));
        }

        private static LevelDTO CreateLevel()
        {
            return new LevelDTO
            {
                animalDTOs = new[]
                {
                    new AnimalDTO { animalType = AnimalType.Elephant, animalContainerRef = new AssetReference("animal-1") },
                    new AnimalDTO { animalType = AnimalType.Lion, animalContainerRef = new AssetReference("animal-2") }
                },
                spawnerType = SpawnerType.Random,
                spawnerConfig = ScriptableObject.CreateInstance<RandomSpawnerConfig>()
            };
        }

        private static Transform[] CreateSpawnPoints(int count)
        {
            var result = new Transform[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = new GameObject($"SpawnPoint_{i}").transform;
            }
            return result;
        }
    }
}
```

Expected RED reason: `GameplayState` constructor and lifecycle logic do not exist yet, and `ISpawnerFactory` is not defined.

- [ ] **Step 2: Run targeted test command and verify RED**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~GameplayStateTests" -v minimal
```

Expected: FAIL at compile stage because `GameplayState` does not have required constructor/body and `ISpawnerFactory` does not exist.

### Task 2: Add Testable Spawner Factory Contract

**Files:**
- Create: `Assets/ZooWorld/Scripts/Animals/Spawner/Base/ISpawnerFactory.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/Spawner/SpawnerFactory.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/AnimalsInstaller.cs`

- [ ] **Step 1: Add the spawner factory interface**

Create `ISpawnerFactory.cs`:

```csharp
namespace ZooWorld.Animals
{
    public interface ISpawnerFactory
    {
        IAnimalsSpawner Create(SpawnerType spawnerType, IAnimalSpawnerConfig config);
    }
}
```

- [ ] **Step 2: Implement the interface in `SpawnerFactory`**

Change class declaration to:

```csharp
public class SpawnerFactory : ISpawnerFactory
```

Keep the existing `Create(SpawnerType spawnerType, IAnimalSpawnerConfig config)` body unchanged.

- [ ] **Step 3: Bind the interface in installer**

Replace:

```csharp
Container.Bind<SpawnerFactory>().AsSingle();
```

with:

```csharp
Container.BindInterfacesAndSelfTo<SpawnerFactory>().AsSingle();
```

- [ ] **Step 4: Run tests again**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~GameplayStateTests" -v minimal
```

Expected: still FAIL, now because `GameplayState` logic is still missing.

### Task 3: Implement GameplayState Lifecycle

**Files:**
- Modify: `Assets/ZooWorld/Scripts/GameStateMachine/States/GameplayState.cs`

- [ ] **Step 1: Replace empty state with minimal implementation**

Use:

```csharp
using System.Collections.Generic;
using ContentLoading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZooWorld.Animals;
using ZooWorld.Levels;

namespace ZooWorld.StateMachine
{
    public class GameplayState : GameState
    {
        private readonly ILevelsProvider levelsProvider;
        private readonly ICurrentLevelRuntime currentLevelRuntime;
        private readonly ISpawnerFactory spawnerFactory;
        private readonly IContentLoader contentLoader;

        private List<Animal> animals;
        private IAnimalsSpawner spawner;

        public override GameStateType Type => GameStateType.Gameplay;

        public GameplayState(
            ILevelsProvider levelsProvider,
            ICurrentLevelRuntime currentLevelRuntime,
            ISpawnerFactory spawnerFactory,
            IContentLoader contentLoader)
        {
            this.levelsProvider = levelsProvider;
            this.currentLevelRuntime = currentLevelRuntime;
            this.spawnerFactory = spawnerFactory;
            this.contentLoader = contentLoader;
        }

        public override UniTask OnEnter()
        {
            if (!levelsProvider.TryGetLevel(out var level))
            {
                Debug.LogError("No current level selected.");
                return UniTask.CompletedTask;
            }

            if (!currentLevelRuntime.TryGetLevelContainer(out var levelContainer))
            {
                Debug.LogError("No current level container in runtime.");
                return UniTask.CompletedTask;
            }

            animals = new List<Animal>();
            spawner = spawnerFactory.Create(level.spawnerType, level.spawnerConfig);

            if (spawner == null)
            {
                Debug.LogError($"Failed to create spawner for '{level.spawnerType}'.");
                return UniTask.CompletedTask;
            }

            spawner.Initialize(levelContainer.SpawnPoint, level.animalDTOs, animals);
            return UniTask.CompletedTask;
        }

        public override void UpdateSelf(float deltaTime)
        {
            spawner?.UpdateSelf(deltaTime);

            if (animals == null)
            {
                return;
            }

            for (var i = 0; i < animals.Count; i++)
            {
                var animal = animals[i];
                if (animal == null)
                {
                    continue;
                }

                animal.UpdateSelf(deltaTime);
            }
        }

        public override async UniTask OnExit()
        {
            if (levelsProvider.TryGetLevel(out var level) && level.animalDTOs != null)
            {
                foreach (var animalDto in level.animalDTOs)
                {
                    if (animalDto.animalContainerRef != null)
                    {
                        contentLoader.Release(animalDto.animalContainerRef);
                    }
                }
            }

            if (currentLevelRuntime.TryGetLoadedScene(out var scene))
            {
                await contentLoader.ReleaseScene(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }

            currentLevelRuntime.Clear();
            animals?.Clear();
            animals = null;
            spawner = null;
        }
    }
}
```

- [ ] **Step 2: Run targeted tests and verify GREEN**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~GameplayStateTests" -v minimal
```

Expected: PASS for `GameplayStateTests`.

### Task 4: Final Build Verification

**Files:**
- Modify: none

- [ ] **Step 1: Run project build verification**

Run:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
dotnet build Assembly-CSharp-Editor.csproj -nologo
```

Expected: both PASS.

- [ ] **Step 2: Review final diff**

Run:

```powershell
git diff -- Assets/ZooWorld/Scripts/Animals/Spawner/Base/ISpawnerFactory.cs Assets/ZooWorld/Scripts/Animals/Spawner/SpawnerFactory.cs Assets/ZooWorld/Scripts/Animals/AnimalsInstaller.cs Assets/ZooWorld/Scripts/GameStateMachine/States/GameplayState.cs Assets/ZooWorld/Tests/Editor/GameStateMachine/GameplayStateTests.cs docs/superpowers/plans/2026-04-23-gameplay-state.md
```

Expected: only planned `GameplayState`, factory interface, binding, test, and plan changes.
