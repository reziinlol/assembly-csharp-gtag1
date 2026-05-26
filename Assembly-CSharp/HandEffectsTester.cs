using System;
using GorillaExtensions;
using TagEffects;
using UnityEngine;

// Token: 0x020003D4 RID: 980
[RequireComponent(typeof(Collider))]
public class HandEffectsTester : MonoBehaviour, IHandEffectsTrigger
{
	// Token: 0x17000239 RID: 569
	// (get) Token: 0x0600174B RID: 5963 RVA: 0x00086269 File Offset: 0x00084469
	public bool Static
	{
		get
		{
			return this.isStatic;
		}
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x0600174C RID: 5964 RVA: 0x00086271 File Offset: 0x00084471
	Transform IHandEffectsTrigger.Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x0600174D RID: 5965 RVA: 0x00035D0D File Offset: 0x00033F0D
	VRRig IHandEffectsTrigger.Rig
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x0600174E RID: 5966 RVA: 0x00086279 File Offset: 0x00084479
	IHandEffectsTrigger.Mode IHandEffectsTrigger.EffectMode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x0600174F RID: 5967 RVA: 0x00086281 File Offset: 0x00084481
	bool IHandEffectsTrigger.FingersDown
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.FistBump || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001750 RID: 5968 RVA: 0x00086298 File Offset: 0x00084498
	bool IHandEffectsTrigger.FingersUp
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.HighFive || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06001751 RID: 5969 RVA: 0x000862AE File Offset: 0x000844AE
	// (set) Token: 0x06001752 RID: 5970 RVA: 0x000862B6 File Offset: 0x000844B6
	public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001753 RID: 5971 RVA: 0x000862BF File Offset: 0x000844BF
	public bool RightHand { get; }

	// Token: 0x06001754 RID: 5972 RVA: 0x000862C7 File Offset: 0x000844C7
	private void Awake()
	{
		this.triggerZone = base.GetComponent<Collider>();
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x000862D5 File Offset: 0x000844D5
	private void OnEnable()
	{
		if (!HandEffectsTriggerRegistry.HasInstance)
		{
			HandEffectsTriggerRegistry.FindInstance();
		}
		HandEffectsTriggerRegistry.Instance.Register(this);
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x000862EE File Offset: 0x000844EE
	private void OnDisable()
	{
		HandEffectsTriggerRegistry.Instance.Unregister(this);
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06001757 RID: 5975 RVA: 0x000862FB File Offset: 0x000844FB
	Vector3 IHandEffectsTrigger.Velocity
	{
		get
		{
			if (this.mode == IHandEffectsTrigger.Mode.HighFive)
			{
				return Vector3.zero;
			}
			IHandEffectsTrigger.Mode mode = this.mode;
			return Vector3.zero;
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06001758 RID: 5976 RVA: 0x00086319 File Offset: 0x00084519
	TagEffectPack IHandEffectsTrigger.CosmeticEffectPack
	{
		get
		{
			return this.cosmeticEffectPack;
		}
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnTriggerEntered(IHandEffectsTrigger other)
	{
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x00086324 File Offset: 0x00084524
	public bool InTriggerZone(IHandEffectsTrigger t)
	{
		if (!(base.transform.position - t.Transform.position).IsShorterThan(this.triggerZone.bounds.size))
		{
			return false;
		}
		RaycastHit raycastHit;
		switch (this.mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			return t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.FistBump:
			return t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.HighFive_And_FistBump:
			return (t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius)) || (t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius));
		}
		return this.triggerZone.Raycast(new Ray(t.Transform.position, this.triggerZone.bounds.center - t.Transform.position), out raycastHit, this.triggerRadius);
	}

	// Token: 0x0400228C RID: 8844
	[SerializeField]
	private TagEffectPack cosmeticEffectPack;

	// Token: 0x0400228D RID: 8845
	private Collider triggerZone;

	// Token: 0x0400228E RID: 8846
	public IHandEffectsTrigger.Mode mode;

	// Token: 0x0400228F RID: 8847
	[SerializeField]
	private float triggerRadius = 0.07f;

	// Token: 0x04002290 RID: 8848
	[SerializeField]
	private bool isStatic = true;
}
