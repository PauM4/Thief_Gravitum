using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 lookAtPos = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z);
        transform.LookAt(lookAtPos);
    }
}
