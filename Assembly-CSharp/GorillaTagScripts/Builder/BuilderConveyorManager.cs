using System;
using Photon.Pun;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000F9C RID: 3996
	public class BuilderConveyorManager : MonoBehaviour
	{
		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x06006394 RID: 25492 RVA: 0x0020083C File Offset: 0x001FEA3C
		// (set) Token: 0x06006395 RID: 25493 RVA: 0x00200843 File Offset: 0x001FEA43
		public static BuilderConveyorManager instance { get; private set; }

		// Token: 0x06006396 RID: 25494 RVA: 0x0020084B File Offset: 0x001FEA4B
		private void Awake()
		{
			if (BuilderConveyorManager.instance != null && BuilderConveyorManager.instance != this)
			{
				Object.Destroy(this);
			}
			if (BuilderConveyorManager.instance == null)
			{
				BuilderConveyorManager.instance = this;
			}
		}

		// Token: 0x06006397 RID: 25495 RVA: 0x00200880 File Offset: 0x001FEA80
		public void UpdateManager()
		{
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.UpdateConveyor();
			}
			bool flag = false;
			bool flag2 = this.pieceTransforms.length >= this.pieceTransforms.capacity - 5;
			for (int i = this.jobSplineTimes.Length - 1; i >= 0; i--)
			{
				BuilderConveyor builderConveyor2 = this.table.conveyors[this.conveyorIndices[i]];
				float num = Time.deltaTime * builderConveyor2.GetFrameMovement();
				float num2 = this.jobSplineTimes[i] + num;
				this.jobSplineTimes[i] = Mathf.Clamp(num2, 0f, 1f);
				if (PhotonNetwork.IsMasterClient && (!flag || flag2) && (double)num2 > 0.999)
				{
					builderConveyor2.RemovePieceFromConveyor(this.pieceTransforms[i]);
					this.RemovePieceFromJobAtIndex(i);
					flag = true;
				}
			}
			for (int j = this.shelfSlice; j < this.table.conveyors.Count; j += BuilderTable.SHELF_SLICE_BUCKETS)
			{
				this.table.conveyors[j].UpdateShelfSliced();
			}
			this.shelfSlice = (this.shelfSlice + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
		}

		// Token: 0x06006398 RID: 25496 RVA: 0x002009FC File Offset: 0x001FEBFC
		public void Setup(BuilderTable mytable)
		{
			if (this.isSetup)
			{
				return;
			}
			this.table = mytable;
			this.conveyorSplines = new NativeArray<NativeSpline>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.conveyorRotations = new NativeArray<Quaternion>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = 0;
			for (int i = 0; i < this.table.conveyors.Count; i++)
			{
				this.conveyorSplines[i] = this.table.conveyors[i].nativeSpline;
				this.conveyorRotations[i] = this.table.conveyors[i].GetSpawnTransform().rotation;
				num += this.table.conveyors[i].GetMaxItemsOnConveyor();
			}
			this.maxItemCount = num;
			this.conveyorIndices = new NativeList<int>(this.maxItemCount, Allocator.Persistent);
			this.jobSplineTimes = new NativeList<float>(this.maxItemCount, Allocator.Persistent);
			this.jobShelfOffsets = new NativeList<Vector3>(this.maxItemCount, Allocator.Persistent);
			this.pieceTransforms = new TransformAccessArray(this.maxItemCount, 3);
			this.isSetup = true;
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x00200B38 File Offset: 0x001FED38
		public float GetSplineProgressForPiece(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					return this.jobSplineTimes[i];
				}
			}
			return 1f;
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x00200B88 File Offset: 0x001FED88
		public int GetPieceCreateTimestamp(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderConveyor builderConveyor = this.table.conveyors[this.conveyorIndices[i]];
					int num = Mathf.RoundToInt(this.jobSplineTimes[i] / builderConveyor.GetFrameMovement() * 1000f);
					return PhotonNetwork.ServerTimestamp - num;
				}
			}
			return PhotonNetwork.ServerTimestamp - 5000;
		}

		// Token: 0x0600639B RID: 25499 RVA: 0x00200C14 File Offset: 0x001FEE14
		public void OnClearTable()
		{
			if (!this.isSetup)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.OnClearTable();
			}
			for (int i = this.pieceTransforms.length - 1; i >= 0; i--)
			{
				this.pieceTransforms.RemoveAtSwapBack(i);
			}
			this.jobSplineTimes.Clear();
			this.jobShelfOffsets.Clear();
			this.conveyorIndices.Clear();
		}

		// Token: 0x0600639C RID: 25500 RVA: 0x00200CB8 File Offset: 0x001FEEB8
		private void OnDestroy()
		{
			this.conveyorSplines.Dispose();
			this.conveyorRotations.Dispose();
			this.conveyorIndices.Dispose();
			this.jobSplineTimes.Dispose();
			this.jobShelfOffsets.Dispose();
			this.pieceTransforms.Dispose();
		}

		// Token: 0x0600639D RID: 25501 RVA: 0x00200D08 File Offset: 0x001FEF08
		public JobHandle ConstructJobHandle()
		{
			BuilderConveyorManager.EvaluateSplineJob jobData = new BuilderConveyorManager.EvaluateSplineJob
			{
				conveyorRotations = this.conveyorRotations,
				conveyorIndices = this.conveyorIndices,
				shelfOffsets = this.jobShelfOffsets,
				splineTimes = this.jobSplineTimes
			};
			for (int i = 0; i < this.conveyorSplines.Length; i++)
			{
				jobData.SetSplineAt(i, this.conveyorSplines[i]);
			}
			return jobData.Schedule(this.pieceTransforms, default(JobHandle));
		}

		// Token: 0x0600639E RID: 25502 RVA: 0x00200D94 File Offset: 0x001FEF94
		public void AddPieceToJob(BuilderPiece piece, float splineTime, int conveyorID)
		{
			if (this.pieceTransforms.length >= this.pieceTransforms.capacity)
			{
				Debug.LogError("Too many pieces on conveyor!");
			}
			this.pieceTransforms.Add(piece.transform);
			this.conveyorIndices.Add(conveyorID);
			this.jobShelfOffsets.Add(piece.desiredShelfOffset);
			this.jobSplineTimes.Add(splineTime);
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x00200DFF File Offset: 0x001FEFFF
		public void RemovePieceFromJobAtIndex(int index)
		{
			BuilderRenderer.RemoveAt(this.pieceTransforms, index);
			this.jobShelfOffsets.RemoveAt(index);
			this.jobSplineTimes.RemoveAt(index);
			this.conveyorIndices.RemoveAt(index);
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x00200E34 File Offset: 0x001FF034
		public void RemovePieceFromJob(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderRenderer.RemoveAt(this.pieceTransforms, i);
					this.jobShelfOffsets.RemoveAt(i);
					this.jobSplineTimes.RemoveAt(i);
					this.conveyorIndices.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0400723A RID: 29242
		private NativeArray<NativeSpline> conveyorSplines;

		// Token: 0x0400723B RID: 29243
		private NativeArray<Quaternion> conveyorRotations;

		// Token: 0x0400723C RID: 29244
		private NativeList<int> conveyorIndices;

		// Token: 0x0400723D RID: 29245
		private NativeList<float> jobSplineTimes;

		// Token: 0x0400723E RID: 29246
		private NativeList<Vector3> jobShelfOffsets;

		// Token: 0x0400723F RID: 29247
		private TransformAccessArray pieceTransforms;

		// Token: 0x04007240 RID: 29248
		private BuilderTable table;

		// Token: 0x04007241 RID: 29249
		private bool isSetup;

		// Token: 0x04007242 RID: 29250
		private int maxItemCount;

		// Token: 0x04007243 RID: 29251
		private int shelfSlice;

		// Token: 0x02000F9D RID: 3997
		[BurstCompile]
		public struct EvaluateSplineJob : IJobParallelForTransform
		{
			// Token: 0x060063A2 RID: 25506 RVA: 0x00200EA1 File Offset: 0x001FF0A1
			public NativeSpline GetSplineAt(int index)
			{
				switch (index)
				{
				case 0:
					return this.conveyorSpline0;
				case 1:
					return this.conveyorSpline1;
				case 2:
					return this.conveyorSpline2;
				case 3:
					return this.conveyorSpline3;
				default:
					return this.conveyorSpline0;
				}
			}

			// Token: 0x060063A3 RID: 25507 RVA: 0x00200EDD File Offset: 0x001FF0DD
			public void SetSplineAt(int index, NativeSpline s)
			{
				switch (index)
				{
				case 0:
					this.conveyorSpline0 = s;
					return;
				case 1:
					this.conveyorSpline1 = s;
					return;
				case 2:
					this.conveyorSpline2 = s;
					return;
				case 3:
					this.conveyorSpline3 = s;
					return;
				default:
					return;
				}
			}

			// Token: 0x060063A4 RID: 25508 RVA: 0x00200F18 File Offset: 0x001FF118
			public void Execute(int index, TransformAccess transform)
			{
				float splineT = this.splineTimes[index];
				Vector3 point = this.shelfOffsets[index];
				int index2 = this.conveyorIndices[index];
				NativeSpline splineAt = this.GetSplineAt(index2);
				Quaternion rotation = this.conveyorRotations[index2];
				float t;
				Vector3 position = CurveUtility.EvaluatePosition(splineAt.GetCurve(splineAt.SplineToCurveT(splineT, out t)), t) + rotation * point;
				transform.position = position;
			}

			// Token: 0x04007245 RID: 29253
			public NativeSpline conveyorSpline0;

			// Token: 0x04007246 RID: 29254
			public NativeSpline conveyorSpline1;

			// Token: 0x04007247 RID: 29255
			public NativeSpline conveyorSpline2;

			// Token: 0x04007248 RID: 29256
			public NativeSpline conveyorSpline3;

			// Token: 0x04007249 RID: 29257
			[ReadOnly]
			public NativeArray<Quaternion> conveyorRotations;

			// Token: 0x0400724A RID: 29258
			[ReadOnly]
			public NativeList<int> conveyorIndices;

			// Token: 0x0400724B RID: 29259
			[ReadOnly]
			public NativeList<float> splineTimes;

			// Token: 0x0400724C RID: 29260
			[ReadOnly]
			public NativeList<Vector3> shelfOffsets;
		}
	}
}
