/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System.Collections.Generic;
using UnityEngine;

public class GenericPoolController<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] [Range(0, 50)] private int _startPoolSize = 0;

    [SerializeField] protected GameObject prefab;

    public List<T> activeObjects { get; private set; } = new List<T>();
    private Queue<GameObject> queue = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < _startPoolSize; i++)
            Instantiate();
    }

    protected virtual GameObject Instantiate()
    {
        var gameObject = Instantiate(prefab) as GameObject;
        gameObject.transform.SetParent(transform);
        gameObject.SetActive(false);
        queue.Enqueue(gameObject);

        return gameObject;
    }

    public virtual T Spawn()
    {
        var pooledItem = queue.Count > 0 ? queue.Dequeue() : Instantiate(prefab) as GameObject;
        pooledItem.transform.SetParent(transform);
        pooledItem.SetActive(true);

        T poolItemController = pooledItem.GetComponent<T>();
        activeObjects.Add(poolItemController);

        return poolItemController;
    }

    public virtual T GetRandomActiveObject()
    {
        if (activeObjects.Count > 0)
            return activeObjects[new System.Random().Next(activeObjects.Count)];

        return null;
    }

    public virtual T[] DestroyAll()
    {
        T[] destroyedObjs = activeObjects.ToArray();

        foreach (var poolObj in destroyedObjs)
        {
            activeObjects.Remove(poolObj);

            queue.Enqueue(poolObj.gameObject);
            poolObj.gameObject.SetActive(false);
        }

        return destroyedObjs;
    }

    public virtual void DestroyItem(T poolObj)
    {
        activeObjects.Remove(poolObj);

        queue.Enqueue(poolObj.gameObject);
        poolObj.gameObject.SetActive(false);
    }
}

