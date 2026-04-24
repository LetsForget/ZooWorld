# Animal Collision Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add physics-driven animal collision detection that routes collision handling through `Animal` interaction strategies and removes dead animals during gameplay update cleanup.

**Architecture:** Keep Unity physics callbacks on a thin relay component and keep gameplay response inside `Animal` via `IAnimalInteractionBehaviour`, mirroring the existing `IDirectionSelector` and `ILocomotion` strategy pattern. Introduce `AnimalRuntime` as the bridge between `AnimalContainer` and the plain C# `Animal`, then update spawner and gameplay state to operate on runtime objects instead of bare animals.

**Tech Stack:** Unity C#, NUnit editor tests, Zenject DI, Unity physics callbacks, `dotnet test` / `dotnet build`.

---

## File Structure

- Create `Assets/ZooWorld/Tests/Editor/Animals/AnimalInteractionTests.cs`: unit tests for `Animal.HandleCollision`, predator/prey behaviors, and dead marker behavior.
- Create `Assets/ZooWorld/Tests/Editor/Animals/AnimalCollisionRelayTests.cs`: editor tests that simulate relay-to-relay collisions and verify one logical collision per pair.
- Create `Assets/ZooWorld/Tests/Editor/GameStateMachine/GameplayStateCollisionCleanupTests.cs`: editor tests for cleanup of dead animals in gameplay update.
- Create `Assets/ZooWorld/Scripts/Animals/Interaction/Base/IAnimalInteractionBehaviour.cs`: interaction strategy contract.
- Create `Assets/ZooWorld/Scripts/Animals/Interaction/AnimalInteractionBehavioursFactory.cs`: maps `AnimalType` to strategy implementation.
- Create `Assets/ZooWorld/Scripts/Animals/Interaction/Types/PredatorInteractionBehaviour.cs`: predator collision rules.
- Create `Assets/ZooWorld/Scripts/Animals/Interaction/Types/PreyInteractionBehaviour.cs`: prey no-op collision rules.
- Create `Assets/ZooWorld/Scripts/Animals/AnimalRuntime.cs`: bridge object for `Animal` + `AnimalContainer`.
- Create `Assets/ZooWorld/Scripts/Animals/AnimalCollisionRelay.cs`: Unity collision callback relay.
- Modify `Assets/ZooWorld/Scripts/Animals/Animal.cs`: add interaction behavior, dead marker, and collision entrypoint.
- Modify `Assets/ZooWorld/Scripts/Animals/AnimalFactory.cs`: assemble `AnimalRuntime`, strategy, and relay.
- Modify `Assets/ZooWorld/Scripts/Animals/AnimalsInstaller.cs`: bind interaction behavior factory.
- Modify `Assets/ZooWorld/Scripts/Animals/Spawner/Base/IAnimalsSpawner.cs`: switch target list from `List<Animal>` to `List<AnimalRuntime>`.
- Modify `Assets/ZooWorld/Scripts/Animals/Spawner/RandomAnimalSpawner/RandomSpawner.cs`: spawn and store `AnimalRuntime`.
- Modify `Assets/ZooWorld/Scripts/GameStateMachine/States/GameplayState.cs`: update `AnimalRuntime` list, tick living animals, and purge dead ones.

No git commits in this plan. Workspace already contains large unrelated staged changes.

### Task 1: Add Failing Interaction Tests

**Files:**
- Create: `Assets/ZooWorld/Tests/Editor/Animals/AnimalInteractionTests.cs`

- [ ] **Step 1: Write the failing interaction tests**

Create this file:

```csharp
using NUnit.Framework;
using UnityEngine;
using ZooWorld.Animals;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Tests.Editor.Animals
{
    [TestFixture]
    public class AnimalInteractionTests
    {
        [Test]
        public void HandleCollision_DelegatesToInteractionBehaviour()
        {
            var behaviour = new TrackingInteractionBehaviour();
            var self = new Animal(
                AnimalType.Prey,
                new StubDirectionSelector(),
                new StubLocomotion(),
                behaviour);
            var other = new Animal(
                AnimalType.Predator,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PreyInteractionBehaviour());

            self.HandleCollision(other);

            Assert.That(behaviour.HandleCallCount, Is.EqualTo(1));
            Assert.That(behaviour.LastSelf, Is.SameAs(self));
            Assert.That(behaviour.LastOther, Is.SameAs(other));
        }

        [Test]
        public void PredatorInteractionBehaviour_MarksPreyDead()
        {
            var predator = new Animal(
                AnimalType.Predator,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PredatorInteractionBehaviour());
            var prey = new Animal(
                AnimalType.Prey,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PreyInteractionBehaviour());

            predator.HandleCollision(prey);

            Assert.That(prey.IsDead, Is.True);
        }

        [Test]
        public void PredatorInteractionBehaviour_MarksPredatorDead()
        {
            var first = new Animal(
                AnimalType.Predator,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PredatorInteractionBehaviour());
            var second = new Animal(
                AnimalType.Predator,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PredatorInteractionBehaviour());

            first.HandleCollision(second);

            Assert.That(second.IsDead, Is.True);
        }

        [Test]
        public void PreyInteractionBehaviour_DoesNothing()
        {
            var prey = new Animal(
                AnimalType.Prey,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PreyInteractionBehaviour());
            var other = new Animal(
                AnimalType.Predator,
                new StubDirectionSelector(),
                new StubLocomotion(),
                new PredatorInteractionBehaviour());

            prey.HandleCollision(other);

            Assert.That(other.IsDead, Is.False);
        }

        private sealed class TrackingInteractionBehaviour : IAnimalInteractionBehaviour
        {
            public int HandleCallCount { get; private set; }
            public Animal LastSelf { get; private set; }
            public Animal LastOther { get; private set; }

            public void HandleCollision(Animal self, Animal other)
            {
                HandleCallCount++;
                LastSelf = self;
                LastOther = other;
            }
        }

        private sealed class StubDirectionSelector : IDirectionSelector
        {
            public Vector3 UpdateDirection(Vector3 oldDirection, float deltaTime) => oldDirection;
        }

        private sealed class StubLocomotion : ILocomotion
        {
            public void Move(Vector3 direction, float deltaTime) { }
        }
    }
}
```

Expected RED reason: `Animal` does not yet accept an interaction behavior, `IAnimalInteractionBehaviour` does not exist, and predator/prey strategy classes do not exist.

- [ ] **Step 2: Run targeted test command and verify RED**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~AnimalInteractionTests" -v minimal
```

Expected: FAIL at compile stage because the interaction behavior types and updated `Animal` constructor do not exist.

### Task 2: Implement Interaction Strategies And Dead Marker

**Files:**
- Create: `Assets/ZooWorld/Scripts/Animals/Interaction/Base/IAnimalInteractionBehaviour.cs`
- Create: `Assets/ZooWorld/Scripts/Animals/Interaction/Types/PredatorInteractionBehaviour.cs`
- Create: `Assets/ZooWorld/Scripts/Animals/Interaction/Types/PreyInteractionBehaviour.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/Animal.cs`

- [ ] **Step 1: Add the interaction behavior interface**

Create `Assets/ZooWorld/Scripts/Animals/Interaction/Base/IAnimalInteractionBehaviour.cs`:

```csharp
namespace ZooWorld.Animals
{
    public interface IAnimalInteractionBehaviour
    {
        void HandleCollision(Animal self, Animal other);
    }
}
```

- [ ] **Step 2: Add the predator and prey strategy classes**

Create `Assets/ZooWorld/Scripts/Animals/Interaction/Types/PredatorInteractionBehaviour.cs`:

```csharp
namespace ZooWorld.Animals
{
    public sealed class PredatorInteractionBehaviour : IAnimalInteractionBehaviour
    {
        public void HandleCollision(Animal self, Animal other)
        {
            if (other == null)
            {
                return;
            }

            switch (other.Type)
            {
                case AnimalType.Prey:
                case AnimalType.Predator:
                    other.MarkDead();
                    break;
            }
        }
    }
}
```

Create `Assets/ZooWorld/Scripts/Animals/Interaction/Types/PreyInteractionBehaviour.cs`:

```csharp
namespace ZooWorld.Animals
{
    public sealed class PreyInteractionBehaviour : IAnimalInteractionBehaviour
    {
        public void HandleCollision(Animal self, Animal other)
        {
        }
    }
}
```

- [ ] **Step 3: Extend `Animal` with collision entrypoint and dead marker**

Update `Assets/ZooWorld/Scripts/Animals/Animal.cs` to:

```csharp
using UnityEngine;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    public class Animal
    {
        private readonly IDirectionSelector directionSelector;
        private readonly ILocomotion locomotion;
        private readonly IAnimalInteractionBehaviour interactionBehaviour;

        private Vector3 currentDirection;

        public AnimalType Type { get; }
        public bool IsDead { get; private set; }

        public Animal(
            AnimalType type,
            IDirectionSelector directionSelector,
            ILocomotion locomotion,
            IAnimalInteractionBehaviour interactionBehaviour)
        {
            this.directionSelector = directionSelector;
            this.locomotion = locomotion;
            this.interactionBehaviour = interactionBehaviour;

            Type = type;
        }

        public void UpdateSelf(float deltaTime)
        {
            currentDirection = directionSelector.UpdateDirection(currentDirection, deltaTime);
            locomotion.Move(currentDirection, deltaTime);
        }

        public void HandleCollision(Animal other)
        {
            if (IsDead || other == null)
            {
                return;
            }

            interactionBehaviour?.HandleCollision(this, other);
        }

        public void MarkDead()
        {
            IsDead = true;
        }
    }
}
```

- [ ] **Step 4: Run the interaction tests and verify GREEN**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~AnimalInteractionTests" -v minimal
```

Expected: PASS.

### Task 3: Add Failing Relay Tests

**Files:**
- Create: `Assets/ZooWorld/Tests/Editor/Animals/AnimalCollisionRelayTests.cs`

- [ ] **Step 1: Write the failing relay tests**

Create this file:

```csharp
using NUnit.Framework;
using UnityEngine;
using ZooWorld.Animals;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Tests.Editor.Animals
{
    [TestFixture]
    public class AnimalCollisionRelayTests
    {
        [Test]
        public void DispatchCollision_ProcessesBothSidesOnce()
        {
            var firstRuntime = CreateRuntime(AnimalType.Predator, out var firstRelay);
            var secondRuntime = CreateRuntime(AnimalType.Prey, out var secondRelay);

            firstRelay.DispatchCollision(secondRelay);
            secondRelay.DispatchCollision(firstRelay);

            Assert.That(firstRuntime.Animal.IsDead, Is.False);
            Assert.That(secondRuntime.Animal.IsDead, Is.True);
            Assert.That(firstRelay.DebugDispatchCount, Is.EqualTo(1));
            Assert.That(secondRelay.DebugDispatchCount, Is.EqualTo(0));
        }

        [Test]
        public void DispatchCollision_IgnoresNullOtherRelay()
        {
            var runtime = CreateRuntime(AnimalType.Predator, out var relay);

            relay.DispatchCollision(null);

            Assert.That(runtime.Animal.IsDead, Is.False);
            Assert.That(relay.DebugDispatchCount, Is.EqualTo(0));
        }

        private static AnimalRuntime CreateRuntime(AnimalType type, out AnimalCollisionRelay relay)
        {
            var gameObject = new GameObject(type.ToString());
            var container = gameObject.AddComponent<AnimalContainerStub>();
            relay = gameObject.AddComponent<AnimalCollisionRelay>();

            var behaviour = type == AnimalType.Predator
                ? new PredatorInteractionBehaviour()
                : new PreyInteractionBehaviour();

            var animal = new Animal(type, new StubDirectionSelector(), new StubLocomotion(), behaviour);
            var runtime = new AnimalRuntime(animal, container);
            relay.Initialize(runtime);
            return runtime;
        }

        private sealed class StubDirectionSelector : IDirectionSelector
        {
            public Vector3 UpdateDirection(Vector3 oldDirection, float deltaTime) => oldDirection;
        }

        private sealed class StubLocomotion : ILocomotion
        {
            public void Move(Vector3 direction, float deltaTime) { }
        }

        private sealed class AnimalContainerStub : AnimalContainer
        {
        }
    }
}
```

Expected RED reason: `AnimalRuntime` and `AnimalCollisionRelay` do not exist, and relay dispatch/dedupe API does not exist.

- [ ] **Step 2: Run targeted test command and verify RED**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~AnimalCollisionRelayTests" -v minimal
```

Expected: FAIL at compile stage because runtime bridge and relay types do not exist.

### Task 4: Implement Runtime Bridge, Relay, And Factory Wiring

**Files:**
- Create: `Assets/ZooWorld/Scripts/Animals/AnimalRuntime.cs`
- Create: `Assets/ZooWorld/Scripts/Animals/AnimalCollisionRelay.cs`
- Create: `Assets/ZooWorld/Scripts/Animals/Interaction/AnimalInteractionBehavioursFactory.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/AnimalFactory.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/AnimalsInstaller.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/Spawner/Base/IAnimalsSpawner.cs`
- Modify: `Assets/ZooWorld/Scripts/Animals/Spawner/RandomAnimalSpawner/RandomSpawner.cs`

- [ ] **Step 1: Add the runtime bridge**

Create `Assets/ZooWorld/Scripts/Animals/AnimalRuntime.cs`:

```csharp
namespace ZooWorld.Animals
{
    public sealed class AnimalRuntime
    {
        public Animal Animal { get; }
        public AnimalContainer Container { get; }

        public AnimalRuntime(Animal animal, AnimalContainer container)
        {
            Animal = animal;
            Container = container;
        }
    }
}
```

- [ ] **Step 2: Add the interaction behavior factory**

Create `Assets/ZooWorld/Scripts/Animals/Interaction/AnimalInteractionBehavioursFactory.cs`:

```csharp
using System.Collections.Generic;
using Logging;

namespace ZooWorld.Animals
{
    public sealed class AnimalInteractionBehavioursFactory
    {
        private readonly IReadOnlyDictionary<AnimalType, IAnimalInteractionBehaviour> behaviours;
        private readonly ILogsWriter logsWriter;

        public AnimalInteractionBehavioursFactory(ILogsWriter logsWriter)
        {
            this.logsWriter = logsWriter;

            behaviours = new Dictionary<AnimalType, IAnimalInteractionBehaviour>
            {
                { AnimalType.Predator, new PredatorInteractionBehaviour() },
                { AnimalType.Prey, new PreyInteractionBehaviour() }
            };
        }

        public IAnimalInteractionBehaviour Create(AnimalType type)
        {
            if (!behaviours.TryGetValue(type, out var behaviour))
            {
                logsWriter.LogError($"Unknown animal interaction type: {type}");
                return new PreyInteractionBehaviour();
            }

            return behaviour;
        }
    }
}
```

- [ ] **Step 3: Add the collision relay**

Create `Assets/ZooWorld/Scripts/Animals/AnimalCollisionRelay.cs`:

```csharp
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalCollisionRelay : MonoBehaviour
    {
        private AnimalRuntime runtime;

        public int DebugDispatchCount { get; private set; }

        public void Initialize(AnimalRuntime runtime)
        {
            this.runtime = runtime;
        }

        public void DispatchCollision(AnimalCollisionRelay otherRelay)
        {
            if (runtime == null || otherRelay == null || otherRelay.runtime == null)
            {
                return;
            }

            if (ReferenceEquals(this, otherRelay))
            {
                return;
            }

            if (GetInstanceID() > otherRelay.GetInstanceID())
            {
                return;
            }

            DebugDispatchCount++;
            runtime.Animal.HandleCollision(otherRelay.runtime.Animal);
            otherRelay.runtime.Animal.HandleCollision(runtime.Animal);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var otherRelay = collision.rigidbody != null
                ? collision.rigidbody.GetComponent<AnimalCollisionRelay>()
                : collision.collider.GetComponent<AnimalCollisionRelay>();

            DispatchCollision(otherRelay);
        }
    }
}
```

- [ ] **Step 4: Update factory, installer, and spawner contracts**

Update `Assets/ZooWorld/Scripts/Animals/AnimalFactory.cs` method and constructor dependencies:

```csharp
private readonly AnimalInteractionBehavioursFactory interactionBehavioursFactory;

public AnimalFactory(
    IContentLoader contentLoader,
    ILogsWriter logsWriter,
    DirectionSelectorsFactory directionSelectorsFactory,
    LocomotionsFactory locomotionsFactory,
    AnimalInteractionBehavioursFactory interactionBehavioursFactory)
{
    this.contentLoader = contentLoader;
    this.logsWriter = logsWriter;
    this.directionSelectorsFactory = directionSelectorsFactory;
    this.locomotionsFactory = locomotionsFactory;
    this.interactionBehavioursFactory = interactionBehavioursFactory;
}

public async UniTask<AnimalRuntime> CreateAnimal(AnimalDTO animalDTO, Vector3 spawnPosition)
{
    var container = await contentLoader.Load<AnimalContainer>(animalDTO.animalContainerRef);
    container.transform.position = spawnPosition;

    var directionSelector = directionSelectorsFactory.Create(
        animalDTO.directionSelectType,
        container,
        animalDTO.directionSelectorConfig);
    var locomotion = locomotionsFactory.Create(
        animalDTO.locomotionType,
        container,
        animalDTO.locomotionConfig);
    var interactionBehaviour = interactionBehavioursFactory.Create(animalDTO.animalType);

    var animal = new Animal(animalDTO.animalType, directionSelector, locomotion, interactionBehaviour);
    var runtime = new AnimalRuntime(animal, container);
    var relay = container.GetComponent<AnimalCollisionRelay>() ?? container.gameObject.AddComponent<AnimalCollisionRelay>();
    relay.Initialize(runtime);
    return runtime;
}
```

Update `Assets/ZooWorld/Scripts/Animals/AnimalsInstaller.cs` binding block:

```csharp
Container.Bind<DirectionSelectorsFactory>().AsSingle();
Container.Bind<LocomotionsFactory>().AsSingle();
Container.Bind<AnimalInteractionBehavioursFactory>().AsSingle();
Container.Bind<AnimalFactory>().AsSingle();
Container.Bind<SpawnerFactory>().AsSingle();
```

Update `Assets/ZooWorld/Scripts/Animals/Spawner/Base/IAnimalsSpawner.cs`:

```csharp
void Initialize(Transform[] spawnPoints, AnimalDTO[] variants, List<AnimalRuntime> targetList);
```

Update `Assets/ZooWorld/Scripts/Animals/Spawner/RandomAnimalSpawner/RandomSpawner.cs` fields and spawn append:

```csharp
private List<AnimalRuntime> targetList;

public void Initialize(Transform[] spawnPoints, AnimalDTO[] variants, List<AnimalRuntime> targetList)
{
    this.spawnPoints = spawnPoints;
    this.variants = variants;
    this.targetList = targetList;
}

var animal = await animalFactory.CreateAnimal(animalDTO, spawnPoint.position);
targetList.Add(animal);
```

- [ ] **Step 5: Run relay tests and verify GREEN**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~AnimalCollisionRelayTests" -v minimal
```

Expected: PASS.

### Task 5: Add Failing Gameplay Cleanup Tests

**Files:**
- Create: `Assets/ZooWorld/Tests/Editor/GameStateMachine/GameplayStateCollisionCleanupTests.cs`

- [ ] **Step 1: Write the failing cleanup tests**

Create this file:

```csharp
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ZooWorld.Animals;
using ZooWorld.Animals.Movement;
using ZooWorld.StateMachine;

namespace ZooWorld.Tests.Editor.GameStateMachine
{
    [TestFixture]
    public class GameplayStateCollisionCleanupTests
    {
        [Test]
        public void UpdateSelf_RemovesDeadRuntimeEntries_AfterUpdatingSpawner()
        {
            var state = GameplayStateTestFactory.Create(out var spawner, out var runtimes);
            runtimes.Add(CreateRuntime(AnimalType.Predator, isDead: false));
            runtimes.Add(CreateRuntime(AnimalType.Prey, isDead: true));
            runtimes.Add(CreateRuntime(AnimalType.Prey, isDead: false));

            state.UpdateSelf(0.25f);

            Assert.That(spawner.UpdateCallCount, Is.EqualTo(1));
            Assert.That(runtimes.Count, Is.EqualTo(2));
            Assert.That(runtimes.TrueForAll(runtime => runtime.Animal.IsDead == false), Is.True);
        }

        private static AnimalRuntime CreateRuntime(AnimalType type, bool isDead)
        {
            var container = new GameObject(type.ToString()).AddComponent<AnimalContainerStub>();
            var behaviour = type == AnimalType.Predator
                ? new PredatorInteractionBehaviour()
                : new PreyInteractionBehaviour();
            var animal = new Animal(type, new StubDirectionSelector(), new StubLocomotion(), behaviour);

            if (isDead)
            {
                animal.MarkDead();
            }

            return new AnimalRuntime(animal, container);
        }

        private sealed class StubDirectionSelector : IDirectionSelector
        {
            public Vector3 UpdateDirection(Vector3 oldDirection, float deltaTime) => oldDirection;
        }

        private sealed class StubLocomotion : ILocomotion
        {
            public void Move(Vector3 direction, float deltaTime) { }
        }

        private sealed class AnimalContainerStub : AnimalContainer
        {
        }
    }
}
```

Expected RED reason: `GameplayState` still stores `List<Animal>`, not `List<AnimalRuntime>`, and it does not purge dead runtimes after update.

- [ ] **Step 2: Run targeted test command and verify RED**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~GameplayStateCollisionCleanupTests" -v minimal
```

Expected: FAIL at compile stage or assertion stage because gameplay cleanup is not implemented.

### Task 6: Implement Gameplay Cleanup And Full Verification

**Files:**
- Modify: `Assets/ZooWorld/Scripts/GameStateMachine/States/GameplayState.cs`

- [ ] **Step 1: Replace bare animal list with runtime list and add purge pass**

Update `Assets/ZooWorld/Scripts/GameStateMachine/States/GameplayState.cs` relevant members and update loop:

```csharp
private readonly List<AnimalRuntime> animals;

public GameplayState(
    ILevelsProvider levelsProvider,
    ICurrentLevelRuntime currentLevelRuntime,
    IContentLoader contentLoader,
    ILogsWriter logsWriter)
{
    this.levelsProvider = levelsProvider;
    this.currentLevelRuntime = currentLevelRuntime;
    this.contentLoader = contentLoader;
    this.logsWriter = logsWriter;

    animals = new List<AnimalRuntime>(128);
}

public override void UpdateSelf(float deltaTime)
{
    spawner.UpdateSelf(deltaTime);

    for (var i = 0; i < animals.Count; i++)
    {
        var runtime = animals[i];

        if (runtime == null || runtime.Animal == null || runtime.Animal.IsDead)
        {
            continue;
        }

        runtime.Animal.UpdateSelf(deltaTime);
    }

    animals.RemoveAll(runtime => runtime == null || runtime.Animal == null || runtime.Animal.IsDead);
}
```

Keep `OnEnter` initialization and `OnExit` cleanup logic the same except for the runtime list type.

- [ ] **Step 2: Run cleanup tests and verify GREEN**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~GameplayStateCollisionCleanupTests" -v minimal
```

Expected: PASS.

- [ ] **Step 3: Run all collision-related editor tests**

Run:

```powershell
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~AnimalInteractionTests" -v minimal
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~AnimalCollisionRelayTests" -v minimal
dotnet test Assembly-CSharp-Editor.csproj --filter "FullyQualifiedName~GameplayStateCollisionCleanupTests" -v minimal
```

Expected: PASS for all three suites.

- [ ] **Step 4: Run editor build verification**

Run:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
dotnet build Assembly-CSharp-Editor.csproj -nologo
```

Expected: both builds succeed with zero compile errors.

## Self-Review

- Spec coverage: detection uses physics relay, handling lives in `Animal`, predator/prey rules are explicit, dead marker and gameplay cleanup are covered, and the `AnimalRuntime` bridge is wired through factory and spawner changes.
- Placeholder scan: no `TODO`/`TBD` placeholders remain in tasks.
- Type consistency: plan uses `IAnimalInteractionBehaviour`, `AnimalRuntime`, `AnimalCollisionRelay`, `HandleCollision`, `MarkDead`, and `IsDead` consistently.
