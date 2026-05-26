using System;
using UnityEngine;

namespace com.AnotherAxiom.SpaceFight
{
	// Token: 0x020010C6 RID: 4294
	public class SpaceFight : ArcadeGame
	{
		// Token: 0x06006B8E RID: 27534 RVA: 0x0022CDCC File Offset: 0x0022AFCC
		private void Update()
		{
			for (int i = 0; i < 2; i++)
			{
				if (base.getButtonState(i, ArcadeButtons.UP))
				{
					this.move(this.player[i], 0.15f);
					this.clamp(this.player[i]);
				}
				if (base.getButtonState(i, ArcadeButtons.RIGHT))
				{
					this.turn(this.player[i], true);
				}
				if (base.getButtonState(i, ArcadeButtons.LEFT))
				{
					this.turn(this.player[i], false);
				}
				if (this.projectilesFired[i])
				{
					this.move(this.projectile[i], 0.5f);
					if (Vector2.Distance(this.player[1 - i].localPosition, this.projectile[i].localPosition) < 0.25f)
					{
						base.PlaySound(1, 2);
						this.player[1 - i].Rotate(0f, 0f, 180f);
						this.projectilesFired[i] = false;
					}
					if (Mathf.Abs(this.projectile[i].localPosition.x) > this.tableSize.x || Mathf.Abs(this.projectile[i].localPosition.y) > this.tableSize.y)
					{
						this.projectilesFired[i] = false;
					}
				}
				if (!this.projectilesFired[i])
				{
					this.projectile[i].position = this.player[i].position;
					this.projectile[i].rotation = this.player[i].rotation;
				}
			}
		}

		// Token: 0x06006B8F RID: 27535 RVA: 0x0022CF5C File Offset: 0x0022B15C
		private void clamp(Transform tr)
		{
			tr.localPosition = new Vector2(Mathf.Clamp(tr.localPosition.x, -this.tableSize.x, this.tableSize.x), Mathf.Clamp(tr.localPosition.y, -this.tableSize.y, this.tableSize.y));
		}

		// Token: 0x06006B90 RID: 27536 RVA: 0x0022CFC7 File Offset: 0x0022B1C7
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.TRIGGER)
			{
				if (!this.projectilesFired[player])
				{
					base.PlaySound(0, 3);
				}
				this.projectilesFired[player] = true;
			}
		}

		// Token: 0x06006B91 RID: 27537 RVA: 0x0022CFEC File Offset: 0x0022B1EC
		private void move(Transform p, float speed)
		{
			p.Translate(p.up * Time.deltaTime * speed, Space.World);
		}

		// Token: 0x06006B92 RID: 27538 RVA: 0x0022D00B File Offset: 0x0022B20B
		private void turn(Transform p, bool cw)
		{
			p.Rotate(0f, 0f, (float)(cw ? 180 : -180) * Time.deltaTime);
		}

		// Token: 0x06006B93 RID: 27539 RVA: 0x0022D034 File Offset: 0x0022B234
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P1LocX = this.player[0].localPosition.x;
			this.netStateCur.P1LocY = this.player[0].localPosition.y;
			this.netStateCur.P1Rot = this.player[0].localRotation.eulerAngles.z;
			this.netStateCur.P2LocX = this.player[1].localPosition.x;
			this.netStateCur.P2LocY = this.player[1].localPosition.y;
			this.netStateCur.P2Rot = this.player[1].localRotation.eulerAngles.z;
			this.netStateCur.P1PrLocX = this.projectile[0].localPosition.x;
			this.netStateCur.P1PrLocY = this.projectile[0].localPosition.y;
			this.netStateCur.P2PrLocX = this.projectile[1].localPosition.x;
			this.netStateCur.P2PrLocY = this.projectile[1].localPosition.y;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06006B94 RID: 27540 RVA: 0x0022D1B4 File Offset: 0x0022B3B4
		public override void SetNetworkState(byte[] b)
		{
			SpaceFight.SpaceFlightNetState spaceFlightNetState = (SpaceFight.SpaceFlightNetState)ArcadeGame.UnwrapNetState(b);
			this.player[0].localPosition = new Vector2(spaceFlightNetState.P1LocX, spaceFlightNetState.P1LocY);
			this.player[0].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P1Rot);
			this.player[1].localPosition = new Vector2(spaceFlightNetState.P2LocX, spaceFlightNetState.P2LocY);
			this.player[1].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P2Rot);
			this.projectile[0].localPosition = new Vector2(spaceFlightNetState.P1PrLocX, spaceFlightNetState.P1PrLocY);
			this.projectile[1].localPosition = new Vector2(spaceFlightNetState.P2PrLocX, spaceFlightNetState.P2PrLocY);
		}

		// Token: 0x06006B95 RID: 27541 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06006B96 RID: 27542 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void OnTimeout()
		{
		}

		// Token: 0x04007BA8 RID: 31656
		[SerializeField]
		private Transform[] player;

		// Token: 0x04007BA9 RID: 31657
		[SerializeField]
		private Transform[] projectile;

		// Token: 0x04007BAA RID: 31658
		[SerializeField]
		private Vector2 tableSize;

		// Token: 0x04007BAB RID: 31659
		private bool[] projectilesFired = new bool[2];

		// Token: 0x04007BAC RID: 31660
		private SpaceFight.SpaceFlightNetState netStateLast;

		// Token: 0x04007BAD RID: 31661
		private SpaceFight.SpaceFlightNetState netStateCur;

		// Token: 0x020010C7 RID: 4295
		[Serializable]
		private struct SpaceFlightNetState : IEquatable<SpaceFight.SpaceFlightNetState>
		{
			// Token: 0x06006B98 RID: 27544 RVA: 0x0022D2B4 File Offset: 0x0022B4B4
			public bool Equals(SpaceFight.SpaceFlightNetState other)
			{
				return this.P1LocX.Approx(other.P1LocX, 1E-06f) && this.P1LocY.Approx(other.P1LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P2LocX.Approx(other.P2LocX, 1E-06f) && this.P2LocY.Approx(other.P2LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P1PrLocX.Approx(other.P1PrLocX, 1E-06f) && this.P1PrLocY.Approx(other.P1PrLocY, 1E-06f) && this.P2PrLocX.Approx(other.P2PrLocX, 1E-06f) && this.P2PrLocY.Approx(other.P2PrLocY, 1E-06f);
			}

			// Token: 0x04007BAE RID: 31662
			public float P1LocX;

			// Token: 0x04007BAF RID: 31663
			public float P1LocY;

			// Token: 0x04007BB0 RID: 31664
			public float P1Rot;

			// Token: 0x04007BB1 RID: 31665
			public float P2LocX;

			// Token: 0x04007BB2 RID: 31666
			public float P2LocY;

			// Token: 0x04007BB3 RID: 31667
			public float P2Rot;

			// Token: 0x04007BB4 RID: 31668
			public float P1PrLocX;

			// Token: 0x04007BB5 RID: 31669
			public float P1PrLocY;

			// Token: 0x04007BB6 RID: 31670
			public float P2PrLocX;

			// Token: 0x04007BB7 RID: 31671
			public float P2PrLocY;
		}
	}
}
