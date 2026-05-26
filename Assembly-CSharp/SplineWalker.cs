using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D87 RID: 3463
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x060054F4 RID: 21748 RVA: 0x001BC2E0 File Offset: 0x001BA4E0
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060054F5 RID: 21749 RVA: 0x001BC2F0 File Offset: 0x001BA4F0
	private void Update()
	{
		if (this.goingForward)
		{
			this.progress += Time.deltaTime / this.duration;
			if (this.progress > 1f)
			{
				if (this.mode == SplineWalkerMode.Once)
				{
					this.progress = 1f;
				}
				else if (this.mode == SplineWalkerMode.Loop)
				{
					this.progress -= 1f;
				}
				else
				{
					this.progress = 2f - this.progress;
					this.goingForward = false;
				}
			}
		}
		else
		{
			this.progress -= Time.deltaTime / this.duration;
			if (this.progress < 0f)
			{
				this.progress = -this.progress;
				this.goingForward = true;
			}
		}
		if (this.linearSpline != null && this.walkLinearPath)
		{
			Vector3 vector = this.linearSpline.Evaluate(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = vector;
			}
			else
			{
				base.transform.localPosition = vector;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(vector + this.linearSpline.GetForwardTangent(this.progress, 0.01f));
				return;
			}
		}
		else if (this.spline != null)
		{
			Vector3 point = this.spline.GetPoint(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = point;
			}
			else
			{
				base.transform.localPosition = point;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(point + this.spline.GetDirection(this.progress));
			}
		}
	}

	// Token: 0x060054F6 RID: 21750 RVA: 0x001BC49E File Offset: 0x001BA69E
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x04006574 RID: 25972
	public BezierSpline spline;

	// Token: 0x04006575 RID: 25973
	public LinearSpline linearSpline;

	// Token: 0x04006576 RID: 25974
	public float duration;

	// Token: 0x04006577 RID: 25975
	public bool lookForward;

	// Token: 0x04006578 RID: 25976
	public SplineWalkerMode mode;

	// Token: 0x04006579 RID: 25977
	public bool walkLinearPath;

	// Token: 0x0400657A RID: 25978
	public bool useWorldPosition;

	// Token: 0x0400657B RID: 25979
	public float progress;

	// Token: 0x0400657C RID: 25980
	private bool goingForward = true;

	// Token: 0x0400657D RID: 25981
	public bool DoNetworkSync = true;

	// Token: 0x0400657E RID: 25982
	private PhotonView _view;
}
