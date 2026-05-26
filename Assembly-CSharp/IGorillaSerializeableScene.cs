using System;

// Token: 0x02000837 RID: 2103
internal interface IGorillaSerializeableScene : IGorillaSerializeable
{
	// Token: 0x06003620 RID: 13856
	void OnSceneLinking(GorillaSerializerScene serializer);

	// Token: 0x06003621 RID: 13857
	void OnNetworkObjectDisable();

	// Token: 0x06003622 RID: 13858
	void OnNetworkObjectEnable();
}
