using System.Collections.Generic;
using UnityEngine;

public class HealthItemPool
{
    private readonly Queue<HealthItem> _pool = new();
    private readonly HealthItem _prefab;
    private readonly Transform _parent;

    public HealthItemPool(HealthItem prefab, int initialSize = 10, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
            AddNewToPool();
    }

    private HealthItem AddNewToPool()
    {
        HealthItem item = Object.Instantiate(_prefab, _parent);
        item.gameObject.SetActive(false);
        _pool.Enqueue(item);
        return item;
    }

    public HealthItem Get(Vector3 position)
    {
        HealthItem item = _pool.Count > 0 ? _pool.Dequeue() : AddNewToPool();

        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;
        item.transform.SetParent(null);
        item.gameObject.SetActive(true);

        return item;
    }

    public void ReturnToPool(HealthItem item)
    {
        item.Stop();
        item.gameObject.SetActive(false);
        item.transform.SetParent(_parent);
        _pool.Enqueue(item);
    }
}
