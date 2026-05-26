using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class DebugTestGrabber : MonoBehaviour
{
	// Token: 0x0600032B RID: 811 RVA: 0x00013255 File Offset: 0x00011455
	private void Awake()
	{
		if (this.grabber == null)
		{
			this.grabber = base.GetComponentInChildren<CrittersGrabber>();
		}
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00013274 File Offset: 0x00011474
	private void LateUpdate()
	{
		if (this.transformToFollow != null)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position;
		}
		if (this.grabber == null)
		{
			return;
		}
		if (!this.isGrabbing && this.setIsGrabbing)
		{
			this.setIsGrabbing = false;
			this.isGrabbing = true;
			this.remainingGrabDuration = this.grabDuration;
		}
		else if (this.isGrabbing && this.setRelease)
		{
			this.setRelease = false;
			this.isGrabbing = false;
			this.DoRelease();
		}
		if (this.isGrabbing && this.remainingGrabDuration > 0f)
		{
			this.remainingGrabDuration -= Time.deltaTime;
			this.DoGrab();
		}
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00013348 File Offset: 0x00011548
	private void DoGrab()
	{
		this.grabber.grabbing = true;
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius, this.colliders, LayerMask.GetMask(new string[]
		{
			"GorillaInteractable"
		}));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CrittersActor componentInParent = this.colliders[i].GetComponentInParent<CrittersActor>();
				if (!(componentInParent == null) && componentInParent.usesRB && componentInParent.CanBeGrabbed(this.grabber))
				{
					this.isHandGrabbingDisabled = true;
					if (componentInParent.equipmentStorable)
					{
						componentInParent.localCanStore = true;
					}
					componentInParent.GrabbedBy(this.grabber, false, default(Quaternion), default(Vector3), false);
					this.grabber.grabbedActors.Add(componentInParent);
					this.remainingGrabDuration = 0f;
					return;
				}
			}
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0001342C File Offset: 0x0001162C
	private void DoRelease()
	{
		this.grabber.grabbing = false;
		for (int i = this.grabber.grabbedActors.Count - 1; i >= 0; i--)
		{
			CrittersActor crittersActor = this.grabber.grabbedActors[i];
			crittersActor.Released(true, crittersActor.transform.rotation, crittersActor.transform.position, this.estimator.linearVelocity, default(Vector3));
			if (i < this.grabber.grabbedActors.Count)
			{
				this.grabber.grabbedActors.RemoveAt(i);
			}
		}
		if (this.isHandGrabbingDisabled)
		{
			this.isHandGrabbingDisabled = false;
		}
	}

	// Token: 0x040003C2 RID: 962
	public bool isGrabbing;

	// Token: 0x040003C3 RID: 963
	public bool setIsGrabbing;

	// Token: 0x040003C4 RID: 964
	public bool setRelease;

	// Token: 0x040003C5 RID: 965
	public Collider[] colliders = new Collider[50];

	// Token: 0x040003C6 RID: 966
	public bool isLeft;

	// Token: 0x040003C7 RID: 967
	public float grabRadius = 0.05f;

	// Token: 0x040003C8 RID: 968
	public Transform transformToFollow;

	// Token: 0x040003C9 RID: 969
	public GorillaVelocityEstimator estimator;

	// Token: 0x040003CA RID: 970
	public CrittersGrabber grabber;

	// Token: 0x040003CB RID: 971
	public CrittersActorGrabber otherHand;

	// Token: 0x040003CC RID: 972
	private bool isHandGrabbingDisabled;

	// Token: 0x040003CD RID: 973
	private float grabDuration = 0.3f;

	// Token: 0x040003CE RID: 974
	private float remainingGrabDuration;
}
