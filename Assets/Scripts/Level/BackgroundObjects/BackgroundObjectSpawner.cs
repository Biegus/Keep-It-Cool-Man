using System;
using System.Collections;
using System.Collections.Generic;
using Cyberultimate;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;
using Random = System.Random;

namespace LetterBattle
{
    public class BackgroundObjectSpawner : MonoBehaviour
    {
        /*[NaughtyAttributes.MinMaxSlider(1,60)]
        [Tooltip("Delay in seconds between debry spawn")]
        [SerializeField] private Vector2 spawnRate = new Vector2(10,30);
        
        [SerializeField] 
        private BackgroundObject.BackgroundObjectData[] backgroundObjects;*/

        public BackgroundTypeAsset CurrentType = null;
        
        private Coroutine spawningCoroutineHandle;
        
        private void Awake()
        {
            LEvents.Base.OnLevelStarted.Raw += StartSpawning;
            
            LEvents.Base.OnLevelLost.Raw    += StopSpawning;
            LEvents.Base.OnLevelWon.Raw     += StopSpawning;

            this.transform.localPosition = new Vector2(0, 0);
        }

        private void OnDestroy()
        {
            LEvents.Base.OnLevelStarted.Raw -= StartSpawning;
            
            LEvents.Base.OnLevelLost.Raw    -= StopSpawning;
            LEvents.Base.OnLevelWon.Raw     -= StopSpawning;
        }

        private void StartSpawning(object sender, EventArgs args)
        {
            CurrentType = GameManager.CurrentLevel.Background;
            
            if (CurrentType == null) //if level doesnt use bg
                return;
            
            spawningCoroutineHandle = StartCoroutine(Spawning());
        }
        
        private void StopSpawning(object sender, EventArgs args)
        {
            if (CurrentType == null) //if level doesnt use bg
                return;
            
            StopCoroutine(spawningCoroutineHandle);
        }

        private IEnumerator Spawning()
        {
            while (true)
            {
                yield return new WaitForSeconds(Randomer.Base.NextFloat(CurrentType.SpawnRate.x,CurrentType.SpawnRate.y));
                
                //Select random object to spawn
                var randomObjData = CurrentType.BackgroundObjects.GetRandomWeightItemFromValue((BackgroundObject.BackgroundObjectData element) =>
                {
                    return element.frequency;
                }, Randomer.Base.NextFloat(0, 1));

                //Spawn object
                BackgroundObject bgObject = Instantiate(randomObjData.prefab, this.transform);
                    
                bgObject.Init(randomObjData);
            }
        }
    }
}