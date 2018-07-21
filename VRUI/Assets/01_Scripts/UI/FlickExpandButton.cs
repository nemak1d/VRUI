//##################################################
/*
 *	FlickExpandButton.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * フリック展開ボタン.
 */
//##################################################
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

namespace N1D
{
	/// <summary>
	/// フリック展開ボタン.
	/// </summary>
	[DisallowMultipleComponent, RequireComponent(typeof(Button), typeof(TypeConfiguration))]
	public class FlickExpandButton : MonoBehaviour 
	{
		const float StartAngle = 90.0f * Mathf.Deg2Rad;

		[Header("Binding Components")]
		[SerializeField]
		TypeConfiguration[] childrenTypeConfig = null;
		[Space]
		
		[Header("Local Parameters")]
		[SerializeField]
		float validateFlickMagnitude = 10.0f;
		[Space]


		bool isTriggered = false;
		Flick flick = new Flick();
		DirectionalIndexSelect selector = new DirectionalIndexSelect();

		Button rootButton = null;
		Button RootButton
		{
			get
			{
				if (null == rootButton)
				{
					rootButton = this.GetOrAddComponent<Button>();
				}
				return rootButton;
			}
		}

		TypeConfiguration typeConfig;
		TypeConfiguration TypeConfig
		{
			get
			{
				if (null == typeConfig)
				{
					typeConfig = this.GetOrAddComponent<TypeConfiguration>();
				}
				return typeConfig;
			}
		}


		void Start()
		{
			SetVisibleChildren(false);

			selector
				.SetReference(StartAngle)
				.SetPartitionCount(childrenTypeConfig.Length);

			RootButton.OnLongPointerDownAsObservable()
				.Where(_ => !isTriggered)
				.Subscribe(_ => OnOpenChildren())
				.AddTo(this);

			var onTriggerStream = RootButton.OnPointerDownAsObservable();
			onTriggerStream
				.Subscribe(x => OnBeginSelect(x))
				.AddTo(this);

			var onReleaseStream = RootButton.OnPointerUpAsObservable();
			onReleaseStream
				.Subscribe(x => OnCloseChildren(x))
				.AddTo(this);

			var eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();
			eventTrigger.OnBeginDragAsObservable()
				.SkipUntil(onTriggerStream)
				.SelectMany(eventTrigger.OnDragAsObservable())
				.TakeUntil(eventTrigger.OnEndDragAsObservable())
				.RepeatUntilDestroy(gameObject)
				.Subscribe(x => OnSelect(x))
				.AddTo(this);
		}

		void OnDestroy()
		{
			if (null != flick)
			{
				flick.Dispose();
				flick = null;
			}

			selector = null;
		}

		/// <summary>
		/// 子ボタンを開く.
		/// </summary>
		/// <param name="eventData"></param>
		void OnOpenChildren()
		{
			SetVisibleChildren(true);
		}

		/// <summary>
		/// 子ボタンを閉じる.
		/// </summary>
		void OnCloseChildren(PointerEventData eventData)
		{
			flick.End(eventData.position);

			int index = (validateFlickMagnitude > flick.SqrMagnitude) 
				? -1 
				: selector.GetIndex(flick.Angle);

			TypeConfiguration target = TypeConfig;
			if (0 <= index)
			{
				target = childrenTypeConfig[index];
			}
			Debug.LogFormat("typed:{0}", target.Output);

			SetVisibleChildren(false);
		}

		/// <summary>
		/// 選択開始.
		/// </summary>
		/// <param name="eventData"></param>
		void OnBeginSelect(PointerEventData eventData)
		{
			flick.Begin(eventData.position);

			isTriggered = false;
		}

		/// <summary>
		/// 対象を選択.
		/// </summary>
		/// <param name="eventData"></param>
		void OnSelect(PointerEventData eventData)
		{
			flick.End(eventData.position);

			if (validateFlickMagnitude > flick.SqrMagnitude)
			{
				if (isTriggered)
				{
					SwitchVisibleChildren(-1);
				}
				return;
			}
			isTriggered = true;
			
			int index = selector.GetIndex(flick.Angle);
			SwitchVisibleChildren(index);

			Debug.LogFormat("angle:{0}, index:{1}", flick.Angle * Mathf.Rad2Deg, index);
		}

		/// <summary>
		/// 子ボタンの表示切替.
		/// </summary>
		/// <param name="index">表示対象のインデックス</param>
		void SwitchVisibleChildren(int index)
		{
			for (int i = 0; i < childrenTypeConfig.Length; ++i)
			{
				childrenTypeConfig[i].gameObject.SetActive(index == i);
			}
		}

		/// <summary>
		/// 子ボタンすべての表示有無.
		/// </summary>
		/// <param name="isVisible">表示するか</param>
		void SetVisibleChildren(bool isVisible)
		{
			for (int i = 0; i < childrenTypeConfig.Length; ++i)
			{
				childrenTypeConfig[i].gameObject.SetActive(isVisible);
			}
		}
	}
}