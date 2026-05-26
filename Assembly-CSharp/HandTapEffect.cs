using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002D9 RID: 729
public class HandTapEffect : MonoBehaviour
{
	// Token: 0x0600129B RID: 4763 RVA: 0x0006300C File Offset: 0x0006120C
	private void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		this.leftHandEffect.handContext = componentInParent.LeftHandEffect;
		this.rightHandEffect.handContext = componentInParent.RightHandEffect;
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00063042 File Offset: 0x00061242
	private void OnEnable()
	{
		this.leftHandEffect.OnEnable();
		this.rightHandEffect.OnEnable();
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x0006305A File Offset: 0x0006125A
	private void OnDisable()
	{
		this.leftHandEffect.OnDisable();
		this.rightHandEffect.OnDisable();
	}

	// Token: 0x040016B0 RID: 5808
	public HandTapEffect.HandTapEffectLeftRight leftHandEffect;

	// Token: 0x040016B1 RID: 5809
	public HandTapEffect.HandTapEffectLeftRight rightHandEffect;

	// Token: 0x020002DA RID: 730
	[Serializable]
	public class HandTapEffectDownUp
	{
		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x0600129F RID: 4767 RVA: 0x00063072 File Offset: 0x00061272
		public bool HasOverrides
		{
			get
			{
				return this.overrides.overrideSurfacePrefab || this.overrides.overrideGamemodePrefab || this.overrides.overrideSound;
			}
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x0006309C File Offset: 0x0006129C
		internal void OnTap(HandEffectContext handContext)
		{
			UnityEvent unityEvent = this.onTapUnityEvents;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			for (int i = 0; i < this.onTapBehaviours.Length; i++)
			{
				this.onTapBehaviours[i].OnTap(handContext);
			}
		}

		// Token: 0x040016B2 RID: 5810
		public HandTapBehaviour[] onTapBehaviours;

		// Token: 0x040016B3 RID: 5811
		public UnityEvent onTapUnityEvents;

		// Token: 0x040016B4 RID: 5812
		[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
		public HashWrapper onTapPrefabToSpawn;

		// Token: 0x040016B5 RID: 5813
		public HandTapOverrides overrides;
	}

	// Token: 0x020002DB RID: 731
	[Serializable]
	public class HandTapEffectLeftRight
	{
		// Token: 0x060012A2 RID: 4770 RVA: 0x000630DC File Offset: 0x000612DC
		public void OnEnable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = true;
			}
			if (this.downTapEffect.onTapPrefabToSpawn != -1)
			{
				this.handContext.AddFXPrefab(this.downTapEffect.onTapPrefabToSpawn);
			}
			if (this.downTapEffect.HasOverrides)
			{
				this.handContext.DownTapOverrides = this.downTapEffect.overrides;
			}
			if (this.upTapEffect.HasOverrides)
			{
				this.handContext.UpTapOverrides = this.upTapEffect.overrides;
			}
			this.handContext.handTapDown += this.downTapEffect.OnTap;
			this.handContext.handTapUp += this.upTapEffect.OnTap;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x000631AC File Offset: 0x000613AC
		public void OnDisable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = false;
			}
			if (this.downTapEffect.onTapPrefabToSpawn != -1)
			{
				this.handContext.RemoveFXPrefab(this.downTapEffect.onTapPrefabToSpawn);
			}
			if (this.downTapEffect.HasOverrides && this.handContext.DownTapOverrides == this.downTapEffect.overrides)
			{
				this.handContext.DownTapOverrides = null;
			}
			if (this.upTapEffect.HasOverrides && this.handContext.UpTapOverrides == this.upTapEffect.overrides)
			{
				this.handContext.UpTapOverrides = null;
			}
			this.handContext.handTapDown -= this.downTapEffect.OnTap;
			this.handContext.handTapUp -= this.upTapEffect.OnTap;
		}

		// Token: 0x040016B6 RID: 5814
		public bool separateUpTapCooldown;

		// Token: 0x040016B7 RID: 5815
		public HandTapEffect.HandTapEffectDownUp downTapEffect;

		// Token: 0x040016B8 RID: 5816
		public HandTapEffect.HandTapEffectDownUp upTapEffect;

		// Token: 0x040016B9 RID: 5817
		internal HandEffectContext handContext;
	}
}
