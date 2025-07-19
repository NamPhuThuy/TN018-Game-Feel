using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class CameraManager : Singleton<CameraManager>
    {
        #region Private Serializable Fields
        public Camera mainCamera;
        public GameObject otherCameraPerspective;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        #endregion

        #region Private Methods
        private IEnumerator ShakeCoroutine(float power, float duration)
        {
            if (mainCamera == null) yield break;

            Vector3 originalPos = mainCamera.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * power;
                float y = Random.Range(-1f, 1f) * power;
                mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = originalPos;
        }
        
        #endregion

        #region Public Methods
        
        public void ShakeCamera(float power, float duration = 0.2f)
        {
            StartCoroutine(ShakeCoroutine(power, duration));
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion

        public void ToggleOtherCamera()
        {
            if (otherCameraPerspective != null)
            {
                otherCameraPerspective.SetActive(!otherCameraPerspective.activeSelf);
            }
            else
            {
                Debug.LogWarning("Other camera perspective is not assigned in CameraManager.");
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(CameraManager))]
    [CanEditMultipleObjects]
    public class CameraManagerEditor : Editor
    {
        private CameraManager script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (CameraManager)target;

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