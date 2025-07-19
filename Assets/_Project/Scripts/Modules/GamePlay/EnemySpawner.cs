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
        [SerializeField] private Transform spawnPoint;
        
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
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(enemyPrefab, position, Quaternion.identity, transform);
        }
        
        #endregion

        #region Public Methods
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
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