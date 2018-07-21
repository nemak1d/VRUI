//##################################################
/*
 *	MonoBehaviourExtentions.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * MonoBehaviour拡張.
 */
//##################################################
using UnityEngine;

namespace N1D
{
	/// <summary>
	/// MonoBehaviour拡張.
	/// </summary>
	public static class MonoBehaviourExtentions
	{
		/// <summary>
		/// コンポーネントの取得もしくは追加.
		/// </summary>
		/// <typeparam name="T">コンポーネント</typeparam>
		/// <param name="obj">ゲームオブジェクト</param>
		/// <returns>コンポーネント</returns>
		public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
		{
			T result = obj.GetComponent<T>();
			if (null == result)
			{
				result = obj.AddComponent<T>();
			}
			return result;
		}

		/// <summary>
		/// コンポーネントの取得もしくは追加.
		/// </summary>
		/// <typeparam name="T">コンポーネント</typeparam>
		/// <param name="component">コンポーネント</param>
		/// <returns>コンポーネント</returns>
		public static T GetOrAddComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.GetOrAddComponent<T>();
		}
	}
}

