using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000F3C RID: 3900
	public class CMSTeleporter : CMSTrigger
	{
		// Token: 0x06006141 RID: 24897 RVA: 0x001F5544 File Offset: 0x001F3744
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(TeleporterSettings))
			{
				TeleporterSettings teleporterSettings = (TeleporterSettings)settings;
				this.TeleportPoints = teleporterSettings.TeleportPoints;
				this.matchTeleportPointRotation = teleporterSettings.matchTeleportPointRotation;
				this.maintainVelocity = teleporterSettings.maintainVelocity;
			}
			for (int i = this.TeleportPoints.Count - 1; i >= 0; i--)
			{
				if (this.TeleportPoints[i] == null)
				{
					this.TeleportPoints.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x06006142 RID: 24898 RVA: 0x001F55D4 File Offset: 0x001F37D4
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally && GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				if (this.TeleportPoints.Count != 0)
				{
					Transform transform = this.TeleportPoints[Random.Range(0, this.TeleportPoints.Count)];
					if (transform != null)
					{
						instance.TeleportTo(transform, this.matchTeleportPointRotation, this.maintainVelocity);
					}
				}
			}
		}

		// Token: 0x04006FE9 RID: 28649
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		[NotNull]
		public List<Transform> TeleportPoints = new List<Transform>();

		// Token: 0x04006FEA RID: 28650
		public bool matchTeleportPointRotation;

		// Token: 0x04006FEB RID: 28651
		public bool maintainVelocity;
	}
}
