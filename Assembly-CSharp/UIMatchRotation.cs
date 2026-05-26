using System;
using UnityEngine;

// Token: 0x02000D96 RID: 3478
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x0600555B RID: 21851 RVA: 0x001BD8AD File Offset: 0x001BBAAD
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x0600555C RID: 21852 RVA: 0x001BD8DC File Offset: 0x001BBADC
	private void Update()
	{
		Vector3 lhs = this.x0z(base.transform.forward);
		Vector3 vector = this.x0z(this.referenceTransform.forward);
		float num = Vector3.Dot(lhs, vector);
		UIMatchRotation.State state = this.state;
		if (state != UIMatchRotation.State.Ready)
		{
			if (state != UIMatchRotation.State.Rotating)
			{
				return;
			}
			base.transform.forward = Vector3.Lerp(base.transform.forward, vector, Time.deltaTime * this.lerpSpeed);
			if (Vector3.Dot(base.transform.forward, vector) > 0.995f)
			{
				this.state = UIMatchRotation.State.Ready;
			}
		}
		else if (num < 1f - this.threshold)
		{
			this.state = UIMatchRotation.State.Rotating;
			return;
		}
	}

	// Token: 0x0600555D RID: 21853 RVA: 0x001BD980 File Offset: 0x001BBB80
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x040065A2 RID: 26018
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x040065A3 RID: 26019
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x040065A4 RID: 26020
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x040065A5 RID: 26021
	private UIMatchRotation.State state;

	// Token: 0x02000D97 RID: 3479
	private enum State
	{
		// Token: 0x040065A7 RID: 26023
		Ready,
		// Token: 0x040065A8 RID: 26024
		Rotating
	}
}
