using System;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000903 RID: 2307
public class GorillaGrabber : MonoBehaviour
{
	// Token: 0x17000568 RID: 1384
	// (get) Token: 0x06003C32 RID: 15410 RVA: 0x00148BA4 File Offset: 0x00146DA4
	public bool isGrabbing
	{
		get
		{
			return this.currentGrabbable != null;
		}
	}

	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x06003C33 RID: 15411 RVA: 0x00148BAF File Offset: 0x00146DAF
	public XRNode XrNode
	{
		get
		{
			return this.xrNode;
		}
	}

	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x06003C34 RID: 15412 RVA: 0x00148BB7 File Offset: 0x00146DB7
	public bool IsLeftHand
	{
		get
		{
			return this.XrNode == XRNode.LeftHand;
		}
	}

	// Token: 0x1700056B RID: 1387
	// (get) Token: 0x06003C35 RID: 15413 RVA: 0x00148BC2 File Offset: 0x00146DC2
	public bool IsRightHand
	{
		get
		{
			return this.XrNode == XRNode.RightHand;
		}
	}

	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x06003C36 RID: 15414 RVA: 0x00148BCD File Offset: 0x00146DCD
	public GTPlayer Player
	{
		get
		{
			return this.player;
		}
	}

	// Token: 0x06003C37 RID: 15415 RVA: 0x00148BD8 File Offset: 0x00146DD8
	private void Start()
	{
		this.hapticStrengthActual = this.hapticStrength;
		this.audioSource = base.GetComponent<AudioSource>();
		this.player = base.GetComponentInParent<GTPlayer>();
		if (!this.player)
		{
			Debug.LogWarning("Gorilla Grabber Component has no player in hierarchy. Disabling this Gorilla Grabber");
			base.GetComponent<GorillaGrabber>().enabled = false;
		}
	}

	// Token: 0x06003C38 RID: 15416 RVA: 0x00148C2C File Offset: 0x00146E2C
	public void CheckGrabber(bool initiateGrab)
	{
		bool grabMomentary = ControllerInputPoller.GetGrabMomentary(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (this.currentGrabbable != null && (grabRelease || this.GrabDistanceOverCheck()))
		{
			this.Ungrab(null);
		}
		if (grabMomentary)
		{
			this.grabTimeStamp = Time.time;
		}
		if (initiateGrab && this.currentGrabbable == null)
		{
			this.currentGrabbable = this.TryGrab(Time.time - this.grabTimeStamp < this.coyoteTimeDuration);
		}
		if (this.currentGrabbable != null && this.hapticStrengthActual > 0f)
		{
			GorillaTagger.Instance.DoVibration(this.xrNode, this.hapticStrengthActual, Time.deltaTime);
			this.hapticStrengthActual -= this.hapticDecay * Time.deltaTime;
		}
	}

	// Token: 0x06003C39 RID: 15417 RVA: 0x00148CEB File Offset: 0x00146EEB
	private bool GrabDistanceOverCheck()
	{
		return this.currentGrabbedTransform == null || Vector3.Distance(base.transform.position, this.currentGrabbedTransform.TransformPoint(this.localGrabbedPosition)) > this.breakDistance;
	}

	// Token: 0x06003C3A RID: 15418 RVA: 0x00148D28 File Offset: 0x00146F28
	internal void Ungrab(IGorillaGrabable specificGrabbable = null)
	{
		if (specificGrabbable != null && specificGrabbable != this.currentGrabbable)
		{
			return;
		}
		this.currentGrabbable.OnGrabReleased(this);
		PlayerGameEvents.DroppedObject(this.currentGrabbable.name);
		this.currentGrabbable = null;
		this.gripEffects.Stop();
		this.hapticStrengthActual = this.hapticStrength;
	}

	// Token: 0x06003C3B RID: 15419 RVA: 0x00148D7C File Offset: 0x00146F7C
	private IGorillaGrabable TryGrab(bool momentary)
	{
		IGorillaGrabable gorillaGrabable = null;
		Debug.DrawRay(base.transform.position, base.transform.forward * (this.grabRadius * this.player.scale), Color.blue, 1f);
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius * this.player.scale, this.grabCastResults);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IGorillaGrabable gorillaGrabable2;
			if (this.grabCastResults[i].TryGetComponent<IGorillaGrabable>(out gorillaGrabable2))
			{
				float num3 = Vector3.Distance(base.transform.position, this.FindClosestPoint(this.grabCastResults[i], base.transform.position));
				if (num3 < num2)
				{
					num2 = num3;
					gorillaGrabable = gorillaGrabable2;
				}
			}
		}
		if (gorillaGrabable != null && (!gorillaGrabable.MomentaryGrabOnly() || momentary) && gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable.OnGrabbed(this, out this.currentGrabbedTransform, out this.localGrabbedPosition);
			PlayerGameEvents.GrabbedObject(gorillaGrabable.name);
		}
		if (gorillaGrabable != null && !gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable = null;
		}
		return gorillaGrabable;
	}

	// Token: 0x06003C3C RID: 15420 RVA: 0x00148E8F File Offset: 0x0014708F
	private Vector3 FindClosestPoint(Collider collider, Vector3 position)
	{
		if (collider is MeshCollider && !(collider as MeshCollider).convex)
		{
			return position;
		}
		return collider.ClosestPoint(position);
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x00148EB0 File Offset: 0x001470B0
	public void Inject(Transform currentGrabbableTransform, Vector3 localGrabbedPosition)
	{
		if (this.currentGrabbable != null)
		{
			this.Ungrab(null);
		}
		if (currentGrabbableTransform != null)
		{
			this.currentGrabbable = currentGrabbableTransform.GetComponent<IGorillaGrabable>();
			this.currentGrabbedTransform = currentGrabbableTransform;
			this.localGrabbedPosition = localGrabbedPosition;
			this.currentGrabbable.OnGrabbed(this, out this.currentGrabbedTransform, out localGrabbedPosition);
		}
	}

	// Token: 0x04004CCE RID: 19662
	private GTPlayer player;

	// Token: 0x04004CCF RID: 19663
	[SerializeField]
	private XRNode xrNode = XRNode.LeftHand;

	// Token: 0x04004CD0 RID: 19664
	private AudioSource audioSource;

	// Token: 0x04004CD1 RID: 19665
	private Transform currentGrabbedTransform;

	// Token: 0x04004CD2 RID: 19666
	private Vector3 localGrabbedPosition;

	// Token: 0x04004CD3 RID: 19667
	private IGorillaGrabable currentGrabbable;

	// Token: 0x04004CD4 RID: 19668
	[SerializeField]
	private float grabRadius = 0.015f;

	// Token: 0x04004CD5 RID: 19669
	[SerializeField]
	private float breakDistance = 0.3f;

	// Token: 0x04004CD6 RID: 19670
	[SerializeField]
	private float hapticStrength = 0.2f;

	// Token: 0x04004CD7 RID: 19671
	private float hapticStrengthActual = 0.2f;

	// Token: 0x04004CD8 RID: 19672
	[SerializeField]
	private float hapticDecay;

	// Token: 0x04004CD9 RID: 19673
	[SerializeField]
	private ParticleSystem gripEffects;

	// Token: 0x04004CDA RID: 19674
	private Collider[] grabCastResults = new Collider[32];

	// Token: 0x04004CDB RID: 19675
	private float grabTimeStamp;

	// Token: 0x04004CDC RID: 19676
	[SerializeField]
	private float coyoteTimeDuration = 0.25f;
}
