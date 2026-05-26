using System;
using System.Collections.Generic;
using Photon.Pun;

// Token: 0x0200004E RID: 78
public class CrittersActorSpawnerPoint : CrittersActor
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x0600018B RID: 395 RVA: 0x00009CE0 File Offset: 0x00007EE0
	// (remove) Token: 0x0600018C RID: 396 RVA: 0x00009D18 File Offset: 0x00007F18
	public event Action<CrittersActor> OnSpawnChanged;

	// Token: 0x0600018D RID: 397 RVA: 0x00009D4D File Offset: 0x00007F4D
	public override void Initialize()
	{
		base.Initialize();
		base.UpdateImpulses(false, false);
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00009D5D File Offset: 0x00007F5D
	public override void OnDisable()
	{
		base.OnDisable();
		this.spawnedActorID = -1;
		this.spawnedActor = null;
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00009D74 File Offset: 0x00007F74
	public void SetSpawnedActor(CrittersActor actor)
	{
		if (this.spawnedActor == actor)
		{
			return;
		}
		this.spawnedActor = actor;
		if (this.spawnedActor != null)
		{
			this.spawnedActorID = this.spawnedActor.actorId;
		}
		else
		{
			this.spawnedActorID = -1;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged != null)
		{
			onSpawnChanged(this.spawnedActor);
		}
		this.updatedSinceLastFrame = true;
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00009DE0 File Offset: 0x00007FE0
	private void UpdateSpawnedActor(int newSpawnedActorID)
	{
		if (this.spawnedActorID == newSpawnedActorID)
		{
			return;
		}
		if (newSpawnedActorID == -1)
		{
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = null;
		}
		else
		{
			CrittersActor crittersActor;
			if (!CrittersManager.instance.actorById.TryGetValue(newSpawnedActorID, out crittersActor))
			{
				return;
			}
			this.spawnedActorID = newSpawnedActorID;
			this.spawnedActor = crittersActor;
		}
		Action<CrittersActor> onSpawnChanged = this.OnSpawnChanged;
		if (onSpawnChanged == null)
		{
			return;
		}
		onSpawnChanged(this.spawnedActor);
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00009E46 File Offset: 0x00008046
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.spawnedActorID);
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00009E60 File Offset: 0x00008060
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num))
		{
			return false;
		}
		if (num < -1 || num >= CrittersManager.instance.universalActorId)
		{
			return false;
		}
		this.UpdateSpawnedActor(num);
		return true;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00009EA6 File Offset: 0x000080A6
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.spawnedActorID);
		return this.TotalActorDataLength();
	}

	// Token: 0x06000194 RID: 404 RVA: 0x00009EC8 File Offset: 0x000080C8
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00009ED4 File Offset: 0x000080D4
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		if (num >= -1 && num < CrittersManager.instance.universalActorId)
		{
			return this.TotalActorDataLength();
		}
		this.UpdateSpawnedActor(num);
		return this.TotalActorDataLength();
	}

	// Token: 0x040001AA RID: 426
	private CrittersActor spawnedActor;

	// Token: 0x040001AB RID: 427
	private int spawnedActorID = -1;
}
