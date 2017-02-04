using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// タップされたときに死ぬ以外は大体電話ジルと同じなので変更箇所以外説明省略
/// </summary>
public class GrajilleDammyController : MonoBehaviour {

	GrajilleGameModel gameModel;

	[SerializeField]
	ParticleSystem moveParticle;
	public bool isActive = false;

	public bool isVisible = false;

	bool isMovable
	{
		get
		{
			if (isVisible || isActive)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public float speed = 0.2f;

	[SerializeField]
	float time = 2.0f;
	[SerializeField] float timeRange = 2.0f;
	[SerializeField] float rangeX = 0.2f;
	[SerializeField] float rangeY = 0.2f;

	float randX;
	float randY;

	Animator anim;

	// Use this for initialization
	void Start ()
	{
		gameModel = GetComponentInParent<GrajilleGameModel>();
		InvokeRepeating("ParamChange", 0, 2.0f);

		anim = GetComponent<Animator>();

		this.UpdateAsObservable()
			.Select(_ => gameModel.isCleared)
			.Where(x => x)
			.Subscribe(_ => anim.SetTrigger("Cleared"));
	}


	public void Dead()
	{
		// タップされたときにイベント中でないならば死亡演出を再生して0.5秒後に自身を破壊
		if (isMovable)
		{
			anim.Play("GraJille_Dead");

			Destroy(this.gameObject, 0.5f);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (isMovable && !gameModel.isCleared)
		{
			if (isVisible && isActive)
			{
				iTween.MoveUpdate(this.gameObject, iTween.Hash("x", randX, "y", randY, "time", time, "easetype", "linear"));
			}
			else
			{
				iTween.MoveUpdate(this.gameObject, iTween.Hash("position", new Vector3(0, 0, 0), "islocal", false, "time", 2, "easetype", "linear"));
			}
			moveParticle.Emit(3);
		}
	}

	void OnWillRenderObject()
	{

		if (Camera.current.name == "Main Camera")
			
		{
			isVisible = true;
		}
		else
		{
			isVisible = false;
		}
	}

	void ParamChange()
	{
		time = Random.Range(0, timeRange);
		randX = Random.Range(-rangeX, rangeX);
		randY = Random.Range(-rangeY, rangeY);
	}
}
