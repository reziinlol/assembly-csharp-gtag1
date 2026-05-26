using System;
using Unity.Collections;

namespace GorillaTagScripts
{
	// Token: 0x02000EF0 RID: 3824
	public class BuilderTableJobs
	{
		// Token: 0x06005EDC RID: 24284 RVA: 0x001E77BC File Offset: 0x001E59BC
		public static void BuildTestPieceListForJob(BuilderPiece testPiece, NativeList<BuilderPieceData> testPieceList, NativeList<BuilderGridPlaneData> testGridPlaneList)
		{
			if (testPiece == null)
			{
				return;
			}
			int length = testPieceList.Length;
			BuilderPieceData builderPieceData = new BuilderPieceData(testPiece);
			testPieceList.Add(builderPieceData);
			for (int i = 0; i < testPiece.gridPlanes.Count; i++)
			{
				BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(testPiece.gridPlanes[i], length);
				testGridPlaneList.Add(builderGridPlaneData);
			}
			BuilderPiece builderPiece = testPiece.firstChildPiece;
			while (builderPiece != null)
			{
				BuilderTableJobs.BuildTestPieceListForJob(builderPiece, testPieceList, testGridPlaneList);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}

		// Token: 0x06005EDD RID: 24285 RVA: 0x001E7844 File Offset: 0x001E5A44
		public static void BuildTestPieceListForJob(BuilderPiece testPiece, NativeList<BuilderGridPlaneData> testGridPlaneList)
		{
			if (testPiece == null)
			{
				return;
			}
			for (int i = 0; i < testPiece.gridPlanes.Count; i++)
			{
				BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(testPiece.gridPlanes[i], -1);
				testGridPlaneList.Add(builderGridPlaneData);
			}
			BuilderPiece builderPiece = testPiece.firstChildPiece;
			while (builderPiece != null)
			{
				BuilderTableJobs.BuildTestPieceListForJob(builderPiece, testGridPlaneList);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}
}
