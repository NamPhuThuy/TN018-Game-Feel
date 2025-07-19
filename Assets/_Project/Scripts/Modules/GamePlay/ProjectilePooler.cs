using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class ProjectilePooler : MonoBehaviour
    {
        #region Private Serializable Fields
        
        [SerializeField] private ProjectileController projectilePrefab;
        [SerializeField] private int poolSize = 20;

        private Queue<ProjectileController> pool = new Queue<ProjectileController>();

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            for (int i = 0; i < poolSize; i++)
            {
                ProjectileController proj = Instantiate(projectilePrefab, transform);
                proj.gameObject.SetActive(false);
                pool.Enqueue(proj);
            }
        }

        void Update()
        {
            
        }

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        public ProjectileController GetProjectile(Vector3 position, Quaternion rotation)
        {
            ProjectileController proj;
            if (pool.Count > 0)
            {
                proj = pool.Dequeue();
            }
            else
            {
                proj = Instantiate(projectilePrefab, transform);
            }
            proj.transform.position = position;
            proj.transform.rotation = rotation;
            proj.gameObject.SetActive(true);
            return proj;
        }

        public void ReturnProjectile(ProjectileController proj)
        {
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ProjectilePooler))]
    [CanEditMultipleObjects]
    public class ProjectilePoolerEditor : Editor
    {
        private ProjectilePooler script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (ProjectilePooler)target;

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