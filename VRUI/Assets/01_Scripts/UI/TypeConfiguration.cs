//##################################################
/*
 *	TypeConfiguration.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * タイプ設定.
 */
//##################################################
using UnityEngine;
using UnityEngine.UI;

namespace N1D
{
	[DisallowMultipleComponent]
	public class TypeConfiguration : MonoBehaviour 
	{
		[Header("Binding Components")]
		[SerializeField]
		Text captionField = null;
		[Space]

		[Header("Local Parameters")]
		[SerializeField]
		string caption = string.Empty;
		[SerializeField]
		char outputCharacter = 'n';
		
		public string Caption
		{
			set
			{
				caption = value;
				captionField.text = value;
			}
			get { return caption; }
		}

		public char Output
		{
			set { outputCharacter = value; }
			get { return outputCharacter; }
		}

		void Start()
		{
			Caption = caption;
		}
	}
}

