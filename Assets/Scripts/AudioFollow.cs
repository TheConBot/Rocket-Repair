using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFollow : MonoBehaviour {

    public Transform target;

	void Update () {
        UpdatePosition();
	}

    public void UpdatePosition()
    {
        transform.position = new Vector2(target.position.x, transform.position.y);
    }
}
