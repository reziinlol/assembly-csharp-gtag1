using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class CameraShakeDispatcher : MonoBehaviour
{
	// Token: 0x060000C3 RID: 195 RVA: 0x0000561D File Offset: 0x0000381D
	private void OnEnable()
	{
		if (this.shakeOnEnable)
		{
			if (this.maxDistance > 0f)
			{
				this.ShakeInProximity(this.maxDistance);
				return;
			}
			this.Shake();
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00005647 File Offset: 0x00003847
	private void OnDisable()
	{
		if (this.haltOnDisable)
		{
			this.Halt();
		}
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00005657 File Offset: 0x00003857
	public void Shake()
	{
		CameraShaker.Shake(this.duration, this.magnitude, this.freqRange, this.rollOffOverDuration);
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00005676 File Offset: 0x00003876
	public void ShakeInProximity(float distance)
	{
		CameraShaker.ShakeInProximity(this.duration, this.magnitude, this.freqRange, this.rollOffOverDuration, base.transform, distance);
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000569C File Offset: 0x0000389C
	public void Halt()
	{
		CameraShaker.Halt();
	}

	// Token: 0x040000D9 RID: 217
	[SerializeField]
	private float magnitude = 1f;

	// Token: 0x040000DA RID: 218
	[SerializeField]
	private float duration = 0.5f;

	// Token: 0x040000DB RID: 219
	[SerializeField]
	private bool rollOffOverDuration = true;

	// Token: 0x040000DC RID: 220
	[SerializeField]
	private bool shakeOnEnable;

	// Token: 0x040000DD RID: 221
	[SerializeField]
	private bool haltOnDisable;

	// Token: 0x040000DE RID: 222
	[SerializeField]
	private Vector2 freqRange = new Vector2(0.02f, 0.1f);

	// Token: 0x040000DF RID: 223
	[SerializeField]
	private float maxDistance;
}
