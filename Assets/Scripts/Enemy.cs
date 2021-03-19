using UnityEngine;

public class Enemy : MonoBehaviour
{
    public virtual void OnTouched(MonoBehaviour whoTouched)
    {
        if (whoTouched is PlayerController touched)
        {
            if (!touched.CanDie()) return;
            touched.LockControls();
            touched.GameOver();
        } else if (whoTouched is Bullet)
        {

            var deletable = GetComponent<OutOfViewDeletable>();
            if (deletable != null)
            {
                deletable.DeleteExternally();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}