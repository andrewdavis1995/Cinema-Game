using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{

    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool current;   // a static instance of the class for easy access

        public int audioVolume = 3;
        public GameObject[] objectsToPool;     // the type of object to pull. i.e. Customer prefab
        public AudioClip[] audioFiles;     // the sounds to play on walkout
        public List<GameObject> pooledObjects;     // the list of objects that are stored in the pool
        int poolSize = 25;      // the size of the pool
        
        // runs once on creation of the script
        void Start()
        {
            // initialise 5 customers - not technically needed but leaving it in 
            Initialise(5);       
        }

        /// <summary>
        /// Initialises the Object Pool
        /// </summary>
        /// <param name="number">The size of the object pool (Number of objects to store)</param>
        public void Initialise(int number)
        {
            // initialise the pool
            poolSize = number;  
            pooledObjects = new List<GameObject>();

            // from 0 to the size of the pool, create a new object
            for (int i = 0; i < poolSize; i++)
            {
                int randomIndex = Random.Range(0, 3);

                GameObject go = AddNewItem();
                pooledObjects.Add(go);      // add the new object to the list of pooled objects
            }

            current = this;
        }

        /// <summary>
        /// Get a game object
        /// </summary>
        /// <returns>An usused game object</returns>
        public GameObject GetGameObject()
        {
            // check for an object that is not in use
            for (int i = 0; i < poolSize; i++)
            {
                if (!pooledObjects[i].activeInHierarchy) return pooledObjects[i];   // return the object that is not in use
            }

            // if no object available, make a new one
            GameObject go = AddNewItem();
            poolSize++;

            // if still no solution, return null
            return go;

        }

        /// <summary>
        /// Add a new item to the pool
        /// </summary>
        /// <returns>The game object which has been created</returns>
        public GameObject AddNewItem()
        { 
            // if there are no available objects, see if we should create a new one    
            int randIndex = Random.Range(0, 4);
            GameObject go = (GameObject)Instantiate(objectsToPool[randIndex]);      // create a new game object
            AudioSource aSource = go.GetComponent<AudioSource>();

            int rand = Random.Range(0, audioFiles.Length);

            aSource.clip = audioFiles[rand];
            aSource.volume = audioVolume;
            go.SetActive(false);        // disable the object while it is not in use
            return go;
        }
    }
}