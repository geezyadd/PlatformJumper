using System.Collections.Generic;
using UnityEngine;

namespace Features.TowerGenerationModule.Scripts {
    public class CylinderSegmentPool {
        private readonly CylinderSegmentView[] _prefabs;
        private readonly Queue<CylinderSegmentView>[] _queues;
        private readonly Transform _parent;

        public CylinderSegmentPool(CylinderSegmentView[] prefabs, Transform parent) {
            var valid = new List<CylinderSegmentView>();
            if (prefabs != null) {
                for (int i = 0; i < prefabs.Length; i++) {
                    if (prefabs[i] != null)
                        valid.Add(prefabs[i]);
                }
            }

            _prefabs = valid.ToArray();
            _parent = parent;
            _queues = new Queue<CylinderSegmentView>[_prefabs.Length];
            for (int i = 0; i < _queues.Length; i++)
                _queues[i] = new Queue<CylinderSegmentView>();
        }

        public void Preload(int countPerPrefab) {
            for (int prefabIndex = 0; prefabIndex < _prefabs.Length; prefabIndex++) {
                for (int i = 0; i < countPerPrefab; i++)
                    _queues[prefabIndex].Enqueue(CreateInstance(prefabIndex));
            }
        }

        public CylinderSegmentView Spawn(int index, Vector3 worldPosition) {
            if (_prefabs.Length == 0)
                return null;

            int prefabIndex = Random.Range(0, _prefabs.Length);
            Queue<CylinderSegmentView> queue = _queues[prefabIndex];

            CylinderSegmentView segment = queue.Count > 0
                ? queue.Dequeue()
                : CreateInstance(prefabIndex);

            segment.OnSpawned(index, worldPosition);
            return segment;
        }

        public void Despawn(CylinderSegmentView segment) {
            if (segment == null || _queues.Length == 0)
                return;

            int prefabIndex = Mathf.Clamp(segment.PrefabIndex, 0, _queues.Length - 1);
            segment.OnDespawned();
            _queues[prefabIndex].Enqueue(segment);
        }

        private CylinderSegmentView CreateInstance(int prefabIndex) {
            CylinderSegmentView instance = Object.Instantiate(_prefabs[prefabIndex], _parent);
            instance.PrefabIndex = prefabIndex;
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
