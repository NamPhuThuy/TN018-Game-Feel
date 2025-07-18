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
        
        public GunController gunController;
        public GameObject shootPivot;
        
        public GameObject projectilePrefab;
        public ShellController bulletShellPrefab;
        
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
                DropShell();
                gunController.ActiveRecoilEffect(); 
                gunController.PlayShootSFX();
                gunController.PlayMuzzleFlash();
                
                CameraManager.Instance.ShakeCamera(0.04f, 0.2f);
                
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
            // Calculate shoot direction: always forward, but offset to left/right based on mouse
            
            Vector3 shootOffset = new Vector3(Random.Range(-0.03f, 0.03f), Random.Range(-0.03f, 0.03f), 0f);
            Vector3 shootDir = (transform.forward + shootOffset).normalized;

            //Quaternion.LookRotation(shootDir)
            GameObject projectile = Instantiate(projectilePrefab, shootPivot.transform.position, Quaternion.identity, GamePlayManager.Instance.projectileContainer.transform);
            
            projectile.transform.localEulerAngles = new Vector3(90f, 0f, 90f);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float shootForce = 20f;
                rb.velocity = shootDir * shootForce;
            }
        }
        
        private void DropShell()
        {
            if (bulletShellPrefab == null) return;
            
            ShellController shell = Instantiate(bulletShellPrefab, shootPivot.transform.position, Quaternion.identity, GamePlayManager.Instance.shellContainer.transform);
            shell.transform.localEulerAngles = new Vector3(0f, 0f, 90f);

            // Calculate end position (eject to the side, then drop)
            Vector3 sideEjectDir = transform.right * Random.Range(0.5f, 1.5f); // Randomize side push
            Vector3 forwardEjectDir = transform.forward * Random.Range(-.6f, .6f); // Optional: add some forward push
            Vector3 end = shootPivot.transform.position + sideEjectDir + forwardEjectDir + Vector3.down * 0.3f;

            StartCoroutine(EjectShellBezier(shell.gameObject, shootPivot.transform.position, end, 1f, 0.3f));
        }
        
        private IEnumerator EjectShellBezier(GameObject shell, Vector3 start, Vector3 end, float height, float duration)
        {
            Rigidbody rb = shell.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; // Disable physics during animation

            Vector3 control = (start + end) * 0.5f + Vector3.up * height;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                Vector3 pos = Mathf.Pow(1 - t, 2) * start +
                              2 * (1 - t) * t * control +
                              Mathf.Pow(t, 2) * end;
                shell.transform.position = pos;
                yield return null;
            }
            shell.transform.position = end;
            if (rb != null) rb.isKinematic = false; // Enable physics for gravity
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