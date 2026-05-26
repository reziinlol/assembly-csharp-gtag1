using System;
using System.Collections.Generic;

// Token: 0x020003C5 RID: 965
[Serializable]
public class ScenePerformanceData
{
	// Token: 0x06001726 RID: 5926 RVA: 0x00085DC0 File Offset: 0x00083FC0
	public ScenePerformanceData(string mapName, int gorillaCount, int droppedFrames, int msHigh, int medianMS, int medianFPS, int medianDrawCalls, List<int> msCaptures)
	{
		this._mapName = mapName;
		this._gorillaCount = gorillaCount;
		this._droppedFrames = droppedFrames;
		this._msHigh = msHigh;
		this._medianMS = medianMS;
		this._medianFPS = medianFPS;
		this._medianDrawCallCount = medianDrawCalls;
		this._msCaptures = new List<int>(msCaptures);
	}

	// Token: 0x04002258 RID: 8792
	public string _mapName;

	// Token: 0x04002259 RID: 8793
	public int _gorillaCount;

	// Token: 0x0400225A RID: 8794
	public int _droppedFrames;

	// Token: 0x0400225B RID: 8795
	public int _msHigh;

	// Token: 0x0400225C RID: 8796
	public int _medianMS;

	// Token: 0x0400225D RID: 8797
	public int _medianFPS;

	// Token: 0x0400225E RID: 8798
	public int _medianDrawCallCount;

	// Token: 0x0400225F RID: 8799
	public List<int> _msCaptures;
}
