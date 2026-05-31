using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickObjectS clickedObject = null;

            // RAY 3D (BoxCollider)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit3D))
            {
                ClickObjectS obj3D = hit3D.collider.GetComponent<ClickObjectS>();

                if (obj3D != null)
                {
                    clickedObject = obj3D;
                    //Debug.Log(" HIT 3D: " + hit3D.collider.name);
                }
            }

            // RAY 2D (PolygonCollider2D) usando el MISMO ray
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

            if (hit2D.collider != null)
            {
                ClickObjectS obj2D = hit2D.collider.GetComponent<ClickObjectS>();

                if (obj2D != null)
                {
                    //Debug.Log(" HIT 2D: " + hit2D.collider.name);

                    // PRIORIDAD: 2D gana si existe (puedes invertirlo si quieres)
                    clickedObject = obj2D;
                }
            }

            // EJECUTAR CLICK FINAL
            if (clickedObject != null)
            {
                //Debug.Log(" CLICK FINAL: " + clickedObject.name);
                clickedObject.OnClicked();
            }
        }
    }
}