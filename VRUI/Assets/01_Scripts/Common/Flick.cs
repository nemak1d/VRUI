//##################################################
/*
 *	Flick.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * フリック.
 */
//##################################################
using System;
using UnityEngine;
using UniRx;

namespace N1D
{
	/// <summary>
	/// フリック.
	/// </summary>
	public class Flick : IDisposable
	{
		/// <summary>
		/// 始点.
		/// </summary>
		public Vector2 BeginPosition { get { return startPosition.Value; } }

		/// <summary>
		/// 終点.
		/// </summary>
		public Vector2 EndPosition { get { return endPosition.Value; } }

		/// <summary>
		/// 差分.
		/// </summary>
		public Vector2 Delta { private set; get; }

		/// <summary>
		/// 長さ.
		/// </summary>
		public float SqrMagnitude { private set; get; }

		/// <summary>
		/// 角度.
		/// </summary>
		public float Angle { private set; get; }

		Vector2ReactiveProperty startPosition = new Vector2ReactiveProperty(Vector2.zero);
		Vector2ReactiveProperty endPosition = new Vector2ReactiveProperty(Vector2.zero);

		CompositeDisposable disposer = new CompositeDisposable();

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		public Flick()
		{
			startPosition
				.Subscribe(x => endPosition.Value = x)
				.AddTo(disposer);

			endPosition
				.Subscribe(_ => UpdateDelta())
				.AddTo(disposer);
		}

		/// <summary>
		/// 破棄.
		/// </summary>
		public void Dispose()
		{
			if (null != disposer)
			{
				disposer.Clear();
				disposer.Dispose();
				disposer = null;
			}

			startPosition = null;
			endPosition = null;
		}

		/// <summary>
		/// 始点の設定.
		/// </summary>
		/// <param name="position">座標</param>
		public void Begin(Vector2 position)
		{
			startPosition.Value = position;
		}
		
		/// <summary>
		/// 終点の設定.
		/// </summary>
		/// <param name="position"></param>
		public void End(Vector2 position)
		{
			endPosition.Value = position;
		}
		
		/// <summary>
		/// 差分の更新.
		/// </summary>
		void UpdateDelta()
		{
			Delta = EndPosition - BeginPosition;
			SqrMagnitude = Delta.sqrMagnitude;
			Angle = MathfExtentions.Atan2To2PI(Delta.y, Delta.x);
		}
	}
}

