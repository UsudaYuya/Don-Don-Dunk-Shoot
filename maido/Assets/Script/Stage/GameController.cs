using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private float _time = 60;
    private int _scoar = 0;
    [SerializeField] private int _enemyScoar = 20;
    [SerializeField] private GameObject[] _stage = new GameObject[2];

    [SerializeField] private GameObject[] _ball = new GameObject[4];
    [SerializeField] private GameObject _goal = null;

    private Transform _player;
    private bool play = false;
    // Start is called before the first frame update
    void Start()
    {
        UiManager.Instance.EnemyScoarMassage(_enemyScoar);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(OnStart());//開始処理
    }

    // Update is called once per frame
    void Update()
    {
        PlayTime();
        MoveMap();

        if (play)
            BallInstance();

        if (_time < 0)
        {
            if (_scoar > _enemyScoar)
                StartCoroutine(OnClear("GameClear"));
            else
                StartCoroutine(OnClear("GameOver"));
        }
    }

    private int num = 0;
    private void MoveMap()
    {
        var pos = _player.position;

        if (pos.x >= num * 30)
        {
            num++;
            var p = _stage[num % 2].transform.position;
            p.x = num * 30;
            _stage[num % 2].transform.position = p;
        }
    }

    private float _Instance_time = 0;
    private void BallInstance()
    {
        if (_player.GetComponent<Player>().State())
        {
            _Instance_time -= Time.deltaTime;
            if (_Instance_time < 0)
            {
                _Instance_time = 1.5f;
                Instantiate(_ball[Random.Range(0, 4)], _player.position + (Vector3.right * 12) + Vector3.up, Quaternion.identity);
            }
        }
        else
            _Instance_time = 2.0f;
    }

    public void AddTime(float AddTime = 0)
    {
        _time += AddTime;
    }

    /// <summary>
    /// スタート時挙動
    /// </summary>
    /// <returns></returns>
    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(1); // 1秒待機
        const float showTimeout = 0.6f;

        UiManager.Instance.ShowMessage("3");
        yield return new WaitForSeconds(showTimeout);
        UiManager.Instance.HideMessage();
        yield return new WaitForSeconds(1 - showTimeout);

        UiManager.Instance.ShowMessage("2");
        yield return new WaitForSeconds(showTimeout);
        UiManager.Instance.HideMessage();
        yield return new WaitForSeconds(1 - showTimeout);

        UiManager.Instance.ShowMessage("1");
        yield return new WaitForSeconds(showTimeout);
        UiManager.Instance.HideMessage();
        yield return new WaitForSeconds(1 - showTimeout);

        UiManager.Instance.ShowMessage("GO!");

        yield return new WaitForSeconds(1); // 1秒待機
        UiManager.Instance.HideMessage();

        play = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Play();
    }

    [SerializeField]private AudioClip _endAudio;
    IEnumerator OnClear(string scene)
    {
        AudioSource.PlayClipAtPoint(_endAudio, _player.transform.position);
        play = false;
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// プレイ中の時間の設定
    /// </summary>
    private void PlayTime()
    {
        if (!play)
            return;
        _time -= Time.deltaTime;
        UiManager.Instance.UpdateTimeUI(_time);
    }

    /// <summary>
    /// ゴールした時のスコアの変動
    /// </summary>
    /// <param name="scoar"></param>
    public void Goal(int scoar, Material material)
    {
        _goal.transform.GetChild(0).position = _goal.transform.position;
        _goal.transform.GetChild(0).GetComponent<Renderer>().material = material;
        _goal.transform.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.zero;

        _scoar += scoar;
        UiManager.Instance.ScoarMassage(_scoar);

    }
}
