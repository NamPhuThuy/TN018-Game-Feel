using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class ProjectileController : RecycleObject
    {
        #region Private Serializable Fields
        
        [SerializeField] private List<ParticleSystem> explosionEffects;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private Rigidbody rigidbody;

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
        }


        private void Update()
        {
            if (Vector3.Distance(transform.position, GamePlayManager.Instance.playerController.transform.position) > 30f)
            {
                //FIRST WAY
                // Destroy(gameObject);
                
                //SECOND WAY
                // Recycle();
                
                // THIRD WAY
                GamePlayManager.Instance.projectilePooler.ReturnProjectile(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(ConstTag.ENEMY))
            {
                meshRenderer.enabled = false;
                meshCollider.enabled = false;
                rigidbody.velocity = Vector3.zero;
                
                PlayExplosionEffects();
                
                // FIRST WAY
                // Destroy(gameObject, 5f);
                
                
                Invoke(nameof(RecycleProj), 3f);
            }
        }

        #endregion

        #region Private Methods
        
        private void PlayExplosionEffects()
        {
            foreach (var effect in explosionEffects)
            {
                if (effect != null)
                {
                    effect.Play();
                }
            }
        }

        private void RecycleProj()
        {
            GamePlayManager.Instance.projectilePooler.ReturnProjectile(this);
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
    [CustomEditor(typeof(ProjectileController))]
    [CanEditMultipleObjects]
    public class ProjectileControllerEditor : Editor
    {
        private ProjectileController script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (ProjectileController)target;

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