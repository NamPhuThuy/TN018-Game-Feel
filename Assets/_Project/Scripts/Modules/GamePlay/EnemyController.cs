using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class EnemyController : MonoBehaviour
    {
        #region Private Serializable Fields

        #region Stats

        [Header("Stats")]
        private float flipChance = 0.3f; // 30% chance to flip
        
        private float jumpDuration = .5f;
        [SerializeField] private float jumpHeightMin = 2f;
        [SerializeField] private float jumpHeightMax = 4.5f;

        private bool isJumping = false;
        
        private float jumpDelayMin = 0.5f;
        private float jumpDelayMax = 1f;
        
        [SerializeField] private float jumpDistanceMin = 1.4f;
        [SerializeField] private float jumpDistanceMax = 2.5f;

        [SerializeField] private float health = 5f;
        [SerializeField] private int healthMin = 3;
        [SerializeField] private int healthMax = 5;

        #endregion


        #region Components

        [Header("Components")]
        public Transform player;
        private Rigidbody rb;
        [SerializeField] private Renderer renderer;

        #endregion

        #region Flash Effect

        [Header("Flash Effect")]
        private Color originalColor;
        private Material originalMaterial;
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField] private Material flashMaterial;
        private float flashDuration = 0.1f;
        private float fadeBackDuration = 0.2f;
#endregion

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (player == null)
                player = GamePlayManager.Instance.playerController.transform;
            
            health = Random.Range(healthMin, healthMax);
            
            if (renderer != null)
                originalMaterial = renderer.material;
            
            StartCoroutine(JumpTowardsPlayerBezier());
        }

        private void OnEnable()
        {
            GamePlayManager.Instance.RegisterEnemy(this);
        }

        private void OnDisable()
        {
            GamePlayManager.Instance.UnregisterEnemy(this);
        }

        private void OnDrawGizmos()
        {
            if (player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);

                // Optionally, draw an arrowhead
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 arrowHead = transform.position + direction * 1.5f;
                Gizmos.DrawSphere(arrowHead, 0.1f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(ConstTag.PROJECTILE))
            {
                // Push back effect
                if (rb != null)
                {
                    Vector3 pushDir = (transform.position - other.transform.position).normalized;
                    float pushForce = 5f; // Adjust as needed
                    rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
                }
                
                // Flash white effect
                if (renderer != null)
                    StartCoroutine(IEFlashMaterial());
                
                TakeDamage(1);
            }
        }

        

        #endregion

        #region Private Methods
        private void DieProcess()
        {
            GUIManager.Instance.GUIHUD.EnemyKilledCount++;
            Destroy(gameObject);
        }

        private void TakeDamage(int amount)
        {
            health -= amount;
            
            if (health <= 0)
            {
                DieProcess();
            }
        }
        
        private IEnumerator DoFrontFlip()
        {
            float flipDuration = 0.5f;
            float elapsed = 0f;
            float startRotation = transform.eulerAngles.x;
            float endRotation = startRotation + 360f;

            while (elapsed < flipDuration)
            {
                float xRotation = Mathf.Lerp(startRotation, endRotation, elapsed / flipDuration);
                transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.eulerAngles = new Vector3(startRotation, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        private IEnumerator JumpTowardsPlayerBezier()
        {
            while (true)
            {
                float startDelay = Random.Range(jumpDelayMin, jumpDelayMax);
                yield return Yielders.Get(startDelay);
                
                if (player == null || isJumping) continue;
                
                Vector3 direction = (player.position - transform.position).normalized;
                
                float jumpDistance = Random.Range(jumpDistanceMin, jumpDistanceMax);

                // StartCoroutine(DoFrontFlip());
                if (Vector3.Distance(transform.position, player.position) < jumpDistance)
                {
                    StartCoroutine(IEJumpBezier(transform.position, player.position));
                }
                else
                {
                    StartCoroutine(IEJumpBezier(transform.position, transform.position + direction.normalized * jumpDistance));
                }
            }
        }

        private IEnumerator IEJumpBezier(Vector3 start, Vector3 end)
        {
            isJumping = true;
            rb.isKinematic = true; // Disable physics for smooth movement

            // Calculate control point (midpoint, raised by jumpHeight)
            float jumpHeight = Random.Range(jumpHeightMin, jumpHeightMax);
            
            Vector3 control = (start + end) * 0.5f + Vector3.up * jumpHeight;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / jumpDuration;
                // Quadratic Bezier formula
                Vector3 pos = Mathf.Pow(1 - t, 2) * start +
                              2 * (1 - t) * t * control +
                              Mathf.Pow(t, 2) * end;
                transform.position = pos;
                yield return null;
            }

            transform.position = end;
            rb.isKinematic = false; // Re-enable physics
            isJumping = false;
        }
        
        private IEnumerator IEFlashMaterial()
        {
            renderer.material = flashMaterial;
            yield return Yielders.Get(flashDuration);

            float elapsed = 0f;
            Color startColor = flashMaterial.color;
            Color endColor = originalMaterial.color;

            while (elapsed < fadeBackDuration)
            {
                elapsed += Time.deltaTime;
                Color lerpedColor = Color.Lerp(startColor, endColor, elapsed / fadeBackDuration);
                renderer.material.color = lerpedColor;
                yield return null;
            }
            renderer.material = originalMaterial;
        }
        
        #endregion

        #region Public Methods
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            jumpHeightMin = 2f;
            jumpHeightMax = 4.5f;
        
            jumpDistanceMin = 1f;
            jumpDistanceMax = 2f;

            health = 5f;
			healthMin = 3;
			healthMax = 4;
            flashColor = Color.red;
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