using System;
using System.IO;

// Token: 0x020006AD RID: 1709
public interface IGameEntitySerialize
{
	// Token: 0x06002A98 RID: 10904
	void OnGameEntitySerialize(BinaryWriter writer);

	// Token: 0x06002A99 RID: 10905
	void OnGameEntityDeserialize(BinaryReader reader);
}
