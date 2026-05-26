using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Reactions
{
	// Token: 0x0200119A RID: 4506
	public class FireInstance : MonoBehaviour
	{
		// Token: 0x06007207 RID: 29191 RVA: 0x00251BB8 File Offset: 0x0024FDB8
		protected void Awake()
		{
			FireManager.Register(this);
		}

		// Token: 0x06007208 RID: 29192 RVA: 0x00251BC0 File Offset: 0x0024FDC0
		protected void OnDestroy()
		{
			FireManager.Unregister(this);
		}

		// Token: 0x06007209 RID: 29193 RVA: 0x00251BC8 File Offset: 0x0024FDC8
		protected void OnEnable()
		{
			FireManager.OnEnable(this);
		}

		// Token: 0x0600720A RID: 29194 RVA: 0x00251BD0 File Offset: 0x0024FDD0
		protected void OnDisable()
		{
			FireManager.OnDisable(this);
		}

		// Token: 0x0600720B RID: 29195 RVA: 0x00251BD8 File Offset: 0x0024FDD8
		protected void OnTriggerEnter(Collider other)
		{
			FireManager.OnTriggerEnter(this, other);
		}

		// Token: 0x040081AC RID: 33196
		[Header("Scene References")]
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[SerializeField]
		internal Collider _collider;

		// Token: 0x040081AD RID: 33197
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[FormerlySerializedAs("_thermalSourceVolume")]
		[SerializeField]
		internal ThermalSourceVolume _thermalVolume;

		// Token: 0x040081AE RID: 33198
		[SerializeField]
		internal ParticleSystem _particleSystem;

		// Token: 0x040081AF RID: 33199
		[FormerlySerializedAs("_audioSource")]
		[SerializeField]
		internal AudioSource _loopingAudioSource;

		// Token: 0x040081B0 RID: 33200
		[Tooltip("The emissive color will be darkened on the materials of these renderers as the fire is extinguished.")]
		[SerializeField]
		internal Renderer[] _emissiveRenderers;

		// Token: 0x040081B1 RID: 33201
		[Header("Asset References")]
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _extinguishSound;

		// Token: 0x040081B2 RID: 33202
		[SerializeField]
		internal float _extinguishSoundVolume = 1f;

		// Token: 0x040081B3 RID: 33203
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _igniteSound;

		// Token: 0x040081B4 RID: 33204
		[SerializeField]
		internal float _igniteSoundVolume = 1f;

		// Token: 0x040081B5 RID: 33205
		[Header("Values")]
		[SerializeField]
		internal bool _despawnOnExtinguish = true;

		// Token: 0x040081B6 RID: 33206
		[SerializeField]
		internal float _maxLifetime = 10f;

		// Token: 0x040081B7 RID: 33207
		[Tooltip("How long it should take to reheat to it's default temperature.")]
		[SerializeField]
		internal float _reheatSpeed = 1f;

		// Token: 0x040081B8 RID: 33208
		[Tooltip("If you completely extinguish the object, how long should it stay extinguished?")]
		[SerializeField]
		internal float _stayExtinguishedDuration = 1f;

		// Token: 0x040081B9 RID: 33209
		internal float _defaultTemperature;

		// Token: 0x040081BA RID: 33210
		internal float _timeSinceExtinguished;

		// Token: 0x040081BB RID: 33211
		internal float _timeSinceDyingStart;

		// Token: 0x040081BC RID: 33212
		internal float _timeAlive;

		// Token: 0x040081BD RID: 33213
		internal float _psDefaultEmissionRate;

		// Token: 0x040081BE RID: 33214
		internal ParticleSystem.EmissionModule _psEmissionModule;

		// Token: 0x040081BF RID: 33215
		internal Vector3Int _spatialGridPosition;

		// Token: 0x040081C0 RID: 33216
		internal bool _isDespawning;

		// Token: 0x040081C1 RID: 33217
		internal float _deathStateDuration;

		// Token: 0x040081C2 RID: 33218
		internal MaterialPropertyBlock[] _emiRenderers_matPropBlocks;

		// Token: 0x040081C3 RID: 33219
		internal Color[] _emiRenderers_defaultColors;
	}
}
