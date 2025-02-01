using System;
using System.Collections.Generic;
using UnityEngine;

namespace Forest
{
    public class Pooler<T> : IDisposable where T : MonoBehaviour
    {
        protected Queue<T> objectPool = new();
        protected T prefab = null;

        public Pooler(T prefab, int base_count = 1)
        {
            this.prefab = prefab;
            for (int i = 0; i < base_count; i++)
            {
                Create();
            }
        }

        public void Dispose()
        {
            foreach (T o in objectPool)
            {
                UnityEngine.Object.Destroy(o.gameObject);
            }
        }

        public virtual T Borrow()
        {
            if (objectPool.Count <= 0)
            {
                Create();
            }
            return objectPool.Dequeue();
        }

        protected virtual T Create()
        {
            T o = UnityEngine.Object.Instantiate(prefab);
            Return(o);
            return o;
        }

        public virtual void Return(T o)
        {
            o.gameObject.SetActive(false);
            objectPool.Enqueue(o);
        }
    }
}
