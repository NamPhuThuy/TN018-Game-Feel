using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class GUIHUD : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("Text Settings")] 
        [SerializeField] private TextMeshProUGUI enemyKilledText;
        [SerializeField] private TextMeshProUGUI bulletCountText;
        
        [SerializeField] private Button resetButton;

        [Header("Stats")] 
        [SerializeField] private int enemyKilledCount = 0;
        public int EnemyKilledCount
        {
            get => enemyKilledCount;
            set
            {
                enemyKilledCount = value;
                UpdateUI();
            }
        }
        
        [SerializeField] private int bulletCount = 0;
        public int BulletCount 
        {
            get => bulletCount;
            set
            {
                bulletCount = value;
                UpdateUI();
            }
        }
        
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            resetButton.onClick.AddListener(OnClickReset);
        }

        private void OnDisable()
        {
            resetButton.onClick.AddListener(OnClickReset);

        }

        private void OnClickReset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            enemyKilledText.text = $"Enemies Killed: {enemyKilledCount}";
            bulletCountText.text = $"Bullets Shot: {bulletCount}";
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
    [CustomEditor(typeof(GUIHUD))]
    [CanEditMultipleObjects]
    public class GUIHUDEditor : Editor
    {
        private GUIHUD script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (GUIHUD)target;

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