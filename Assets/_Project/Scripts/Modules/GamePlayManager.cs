using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        #region Private Serializable Fields

        [SerializeField] private List<EnemyController> enemyList;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        
        public EnemyController GetClosestEnemy(Vector3 position)
        {
            EnemyController closest = null;
            float minDist = float.MaxValue;
            foreach (var enemy in enemyList)
            {
                if (enemy == null) continue;
                float dist = Vector3.Distance(position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }
            return closest;
        }
        
        public void RegisterEnemy(EnemyController enemy)
        {
            if (enemy != null && !enemyList.Contains(enemy))
            {
                enemyList.Add(enemy);
            }
        }

        public void UnregisterEnemy(EnemyController enemy)
        {
            if (enemy != null)
            {
                enemyList.Remove(enemy);
            }
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(GamePlayManager))]
    [CanEditMultipleObjects]
    public class GamePlayManagerEditor : Editor
    {
        private GamePlayManager script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (GamePlayManager)target;

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