using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 0.3f;

    public float endPoint = 85f;

    void Update()
    {
        // Solo avanzar si no llegó al final
        if (transform.position.z < endPoint)
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
    }
}