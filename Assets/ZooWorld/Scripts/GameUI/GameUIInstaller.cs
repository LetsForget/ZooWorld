using UnityEngine;
using Zenject;

namespace ZooWorld.UI
{
    public class GameUIInstaller : MonoInstaller<GameUIInstaller>
    {
        [SerializeField] private UIContainer container;
        [SerializeField] private UIConfig config;

        public override void InstallBindings()
        {
            Container.Bind<IUIManager<GameUIType>>().To<GameUIManager>().AsSingle().WithArguments(container, config);
        }
    }
}