using System;
using UnityEngine;

// Token: 0x02000AF3 RID: 2803
[Serializable]
public class VelocityHelper
{
	// Token: 0x060047D6 RID: 18390 RVA: 0x001815A5 File Offset: 0x0017F7A5
	public VelocityHelper(int historySize = 12)
	{
		this._size = historySize;
		this._samples = new float[historySize * 4];
	}

	// Token: 0x060047D7 RID: 18391 RVA: 0x001815C4 File Offset: 0x0017F7C4
	public void SamplePosition(Transform target, float dt)
	{
		Vector3 position = target.position;
		if (!this._initialized)
		{
			this._InitSamples(position, dt);
		}
		this._SetSample(this._latest, position, dt);
		this._latest = (this._latest + 1) % this._size;
	}

	// Token: 0x060047D8 RID: 18392 RVA: 0x0018160C File Offset: 0x0017F80C
	private void _InitSamples(Vector3 position, float dt)
	{
		for (int i = 0; i < this._size; i++)
		{
			this._SetSample(i, position, dt);
		}
		this._initialized = true;
	}

	// Token: 0x060047D9 RID: 18393 RVA: 0x0018163A File Offset: 0x0017F83A
	private void _SetSample(int i, Vector3 position, float dt)
	{
		this._samples[i] = position.x;
		this._samples[i + 1] = position.y;
		this._samples[i + 2] = position.z;
		this._samples[i + 3] = dt;
	}

	// Token: 0x040059DF RID: 23007
	private float[] _samples;

	// Token: 0x040059E0 RID: 23008
	private int _latest;

	// Token: 0x040059E1 RID: 23009
	private int _size;

	// Token: 0x040059E2 RID: 23010
	private bool _initialized;
}
