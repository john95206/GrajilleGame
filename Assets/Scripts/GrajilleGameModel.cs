using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using UnityEngine.UI;

/// <summary>
/// ゲームの流れを管理するクラス
/// </summary>
public class GrajilleGameModel : MonoBehaviour {

	// 電話ジルのダミーくんのクラス
	public GrajilleDammyController dammy = null;

	// 本物の電話ジル。Inspectorでアタッチして取得
	[SerializeField]
	GraJilleController player = null;
	// ゲームオーバーした時のTheEnd表記
	[SerializeField]
	Text gameOverText = null;

	// 入力を管理するクラス。
	[Inject]
	InputModel inputModel;

	// 制限時間
	public float gameTime = 10.0f;

	// クリア判定
	public bool isCleared = false;

	/// <summary>
	/// ゲームの初期化。
	/// </summary>
	public void InitializeGame()
	{
		// ダミー君を取得できていない→Inspectorでアタッチできていない
		if(dammy == null)
		{
			Debug.Log("アタッチ忘れ");
		}
		// 取得できているならゲームスタート
		else
		{
			// ダミー君をたくさん生み出す
			Spawn();

			// 本物を動かす
			player.isActive = true;

			// タイマー開始・ゲームスタート
			StartCoroutine("GameLoop");
		}
	}

	/// <summary>
	/// ランダムにダミー君の数を決めてその数だけ一瞬で生成する
	/// </summary>
	void Spawn()
	{
		// 生成するダミー君の数を決める
		var dammyCount = Mathf.CeilToInt(Random.Range(25, 50));

		// 決めた数だけダミー君を生成
		for (var i = 0; i < dammyCount; i++)
		{
			// ダミー君の個体情報を取得、生成
			GrajilleDammyController dammyItem = Instantiate<GrajilleDammyController>(dammy, new Vector3(0, 0, 0), transform.rotation) as GrajilleDammyController;
			// ダミー君をこのオブジェクトの子供にする(管理しやすくする)
			dammyItem.transform.parent = this.transform;
			// ダミー君の名前を生成順に名づける(デバッグしやすくする)
			dammyItem.name = "dammy" + i.ToString();
			// ダミー君に動く許可を与える
			dammyItem.isActive = true;
		}
	}

	/// <summary>
	/// クリア時のイベント
	/// </summary>
	public void GameClear()
	{
		// 本物が生きているかどうか
		if (player.isActive)
		{
			// 生きていたらクリアフラグをTrueにする
			Debug.Log("Clear!");
			isCleared = true;
		}
	}

	/// <summary>
	/// ゲームの主な流れ
	/// </summary>
	/// <returns></returns>
	IEnumerator GameLoop()
	{
		// クリアしていない限り時間を進め続ける
		while (!isCleared)
		{
			// 時間切れになっていないならゲーム時間を進める
			if (gameTime > 0)
			{
				gameTime -= Time.fixedDeltaTime;
			}
			// 時間切れ（残りタイマーが0未満）ならば
			else
			{
				// ゲームオーバー
				GameOver();

				// ループ終了
				yield break;
			}

			// クリアしていない限りはループを続行し続ける
			yield return null;
		}
	}

	/// <summary>
	/// ゲームオーバー演出
	/// </summary>
	void GameOver()
	{
		// 全てのダミー君を検索し、サーチアンドデストロイ
		for(var i = 0;i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		// 本物の動きを止める
		player.isActive = false;
		// 本物を画面左に立ち去らせる
		iTween.MoveTo(player.gameObject, new Vector3(-10, 0, 0), 30);
		// ゲームオーバーのテキストの透明度を1にして表示。
		gameOverText.color = Color.white;
	}
}
