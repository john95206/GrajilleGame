using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// プレイヤーの入力を司るクラス
/// </summary>
public class InputModel : MonoBehaviour {

	// タップした時の白○エフェクト
	[SerializeField]
	ParticleSystem tapEffect;
	// エフェクト再生用のカメラ
	[SerializeField]
	Camera effectCamera;

	// ゲーム全体をつかさどるクラス
	[Inject]
	GrajilleGameModel gameModel;

	void Start ()
	{
		
	}
	
	// 毎秒60回呼び出される
	void Update ()
	{
		// 左クリックまたはスマホでタップされたとき
		if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
		{
			// タップした位置を3次元ベクトルで取得
			Vector3 tapPointRaw = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
			// タップした位置をカメラ座標に変換
			Vector3 tapPoint = Camera.main.ScreenToWorldPoint(tapPointRaw);
			// タップした位置にヒットするオブジェクトを取得
			Collider2D collider2D = Physics2D.OverlapPoint(tapPoint);

			// マウスのワールド座標までパーティクルを移動し、パーティクルエフェクトを1つ生成する
			var pos = effectCamera.ScreenToWorldPoint(Input.mousePosition + effectCamera.transform.forward * 10);
			tapEffect.transform.position = pos;
			tapEffect.Emit(1);

			// タップした位置にヒットするオブジェクトが存在するならば
			if (collider2D)
			{
				// ヒットしたオブジェクトを取得
				var hitObject = collider2D.transform.gameObject;
				// ヒットしたオブジェクトが取得できたら
				// Tips : ここで取得できないことはあまりないが、以降の処理は万が一取得できなかった時に
				// 参照先を見失いゲームがフリーズするのでそれを回避する
				if (hitObject)
				{
					// ヒットしたオブジェクトが本物だったら
					if(hitObject.tag == "Player")
					{
						// ゲーム全体を管理するクラスからゲームクリア関数を呼び出す
						gameModel.GameClear();
					}
					else
					{
						// 偽物だったらそいつを殺す
						hitObject.GetComponent<GrajilleDammyController>().Dead();
					}
				}
			}
		}
	}
}
