//##################################################
/*
 *	OculusGoObservable.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * OculusGo用コントローライベント用ストリームソース.
 */
//##################################################
using System;
using UnityEngine;
using UniRx;

namespace N1D
{
	/// <summary>
	/// OculusGo用コントローライベント用ストリームソース.
	/// </summary>
	public class OculusGoObservable
    {
        private static readonly OculusGoObservable instance = new OculusGoObservable();

        public static OculusGoObservable Instance { get { return instance; } }

		/// <summary>
		/// Primary:タッチパッドボタン/入力時.
		/// </summary>
		public IObservable<Unit> PrimaryTouchPadButtonOnTrigger { private set; get; }
		/// <summary>
		/// Primary:タッチパッドボタン/入力中.
		/// </summary>
        public IObservable<Unit> PrimaryTouchPadButtonOnPressing { private set; get; }
		/// <summary>
		/// Primary:タッチパッドボタン/解放時.
		/// </summary>
        public IObservable<Unit> PrimaryTouchPadButtonOnRelease { private set; get; }

		/// <summary>
		/// Primary:タッチパッド/入力時.
		/// </summary>
        public IObservable<Unit> PrimaryTouchPadOnTrigger { private set; get; }
		/// <summary>
		/// Primary:タッチパッド/入力中.
		/// </summary>
        public IObservable<Unit> PrimaryTouchPadOnPressing { private set; get; }
		/// <summary>
		/// Primary:タッチパッド/解放時.
		/// </summary>
        public IObservable<Unit> PrimaryTouchPadOnRelease { private set; get; }

		/// <summary>
		/// Primary:タッチパッド/入力座標.
		/// </summary>
        public IObservable<Vector2> PrimaryTouchPadAxis { private set; get; }

		/// <summary>
		/// Primary:トリガー/入力時.
		/// </summary>
        public IObservable<Unit> PrimaryIndexTriggerOnTrigger { private set; get; }
		/// <summary>
		/// Primary:トリガー/入力中.
		/// </summary>
        public IObservable<Unit> PrimaryIndexTriggerOnPressing { private set; get; }
		/// <summary>
		/// Primary:トリガー/解放時.
		/// </summary>
        public IObservable<Unit> PrimaryIndexTriggerOnRelease { private set; get; }
        
		/// <summary>
		/// バックボタン/入力時.
		/// </summary>
        public IObservable<Unit> BackButtonOnTrigger { private set; get; }
		/// <summary>
		/// バックボタン/入力中.
		/// </summary>
        public IObservable<Unit> BackButtonOnPressing { private set; get; }
		/// <summary>
		/// バックボタン/解放時.
		/// </summary>
        public IObservable<Unit> BackButtonOnRelease { private set; get; }

		/// <summary>
		/// コンストラクタ.
		/// </summary>
        private OculusGoObservable()
		{
            PrimaryTouchPadButtonOnTrigger 
                = CreateUpdateBoolStream(_ => OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad));
            PrimaryTouchPadButtonOnPressing
                = CreateUpdateBoolStream(_ => OVRInput.Get(OVRInput.Button.PrimaryTouchpad));
            PrimaryTouchPadButtonOnRelease
                = CreateUpdateBoolStream(_ => OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad));

            PrimaryTouchPadOnTrigger
                = CreateUpdateBoolStream(_ => OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad));
            PrimaryTouchPadOnPressing
                = CreateUpdateBoolStream(_ => OVRInput.Get(OVRInput.Touch.PrimaryTouchpad));
            PrimaryTouchPadOnRelease
                = CreateUpdateBoolStream(_ => OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad));
            
            PrimaryTouchPadAxis = Observable.EveryUpdate()
                .Where(_ => OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
                .Select(_ => OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad))
                .Publish()
                .RefCount();

            PrimaryIndexTriggerOnTrigger
                = CreateUpdateBoolStream(_ => OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger));
            PrimaryIndexTriggerOnPressing
                = CreateUpdateBoolStream(_ => OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger));
            PrimaryIndexTriggerOnRelease
                = CreateUpdateBoolStream(_ => OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger));

            BackButtonOnTrigger
                = CreateUpdateBoolStream(_ => OVRInput.GetDown(OVRInput.Button.Back));
            BackButtonOnPressing
                = CreateUpdateBoolStream(_ => OVRInput.Get(OVRInput.Button.Back));
            BackButtonOnRelease
                = CreateUpdateBoolStream(_ => OVRInput.GetUp(OVRInput.Button.Back));
        }
		
		/// <summary>
		/// 汎用Boolストリームソース生成.
		/// </summary>
		/// <param name="predicate">通知するための事前条件</param>
		/// <returns>ストリームソース</returns>
        private static IObservable<Unit> CreateUpdateBoolStream(Func<long, bool> predicate)
        {
            return Observable.EveryUpdate()
                .Where(predicate)
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }
	}
}

