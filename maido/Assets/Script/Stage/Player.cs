using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance
    {
        get { return instance; }
    }
    static Player instance = null;

    private enum STATE { Non, Base, Ball, Goal, Anima }
    private STATE _state;

    private readonly float MOVE_SPEED = 4.0f;       //通常移動速度
    private readonly float DASH_SPEED = 8.0f;       //ダッシュ移動速度
    private readonly float JUMPPOWER = 8.0f;        //ジャンプ時の力
    private readonly float GOAL_POWER = 8.0f;       //ゴール時のジャンプ力
    private readonly float GOAL_WAITINPUT = 1.0f;   //ゴール時の入力待ちの時間

    private float _time;            //時間計測用
    private Vector3 _vec;           //現在の速度
    private bool _isGround = false; //接地判定

    [SerializeField] private Transform _hand = null;//手
    [SerializeField] private Transform _ball = null;//ボール

    [SerializeField] private GameObject _goal;//

    private int _handItemScoar;//現在持っているアイテムのスコア

    // AnimatorのパラメーターID
    static readonly int _dashId = Animator.StringToHash("IsDash");          //ダッシュのID
    static readonly int _isGroundedId = Animator.StringToHash("IsGrounded");//接地判定のID
    static readonly int _speedId = Animator.StringToHash("Speed");          //速度のID
    static readonly int _JumoWaitId = Animator.StringToHash("JumpWait");    //ジャンプ待ち状態かのID

    private Animator _anima;
    private Rigidbody _rigid;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = this.GetComponent<Rigidbody>();
        _anima = this.transform.GetChild(0).GetComponent<Animator>();

        _ball.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == STATE.Base)
        {
            Base();
        }
        else if (_state == STATE.Ball)
        {
            Ball();
        }
        else if (_state == STATE.Goal)
        {
            Goal();
        }
        else if (_state == STATE.Anima)
        {
            Anima();
        }
    }

    private void FixedUpdate()
    {
        var vel = _vec;
        vel.y = this._rigid.velocity.y;
        this._rigid.velocity = vel;     //プレイヤーの速度の適応

        _isGround = Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1);//接地判定

        _anima.SetBool(_isGroundedId, _isGround);//アニメーションの接地判定
    }

    /// <summary>
    /// 通常状態
    /// </summary>
    private void Base()
    {
        GaugeReset();
        if (_isGround)
        {
            if (Input.GetKey(KeyCode.D))//ダッシュ処理
            {
                Movement(DASH_SPEED);

                _anima.SetBool(_dashId, true);
            }
            else if (Input.GetKeyDown(KeyCode.W))//ジャンプ処理
            {
                _rigid.AddForce(Vector3.up * JUMPPOWER, ForceMode.Impulse);

                _anima.SetBool(_isGroundedId, false);
                _anima.SetBool(_dashId, false);
            }
            else //通常移動
            {
                Movement(MOVE_SPEED);

                _anima.SetFloat(_speedId, 1);
                _anima.SetBool(_dashId, false);
            }
        }
    }

    /// <summary>
    /// ボール所持状態
    /// </summary>
    private void Ball()
    {
        GaugeReset();
        if (_isGround)
        {
            if (Input.GetKey(KeyCode.D))//ダッシュ処理
            {
                Movement(DASH_SPEED);

                _anima.SetBool(_dashId, true);
            }
            else //通常移動
            {
                Movement(MOVE_SPEED);

                _anima.SetFloat(_speedId, 1);
                _anima.SetBool(_dashId, false);
            }
        }

        //手の位置にボールの生成
        _ball.transform.position = _hand.transform.position;
    }

    [SerializeField] private SpriteRenderer _rend;
    /// <summary>
    /// ゴール可能位置
    /// </summary>
    private void Goal()
    {
        _time -= Time.deltaTime;//時間の計測

        JumpGauge();

        if (_time < 0)
        {
            _ball.gameObject.SetActive(false);
            _state = STATE.Base;
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _rigid.AddForce(Vector2.up * GOAL_POWER, ForceMode.Impulse);

            _anima.SetBool(_isGroundedId, false);
            _anima.SetBool(_dashId, false);
            _state = STATE.Anima;
        }
    }

    private void JumpGauge()
    {
        var size = _rend.size;
        size.x = _time * 2;
        _rend.size = size;
    }

    private void GaugeReset()
    {
        var size = _rend.size;
        size.x = 0;
        _rend.size = size;
    }

    /// <summary>
    /// ゴールアニメーション
    /// </summary>
    private void Anima()
    {
        _ball.transform.position = _hand.transform.position;
        if (_isGround)
        {
            _ball.gameObject.SetActive(false);
            UiManager.Instance.BallSprite(null);
            //スコアの適応
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().Goal(_handItemScoar, _ball.GetComponent<Renderer>().material);
            _state = STATE.Base;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="speed">移動速度</param>
    private void Movement(float speed)
    {
        var vec = Vector3.right * speed;
        _vec = Vector3.Lerp(_vec, vec, 5.0f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            _vec = Vector3.zero;
            _state = STATE.Goal;
            _time = GOAL_WAITINPUT;
        }
    }

    public bool State()
    {
        if (_state == STATE.Goal || _state == STATE.Ball)
            return false;
        return true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")//ボールとの接触
        {
            if (_state == STATE.Base)//通常状態での判定
            {
                _state = STATE.Ball;
                _ball.GetComponent<Renderer>().material = collision.gameObject.GetComponent<Renderer>().material;
                _ball.gameObject.SetActive(true);
                _goal.transform.position = new Vector3(this.transform.position.x + 14.0f, 0, 0);

                _handItemScoar = collision.gameObject.GetComponent<Ball>().Scoar();

                Destroy(collision.gameObject);
            }
            else if (_state == STATE.Ball)//他のボールとの接触
            {
                _anima.SetBool(_isGroundedId, false);
                _anima.SetBool(_dashId, false);

                _state = STATE.Base;
            }
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "TimeBall")
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().AddTime(collision.gameObject.gameObject.GetComponent<Ball>().Time());
            Destroy(collision.gameObject);
        }
    }

    public void Play()
    {
        _state = STATE.Base;
    }
}