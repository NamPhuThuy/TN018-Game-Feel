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
        
        [Header("Stats")]
        private float flipChance = 0.3f; // 30% chance to flip
        
        private float jumpDuration = .5f;
        [SerializeField] private float jumpHeightMin = 2f;
        [SerializeField] private float jumpHeightMax = 4.5f;

        private bool isJumping = false;
        
        [SerializeField] private float jumpDelayMin = 0.4f;
        [SerializeField] private float jumpDelayMax = 1.2f;
        
        [SerializeField] private float jumpDistanceMin = 2f;
        [SerializeField] private float jumpDistanceMax = 3.5f;

        [SerializeField] private float health = 5f;
        [SerializeField] private int healthMin = 3;
        [SerializeField] private int healthMax = 5;
        
        
        [Header("Components")]
        public Transform player;
        private Rigidbody rb;

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
                health--;
                
                // Push back effect
                if (rb != null)
                {
                    Vector3 pushDir = (transform.position - other.transform.position).normalized;
                    float pushForce = 5f; // Adjust as needed
                    rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
                }
                
                if (health <= 0)
                {
                    DieProcess();
                }
            }
        }

        

        #endregion

        #region Private Methods
        private void DieProcess()
        {
            Destroy(gameObject);
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
            float startDelay = Random.Range(jumpDelayMin, jumpDelayMax);
            startDelay /= 2f;
            yield return new WaitForSeconds(startDelay);
            
            while (true)
            {
                if (player == null || isJumping) continue;
                
                Vector3 direction = (player.position - transform.position).normalized;
                
                float jumpDistance = Random.Range(jumpDistanceMin, jumpDistanceMax);

                DoFrontFlip();
                if (Vector3.Distance(transform.position, player.position) < jumpDistance)
                {
                    StartCoroutine(JumpBezier(transform.position, player.position));
                }
                else
                {
                    StartCoroutine(JumpBezier(transform.position, transform.position + direction.normalized * jumpDistance));
                }
                
                startDelay = Random.Range(jumpDelayMin, jumpDelayMax);
                yield return new WaitForSeconds(startDelay);
            }
        }

        private IEnumerator JumpBezier(Vector3 start, Vector3 end)
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
        
        #endregion

        #region Public Methods
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            jumpDelayMin = 1f;
            jumpDelayMax = 2f;
            jumpHeightMin = 2f;
            jumpHeightMax = 4.5f;
        
            jumpDistanceMin = 1.4f;
            jumpDistanceMax = 2.5f;

            health = 5f;
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