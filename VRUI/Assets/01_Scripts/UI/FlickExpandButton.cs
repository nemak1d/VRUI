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
	[RequireComponent(typeof(Button))]
	public class FlickExpandButton : MonoBehaviour 
	{
		const float StartAngle = 90.0f * Mathf.Deg2Rad;

		[Header("Binding Components")]
		[SerializeField]
		Button[] childrenButton = null;
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

		void Start()
		{
			selector
				.SetReference(StartAngle)
				.SetPartitionCount(childrenButton.Length);
			
			var onTriggerStream = RootButton.OnPointerDownAsObservable();
			onTriggerStream
				.Subscribe(x => OnOpenChildren(x))
				.AddTo(this);

			var onReleaseStream = RootButton.OnPointerUpAsObservable();
			onReleaseStream
				.Subscribe(_ => OnCloseChildren())
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
		void OnOpenChildren(PointerEventData eventData)
		{
			if (null != childrenButton)
			{
				for (int i = 0; i < childrenButton.Length; ++i)
				{
					childrenButton[i].gameObject.SetActive(true);
				}
			}

			flick.Begin(eventData.position);

			isTriggered = false;
		}

		/// <summary>
		/// 子ボタンを閉じる.
		/// </summary>
		void OnCloseChildren()
		{
			if (null != childrenButton)
			{
				for (int i = 0; i < childrenButton.Length; ++i)
				{
					childrenButton[i].gameObject.SetActive(false);
				}
			}
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
			for (int i = 0; i < childrenButton.Length; ++i)
			{
				childrenButton[i].gameObject.SetActive(index == i);
			}
		}
	}
}