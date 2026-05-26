using System;
using System.IO;
using UnityEngine;

// Token: 0x0200062D RID: 1581
[Serializable]
public struct SnapBounds
{
	// Token: 0x06002749 RID: 10057 RVA: 0x000D035A File Offset: 0x000CE55A
	public SnapBounds(Vector2Int min, Vector2Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000D036A File Offset: 0x000CE56A
	public SnapBounds(int minX, int minY, int maxX, int maxY)
	{
		this.min = new Vector2Int(minX, minY);
		this.max = new Vector2Int(maxX, maxY);
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000D0387 File Offset: 0x000CE587
	public void Clear()
	{
		this.min = new Vector2Int(int.MinValue, int.MinValue);
		this.max = new Vector2Int(int.MinValue, int.MinValue);
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000D03B4 File Offset: 0x000CE5B4
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.min.x);
		writer.Write(this.min.y);
		writer.Write(this.max.x);
		writer.Write(this.max.y);
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000D0408 File Offset: 0x000CE608
	public void Read(BinaryReader reader)
	{
		this.min.x = reader.ReadInt32();
		this.min.y = reader.ReadInt32();
		this.max.x = reader.ReadInt32();
		this.max.y = reader.ReadInt32();
	}

	// Token: 0x040032E4 RID: 13028
	public Vector2Int min;

	// Token: 0x040032E5 RID: 13029
	public Vector2Int max;
}
