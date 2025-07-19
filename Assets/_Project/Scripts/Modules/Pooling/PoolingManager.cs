using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NamPhuThuy;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    public class PoolingManager : Singleton<PoolingManager>
    {
        public Dictionary<string, Queue<RecycleObject>> _dictPooler = new Dictionary<string, Queue<RecycleObject>>();

        public virtual RecycleObject Spawn(RecycleObject recycleObject, Transform parent = null)
        {
            if (_dictPooler.ContainsKey(recycleObject.KeyName))
            {
                if (_dictPooler[recycleObject.KeyName].Count > 0)
                {
                    return _dictPooler[recycleObject.KeyName].Dequeue();
                }
                else
                {
                    Debug.Log($"PoolingManager: create new projectiles for {recycleObject.KeyName}");
                    return SpawnObject(recycleObject, parent);
                }
            }
            else
            {
                _dictPooler.Add(recycleObject.KeyName, new Queue<RecycleObject>());
                return SpawnObject(recycleObject, parent);
            }
        }

        public virtual T Spawn<T>(RecycleObject recycleObject, Transform parent = null) where T : RecycleObject
        {
            return Spawn(recycleObject, parent) as T;
        }

        int count = 0;

        protected virtual RecycleObject SpawnObject(RecycleObject recycleObject, Transform parent = null)
        {
            if (parent == null) parent = this.transform;
            GameObject recycle = Instantiate(recycleObject.gameObject, parent);
            recycle.name = recycle.name + " _ " + count;
            count++;
            RecycleObject recycleScripts = recycle.GetComponent<RecycleObject>();
            return recycleScripts;
        }

        public virtual void ResetRecycle(RecycleObject recycle)
        {
            if (!_dictPooler.ContainsKey(recycle.KeyName))
            {
                _dictPooler.Add(recycle.KeyName, new Queue<RecycleObject>());
            }
            
            Debug.Log($"Pooling Manager: ResetRecycle {recycle.KeyName} - {recycle.gameObject.name}");
            _dictPooler[recycle.KeyName].Enqueue(recycle);
            recycle.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PoolingManager))]
    public class PoolerManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PoolingManager poolingManager = (PoolingManager)target;

            EditorGUILayout.LabelField("Recycle Objects in Pooler", EditorStyles.boldLabel);

            var groupedDict = poolingManager._dictPooler
                .GroupBy(kvp => kvp.Key.Split('(')[0])
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        TotalCount = g.SelectMany(kvp => kvp.Value).Count(),
                        ActiveCount = g.SelectMany(kvp => kvp.Value).Count(ro => ro.gameObject.activeInHierarchy)
                    });

            foreach (var kvp in groupedDict)
            {
                EditorGUILayout.LabelField($"Key: {kvp.Key}",
                    $"{kvp.Key}: {kvp.Value.TotalCount} (Active: {kvp.Value.ActiveCount})");
            }
        }
    }
#endif
}