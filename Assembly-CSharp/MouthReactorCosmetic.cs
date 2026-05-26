using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000933 RID: 2355
public class MouthReactorCosmetic : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06003DBF RID: 15807 RVA: 0x0014E4F8 File Offset: 0x0014C6F8
	private void ResetReactorTransform()
	{
		if (this.reactorTransform == null)
		{
			this.reactorTransform = base.transform;
		}
	}

	// Token: 0x06003DC0 RID: 15808 RVA: 0x0014E514 File Offset: 0x0014C714
	private void ResetRadius()
	{
		this.reactorRadius = 0.1666667f;
	}

	// Token: 0x17000591 RID: 1425
	// (get) Token: 0x06003DC1 RID: 15809 RVA: 0x0014E521 File Offset: 0x0014C721
	private bool IsRadiusChanged
	{
		get
		{
			return this.reactorRadius != 0.1666667f;
		}
	}

	// Token: 0x06003DC2 RID: 15810 RVA: 0x0014E533 File Offset: 0x0014C733
	private void ResetOffset()
	{
		this.mouthOffset = MouthReactorCosmetic.DEFAULT_OFFSET;
	}

	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x06003DC3 RID: 15811 RVA: 0x0014E540 File Offset: 0x0014C740
	private bool IsOffsetChanged
	{
		get
		{
			return this.mouthOffset != MouthReactorCosmetic.DEFAULT_OFFSET;
		}
	}

	// Token: 0x06003DC4 RID: 15812 RVA: 0x0014E552 File Offset: 0x0014C752
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003DC5 RID: 15813 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x06003DC6 RID: 15814 RVA: 0x0014E56E File Offset: 0x0014C76E
	// (set) Token: 0x06003DC7 RID: 15815 RVA: 0x0014E576 File Offset: 0x0014C776
	public bool TickRunning { get; set; }

	// Token: 0x06003DC8 RID: 15816 RVA: 0x0014E580 File Offset: 0x0014C780
	public void Tick()
	{
		Vector3 b = this.myRig.head.rigTarget.TransformPoint(this.mouthOffset);
		float sqrMagnitude = (this.reactorTransform.TransformPoint(this.reactorOffset) - b).sqrMagnitude;
		if (sqrMagnitude < this.reactorRadius * this.reactorRadius)
		{
			if ((!this.mustExitBeforeRefire || !this.wasInside) && Time.time - this.lastInsideTime >= this.eventRefireDelay)
			{
				UnityEvent unityEvent = this.onInsideMouth;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this.lastInsideTime = Time.time;
			}
			this.wasInside = true;
		}
		else
		{
			this.wasInside = false;
		}
		if (this.continuousProperties.Count > 0)
		{
			this.continuousProperties.ApplyAll(Mathf.Min(0f, Mathf.Sqrt(sqrMagnitude) - this.reactorRadius));
		}
	}

	// Token: 0x04004E10 RID: 19984
	private static readonly Vector3 DEFAULT_OFFSET = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x04004E11 RID: 19985
	private const float DEFAULT_RADIUS = 0.1666667f;

	// Token: 0x04004E12 RID: 19986
	[Tooltip("The transform to check against the mouth's position. Defaults to the transform this script is attached to.")]
	public Transform reactorTransform;

	// Token: 0x04004E13 RID: 19987
	[Tooltip("Offset the relative position of the reactor transform.")]
	public Vector3 reactorOffset = Vector3.zero;

	// Token: 0x04004E14 RID: 19988
	[Tooltip("How close the reactor needs to be to the mouth to trigger the event.")]
	public float reactorRadius = 0.1666667f;

	// Token: 0x04004E15 RID: 19989
	[Tooltip("The continuous value is the distance to the mouth. When inside the mouth radius, the value will always be 0.")]
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04004E16 RID: 19990
	[Tooltip("After the event fires, it must wait this many seconds before it fires again.")]
	public float eventRefireDelay = 0.6f;

	// Token: 0x04004E17 RID: 19991
	[Tooltip("After the event fires, prevent firing again until the reactor transform is moved outside the mouth and then back in.")]
	public bool mustExitBeforeRefire = true;

	// Token: 0x04004E18 RID: 19992
	public UnityEvent onInsideMouth;

	// Token: 0x04004E19 RID: 19993
	public Vector3 mouthOffset = MouthReactorCosmetic.DEFAULT_OFFSET;

	// Token: 0x04004E1A RID: 19994
	private VRRig myRig;

	// Token: 0x04004E1B RID: 19995
	private float lastInsideTime;

	// Token: 0x04004E1C RID: 19996
	private bool wasInside;
}
