//##################################################
/*
 *	DirectionalIndexSelect.cs
 *
 *	develop by nemak1d_sys
 *
 * -------------------------------------------------
 * 方向に応じたインデックスの選択.
 * 
 * 基準角度と分割数から渡された角度を元にインデックスを返します.
 */
//##################################################
using UnityEngine;

namespace N1D
{
	/// <summary>
	/// 方向に応じたインデックスの選択.
	/// </summary>
	public class DirectionalIndexSelect 
	{
		int partitionCount = 1;
		float partitionRange = MathfExtentions.PI2;
		float referenceAngle = 0.0f;

		/// <summary>
		/// 基準となる角度の設定.
		/// </summary>
		/// <param name="radian">角度[rad]</param>
		/// <returns>自身</returns>
		public DirectionalIndexSelect SetReference(float radian)
		{
			referenceAngle = MathfExtentions.RepeatPI2(radian);
			return this;
		}

		/// <summary>
		/// 分割数の設定.
		/// </summary>
		/// <param name="count">分割数(0 < count)</param>
		/// <returns>自身</returns>
		public DirectionalIndexSelect SetPartitionCount(int count)
		{
			Debug.AssertFormat(0 < count,
				"partition count is zero or minus: {0}", count);

			partitionCount = count;
			partitionRange = (0 < count) ? MathfExtentions.PI2 / count : 0.0f;
			return this;
		}

		/// <summary>
		/// インデックスの取得.
		/// 基準となる角度が含まれる方向を0として反時計回りに加算されるインデックスを返します.
		/// </summary>
		/// <param name="radian">選択基準となる角度[rad]</param>
		/// <returns>)0以上のインデックス.無効な場合は-1</returns>
		public int GetIndex(float radian)
		{
			float startAngle = MathfExtentions.RepeatPI2(referenceAngle - partitionRange / 2.0f);
			float correctRadian = MathfExtentions.RepeatPI2(radian - startAngle);
			
			for (int i = 0; i < partitionCount - 1; ++i)
			{
				if ((i + 1) * partitionRange >= correctRadian)
				{
					return i;
				}
			}
			return (0 < partitionCount) ? partitionCount - 1 : -1;
		}
	}
}