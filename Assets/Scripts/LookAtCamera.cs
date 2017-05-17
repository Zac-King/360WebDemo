using UnityEngine;

/// <summary>
/// Makes object look at the main Camera
/// </summary>
 
public class LookAtCamera : MonoBehaviour
{
    public bool LookAway;
    Transform m_view;

    private void Start()
    {
        m_view = Camera.main.transform;
    }

    private void Update()
    {
        if(transform.parent != null)
            transform.localRotation = Quaternion.Inverse(transform.parent.transform.localRotation);

        if (LookAway)
            transform.LookAt(transform.position + (transform.position - m_view.position));

        else
            transform.LookAt(m_view);
    }
}
