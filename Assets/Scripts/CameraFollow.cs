using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Vector3 offset;
    public Transform targetObject;

	private void Update () {
        setCameraPosition();
	}

    public void setCameraPosition()
    {
        Vector3 target = new Vector3(targetObject.position.x + offset.x, targetObject.position.y + offset.y, transform.position.z + offset.z);
        transform.position = target;
    }
}
