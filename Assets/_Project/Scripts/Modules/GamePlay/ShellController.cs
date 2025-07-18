using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class ShellController : MonoBehaviour
    {
        #region Private Serializable Fields

        [SerializeField] private float lifetime = 5f; // Time before the shell is destroyed
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            Destroy(gameObject, lifetime);
        }
        

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            lifetime = 5f;
        }

        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ShellController))]
    [CanEditMultipleObjects]
    public class ShellControllerEditor : Editor
    {
        private ShellController script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (ShellController)target;

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