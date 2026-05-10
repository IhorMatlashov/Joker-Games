using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public sealed class ChipPool : MonoBehaviour
    {
        public static ChipPool Instance { get; private set; }

        private readonly Dictionary<GameObject, Stack<GameObject>> _pools = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            if (prefab == null) return null;
            if (_pools.TryGetValue(prefab, out var stack) && stack.Count > 0)
            {
                var go = stack.Pop();
                go.transform.SetParent(parent, worldPositionStays: false);
                go.transform.SetPositionAndRotation(pos, rot);
                go.SetActive(true);
                return go;
            }
            var fresh = Object.Instantiate(prefab, pos, rot, parent);
            fresh.AddComponent<ChipPoolTag>().Prefab = prefab;
            return fresh;
        }

        public void Release(GameObject chip)
        {
            if (chip == null) return;
            var tag = chip.GetComponent<ChipPoolTag>();
            if (tag == null || tag.Prefab == null)
            {
                Destroy(chip);
                return;
            }
            chip.SetActive(false);
            chip.transform.SetParent(transform, worldPositionStays: false);
            if (!_pools.TryGetValue(tag.Prefab, out var stack))
                _pools[tag.Prefab] = stack = new Stack<GameObject>();
            stack.Push(chip);
        }

        private sealed class ChipPoolTag : MonoBehaviour { public GameObject Prefab; }
    }
}
