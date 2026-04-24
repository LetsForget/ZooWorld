using ContentLoading;
using Logging.UnityConsoleLogging;
using Zenject;
using ZooWorld.StateMachine;

namespace Common
{
    public class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CommonFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<AddressablesContentLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<UnityLogsWriter>().AsSingle();
            
            Container.Bind<GameStateMachine>().AsSingle();
        }
    }
}