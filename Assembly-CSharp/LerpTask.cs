using System;
using UnityEngine;

// Token: 0x020008F4 RID: 2292
public class LerpTask<T>
{
	// Token: 0x06003BEC RID: 15340 RVA: 0x00147A6E File Offset: 0x00145C6E
	public void Reset()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 0f);
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06003BED RID: 15341 RVA: 0x00147A9E File Offset: 0x00145C9E
	public void Start(T from, T to, float duration)
	{
		this.lerpFrom = from;
		this.lerpTo = to;
		this.duration = duration;
		this.elapsed = 0f;
		this.active = true;
	}

	// Token: 0x06003BEE RID: 15342 RVA: 0x00147AC8 File Offset: 0x00145CC8
	public void Finish()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 1f);
		Action action = this.onLerpEnd;
		if (action != null)
		{
			action();
		}
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06003BEF RID: 15343 RVA: 0x00147B14 File Offset: 0x00145D14
	public void Update()
	{
		if (!this.active)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.elapsed < this.duration)
		{
			float arg = (this.elapsed + deltaTime >= this.duration) ? 1f : (this.elapsed / this.duration);
			this.onLerp(this.lerpFrom, this.lerpTo, arg);
			this.elapsed += deltaTime;
			return;
		}
		this.Finish();
	}

	// Token: 0x04004C76 RID: 19574
	public float elapsed;

	// Token: 0x04004C77 RID: 19575
	public float duration;

	// Token: 0x04004C78 RID: 19576
	public T lerpFrom;

	// Token: 0x04004C79 RID: 19577
	public T lerpTo;

	// Token: 0x04004C7A RID: 19578
	public Action<T, T, float> onLerp;

	// Token: 0x04004C7B RID: 19579
	public Action onLerpEnd;

	// Token: 0x04004C7C RID: 19580
	public bool active;
}
