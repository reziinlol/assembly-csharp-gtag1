using System;
using UnityEngine;

// Token: 0x02000D15 RID: 3349
public class KinematicTestMotion : MonoBehaviour
{
	// Token: 0x060052CF RID: 21199 RVA: 0x001B29A2 File Offset: 0x001B0BA2
	private void FixedUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.FixedUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x060052D0 RID: 21200 RVA: 0x001B29B9 File Offset: 0x001B0BB9
	private void Update()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.Update)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x060052D1 RID: 21201 RVA: 0x001B29CF File Offset: 0x001B0BCF
	private void LateUpdate()
	{
		if (this.updateType != KinematicTestMotion.UpdateType.LateUpdate)
		{
			return;
		}
		this.UpdatePosition(Time.time);
	}

	// Token: 0x060052D2 RID: 21202 RVA: 0x001B29E8 File Offset: 0x001B0BE8
	private void UpdatePosition(float time)
	{
		float t = Mathf.Sin(time * 2f * 3.1415927f * this.period) * 0.5f + 0.5f;
		Vector3 position = Vector3.Lerp(this.start.position, this.end.position, t);
		if (this.moveType == KinematicTestMotion.MoveType.TransformPosition)
		{
			base.transform.position = position;
			return;
		}
		if (this.moveType == KinematicTestMotion.MoveType.RigidbodyMovePosition)
		{
			this.rigidbody.MovePosition(position);
		}
	}

	// Token: 0x04006436 RID: 25654
	public Transform start;

	// Token: 0x04006437 RID: 25655
	public Transform end;

	// Token: 0x04006438 RID: 25656
	public Rigidbody rigidbody;

	// Token: 0x04006439 RID: 25657
	public KinematicTestMotion.UpdateType updateType;

	// Token: 0x0400643A RID: 25658
	public KinematicTestMotion.MoveType moveType = KinematicTestMotion.MoveType.RigidbodyMovePosition;

	// Token: 0x0400643B RID: 25659
	public float period = 4f;

	// Token: 0x02000D16 RID: 3350
	public enum UpdateType
	{
		// Token: 0x0400643D RID: 25661
		Update,
		// Token: 0x0400643E RID: 25662
		LateUpdate,
		// Token: 0x0400643F RID: 25663
		FixedUpdate
	}

	// Token: 0x02000D17 RID: 3351
	public enum MoveType
	{
		// Token: 0x04006441 RID: 25665
		TransformPosition,
		// Token: 0x04006442 RID: 25666
		RigidbodyMovePosition
	}
}
