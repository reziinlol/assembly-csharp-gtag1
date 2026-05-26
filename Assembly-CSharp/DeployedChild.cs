using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class DeployedChild : MonoBehaviour
{
	// Token: 0x06001205 RID: 4613 RVA: 0x00060AE4 File Offset: 0x0005ECE4
	public void Deploy(DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this._parent = parent;
		this._parent.DeployChild();
		Transform transform = base.transform;
		transform.position = launchPos;
		transform.rotation = launchRot;
		transform.localScale = this._parent.transform.lossyScale;
		this._rigidbody.linearVelocity = releaseVel;
		this._isRemote = isRemote;
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x00060B41 File Offset: 0x0005ED41
	public void ReturnToParent(float delay)
	{
		if (delay > 0f)
		{
			base.StartCoroutine(this.ReturnToParentDelayed(delay));
			return;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00060B73 File Offset: 0x0005ED73
	private IEnumerator ReturnToParentDelayed(float delay)
	{
		float start = Time.time;
		while (Time.time < start + delay)
		{
			yield return null;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
		yield break;
	}

	// Token: 0x040015CB RID: 5579
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040015CC RID: 5580
	[SerializeReference]
	private DeployableObject _parent;

	// Token: 0x040015CD RID: 5581
	private bool _isRemote;
}
