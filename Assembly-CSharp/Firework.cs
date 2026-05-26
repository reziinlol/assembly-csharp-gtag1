using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000DBD RID: 3517
public class Firework : MonoBehaviour
{
	// Token: 0x06005644 RID: 22084 RVA: 0x001BFF58 File Offset: 0x001BE158
	private void Launch()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._controller)
		{
			this._controller.Launch(this);
		}
	}

	// Token: 0x06005645 RID: 22085 RVA: 0x001BFF7C File Offset: 0x001BE17C
	private void OnValidate()
	{
		if (!this._controller)
		{
			this._controller = base.GetComponentInParent<FireworksController>();
		}
		if (!this._controller)
		{
			return;
		}
		Firework[] array = this._controller.fireworks;
		if (array.Contains(this))
		{
			return;
		}
		array = (from x in array.Concat(new Firework[]
		{
			this
		})
		where x != null
		select x).ToArray<Firework>();
		this._controller.fireworks = array;
	}

	// Token: 0x06005646 RID: 22086 RVA: 0x001C000C File Offset: 0x001BE20C
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x06005647 RID: 22087 RVA: 0x001C002D File Offset: 0x001BE22D
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x04006616 RID: 26134
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x04006617 RID: 26135
	[Space]
	public Transform origin;

	// Token: 0x04006618 RID: 26136
	public Transform target;

	// Token: 0x04006619 RID: 26137
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x0400661A RID: 26138
	public Color colorTarget = Color.magenta;

	// Token: 0x0400661B RID: 26139
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x0400661C RID: 26140
	public AudioSource sourceTarget;

	// Token: 0x0400661D RID: 26141
	[Space]
	public ParticleSystem trail;

	// Token: 0x0400661E RID: 26142
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x0400661F RID: 26143
	[Space]
	public bool doTrail = true;

	// Token: 0x04006620 RID: 26144
	public bool doTrailAudio = true;

	// Token: 0x04006621 RID: 26145
	public bool doExplosion = true;
}
