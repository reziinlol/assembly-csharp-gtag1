using System;

// Token: 0x020006BC RID: 1724
public struct GameEntityId
{
	// Token: 0x06002B0E RID: 11022 RVA: 0x000E5FEC File Offset: 0x000E41EC
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x000E5FFA File Offset: 0x000E41FA
	public static bool operator ==(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000E600A File Offset: 0x000E420A
	public static bool operator !=(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000E6020 File Offset: 0x000E4220
	public override bool Equals(object obj)
	{
		GameEntityId gameEntityId = (GameEntityId)obj;
		return this.index == gameEntityId.index;
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x000E6042 File Offset: 0x000E4242
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x040037B8 RID: 14264
	public static GameEntityId Invalid = new GameEntityId
	{
		index = -1
	};

	// Token: 0x040037B9 RID: 14265
	public int index;
}
