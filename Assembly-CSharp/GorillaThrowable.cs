using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000A25 RID: 2597
public class GorillaThrowable : MonoBehaviourPun, IPunObservable, IPhotonViewCallback
{
	// Token: 0x06004270 RID: 17008 RVA: 0x00162CF8 File Offset: 0x00160EF8
	public virtual void Start()
	{
		this.offset = Vector3.zero;
		this.headsetTransform = GTPlayer.Instance.headCollider.transform;
		this.velocityHistory = new Vector3[this.trackingHistorySize];
		this.positionHistory = new Vector3[this.trackingHistorySize];
		this.headsetPositionHistory = new Vector3[this.trackingHistorySize];
		this.rotationHistory = new Vector3[this.trackingHistorySize];
		this.rotationalVelocityHistory = new Vector3[this.trackingHistorySize];
		for (int i = 0; i < this.trackingHistorySize; i++)
		{
			this.velocityHistory[i] = Vector3.zero;
			this.positionHistory[i] = base.transform.position - this.headsetTransform.position;
			this.headsetPositionHistory[i] = this.headsetTransform.position;
			this.rotationHistory[i] = base.transform.eulerAngles;
			this.rotationalVelocityHistory[i] = Vector3.zero;
		}
		this.currentIndex = 0;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
	}

	// Token: 0x06004271 RID: 17009 RVA: 0x00162E18 File Offset: 0x00161018
	public virtual void LateUpdate()
	{
		if (this.isHeld && base.photonView.IsMine)
		{
			base.transform.rotation = this.transformToFollow.rotation * this.offsetRotation;
			if (!this.initialLerp && (base.transform.position - this.transformToFollow.position).magnitude > this.lerpDistanceLimit)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.transformToFollow.position + this.transformToFollow.rotation * this.offset, this.pickupLerp);
			}
			else
			{
				this.initialLerp = true;
				base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
			}
		}
		if (!base.photonView.IsMine)
		{
			this.rigidbody.isKinematic = true;
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.lerpValue);
		}
		this.StoreHistories();
	}

	// Token: 0x06004272 RID: 17010 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void IsHandPushing(XRNode node)
	{
	}

	// Token: 0x06004273 RID: 17011 RVA: 0x00162F84 File Offset: 0x00161184
	private void StoreHistories()
	{
		this.previousPosition = this.positionHistory[this.currentIndex];
		this.previousRotation = this.rotationHistory[this.currentIndex];
		this.previousHeadsetPosition = this.headsetPositionHistory[this.currentIndex];
		this.currentIndex = (this.currentIndex + 1) % this.trackingHistorySize;
		this.currentVelocity = (base.transform.position - this.headsetTransform.position - this.previousPosition) / Time.deltaTime;
		this.currentHeadsetVelocity = (this.headsetTransform.position - this.previousHeadsetPosition) / Time.deltaTime;
		this.currentRotationalVelocity = (base.transform.eulerAngles - this.previousRotation) / Time.deltaTime;
		this.denormalizedVelocityAverage = Vector3.zero;
		this.denormalizedRotationalVelocityAverage = Vector3.zero;
		this.loopIndex = 0;
		while (this.loopIndex < this.trackingHistorySize)
		{
			this.denormalizedVelocityAverage += this.velocityHistory[this.loopIndex];
			this.denormalizedRotationalVelocityAverage += this.rotationalVelocityHistory[this.loopIndex];
			this.loopIndex++;
		}
		this.denormalizedVelocityAverage /= (float)this.trackingHistorySize;
		this.denormalizedRotationalVelocityAverage /= (float)this.trackingHistorySize;
		this.velocityHistory[this.currentIndex] = this.currentVelocity;
		this.positionHistory[this.currentIndex] = base.transform.position - this.headsetTransform.position;
		this.headsetPositionHistory[this.currentIndex] = this.headsetTransform.position;
		this.rotationHistory[this.currentIndex] = base.transform.eulerAngles;
		this.rotationalVelocityHistory[this.currentIndex] = this.currentRotationalVelocity;
	}

	// Token: 0x06004274 RID: 17012 RVA: 0x001631B0 File Offset: 0x001613B0
	public virtual void Grabbed(Transform grabTransform)
	{
		this.grabbingTransform = grabTransform;
		this.isHeld = true;
		this.transformToFollow = this.grabbingTransform;
		this.offsetRotation = base.transform.rotation * Quaternion.Inverse(this.transformToFollow.rotation);
		this.initialLerp = false;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		base.photonView.RequestOwnership();
	}

	// Token: 0x06004275 RID: 17013 RVA: 0x00163228 File Offset: 0x00161428
	public virtual void ThrowThisThingo()
	{
		this.transformToFollow = null;
		this.isHeld = false;
		this.synchThrow = true;
		this.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		this.rigidbody.isKinematic = false;
		this.rigidbody.useGravity = true;
		if (this.isLinear || this.denormalizedVelocityAverage.magnitude < this.linearMax)
		{
			if (this.denormalizedVelocityAverage.magnitude * this.throwMultiplier < this.throwMagnitudeLimit)
			{
				this.rigidbody.linearVelocity = this.denormalizedVelocityAverage * this.throwMultiplier + this.currentHeadsetVelocity;
			}
			else
			{
				this.rigidbody.linearVelocity = this.denormalizedVelocityAverage.normalized * this.throwMagnitudeLimit + this.currentHeadsetVelocity;
			}
		}
		else
		{
			this.rigidbody.linearVelocity = this.denormalizedVelocityAverage.normalized * Mathf.Max(Mathf.Min(Mathf.Pow(this.throwMultiplier * this.denormalizedVelocityAverage.magnitude / this.linearMax, this.exponThrowMultMax), 0.1f) * this.denormalizedHeadsetVelocityAverage.magnitude, this.throwMagnitudeLimit) + this.currentHeadsetVelocity;
		}
		this.rigidbody.angularVelocity = this.denormalizedRotationalVelocityAverage * 3.1415927f / 180f;
		this.rigidbody.MovePosition(this.rigidbody.transform.position + this.rigidbody.linearVelocity * Time.deltaTime);
	}

	// Token: 0x06004276 RID: 17014 RVA: 0x001633C4 File Offset: 0x001615C4
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.rigidbody.linearVelocity);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.targetPosition.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.targetRotation.SetValueSafe(quaternion);
		Vector3 linearVelocity = this.rigidbody.linearVelocity;
		vector = (Vector3)stream.ReceiveNext();
		ref linearVelocity.SetValueSafe(vector);
		this.rigidbody.linearVelocity = linearVelocity;
	}

	// Token: 0x06004277 RID: 17015 RVA: 0x0016347C File Offset: 0x0016167C
	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<GorillaSurfaceOverride>() != null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("PlaySurfaceHit", RpcTarget.Others, new object[]
				{
					this.bounceAudioClip,
					this.InterpolateVolume()
				});
			}
			this.PlaySurfaceHit(collision.collider.GetComponent<GorillaSurfaceOverride>().overrideIndex, this.InterpolateVolume());
		}
	}

	// Token: 0x06004278 RID: 17016 RVA: 0x001634F8 File Offset: 0x001616F8
	public void PlaySurfaceHit(int soundIndex, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			this.audioSource.volume = tapVolume;
			this.audioSource.clip = (GTPlayer.Instance.materialData[soundIndex].overrideAudio ? GTPlayer.Instance.materialData[soundIndex].audio : GTPlayer.Instance.materialData[0].audio);
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
		}
	}

	// Token: 0x06004279 RID: 17017 RVA: 0x00163594 File Offset: 0x00161794
	public float InterpolateVolume()
	{
		return (Mathf.Clamp(this.rigidbody.linearVelocity.magnitude, this.minVelocity, this.maxVelocity) - this.minVelocity) / (this.maxVelocity - this.minVelocity) * (this.maxVolume - this.minVolume) + this.minVolume;
	}

	// Token: 0x04005440 RID: 21568
	public int trackingHistorySize;

	// Token: 0x04005441 RID: 21569
	public float throwMultiplier;

	// Token: 0x04005442 RID: 21570
	public float throwMagnitudeLimit;

	// Token: 0x04005443 RID: 21571
	private Vector3[] velocityHistory;

	// Token: 0x04005444 RID: 21572
	private Vector3[] headsetVelocityHistory;

	// Token: 0x04005445 RID: 21573
	private Vector3[] positionHistory;

	// Token: 0x04005446 RID: 21574
	private Vector3[] headsetPositionHistory;

	// Token: 0x04005447 RID: 21575
	private Vector3[] rotationHistory;

	// Token: 0x04005448 RID: 21576
	private Vector3[] rotationalVelocityHistory;

	// Token: 0x04005449 RID: 21577
	private Vector3 previousPosition;

	// Token: 0x0400544A RID: 21578
	private Vector3 previousRotation;

	// Token: 0x0400544B RID: 21579
	private Vector3 previousHeadsetPosition;

	// Token: 0x0400544C RID: 21580
	private int currentIndex;

	// Token: 0x0400544D RID: 21581
	private Vector3 currentVelocity;

	// Token: 0x0400544E RID: 21582
	private Vector3 currentHeadsetVelocity;

	// Token: 0x0400544F RID: 21583
	private Vector3 currentRotationalVelocity;

	// Token: 0x04005450 RID: 21584
	public Vector3 denormalizedVelocityAverage;

	// Token: 0x04005451 RID: 21585
	private Vector3 denormalizedHeadsetVelocityAverage;

	// Token: 0x04005452 RID: 21586
	private Vector3 denormalizedRotationalVelocityAverage;

	// Token: 0x04005453 RID: 21587
	private Transform headsetTransform;

	// Token: 0x04005454 RID: 21588
	private Vector3 targetPosition;

	// Token: 0x04005455 RID: 21589
	private Quaternion targetRotation;

	// Token: 0x04005456 RID: 21590
	public bool initialLerp;

	// Token: 0x04005457 RID: 21591
	public float lerpValue = 0.4f;

	// Token: 0x04005458 RID: 21592
	public float lerpDistanceLimit = 0.01f;

	// Token: 0x04005459 RID: 21593
	public bool isHeld;

	// Token: 0x0400545A RID: 21594
	public Rigidbody rigidbody;

	// Token: 0x0400545B RID: 21595
	private int loopIndex;

	// Token: 0x0400545C RID: 21596
	private Transform transformToFollow;

	// Token: 0x0400545D RID: 21597
	private Vector3 offset;

	// Token: 0x0400545E RID: 21598
	private Quaternion offsetRotation;

	// Token: 0x0400545F RID: 21599
	public AudioSource audioSource;

	// Token: 0x04005460 RID: 21600
	public int timeLastReceived;

	// Token: 0x04005461 RID: 21601
	public bool synchThrow;

	// Token: 0x04005462 RID: 21602
	public float tempFloat;

	// Token: 0x04005463 RID: 21603
	public Transform grabbingTransform;

	// Token: 0x04005464 RID: 21604
	public float pickupLerp;

	// Token: 0x04005465 RID: 21605
	public float minVelocity;

	// Token: 0x04005466 RID: 21606
	public float maxVelocity;

	// Token: 0x04005467 RID: 21607
	public float minVolume;

	// Token: 0x04005468 RID: 21608
	public float maxVolume;

	// Token: 0x04005469 RID: 21609
	public bool isLinear;

	// Token: 0x0400546A RID: 21610
	public float linearMax;

	// Token: 0x0400546B RID: 21611
	public float exponThrowMultMax;

	// Token: 0x0400546C RID: 21612
	public int bounceAudioClip;
}
