using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class EnemyController : MonoBehaviour
    {
        #region Private Serializable Fields

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            GamePlayManager.Instance.RegisterEnemy(this);
        }

        private void OnDisable()
        {
            GamePlayManager.Instance.UnregisterEnemy(this);
        }

        #endregion

        #region Private Methods
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
    [CustomEditor(typeof(EnemyController))]
    [CanEditMultipleObjects]
    public class EnemyControllerEditor : Editor
    {
        private EnemyController script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (EnemyController)target;

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