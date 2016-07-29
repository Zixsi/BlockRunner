using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraLook : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float height = 5.0f;

	void Update ()
    {
	    if(target != null)
        {
            Vector3 dest = new Vector3(transform.position.x, target.position.y + height, target.position.z + distance);
            transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime * 5.0f);
        }
	}

}
