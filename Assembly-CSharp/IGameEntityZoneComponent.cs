using System;
using System.IO;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
public interface IGameEntityZoneComponent
{
	// Token: 0x06002AFC RID: 11004
	void OnZoneCreate();

	// Token: 0x06002AFD RID: 11005
	void OnZoneInit();

	// Token: 0x06002AFE RID: 11006
	void OnZoneClear(ZoneClearReason reason);

	// Token: 0x06002AFF RID: 11007
	void OnCreateGameEntity(GameEntity entity);

	// Token: 0x06002B00 RID: 11008
	void SerializeZoneData(BinaryWriter writer);

	// Token: 0x06002B01 RID: 11009
	void DeserializeZoneData(BinaryReader reader);

	// Token: 0x06002B02 RID: 11010
	void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity);

	// Token: 0x06002B03 RID: 11011
	void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity);

	// Token: 0x06002B04 RID: 11012
	void SerializeZonePlayerData(BinaryWriter writer, int actorNumber);

	// Token: 0x06002B05 RID: 11013
	void DeserializeZonePlayerData(BinaryReader reader, int actorNumber);

	// Token: 0x06002B06 RID: 11014
	bool IsZoneReady();

	// Token: 0x06002B07 RID: 11015
	bool ShouldClearZone();

	// Token: 0x06002B08 RID: 11016
	long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData);

	// Token: 0x06002B09 RID: 11017
	bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr);

	// Token: 0x06002B0A RID: 11018
	bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount);

	// Token: 0x06002B0B RID: 11019
	bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId);

	// Token: 0x06002B0C RID: 11020
	bool ValidateCreateItemBatchSize(int size);
}
