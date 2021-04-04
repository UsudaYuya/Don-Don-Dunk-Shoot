using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    #region インスタンスへのstaticなアクセスポイント
    /// <summary>
    /// このクラスのインスタンスを取得します。
    /// </summary>
    public static UiManager Instance
    {
        get { return instance; }
    }
    static UiManager instance = null;

    /// <summary>
    /// Start()より先に実行されます。
    /// </summary>
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region 「Message」UI
    /// <summary>
    /// 「Message」UIを指定します。
    /// </summary>
    [SerializeField]
    private GameObject message = null;

    /// <summary>
    /// 「Message」UIにメッセージを表示します。
    /// </summary>
    /// <param name="text">表示させたいテキスト</param>
    public void ShowMessage(string text)
    {
        message.transform.GetChild(0).GetComponent<Text>().text = text;
        message.SetActive(true);
    }

    /// <summary>
    /// 「Message」UIを隠します。
    /// </summary>
    public void HideMessage()
    {
        message.SetActive(false);
    }
    #endregion

    #region 「TIME: 00.00」表示用のUI
    /// <summary>
    /// 「TIME: 00.00」表示用のUIを指定します。
    /// </summary>
    [SerializeField]
    private Text timeUI = null;

    /// <summary>
    /// 「TIME: 00.00」UIの表示を更新します。
    /// </summary>
    public void UpdateTimeUI(float time)
    {
        timeUI.text = time.ToString("00.00");
    }
    #endregion

    #region スコア表示用
    [SerializeField]
    private Text Scoartext = null;
    public void ScoarMassage(int scoar)
    {
        Scoartext.text = scoar.ToString();
    }

    [SerializeField]
    private Text Enemytext = null;
    public void EnemyScoarMassage(int scoar)
    {
        Enemytext.text = scoar.ToString();
    }
    #endregion

    #region ボールの画像変更
    [SerializeField]
    private Image image;
    public void BallSprite(Sprite ball)
    {
        image.sprite = ball;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
