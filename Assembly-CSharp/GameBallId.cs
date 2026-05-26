using System;

// Token: 0x020005E9 RID: 1513
public struct GameBallId
{
	// Token: 0x0600259D RID: 9629 RVA: 0x000C6F60 File Offset: 0x000C5160
	public GameBallId(int index)
	{
		this.index = index;
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000C6F69 File Offset: 0x000C5169
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000C6F77 File Offset: 0x000C5177
	public static bool operator ==(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000C6F87 File Offset: 0x000C5187
	public static bool operator !=(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000C6F9C File Offset: 0x000C519C
	public override bool Equals(object obj)
	{
		GameBallId gameBallId = (GameBallId)obj;
		return this.index == gameBallId.index;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000C6FBE File Offset: 0x000C51BE
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x0400311B RID: 12571
	public static GameBallId Invalid = new GameBallId(-1);

	// Token: 0x0400311C RID: 12572
	public int index;
}
