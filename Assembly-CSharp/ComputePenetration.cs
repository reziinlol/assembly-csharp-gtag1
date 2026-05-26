using System;
using Drawing;
using UnityEngine;

// Token: 0x02000ABF RID: 2751
public class ComputePenetration : MonoBehaviour
{
	// Token: 0x0600466A RID: 18026 RVA: 0x0017D9A3 File Offset: 0x0017BBA3
	public void Compute()
	{
		if (this.colliderA == null)
		{
			return;
		}
		this.colliderB == null;
	}

	// Token: 0x0600466B RID: 18027 RVA: 0x0017D9C4 File Offset: 0x0017BBC4
	public void OnDrawGizmos()
	{
		if (this.colliderA.AsNull<Collider>() == null)
		{
			return;
		}
		if (this.colliderB.AsNull<Collider>() == null)
		{
			return;
		}
		Transform transform = this.colliderA.transform;
		Transform transform2 = this.colliderB.transform;
		if (this.lastUpdate.HasElapsed(0.5f, true))
		{
			this.overlapped = Physics.ComputePenetration(this.colliderA, transform.position, transform.rotation, this.colliderB, transform2.position, transform2.rotation, out this.direction, out this.distance);
		}
		Color color = this.overlapped ? Color.red : Color.green;
		this.DrawCollider(this.colliderA, color);
		this.DrawCollider(this.colliderB, color);
		if (this.overlapped)
		{
			Vector3 position = this.colliderB.transform.position;
			Vector3 to = position + this.direction * this.distance;
			Gizmos.DrawLine(position, to);
		}
	}

	// Token: 0x0600466C RID: 18028 RVA: 0x0017DAC4 File Offset: 0x0017BCC4
	private unsafe void DrawCollider(Collider c, Color color)
	{
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(c.transform.localToWorldMatrix))
		{
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = c as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = c as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = c as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.PopColor();
		}
	}

	// Token: 0x040058DA RID: 22746
	public Collider colliderA;

	// Token: 0x040058DB RID: 22747
	public Collider colliderB;

	// Token: 0x040058DC RID: 22748
	public bool overlapped;

	// Token: 0x040058DD RID: 22749
	public Vector3 direction;

	// Token: 0x040058DE RID: 22750
	public float distance;

	// Token: 0x040058DF RID: 22751
	private TimeSince lastUpdate = TimeSince.Now();
}
