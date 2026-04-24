# Animal Collision Design

## Goal

Add animal-vs-animal collision handling that fits the current `Animal` architecture and can evolve from `MovePosition` to `AddForce`-driven physics.

Current gameplay rules:

- `Predator` reacts to `Prey`
- `Predator` reacts to `Predator`
- `Prey` does nothing

For now, collision results are placeholders only. "Eat" means marking the target animal as dead. Cleanup happens later in `GameplayState`.

## Recommended Approach

Use Unity physics callbacks for collision detection and keep gameplay reaction inside `Animal` through a strategy interface.

This keeps responsibilities separated:

- Unity-side objects detect collision
- `Animal` owns gameplay response
- strategy classes define per-type interaction rules

This matches the existing direction and locomotion design, where `Animal` delegates behavior through interfaces.

## Architecture

### `Animal`

Extend `Animal` with:

- `IAnimalInteractionBehaviour interactionBehaviour`
- `bool IsDead`
- `void HandleCollision(Animal other)`
- `void MarkDead()`

`HandleCollision` delegates to `interactionBehaviour`.

### `IAnimalInteractionBehaviour`

Add runtime strategy interface:

```csharp
public interface IAnimalInteractionBehaviour
{
    void HandleCollision(Animal self, Animal other);
}
```

Implementations:

- `PredatorInteractionBehaviour`
- `PreyInteractionBehaviour`

Behavior rules:

- `Predator + Prey` -> placeholder eat path
- `Predator + Predator` -> placeholder eat path
- `Prey + anything` -> no-op

### `AnimalCollisionRelay`

Add a Unity `MonoBehaviour` on the animal prefab or root container.

Responsibilities:

- receive `OnCollisionEnter`
- resolve the other animal relay
- ignore collisions with non-animal objects
- deduplicate a pair so one physical contact is handled once
- call collision handling for both animals

After dedupe:

- `selfAnimal.HandleCollision(otherAnimal)`
- `otherAnimal.HandleCollision(selfAnimal)`

Calling both sides is important because interaction is asymmetric.

### `AnimalRuntime`

Add a thin bridge object that stores:

- `Animal Animal`
- `AnimalContainer Container`

Purpose:

- connect plain C# gameplay object with Unity-side container
- let relay reach the runtime `Animal` without a global registry

## Factory Integration

Update `AnimalFactory` to assemble the full runtime object:

1. load `AnimalContainer`
2. create `IDirectionSelector`
3. create `ILocomotion`
4. create `IAnimalInteractionBehaviour`
5. create `Animal`
6. create `AnimalRuntime`
7. attach or initialize `AnimalCollisionRelay`
8. return `AnimalRuntime`

`AnimalInteractionBehavioursFactory` should mirror the existing selector and locomotion factories and map `AnimalType` to behavior implementation.

## Cleanup Flow

Do not remove animals inside physics callbacks.

Instead:

1. collision marks target as dead through `MarkDead()`
2. `GameplayState.UpdateSelf` finishes normal update loop
3. a cleanup pass removes all animals where `IsDead == true`

This avoids mutating runtime collections during collision callbacks and keeps removal centralized.

## Constraints

- process only `OnCollisionEnter` for first version
- ignore self-collision
- ignore missing relay/runtime safely
- keep destroy/release logic as future work
- design must remain valid after locomotion moves from `MovePosition` to `AddForce`

## Testing

Minimum test coverage:

- `Animal.HandleCollision` delegates to behavior
- `PredatorInteractionBehaviour` marks prey dead
- `PredatorInteractionBehaviour` marks predator dead
- `PreyInteractionBehaviour` does nothing
- relay pair dedupe triggers one logical collision per contact
- cleanup pass removes dead animals from gameplay runtime
