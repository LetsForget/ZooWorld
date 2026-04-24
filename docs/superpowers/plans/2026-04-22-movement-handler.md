# Movement Handler Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement `MovementHandler` runtime dispatch by `Animal.MovementType`.

**Architecture:** Add a non-generic `IMovement` contract for runtime dispatch and keep the existing generic `IMovement<TMoveComponent>` for typed movement code. `MovementHandler` stores a `MovementType -> IMovement` map and routes each `Animal.MovementComponent` to the correct movement implementation using `Time.deltaTime`.

**Tech Stack:** Unity C#, Zenject DI, .NET build verification.

---

## File Structure

- Modify `Assets/ZooWorld/Scripts/Animals/Movement/Types/Base/IMovement.cs`: add non-generic `IMovement` and make generic `IMovement<TMoveComponent>` inherit it.
- Modify `Assets/ZooWorld/Scripts/Animals/Movement/Types/LinearMovement/LinearMovement.cs`: implement non-generic adapter method with component validation.
- Modify `Assets/ZooWorld/Scripts/Animals/Movement/Types/JumpMovement/JumpMovement.cs`: implement non-generic adapter method with component validation.
- Modify `Assets/ZooWorld/Scripts/Animals/Movement/MovementHandler.cs`: add constructor dependencies, movement map, validation, and dispatch.
- Modify `Assets/ZooWorld/Scripts/Animals/Movement/MovementInstaller.cs`: bind `LinearMovement`, `JumpMovement`, and `MovementHandler`.

No git commits. User explicitly requested no commits.

### Task 1: Movement Interface Adapter

**Files:**
- Modify: `Assets/ZooWorld/Scripts/Animals/Movement/Types/Base/IMovement.cs`

- [ ] **Step 1: Write compile-failing contract change**

Replace file with:

```csharp
namespace ZooWorld.Movement
{
    public interface IMovement
    {
        void Move(IMovementComponent component, float deltaTime);
    }

    public interface IMovement<in TMoveComponent> : IMovement where TMoveComponent : IMovementComponent
    {
        void Move(TMoveComponent component, float deltaTime);
    }
}
```

- [ ] **Step 2: Run build and verify expected RED**

Run:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
```

Expected: FAIL because `LinearMovement` and `JumpMovement` do not implement `IMovement.Move(IMovementComponent, float)`.

- [ ] **Step 3: Implement adapter methods in concrete movement classes**

In `LinearMovement.cs`, add `using System;` and add:

```csharp
void IMovement.Move(IMovementComponent component, float deltaTime)
{
    if (component == null)
    {
        throw new ArgumentNullException(nameof(component));
    }

    if (component is not LinearMovementComponent linearMovementComponent)
    {
        throw new ArgumentException(
            $"Expected {nameof(LinearMovementComponent)}, got {component.GetType().Name}.",
            nameof(component));
    }

    Move(linearMovementComponent, deltaTime);
}
```

In `JumpMovement.cs`, add:

```csharp
void IMovement.Move(IMovementComponent component, float deltaTime)
{
    if (component == null)
    {
        throw new ArgumentNullException(nameof(component));
    }

    if (component is not JumpMovementComponent jumpMovementComponent)
    {
        throw new ArgumentException(
            $"Expected {nameof(JumpMovementComponent)}, got {component.GetType().Name}.",
            nameof(component));
    }

    Move(jumpMovementComponent, deltaTime);
}
```

`JumpMovement.cs` already has `using System;`.

- [ ] **Step 4: Run build and verify GREEN for adapter**

Run:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
```

Expected: PASS for interface adapter. If build fails because `MovementHandler` is still not implemented, continue to Task 2 and verify final build there.

### Task 2: MovementHandler Dispatch

**Files:**
- Modify: `Assets/ZooWorld/Scripts/Animals/Movement/MovementHandler.cs`

- [ ] **Step 1: Replace current `NotImplementedException` body with dispatch implementation**

Use:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Movement;

namespace ZooWorld.Animals
{
    public class MovementHandler : IMovementHandler
    {
        private readonly IReadOnlyDictionary<MovementType, IMovement> movements;

        public MovementHandler(LinearMovement linearMovement, JumpMovement jumpMovement)
        {
            movements = new Dictionary<MovementType, IMovement>
            {
                { MovementType.Linear, linearMovement },
                { MovementType.Jump, jumpMovement }
            };
        }

        public void HandleMovement(Animal animal)
        {
            if (animal.MovementComponent == null)
            {
                throw new ArgumentNullException(nameof(animal.MovementComponent));
            }

            if (!movements.TryGetValue(animal.MovementType, out var movement))
            {
                throw new ArgumentException($"Movement type {animal.MovementType} is not registered.", nameof(animal));
            }

            movement.Move(animal.MovementComponent, Time.deltaTime);
        }
    }
}
```

- [ ] **Step 2: Run build**

Run:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
```

Expected: PASS unless DI bindings are still missing compile-only dependencies.

### Task 3: Zenject Bindings

**Files:**
- Modify: `Assets/ZooWorld/Scripts/Animals/Movement/MovementInstaller.cs`

- [ ] **Step 1: Add movement bindings**

Add these bindings after checker bindings:

```csharp
Container.BindInterfacesAndSelfTo<LinearMovement>().AsSingle();
Container.BindInterfacesAndSelfTo<JumpMovement>().AsSingle();
Container.BindInterfacesAndSelfTo<MovementHandler>().AsSingle();
```

Add `using ZooWorld.Movement;` already exists in file.

- [ ] **Step 2: Run final build**

Run:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
```

Expected: PASS.

- [ ] **Step 3: Review changed files**

Run:

```powershell
git diff -- Assets\ZooWorld\Scripts\Animals\Movement\Types\Base\IMovement.cs Assets\ZooWorld\Scripts\Animals\Movement\Types\LinearMovement\LinearMovement.cs Assets\ZooWorld\Scripts\Animals\Movement\Types\JumpMovement\JumpMovement.cs Assets\ZooWorld\Scripts\Animals\Movement\MovementHandler.cs Assets\ZooWorld\Scripts\Animals\Movement\MovementInstaller.cs
```

Expected: only planned movement dispatch changes.
