using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x020010CD RID: 4301
	public class JoustPlayer : MonoBehaviour
	{
		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06006BB3 RID: 27571 RVA: 0x0022E2C1 File Offset: 0x0022C4C1
		// (set) Token: 0x06006BB4 RID: 27572 RVA: 0x0022E2C9 File Offset: 0x0022C4C9
		public float HorizontalSpeed
		{
			get
			{
				return this.HSpeed;
			}
			set
			{
				this.HSpeed = value;
			}
		}

		// Token: 0x06006BB5 RID: 27573 RVA: 0x0022E2D4 File Offset: 0x0022C4D4
		private void LateUpdate()
		{
			this.velocity.x = this.HSpeed * 0.001f;
			if (this.flap)
			{
				this.velocity.y = Mathf.Min(this.velocity.y + 0.0005f, 0.0005f);
				this.flap = false;
			}
			else
			{
				this.velocity.y = Mathf.Max(this.velocity.y - Time.deltaTime * 0.0001f, -0.001f);
				int i = 0;
				while (i < Physics2D.RaycastNonAlloc(base.transform.position, this.velocity.normalized, this.raycastHitResults, this.velocity.magnitude))
				{
					JoustTerrain joustTerrain;
					if (this.raycastHitResults[i].collider.TryGetComponent<JoustTerrain>(out joustTerrain))
					{
						this.velocity.y = 0f;
						if (joustTerrain.transform.localPosition.y < base.transform.localPosition.y)
						{
							base.transform.localPosition = new Vector2(base.transform.localPosition.x, joustTerrain.transform.localPosition.y + this.raycastHitResults[i].collider.bounds.size.y);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.transform.Translate(this.velocity);
			if ((double)Mathf.Abs(base.transform.localPosition.x) > 4.5)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x * -0.95f, base.transform.localPosition.y);
			}
		}

		// Token: 0x06006BB6 RID: 27574 RVA: 0x0022E4BA File Offset: 0x0022C6BA
		public void Flap()
		{
			this.flap = true;
		}

		// Token: 0x04007BEE RID: 31726
		private Vector2 velocity;

		// Token: 0x04007BEF RID: 31727
		private RaycastHit2D[] raycastHitResults = new RaycastHit2D[8];

		// Token: 0x04007BF0 RID: 31728
		private float HSpeed;

		// Token: 0x04007BF1 RID: 31729
		private bool flap;
	}
}
