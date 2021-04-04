using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Transform _target;

    [SerializeField] private Vector3 _angle = Vector3.zero;
    [SerializeField] private float _power = 0.1f;

    [SerializeField] private int _scoar;
    [SerializeField] private Sprite _sprite;

    Rigidbody _rigid;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = this.GetComponent<Rigidbody>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;

        BallPlay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// バスケットボールの挙動管理
    /// </summary>
    private void BallPlay()
    {
        _rigid.AddForce(_angle * _power, ForceMode.Impulse);
    }
    /// <summary>
    /// スコアを送る
    /// </summary>
    /// <returns></returns>
    public int Scoar()
    {
        UiManager.Instance.BallSprite(_sprite);
        return _scoar;
    }

    [SerializeField] private float _addTime = 0;
    public float Time()
    {
        return _addTime;
    }
}
