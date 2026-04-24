using UnityEngine;
using Zenject;

namespace ZooWorld.Levels
{
    public class LevelsInstaller : MonoInstaller
    {
        [SerializeField] private LevelsConfig levelsConfig;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LevelProviderSO>().AsSingle().WithArguments(levelsConfig);
            Container.BindInterfacesAndSelfTo<CurrentLevelRuntime>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelSceneResolver>().AsSingle();
        }
    }
}
