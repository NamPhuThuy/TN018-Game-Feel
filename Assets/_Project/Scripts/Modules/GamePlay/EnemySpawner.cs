using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class EnemySpawner : MonoBehaviour
    {
        #region Private Serializable Fields

        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private List<Transform> spawnPoints;
        
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            StartCoroutine(SpawnEnemies());
        }

        #endregion

        #region Private Methods
        
        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                SpawnEnemy();
                yield return Yielders.Get(spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector3 spawnPosition = spawnPoints.Count > 0 ? spawnPoints[spawnIndex].position : transform.position;
            
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            
            Instantiate(enemyPrefab, spawnPosition + randomOffset, Quaternion.identity, transform);
        }
        
        #endregion

        #region Public Methods
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            spawnInterval = 1f;
        }

        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(EnemySpawner))]
    [CanEditMultipleObjects]
    public class EnemySpawnerEditor : Editor
    {
        private EnemySpawner script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (EnemySpawner)target;

            ButtonResetValues();
        }

        private void ButtonResetValues()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Values", GUILayout.Width(ConstInspector.BUTTON_WIDTH_MEDIUM)))
            {
                script.ResetValues();
                EditorUtility.SetDirty(script); // Mark the object as dirty
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    #endif
}