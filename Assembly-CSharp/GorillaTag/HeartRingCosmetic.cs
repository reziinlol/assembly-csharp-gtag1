using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001130 RID: 4400
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x06006FC1 RID: 28609 RVA: 0x00247F52 File Offset: 0x00246152
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				base.enabled = false;
			};
		}

		// Token: 0x06006FC2 RID: 28610 RVA: 0x00247F68 File Offset: 0x00246168
		protected void OnEnable()
		{
			this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
			this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
			this.ownerRig = base.GetComponentInParent<VRRig>();
			bool flag = this.ownerRig != null && this.ownerRig.head != null && this.ownerRig.head.rigTarget != null;
			base.enabled = flag;
			this.effects.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + base.transform.GetPath(), this);
				return;
			}
			this.ownerHead = ((this.ownerRig != null) ? this.ownerRig.head.rigTarget.transform : base.transform);
			this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
			this.maxVolume = this.audioSource.volume;
		}

		// Token: 0x06006FC3 RID: 28611 RVA: 0x00248070 File Offset: 0x00246270
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
			bool flag = (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < num;
			ParticleSystem.EmissionModule emission = this.particleSystem.emission;
			emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0f, Time.deltaTime / 0.1f);
			this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0f, Time.deltaTime / 2f);
			this.ownerRig.UsingHauntedRing = (this.isHauntedVoiceChanger && flag);
			if (this.ownerRig.UsingHauntedRing)
			{
				this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
			}
		}

		// Token: 0x04007FBC RID: 32700
		public GameObject effects;

		// Token: 0x04007FBD RID: 32701
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x04007FBE RID: 32702
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x04007FBF RID: 32703
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x04007FC0 RID: 32704
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04007FC1 RID: 32705
		private VRRig ownerRig;

		// Token: 0x04007FC2 RID: 32706
		private Transform ownerHead;

		// Token: 0x04007FC3 RID: 32707
		private ParticleSystem particleSystem;

		// Token: 0x04007FC4 RID: 32708
		private AudioSource audioSource;

		// Token: 0x04007FC5 RID: 32709
		private float maxEmissionRate;

		// Token: 0x04007FC6 RID: 32710
		private float maxVolume;

		// Token: 0x04007FC7 RID: 32711
		private const float emissionFadeTime = 0.1f;

		// Token: 0x04007FC8 RID: 32712
		private const float volumeFadeTime = 2f;
	}
}
