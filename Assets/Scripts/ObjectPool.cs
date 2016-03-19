using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{

    public class ObjectPool : MonoBehaviour
    {

        public static ObjectPool current;

        bool willGrow = true;   // whether or not a new item should be added if the pool is empty
        public GameObject[] objectsToPool;     // the type of object to pull. i.e. Customer prefab
        List<GameObject> pooledObjects;     // the list of objects that are stored in the pool
        int poolSize = 25;      // the size of the pool


        void Start()
        {

            pooledObjects = new List<GameObject>();     // initialise the pool

            for (int i = 0; i < poolSize; i++)
            {
                int randomIndex = Random.Range(0, 3);

                GameObject go = (GameObject)Instantiate(objectsToPool[randomIndex]);      // create a new game object
                go.SetActive(false);        // disable the object while it is not in use
                pooledObjects.Add(go);      // add the new object to the list of pooled objects
            }

            current = this;

        }

        public GameObject GetGameObject()
        {
            // check for an object that is not in use
            for (int i = 0; i < poolSize; i++)
            {
                if (!pooledObjects[i].activeInHierarchy) return pooledObjects[i];   // return the object that is not in use
            }

            // if there are no available objects, see if we should create a new one
            if (willGrow)
            {
                int randIndex = Random.Range(0, 3);
                GameObject go = (GameObject)Instantiate(objectsToPool[randIndex]);      // create a new game object
                go.SetActive(false);        // disable the object while it is not in use
                pooledObjects.Add(go);      // add the new object to the list of pooled objects
                poolSize++;
                return go;
            }

            // if still no solution, return null
            return null;

        }
    }
}