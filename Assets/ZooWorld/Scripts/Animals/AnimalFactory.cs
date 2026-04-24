using ContentLoading;
using Cysharp.Threading.Tasks;
using Logging;
using UnityEngine;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    public class AnimalFactory
    {
        private readonly IContentLoader contentLoader;
        private readonly ILogsWriter logsWriter;

        private readonly DirectionSelectorsFactory directionSelectorsFactory;
        private readonly LocomotionsFactory locomotionsFactory;
        private readonly AnimalInteractionBehavioursFactory interactionBehavioursFactory;
        
        public AnimalFactory(IContentLoader contentLoader, ILogsWriter logsWriter,
            DirectionSelectorsFactory directionSelectorsFactory, LocomotionsFactory locomotionsFactory,
            AnimalInteractionBehavioursFactory interactionBehavioursFactory)
        {
            this.contentLoader = contentLoader;
            this.logsWriter = logsWriter;
            
            this.directionSelectorsFactory = directionSelectorsFactory;
            this.locomotionsFactory = locomotionsFactory;
            this.interactionBehavioursFactory = interactionBehavioursFactory;
        }

        public async UniTask<AnimalRuntime> CreateAnimal(AnimalDTO animalDTO, Transform spawn)
        {
            if (animalDTO.animalContainerRef == null)
            {
                logsWriter.LogError("No animalContainer ref was provided");
                return null;
            }

            var container = await  contentLoader.LoadComponent<AnimalContainer>(animalDTO.animalContainerRef);
            
            if (!container)
            {
                logsWriter.LogError("Failed to create animal");
                return null;
            }
            
            container = GameObject.Instantiate(container, spawn);
            container.transform.position = spawn.position;
            container.transform.rotation = Quaternion.identity;

            var dirType = animalDTO.directionSelectType;
            var dirConfig = animalDTO.directionSelectorConfig;
            var directionSelector = directionSelectorsFactory.Create(dirType, container, dirConfig);

            if (directionSelector == null)
            {
                logsWriter.LogError("Failed to create direction selector");
                return null;
            }
            
            var locType = animalDTO.locomotionType;
            var locConfig = animalDTO.locomotionConfig;
            var locomotion = locomotionsFactory.Create(locType, container, locConfig);

            if (locomotion == null)
            {
                logsWriter.LogError("Failed to create locomotion");
                return null;
            }

            var interactionBehaviour = interactionBehavioursFactory.Create(animalDTO.animalType);
            
            var animal = new Animal(animalDTO.animalType, directionSelector, locomotion, interactionBehaviour);
            var runtime = new AnimalRuntime(animal, container);

            var relay = container.Relay;
            relay.Initialize(runtime);

            return runtime;
        }
    }
}