using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using N1D;

public class MathfExtentionsTest
{

    [Test]
    public void RepeatPI2_範囲境界上_Min()
	{
		Assert.AreEqual(0.0f, MathfExtentions.RepeatPI2(0.0f));
    }

	[Test]
	public void RepeatPI2_範囲境界上_Max()
	{
		Assert.AreEqual(0.0f, MathfExtentions.RepeatPI2(MathfExtentions.PI2));
	}

	[Test]
	public void RepeatPI2_範囲未満()
	{
		Assert.AreEqual(MathfExtentions.PI2 - 0.1f, MathfExtentions.RepeatPI2(-0.1f));
	}
	
	[Test]
	public void RepeatPI2_1週回って範囲未満()
	{
		Assert.AreEqual(0.0f, MathfExtentions.RepeatPI2(-MathfExtentions.PI2 - MathfExtentions.PI2));
	}

	[Test]
	public void RepeatPI2_半週回って範囲未満()
	{
		Assert.AreEqual(Mathf.PI, MathfExtentions.RepeatPI2(-Mathf.PI));
	}
}
