using System;
using GorillaExtensions;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000C5D RID: 3165
public class ForceVolume : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004E2F RID: 20015 RVA: 0x0019F60C File Offset: 0x0019D80C
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x06004E30 RID: 20016 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06004E31 RID: 20017 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06004E32 RID: 20018 RVA: 0x0019F624 File Offset: 0x0019D824
	public void SliceUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x06004E33 RID: 20019 RVA: 0x0019F674 File Offset: 0x0019D874
	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		return rb != null && xf != null;
	}

	// Token: 0x06004E34 RID: 20020 RVA: 0x0019F6D4 File Offset: 0x0019D8D4
	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.enterClip == null)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.enterClip, 1f);
			this.audioState = ForceVolume.AudioState.Enter;
		}
		this.enterPos = transform.position;
	}

	// Token: 0x06004E35 RID: 20021 RVA: 0x0019F744 File Offset: 0x0019D944
	public void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			this.audioState = ForceVolume.AudioState.None;
		}
	}

	// Token: 0x06004E36 RID: 20022 RVA: 0x0019F79C File Offset: 0x0019D99C
	public void OnTriggerStay(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource && !this.audioSource.isPlaying)
		{
			ForceVolume.AudioState audioState = this.audioState;
			if (audioState != ForceVolume.AudioState.Enter)
			{
				if (audioState == ForceVolume.AudioState.Loop)
				{
					if (this.loopClip != null)
					{
						this.audioSource.enabled = true;
						this.audioSource.GTPlayOneShot(this.loopClip, 1f);
					}
					this.audioState = ForceVolume.AudioState.Loop;
				}
			}
			else
			{
				if (this.loopCresendoClip != null)
				{
					this.audioSource.enabled = true;
					this.audioSource.GTPlayOneShot(this.loopCresendoClip, 1f);
				}
				this.audioState = ForceVolume.AudioState.Crescendo;
			}
		}
		if (this.disableGrip)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		VRRig.LocalRig.BreakHandLinks();
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null)
		{
			Vector3 up = base.transform.up;
			localPlayer.PlayerKnockback(up * 0.1f, false, false);
		}
		SizeManager sizeManager = null;
		if (this.scaleWithSize)
		{
			sizeManager = rigidbody.GetComponent<SizeManager>();
		}
		Vector3 vector = rigidbody.linearVelocity;
		if (this.scaleWithSize && sizeManager)
		{
			vector /= sizeManager.currentScale;
		}
		Vector3 b = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 a = base.transform.position + b - transform.position;
		float num = a.magnitude + 0.0001f;
		Vector3 vector2 = a / num;
		float num2 = Vector3.Dot(vector, vector2);
		float d = this.accel;
		if (this.maxDepth > -1f)
		{
			float num3 = Vector3.Dot(transform.position - this.enterPos, vector2);
			float num4 = this.maxDepth - num3;
			float b2 = 0f;
			if (num4 > 0.0001f)
			{
				b2 = num2 * num2 / num4;
			}
			d = Mathf.Max(this.accel, b2);
		}
		float deltaTime = Time.deltaTime;
		Vector3 b3 = base.transform.up * d * deltaTime;
		vector += b3;
		Vector3 a2 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
		Vector3 a3 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
		Vector3 a4 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
		float d2 = 1f;
		float d3 = 1f;
		if (this.dampenLateralVelocity)
		{
			d2 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
			d3 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
		}
		vector = a2 + d2 * a3 + d3 * a4;
		if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
		{
			vector -= num2 * vector2;
			if (num > this.pullTOCenterMinDistance)
			{
				num2 += this.pullToCenterAccel * deltaTime;
				float b4 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, b4);
			}
			else
			{
				num2 = 0f;
			}
			vector += num2 * vector2;
			if (vector.magnitude > 0.0001f)
			{
				Vector3 vector3 = Vector3.Cross(base.transform.up, vector2);
				float magnitude = vector3.magnitude;
				if (magnitude > 0.0001f)
				{
					vector3 /= magnitude;
					num2 = Vector3.Dot(vector, vector3);
					vector -= num2 * vector3;
					num2 -= this.pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					vector += num2 * vector3;
				}
			}
		}
		if (this.scaleWithSize && sizeManager)
		{
			vector *= sizeManager.currentScale;
		}
		rigidbody.linearVelocity = vector;
	}

	// Token: 0x06004E37 RID: 20023 RVA: 0x0019FC04 File Offset: 0x0019DE04
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x06004E38 RID: 20024 RVA: 0x0019FC74 File Offset: 0x0019DE74
	public void SetPropertiesFromPlaceholder(ForceVolumeProperties properties, AudioSource volumeAudioSource, Collider colliderVolume)
	{
		this.accel = properties.accel;
		this.maxDepth = properties.maxDepth;
		this.maxSpeed = properties.maxSpeed;
		this.disableGrip = properties.disableGrip;
		this.dampenLateralVelocity = properties.dampenLateralVelocity;
		this.dampenXVelPerc = properties.dampenXVel;
		this.dampenZVelPerc = properties.dampenZVel;
		this.applyPullToCenterAcceleration = properties.applyPullToCenterAcceleration;
		this.pullToCenterAccel = properties.pullToCenterAccel;
		this.pullToCenterMaxSpeed = properties.pullToCenterMaxSpeed;
		this.pullTOCenterMinDistance = properties.pullToCenterMinDistance;
		this.enterClip = properties.enterClip;
		this.exitClip = properties.exitClip;
		this.loopClip = properties.loopClip;
		this.loopCresendoClip = properties.loopCrescendoClip;
		if (volumeAudioSource.IsNotNull())
		{
			this.audioSource = volumeAudioSource;
		}
		if (colliderVolume.IsNotNull())
		{
			this.volume = colliderVolume;
		}
	}

	// Token: 0x04006027 RID: 24615
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04006028 RID: 24616
	[SerializeField]
	private float accel;

	// Token: 0x04006029 RID: 24617
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x0400602A RID: 24618
	[SerializeField]
	private float maxSpeed;

	// Token: 0x0400602B RID: 24619
	[SerializeField]
	private bool disableGrip;

	// Token: 0x0400602C RID: 24620
	[SerializeField]
	private bool dampenLateralVelocity = true;

	// Token: 0x0400602D RID: 24621
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x0400602E RID: 24622
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x0400602F RID: 24623
	[SerializeField]
	private bool applyPullToCenterAcceleration = true;

	// Token: 0x04006030 RID: 24624
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x04006031 RID: 24625
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x04006032 RID: 24626
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x04006033 RID: 24627
	private Collider volume;

	// Token: 0x04006034 RID: 24628
	public AudioClip enterClip;

	// Token: 0x04006035 RID: 24629
	public AudioClip exitClip;

	// Token: 0x04006036 RID: 24630
	public AudioClip loopClip;

	// Token: 0x04006037 RID: 24631
	public AudioClip loopCresendoClip;

	// Token: 0x04006038 RID: 24632
	public AudioSource audioSource;

	// Token: 0x04006039 RID: 24633
	private Vector3 enterPos;

	// Token: 0x0400603A RID: 24634
	private ForceVolume.AudioState audioState;

	// Token: 0x02000C5E RID: 3166
	private enum AudioState
	{
		// Token: 0x0400603C RID: 24636
		None,
		// Token: 0x0400603D RID: 24637
		Enter,
		// Token: 0x0400603E RID: 24638
		Crescendo,
		// Token: 0x0400603F RID: 24639
		Loop,
		// Token: 0x04006040 RID: 24640
		Exit
	}
}
