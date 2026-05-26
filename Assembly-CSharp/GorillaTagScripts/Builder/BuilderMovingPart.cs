using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000F9E RID: 3998
	public class BuilderMovingPart : MonoBehaviour
	{
		// Token: 0x060063A5 RID: 25509 RVA: 0x00200F98 File Offset: 0x001FF198
		private void Awake()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				builderAttachGridPlane.movesOnPlace = true;
				builderAttachGridPlane.movingPart = this;
			}
			this.initLocalPos = base.transform.localPosition;
			this.initLocalRotation = base.transform.localRotation;
		}

		// Token: 0x060063A6 RID: 25510 RVA: 0x00200FEC File Offset: 0x001FF1EC
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x00201021 File Offset: 0x001FF221
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x060063A8 RID: 25512 RVA: 0x00201030 File Offset: 0x001FF230
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x060063A9 RID: 25513 RVA: 0x0020105B File Offset: 0x001FF25B
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x0020106B File Offset: 0x001FF26B
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x0020108B File Offset: 0x001FF28B
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x060063AC RID: 25516 RVA: 0x00201098 File Offset: 0x001FF298
		public void ActivateAtNode(byte node, int timestamp)
		{
			float num = (float)node;
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (num >= this.startPercentage)
			{
				int num2 = (int)((num - this.startPercentage) * (float)this.CycleLengthMs());
				int num3 = timestamp - num2;
				if (flag)
				{
					num3 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = num3;
			}
			else
			{
				int num4 = (int)((num + 2f - this.startPercentage) * (float)this.CycleLengthMs());
				if (flag)
				{
					num4 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = timestamp - num4;
			}
			this.SetMoving(true);
		}

		// Token: 0x060063AD RID: 25517 RVA: 0x00201150 File Offset: 0x001FF350
		public int GetTimeOffsetMS()
		{
			int num = PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp;
			uint num2 = (uint)(this.CycleLengthMs() * 2L);
			return num % (int)num2;
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x0020117C File Offset: 0x001FF37C
		public byte GetNearestNode()
		{
			int num = Mathf.RoundToInt(this.currT * (float)BuilderMovingPart.NUM_PAUSE_NODES);
			if (!this.IsEvenCycle())
			{
				num += BuilderMovingPart.NUM_PAUSE_NODES;
			}
			return (byte)num;
		}

		// Token: 0x060063AF RID: 25519 RVA: 0x002011AE File Offset: 0x001FF3AE
		public byte GetStartNode()
		{
			return (byte)Mathf.RoundToInt(this.startPercentage * (float)BuilderMovingPart.NUM_PAUSE_NODES);
		}

		// Token: 0x060063B0 RID: 25520 RVA: 0x002011C4 File Offset: 0x001FF3C4
		public void PauseMovement(byte node)
		{
			this.SetMoving(false);
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			float num = (float)node;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (this.reverseDirOnCycle)
			{
				num = (flag ? (1f - num) : num);
			}
			if (this.reverseDir)
			{
				num = 1f - num;
			}
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(num);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(num);
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x0020125C File Offset: 0x001FF45C
		public void SetMoving(bool isMoving)
		{
			this.isMoving = isMoving;
			BuilderAttachGridPlane[] array = this.myGridPlanes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].isMoving = isMoving;
			}
			if (!isMoving)
			{
				this.ResetMovingGrid();
			}
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x00201298 File Offset: 0x001FF498
		public void InitMovingGrid()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / (this.velocity * this.myPiece.GetScale());
				this.cycleDuration = num + this.cycleDelay;
				float num2 = this.cycleDelay / this.cycleDuration;
				Vector2 vector = new Vector2(num2 / 2f, 0f);
				Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
				float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
				this.lerpAlpha = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(num2 / 2f, 0f, 0f, num3),
					new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
				});
			}
			else
			{
				this.cycleDuration = 1f / this.velocity;
			}
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x060063B3 RID: 25523 RVA: 0x0020140C File Offset: 0x001FF60C
		public void UpdateMovingGrid()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x060063B4 RID: 25524 RVA: 0x0020145C File Offset: 0x001FF65C
		private Vector3 UpdatePointToPoint(float perc)
		{
			float t = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, t);
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x00201494 File Offset: 0x001FF694
		private void UpdateRotation(float perc)
		{
			Quaternion localRotation = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = localRotation;
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x002014BF File Offset: 0x001FF6BF
		private void ResetMovingGrid()
		{
			base.transform.SetLocalPositionAndRotation(this.initLocalPos, this.initLocalRotation);
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x002014D8 File Offset: 0x001FF6D8
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x060063B8 RID: 25528 RVA: 0x00201550 File Offset: 0x001FF750
		public bool IsAnchoredToTable()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				if (builderAttachGridPlane.attachIndex == builderAttachGridPlane.piece.attachIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060063B9 RID: 25529 RVA: 0x0020158C File Offset: 0x001FF78C
		public void OnPieceDestroy()
		{
			this.ResetMovingGrid();
		}

		// Token: 0x0400724D RID: 29261
		public BuilderPiece myPiece;

		// Token: 0x0400724E RID: 29262
		public BuilderAttachGridPlane[] myGridPlanes;

		// Token: 0x0400724F RID: 29263
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04007250 RID: 29264
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x04007251 RID: 29265
		[SerializeField]
		private float velocity;

		// Token: 0x04007252 RID: 29266
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x04007253 RID: 29267
		[SerializeField]
		private bool reverseDir;

		// Token: 0x04007254 RID: 29268
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x04007255 RID: 29269
		[SerializeField]
		protected Transform startXf;

		// Token: 0x04007256 RID: 29270
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04007257 RID: 29271
		public static int NUM_PAUSE_NODES = 32;

		// Token: 0x04007258 RID: 29272
		private AnimationCurve lerpAlpha;

		// Token: 0x04007259 RID: 29273
		public bool isMoving;

		// Token: 0x0400725A RID: 29274
		private Quaternion initLocalRotation = Quaternion.identity;

		// Token: 0x0400725B RID: 29275
		private Vector3 initLocalPos = Vector3.zero;

		// Token: 0x0400725C RID: 29276
		private float cycleDuration;

		// Token: 0x0400725D RID: 29277
		private float distance;

		// Token: 0x0400725E RID: 29278
		private float currT;

		// Token: 0x0400725F RID: 29279
		private float percent;

		// Token: 0x04007260 RID: 29280
		private bool currForward;

		// Token: 0x04007261 RID: 29281
		private float dtSinceServerUpdate;

		// Token: 0x04007262 RID: 29282
		private int lastServerTimeStamp;

		// Token: 0x04007263 RID: 29283
		private float rotateStartAmt;

		// Token: 0x04007264 RID: 29284
		private float rotateAmt;

		// Token: 0x04007265 RID: 29285
		private uint startPercentageCycleOffset;

		// Token: 0x02000F9F RID: 3999
		public enum BuilderMovingPartType
		{
			// Token: 0x04007267 RID: 29287
			Translation,
			// Token: 0x04007268 RID: 29288
			Rotation
		}
	}
}
