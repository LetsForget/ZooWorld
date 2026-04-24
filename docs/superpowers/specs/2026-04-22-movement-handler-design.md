# Movement Handler Design

## Context

`Animal` already contains both `AnimalType` and `MovementType`. Movement dispatch must use `MovementType`; `AnimalType` remains independent data.

Existing movement implementations are strongly typed:

- `LinearMovement : IMovement<LinearMovementComponent>`
- `JumpMovement : IMovement<JumpMovementComponent>`

`MovementHandler` is currently empty and must route each animal to the correct movement implementation.

## Chosen Approach

Use a non-generic movement adapter interface for runtime dispatch while keeping typed generic movement implementations.

`MovementHandler` will store a `Dictionary<MovementType, IMovement>` and call the registered movement for `animal.MovementType`.

## Architecture

Introduce a non-generic `IMovement` contract:

```csharp
void Move(IMovementComponent component, float deltaTime);
```

Keep the current generic contract:

```csharp
public interface IMovement<in TMoveComponent> : IMovement
    where TMoveComponent : IMovementComponent
```

Each concrete movement implements its typed method and the non-generic method. The non-generic method validates the component type, casts it, then delegates to the typed method.

`MovementHandler` receives the movement implementations through DI and builds this map:

- `MovementType.Linear -> LinearMovement`
- `MovementType.Jump -> JumpMovement`

## Data Flow

1. Caller passes `Animal` to `IMovementHandler.HandleMovement`.
2. Handler reads `animal.MovementType`.
3. Handler finds the matching non-generic `IMovement`.
4. Handler passes `animal.MovementComponent` and `Time.deltaTime`.
5. Concrete movement validates and executes typed movement logic.

## Error Handling

Unknown `MovementType` throws `ArgumentException`.

Null `MovementComponent` throws `ArgumentNullException`.

Movement/component mismatch throws `ArgumentException` with expected and actual component type names.

These errors should fail fast because they mean animal construction or config is invalid.

## Dependency Injection

`MovementInstaller` will bind:

- `LinearMovement` as single
- `JumpMovement` as single
- `MovementHandler` as `IMovementHandler` single

Existing bindings for `GroundChecker` and `BoundsDirectionChecker` stay unchanged.

## Testing And Verification

Primary verification:

```powershell
dotnet build Assembly-CSharp.csproj -nologo
```

If a local test assembly is practical, add focused tests for:

- Linear movement dispatch calls linear movement with a linear component.
- Jump movement dispatch calls jump movement with a jump component.
- Unknown movement type fails with a clear exception.
- Mismatched movement component fails with a clear exception.
