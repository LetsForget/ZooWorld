using UnityEngine;
using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class AnimalsInstaller : MonoInstaller
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Collider field;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GroundChecker>().AsSingle().WithArguments(groundLayer);
            Container.BindInterfacesAndSelfTo<BoundsDirectionChecker>().AsSingle().WithArguments(field);
            
            Container.Bind<DirectionSelectorsFactory>().AsSingle();
            Container.Bind<LocomotionsFactory>().AsSingle();
            Container.Bind<AnimalInteractionBehavioursFactory>().AsSingle();
            Container.Bind<AnimalFactory>().AsSingle();

            Container.Bind<SpawnerFactory>().AsSingle();
        }
    }
}