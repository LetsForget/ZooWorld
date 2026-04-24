using System.Collections.Generic;
using UnityEngine;

namespace Common.Pool
{
    public class Pool <T> where T : IPoolable
    {
        private LinkedList<T> usedObjects;
        private Stack<T> unusedObjects;

        public LinkedList<T> UsedObjects => usedObjects;
        
        public Pool(IEnumerable<T> objects)
        {
            usedObjects = new LinkedList<T>();
            unusedObjects = new Stack<T>();
            
            foreach (var obj in objects)
            {
                ReturnObject(obj);
            }
        }

        public T GetObject()
        {
            if (!unusedObjects.TryPop(out var result))
            {
                if (TryGetUsedObject(out result))
                {
                    ReturnObject(result);
                }
                else
                {
                    Debug.LogError("Empty pool");
                    return default;
                }
            }
            
            result.OnTakenFromPool();
            usedObjects.AddFirst(result);
            
            return result;
        }

        private bool TryGetUsedObject(out T obj)
        {
            if (usedObjects.Count > 0)
            {
                obj = usedObjects.Last.Value;
                return true;
            }

            obj = default;
            return false;
        }

        public void ReturnObject(T poolable)
        {
            poolable.OnReturnedBackToPool();
            unusedObjects.Push(poolable);

            if (usedObjects.Contains(poolable))
            {
                usedObjects.Remove(poolable);
            }
        }

        public IEnumerable<T> EnumerateAllObjectsInPool()
        {
            foreach (var obj in usedObjects)
            {
                yield return obj;
            }

            foreach (var obj in unusedObjects)
            {
                yield return obj;
            }
        }
    }
}