using System;
using UnityEngine;

// Token: 0x02000AC9 RID: 2761
[Serializable]
public struct FrameStamp
{
	// Token: 0x17000692 RID: 1682
	// (get) Token: 0x0600468F RID: 18063 RVA: 0x0017E0D1 File Offset: 0x0017C2D1
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06004690 RID: 18064 RVA: 0x0017E0E0 File Offset: 0x0017C2E0
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06004691 RID: 18065 RVA: 0x0017E102 File Offset: 0x0017C302
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06004692 RID: 18066 RVA: 0x0017E119 File Offset: 0x0017C319
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x06004693 RID: 18067 RVA: 0x0017E126 File Offset: 0x0017C326
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x06004694 RID: 18068 RVA: 0x0017E130 File Offset: 0x0017C330
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x040058F3 RID: 22771
	private int _lastFrame;
}
