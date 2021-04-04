using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    Transform _target;//追いかけるターゲット
    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = this.transform.position;
        pos.x = _target.position.x;

        this.transform.position = pos;
    }
}
