using System;

// Token: 0x020001EC RID: 492
[Serializable]
public struct VoxelAction
{
	// Token: 0x06000CEA RID: 3306 RVA: 0x00046AE5 File Offset: 0x00044CE5
	public VoxelAction(OperationType operation, float radius, float strength, byte material = 0)
	{
		this.operation = operation;
		this.radius = radius;
		this.strength = strength;
		this.material = material;
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00046B04 File Offset: 0x00044D04
	public bool IsValid()
	{
		return float.IsFinite(this.radius) && this.radius > 0f && float.IsFinite(this.strength) && this.strength > 0f && (this.operation == OperationType.Add || this.operation == OperationType.Subtract);
	}

	// Token: 0x04000F8A RID: 3978
	public OperationType operation;

	// Token: 0x04000F8B RID: 3979
	public float radius;

	// Token: 0x04000F8C RID: 3980
	public float strength;

	// Token: 0x04000F8D RID: 3981
	public byte material;
}
