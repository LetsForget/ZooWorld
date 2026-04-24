# Gameplay State Design

## Context

`GameplayState` currently exists but is empty.

Runtime data needed for gameplay already exists in other systems:

- `ILevelsProvider` provides current `LevelDTO`.
- `ICurrentLevelRuntime` stores loaded `SceneInstance` and `LevelContainer`.
- `LevelContainer` exposes `SpawnPoint`.
- `SpawnerFactory` creates `IAnimalsSpawner`.
- `Animal` already has `UpdateSelf(float deltaTime)`.

`GameplayState` must own active gameplay loop for current level:

- create runtime `List<Animal>`
- create and initialize spawner from current level config
- tick spawner and spawned animals every frame
- unload animal addressable refs and loaded level scene on exit

## Chosen Approach

Keep `GameplayState` as thin runtime orchestrator.

It will not introduce a separate `GameplaySession` abstraction yet. Current logic is small, and existing dependencies already provide all data needed for orchestration.

## Architecture

`GameplayState` will receive these dependencies through constructor injection:

- `ILevelsProvider`
- `ICurrentLevelRuntime`
- `SpawnerFactory`
- `IContentLoader`

It will store these runtime fields:

- `List<Animal> animals`
- `IAnimalsSpawner spawner`

No separate cached `LevelContainer` or `SceneInstance` field is required because both can be read from `ICurrentLevelRuntime` during enter and exit.

## State Lifecycle

### OnEnter

1. Read current `LevelDTO` from `ILevelsProvider`.
2. Read current `LevelContainer` from `ICurrentLevelRuntime`.
3. If either read fails, log error and stop enter flow.
4. Create empty `List<Animal>`.
5. Create spawner via `SpawnerFactory.Create(level.spawnerType, level.spawnerConfig)`.
6. If spawner creation fails, log error and stop enter flow.
7. Initialize spawner with:
   - `levelContainer.SpawnPoint`
   - `level.animalDTOs`
   - created `animals` list

### UpdateSelf

1. Call `spawner.UpdateSelf(deltaTime)` if spawner exists.
2. Iterate current `animals` list with index-based loop.
3. Skip null entries.
4. Call `animal.UpdateSelf(deltaTime)` for each non-null animal.

### OnExit

1. Read current `LevelDTO` from `ILevelsProvider`.
2. For each `AnimalDTO` with non-null `animalContainerRef`, call `contentLoader.Release(animalContainerRef)`.
3. Read loaded scene from `ICurrentLevelRuntime`.
4. If scene exists, call `contentLoader.ReleaseScene(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)`.
5. Call `currentLevelRuntime.Clear()`.
6. Clear `animals`, then set `animals = null`.
7. Set `spawner = null`.

## Error Handling

Fail fast with logging when required runtime data is missing:

- no current level selected
- no current level container in runtime
- spawner factory returned null

`UpdateSelf` must be null-safe because current `RandomSpawner` can append `null` if animal creation fails.

Repeated `contentLoader.Release` calls are acceptable because loader already ignores refs absent from cache.

## Testing Strategy

Primary unit coverage should verify:

- `OnEnter` creates list and initializes spawner with current level data and current level runtime spawn points.
- `UpdateSelf` updates spawner and all non-null animals in list.
- `OnExit` releases all animal refs from current level.
- `OnExit` unloads loaded scene when runtime has one.
- `OnExit` clears runtime and local state fields.
- Missing level or level container does not throw and does not initialize spawner.

## Notes

This design intentionally keeps spawned animal object destruction out of scope. Current requested cleanup is:

- release animal addressable refs through `IContentLoader`
- unload current level scene
- clear runtime references
