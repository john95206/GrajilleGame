using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 本物の電話ジルの動きをつかさどるクラス
/// </summary>
public class GraJilleController : MonoBehaviour {

	// 全体をつかさどるクラス
	[Inject]
	GrajilleGameModel gameModel;

	// キラキラエフェクト
	[SerializeField]
	ParticleSystem moveParticle;
	// エフェクト再生用カメラ
	[SerializeField]
	Camera effectCamera;

	// 電話ジルのアニメーションをつかさどるクラス
	Animator anim;

	// 出現演出に使う時間
	[SerializeField]
	float showUpTime = 3.0f;

	// 電話ジルが生きているか
	public bool isActive = false;
	// 電話ジルがカメラに収まっているか
	public bool isVisible = false;

	// 電話ジルがひょこひょこと飛び回ることができるか
	bool isMovable
	{
		get
		{
			// 生きているか、カメラ内に収まっていれば自由に飛び回っても良い
			if (isVisible || isActive)
			{
				return true;
			}
			// 生きてないしカメラにも収まっていないならばイベント演出中なので強制的に動きを禁止する
			else
			{
				return false;
			}
		}
	}

	// 電話ジルのスピード
	public float speed = 0.2f;

	// ランダムに決める移動先のX座標のランダム範囲
	[SerializeField]
	float rangeX = 0.2f;
	// ランダムに決める移動先のY座標のランダム範囲
	[SerializeField]
	float rangeY = 0.2f;
	// ランダムで決まった移動先のX座標
	float randX;
	// ランダムで決まった移動先のY座標
	float randY;

	/// <summary>
	/// ゲーム開始時に一度だけ呼ばれる
	/// </summary>
	void Start ()
	{
		// 本物ジルを初期化
		Initialize();
	}
	
	/// <summary>
	/// 本物ジルの初期化
	/// </summary>
	void Initialize()
	{
		// 2秒ごとにParamChangeを呼び続けますよと宣言
		// Tips:これを毎秒60回も読んでしまうと1秒後には毎秒ParamChangeを60回呼び出してしまい、
		// 2秒後には毎秒ParamChangeを120回呼び出してしまい…やがてPCが死ぬ
		InvokeRepeating("ParamChange", 0, 2.0f);

		// プレイヤーのアニメーション情報取得
		anim = GetComponent<Animator>();

		// ゲームのクリア状況の監視を始めます宣言。
		// クリアフラグがTrueになるまで監視を続け、Trueになったら勝利アニメーション再生して監視を修了する
		this.UpdateAsObservable()
			.Select(_ => gameModel.isCleared)
			.Where(x => x)
			.Subscribe(_ => anim.SetTrigger("Cleared"));
	}
	
	/// <summary>
	/// 本物ジルが死んだ時
	/// </summary>
	public void Dead()
	{
		// ぶっちゃけ未実装
		if (isMovable)
		{
			//Destroy(gameObject);
		}
	}

	/// <summary>
	/// 毎秒60回呼ばれる処理
	/// </summary>
	void Update()
	{
		// ジルが動ける状態で、かつクリアがまだなら
		if (isMovable && !gameModel.isCleared)
		{
			// カメラ内に収まっていて、かつイベント中でないならば
			if (isVisible && isActive)
			{
				// ランダムな位置に定期的に移動する
				iTween.MoveUpdate(this.gameObject, iTween.Hash("x", randX, "y", randY, "speed", speed, "easetype", "linear"));
			}
			// カメラ内に収まっていないまたはイベント中ならば（ここバグかも）
			else
			{
				// 画面の真ん中に強制的に戻らせる
				iTween.MoveUpdate(this.gameObject, iTween.Hash("position", new Vector3(0, 0, 0), "islocal", false, "time", 2, "easetype", "linear"));
			}
		}
		// キラキラを毎秒3つまで生成する
		moveParticle.Emit(3);
	}

	/// <summary>
	/// カメラ内に自分が収まっているときに毎秒60回呼ばれる
	/// </summary>
	void OnWillRenderObject()
	{
		// そのカメラはメインカメラ？
		if (Camera.current.name == "Main Camera")
		{
			// 見えているぞ
			isVisible = true;
		}
		else
		{
			// 観なかったことにしてやろう
			isVisible = false;
		}
	}

	// 移動先をランダムに変える
	void ParamChange()
	{
		randX = Random.Range(-rangeX, rangeX);
		randY = Random.Range(-rangeY, rangeY);
	}

	/// <summary>
	/// ゲーム開始演出が終了したときに一度だけ呼ばれる
	/// </summary>
	void ShowUpEnd()
	{
		// ゲーム開始イベントが終わったのでゲームスタート。
		gameModel.InitializeGame();
	}
}
