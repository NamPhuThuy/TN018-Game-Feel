using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    
    public class PlayerController : MonoBehaviour
    {
        #region Private Serializable Fields

        [SerializeField] EnemyController closestEnemy;
        
        [Header("Stats")]
        [SerializeField] private float fireRate = 2f; //shots per sec
        private float fireTimer = 0f;
        
        #endregion

        #region Public Fields
        
        public GameObject gunGameObj;
        public GameObject shootPivot;
        
        public GameObject projectilePrefab;
        
        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            
        }

        void Update()
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                ShootTowardsMouse();
                fireTimer = 1f / fireRate;
            }
            
            closestEnemy = GamePlayManager.Instance.GetClosestEnemy(transform.position);
            if (closestEnemy != null)
            {
                Vector3 lookPos = closestEnemy.transform.position - transform.position;
                // lookPos.y = 0; // Optional: keep only horizontal rotation
                if (lookPos != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(lookPos);
            }
        }

        #endregion

        #region Private Methods
        
        private void ShootTowardsMouse()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.transform.position.y - transform.position.y));
            Vector3 toMouse = mouseWorldPos - transform.position;

            // Project toMouse onto the player's right vector (local X)
            float side = Vector3.Dot(toMouse, transform.right);

            // Calculate shoot direction: always forward, but offset to left/right based on mouse
            // Vector3 shootDir = (transform.forward + transform.right * Mathf.Sign(side)).normalized;
            Vector3 shootDir = (transform.forward).normalized;

            GameObject projectile = Instantiate(projectilePrefab, shootPivot.transform.position, Quaternion.LookRotation(shootDir));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float shootForce = 20f;
                rb.velocity = shootDir * shootForce;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            fireRate = 8f; // Reset to default value
        }

        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(PlayerController))]
    [CanEditMultipleObjects]
    public class PlayerControllerEditor : Editor
    {
        private PlayerController script;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (PlayerController)target;

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