using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    protected virtual void Awake()
    {
        if (instance == null && this is T)
        {
            instance = this as T;
        }
        else
        {
            Debug.Log("Deleting duplicate instance of " + GetType().ToString() + ", " + gameObject.name + ": class is marked as singleton.");
            Destroy(gameObject);
        }
    }
}
