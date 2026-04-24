using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZooWorld.Animals
{
    public class RandomSpawner : IAnimalsSpawner
    {
        private readonly RandomSpawnerConfig config;
        private readonly AnimalFactory animalFactory;

        private Transform[] spawnPoints;
        private AnimalSO[] variants;
        private List<AnimalRuntime> targetList;
        
        private DateTime lastSuccessfulSpawnTime;
        private bool isSpawnInProgress;

        public RandomSpawner(RandomSpawnerConfig config, AnimalFactory animalFactory)
        {
            this.config = config;
            this.animalFactory = animalFactory;
        }

        public void Initialize(Transform[] spawnPoints, AnimalSO[] variants, List<AnimalRuntime> targetList)
        {
            this.spawnPoints = spawnPoints;
            this.variants = variants;
            this.targetList = targetList;
        }

        public void UpdateSelf(float deltaTime)
        {
            if (isSpawnInProgress)
            {
                return;
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return;
            }

            var timeSinceLastSpawn = DateTime.Now - lastSuccessfulSpawnTime;

            if (timeSinceLastSpawn.TotalSeconds < config.spawnPeriod)
            {
                return;
            }

            isSpawnInProgress = true;
            SpawnAnimalAsync().Forget();
        }

        private async UniTaskVoid SpawnAnimalAsync()
        {
            try
            {
                var randomSpawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
                var spawnPoint = spawnPoints[randomSpawnPointIndex];

                var randomAnimalIndex = UnityEngine.Random.Range(0, variants.Length);
                var animalDTO = variants[randomAnimalIndex].DTO;

                var animal = await animalFactory.CreateAnimal(animalDTO, spawnPoint);

                if (animal != null)
                {
                    lastSuccessfulSpawnTime = DateTime.Now;
                }
                
                targetList.Add(animal);
            }
            finally
            {
                isSpawnInProgress = false;
            }
        }
    }
}
