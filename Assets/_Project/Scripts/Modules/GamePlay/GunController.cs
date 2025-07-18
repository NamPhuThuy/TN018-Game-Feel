using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class GunController : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("Stats")]
        [SerializeField] private float recoilDistance = 0.1f;
        [SerializeField] private float recoilAngle = 10f;
        [SerializeField] private float recoilDuration = 0.1f;
        
        [Header("SFX")]
        [SerializeField] private AudioClip shootSFX;
        [SerializeField] private AudioSource audioSource;
        
        [Header("VFX")]
        [SerializeField] private ParticleSystem muzzleFlash;
        #endregion

        #region Private Fields
        private Vector3 gunOriginalPos;
        private Quaternion gunOriginalRot;
        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            gunOriginalPos = transform.localPosition;
            gunOriginalRot = transform.localRotation;
        }

        #endregion

        #region Private Methods
        
        
        
        private IEnumerator RecoilCoroutine()
        {
            // Move gun back and rotate up
            Vector3 recoilPos = gunOriginalPos - Vector3.forward * recoilDistance;
            // Quaternion recoilRot = gunOriginalRot * Quaternion.Euler(-recoilAngle, 0, 0);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / recoilDuration;
                transform.localPosition = Vector3.Lerp(gunOriginalPos, recoilPos, t);
                // transform.localRotation = Quaternion.Slerp(gunOriginalRot, recoilRot, t);
                yield return null;
            }

            // Return to original
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / recoilDuration;
                transform.localPosition = Vector3.Lerp(recoilPos, gunOriginalPos, t);
                // transform.localRotation = Quaternion.Slerp(recoilRot, gunOriginalRot, t);
                yield return null;
            }
        }
        #endregion

        #region Public Methods
        
        public void ActiveRecoilEffect()
        {
            StartCoroutine(RecoilCoroutine());
        }
        
        public void PlayShootSFX()
        {
            audioSource.PlayOneShot(shootSFX);
        }
        
        public void PlayMuzzleFlash()
        {
            muzzleFlash.Play();
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(GunController))]
    [CanEditMultipleObjects]
    public class GunControllerEditor : Editor
    {
        private GunController script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (GunController)target;

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