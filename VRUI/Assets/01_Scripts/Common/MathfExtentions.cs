//##################################################
/*
 *	MathfExtentions.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * Mathf拡張.
 */
//##################################################
using UnityEngine;

namespace N1D
{
	/// <summary>
	/// Mathf拡張.
	/// </summary>
	public struct MathfExtentions 
	{
		public const float PI2 = Mathf.PI * 2.0f;

		/// <summary>
		/// 0-2πの範囲に置き換えたAtan2.
		/// </summary>
		/// <param name="y">y軸差分</param>
		/// <param name="x">x軸差分</param>
		/// <returns>角度(0-2π)[rad]</returns>
		public static float Atan2To2PI(float y, float x)
		{
			float radian = Mathf.Atan2(y, x);
			if (0.0f > radian)
			{
				radian += PI2;
			}
			return radian;
		}

		/// <summary>
		/// 0-2πの範囲をループさせて値を収める.
		/// </summary>
		/// <param name="radian">任意の角度[rad]</param>
		/// <returns>角度(0-2π)[rad]</returns>
		public static float RepeatPI2(float radian)
		{
			return Mathf.Repeat(radian, PI2);
		}
	}
}