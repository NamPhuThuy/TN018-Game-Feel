using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NamPhuThuy
{
    public class RecycleObject : MonoBehaviour
    {
        public bool IsRecycleOnDisable;
        public string Name;
    
        // It returns the value of Name if it is not null or empty; otherwise, it returns the gameObject's name.
        // This ensures KeyName always has a valid string, using a custom name if set, or falling back to the object's name.
        public string KeyName => string.IsNullOrEmpty(Name) ? gameObject.name : Name;

        public virtual void Recycle()
        {
            PoolingManager.Instance.ResetRecycle(this);
        }

        private void OnDisable()
        {
            if(IsRecycleOnDisable) Recycle();
        }
    }
}