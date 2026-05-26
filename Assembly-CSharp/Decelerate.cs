using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200067C RID: 1660
public class Decelerate : MonoBehaviour
{
	// Token: 0x06002976 RID: 10614 RVA: 0x000DFC5C File Offset: 0x000DDE5C
	public void Restart()
	{
		base.enabled = true;
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000DFC68 File Offset: 0x000DDE68
	private void Update()
	{
		if (!this._rigidbody)
		{
			return;
		}
		Vector3 vector = this._rigidbody.linearVelocity;
		vector *= this._friction;
		if (vector.Approx0(0.001f))
		{
			this._rigidbody.linearVelocity = Vector3.zero;
			UnityEvent unityEvent = this.onStop;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			base.enabled = false;
		}
		else
		{
			this._rigidbody.linearVelocity = vector;
		}
		if (this._resetOrientationOnRelease && !this._rigidbody.rotation.Approx(Quaternion.identity, 1E-06f))
		{
			this._rigidbody.rotation = Quaternion.identity;
		}
	}

	// Token: 0x040035EE RID: 13806
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040035EF RID: 13807
	[SerializeField]
	private float _friction = 0.875f;

	// Token: 0x040035F0 RID: 13808
	[SerializeField]
	private bool _resetOrientationOnRelease;

	// Token: 0x040035F1 RID: 13809
	public UnityEvent onStop;
}
