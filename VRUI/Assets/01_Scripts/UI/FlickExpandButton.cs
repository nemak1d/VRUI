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
		InputField inputField = null;
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

		TypeConfiguration typeConfig = null;
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

		InputField InputField
		{
			get
			{
				if (null == inputField)
				{
					inputField = this.GetOrAddComponent<InputField>();
				}
				return inputField;
			}
		}
		
		Subject<int> onSelectIndex = new Subject<int>();
		IntReactiveProperty tapCount = new IntReactiveProperty(-1);
		bool isUpdateInput = false;

		void Start()
		{
			SetVisibleChildren(false);

			selector
				.SetReference(StartAngle)
				.SetPartitionCount(childrenTypeConfig.Length);

			RootButton.OnLongPointerDownAsObservable()
				.Where(_ => !isTriggered)
				.Subscribe(_ => ShowChildren())
				.AddTo(this);

			var onTriggerStream = RootButton.OnPointerDownAsObservable();
			onTriggerStream
				.Subscribe(x => StartSelect(x))
				.AddTo(this);
			
			var eventTrigger = gameObject.AddComponent<ObservableEventTrigger>();
			eventTrigger.OnBeginDragAsObservable()
				.SkipUntil(onTriggerStream)
				.SelectMany(eventTrigger.OnDragAsObservable())
				.TakeUntil(eventTrigger.OnEndDragAsObservable())
				.RepeatUntilDestroy(gameObject)
				.Subscribe(x => UpdateSelect(x))
				.AddTo(this);

			var onReleaseStream = RootButton.OnPointerUpAsObservable();
			onReleaseStream
				.Subscribe(x => EndSelect(x))
				.AddTo(this);
			
			onSelectIndex.Where(x => 0 <= x)
				.Subscribe(x => OnEndFlick(x))
				.AddTo(this);
			onSelectIndex.Where(x => 0 > x)
				.Do(x => OnEndTap(x))
				.Throttle(System.TimeSpan.FromSeconds(1.0f))
				.Where(_ => !isTriggered)
				.Subscribe(_ => { tapCount.Value = -1; })
				.AddTo(this);

			tapCount.Where(x => 0 <= x)
				.Subscribe(x => Output(x))
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
		void ShowChildren()
		{
			SetVisibleChildren(true);
		}


		/// <summary>
		/// 選択開始.
		/// </summary>
		/// <param name="eventData"></param>
		void StartSelect(PointerEventData eventData)
		{
			flick.Begin(eventData.position);

			isTriggered = false;
		}

		/// <summary>
		/// 対象を選択.
		/// </summary>
		/// <param name="eventData"></param>
		void UpdateSelect(PointerEventData eventData)
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
		}
		
		/// <summary>
		/// 子ボタンを閉じる.
		/// </summary>
		void EndSelect(PointerEventData eventData)
		{
			flick.End(eventData.position);

			int index = (!isTriggered)
				? -1	// タップ判定
				: selector.GetIndex(flick.Angle);
			
			onSelectIndex.OnNext(index);
		}

		/// <summary>
		/// 出力.
		/// </summary>
		/// <param name="index"></param>
		void Output(int index)
		{
			TypeConfiguration target = childrenTypeConfig[index];
			Debug.LogFormat("typed:{0}", target.Output);

			if (isUpdateInput && 0 < inputField.text.Length)
			{
				inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
			}
			inputField.text += target.Output;

			SetVisibleChildren(false);
		}

		/// <summary>
		/// フリック処理.
		/// </summary>
		/// <param name="index"></param>
		void OnEndFlick(int index)
		{
			isUpdateInput = false;
			Output(index);

			tapCount.Value = -1;
		}

		/// <summary>
		/// タップ処理.
		/// </summary>
		/// <param name="index"></param>
		void OnEndTap(int index)
		{
			tapCount.Value = (1 + tapCount.Value) % childrenTypeConfig.Length;
			isUpdateInput = true;
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