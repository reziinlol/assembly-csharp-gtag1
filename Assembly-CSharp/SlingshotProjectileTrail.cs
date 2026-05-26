using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020004C2 RID: 1218
public class SlingshotProjectileTrail : MonoBehaviour
{
	// Token: 0x06001DC0 RID: 7616 RVA: 0x000A0654 File Offset: 0x0009E854
	private void Awake()
	{
		this.initialWidthMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001DC1 RID: 7617 RVA: 0x000A0668 File Offset: 0x0009E868
	public void AttachTrail(GameObject obj, bool blueTeam, bool redTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		this.followObject = obj;
		this.followXform = this.followObject.transform;
		Transform transform = base.transform;
		transform.position = this.followXform.position;
		this.initialScale = transform.localScale.x;
		transform.localScale = this.followXform.localScale;
		this.trailRenderer.widthMultiplier = this.initialWidthMultiplier * this.followXform.localScale.x;
		this.trailRenderer.Clear();
		if (shouldOverrideColor)
		{
			this.SetColor(overrideColor);
		}
		else if (blueTeam)
		{
			this.SetColor(this.blueColor);
		}
		else if (redTeam)
		{
			this.SetColor(this.orangeColor);
		}
		else
		{
			this.SetColor(this.defaultColor);
		}
		this.timeToDie = -1f;
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x000A073C File Offset: 0x0009E93C
	protected void LateUpdate()
	{
		if (this.followObject.IsNull())
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		base.gameObject.transform.position = this.followXform.position;
		if (!this.followObject.activeSelf && this.timeToDie < 0f)
		{
			this.timeToDie = Time.time + this.trailRenderer.time;
		}
		if (this.timeToDie > 0f && Time.time > this.timeToDie)
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x000A07F8 File Offset: 0x0009E9F8
	public void SetColor(Color color)
	{
		TrailRenderer trailRenderer = this.trailRenderer;
		this.trailRenderer.endColor = color;
		trailRenderer.startColor = color;
	}

	// Token: 0x04002806 RID: 10246
	public TrailRenderer trailRenderer;

	// Token: 0x04002807 RID: 10247
	public Color defaultColor = Color.white;

	// Token: 0x04002808 RID: 10248
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x04002809 RID: 10249
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x0400280A RID: 10250
	private GameObject followObject;

	// Token: 0x0400280B RID: 10251
	private Transform followXform;

	// Token: 0x0400280C RID: 10252
	private float timeToDie = -1f;

	// Token: 0x0400280D RID: 10253
	private float initialScale;

	// Token: 0x0400280E RID: 10254
	private float initialWidthMultiplier;
}
