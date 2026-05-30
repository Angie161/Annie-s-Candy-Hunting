using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool canMove = true;

    public float speed = 0.6f;

    public float endPoint = 85f;

    void Update()
    {
        if (canMove)
        {
            // Solo avanzar si no llegó al final
            if (transform.position.z < endPoint)
            {
                transform.position += Vector3.forward * speed * Time.deltaTime;
            }   
        }
    }
}