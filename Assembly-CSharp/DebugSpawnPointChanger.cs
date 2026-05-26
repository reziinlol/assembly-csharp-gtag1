using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005A5 RID: 1445
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x0600249C RID: 9372 RVA: 0x000C4538 File Offset: 0x000C2738
	private void AttachSpawnPoint(VRRig rig, Transform[] spawnPts, int locationIndex)
	{
		if (spawnPts == null)
		{
			return;
		}
		GTPlayer gtplayer = Object.FindAnyObjectByType<GTPlayer>();
		if (gtplayer == null)
		{
			return;
		}
		this.lastLocationIndex = locationIndex;
		int i = 0;
		while (i < spawnPts.Length)
		{
			Transform transform = spawnPts[i];
			if (transform.name == this.levelTriggers[locationIndex].levelName)
			{
				rig.transform.position = transform.position;
				rig.transform.rotation = transform.rotation;
				gtplayer.transform.position = transform.position;
				gtplayer.transform.rotation = transform.rotation;
				gtplayer.InitializeValues();
				SpawnPoint component = transform.GetComponent<SpawnPoint>();
				if (component != null)
				{
					gtplayer.SetScaleMultiplier(component.startSize);
					ZoneManagement.SetActiveZone(component.startZone);
					return;
				}
				Debug.LogWarning("Attempt to spawn at transform that does not have SpawnPoint component will be ignored: " + transform.name);
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000C462C File Offset: 0x000C282C
	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindAnyObjectByType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] spawnPts = spawnManager.ChildrenXfs();
			foreach (VRRig rig in Object.FindObjectsByType<VRRig>(FindObjectsSortMode.None))
			{
				this.AttachSpawnPoint(rig, spawnPts, index);
			}
		}
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000C4673 File Offset: 0x000C2873
	public List<string> GetPlausibleJumpLocation()
	{
		return (from index in this.levelTriggers[this.lastLocationIndex].canJumpToIndex
		select this.levelTriggers[index].levelName).ToList<string>();
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000C46A4 File Offset: 0x000C28A4
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000C46D4 File Offset: 0x000C28D4
	public void SetLastLocation(string levelName)
	{
		for (int i = 0; i < this.levelTriggers.Length; i++)
		{
			if (!(this.levelTriggers[i].levelName != levelName))
			{
				this.lastLocationIndex = i;
				return;
			}
		}
	}

	// Token: 0x04002FF7 RID: 12279
	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	// Token: 0x04002FF8 RID: 12280
	private int lastLocationIndex;

	// Token: 0x020005A6 RID: 1446
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x04002FF9 RID: 12281
		public string levelName;

		// Token: 0x04002FFA RID: 12282
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04002FFB RID: 12283
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04002FFC RID: 12284
		public int[] canJumpToIndex;
	}
}
