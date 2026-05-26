using System;

// Token: 0x020008D4 RID: 2260
[Serializable]
public struct GTSignalID : IEquatable<GTSignalID>, IEquatable<int>
{
	// Token: 0x06003B13 RID: 15123 RVA: 0x00143F24 File Offset: 0x00142124
	public override bool Equals(object obj)
	{
		if (obj is GTSignalID)
		{
			GTSignalID other = (GTSignalID)obj;
			return this.Equals(other);
		}
		if (obj is int)
		{
			int other2 = (int)obj;
			return this.Equals(other2);
		}
		return false;
	}

	// Token: 0x06003B14 RID: 15124 RVA: 0x00143F60 File Offset: 0x00142160
	public bool Equals(GTSignalID other)
	{
		return this._id == other._id;
	}

	// Token: 0x06003B15 RID: 15125 RVA: 0x00143F70 File Offset: 0x00142170
	public bool Equals(int other)
	{
		return this._id == other;
	}

	// Token: 0x06003B16 RID: 15126 RVA: 0x00143F7B File Offset: 0x0014217B
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06003B17 RID: 15127 RVA: 0x00143F83 File Offset: 0x00142183
	public static bool operator ==(GTSignalID x, GTSignalID y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003B18 RID: 15128 RVA: 0x00143F8D File Offset: 0x0014218D
	public static bool operator !=(GTSignalID x, GTSignalID y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003B19 RID: 15129 RVA: 0x00143F7B File Offset: 0x0014217B
	public static implicit operator int(GTSignalID sid)
	{
		return sid._id;
	}

	// Token: 0x06003B1A RID: 15130 RVA: 0x00143F9C File Offset: 0x0014219C
	public static implicit operator GTSignalID(string s)
	{
		return new GTSignalID
		{
			_id = GTSignal.ComputeID(s)
		};
	}

	// Token: 0x04004B74 RID: 19316
	private int _id;
}
