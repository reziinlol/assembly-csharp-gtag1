using System;
using System.Collections.Generic;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000F38 RID: 3896
	public class CMSObjectActivationTrigger : CMSTrigger
	{
		// Token: 0x06006123 RID: 24867 RVA: 0x001F489C File Offset: 0x001F2A9C
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(ObjectActivationTriggerSettings))
			{
				ObjectActivationTriggerSettings objectActivationTriggerSettings = (ObjectActivationTriggerSettings)settings;
				this.objectsToActivate = objectActivationTriggerSettings.objectsToActivate;
				this.objectsToDeactivate = objectActivationTriggerSettings.objectsToDeactivate;
				this.triggersToReset = objectActivationTriggerSettings.triggersToReset;
				this.onlyResetTriggerCount = objectActivationTriggerSettings.onlyResetTriggerCount;
			}
			for (int i = this.objectsToActivate.Count - 1; i >= 0; i--)
			{
				if (this.objectsToActivate[i] == null)
				{
					this.objectsToActivate.RemoveAt(i);
				}
			}
			for (int j = this.objectsToDeactivate.Count - 1; j >= 0; j--)
			{
				if (this.objectsToDeactivate[j] == null)
				{
					this.objectsToDeactivate.RemoveAt(j);
				}
			}
			for (int k = this.triggersToReset.Count - 1; k >= 0; k--)
			{
				if (this.triggersToReset[k] == null)
				{
					this.triggersToReset.RemoveAt(k);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x06006124 RID: 24868 RVA: 0x001F49A8 File Offset: 0x001F2BA8
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			foreach (GameObject gameObject in this.objectsToDeactivate)
			{
				if (gameObject.IsNotNull())
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.objectsToActivate)
			{
				if (gameObject2.IsNotNull())
				{
					gameObject2.SetActive(true);
				}
			}
			foreach (GameObject gameObject3 in this.triggersToReset)
			{
				if (!gameObject3.IsNull())
				{
					CMSTrigger[] components = gameObject3.GetComponents<CMSTrigger>();
					if (components != null)
					{
						CMSTrigger[] array = components;
						for (int i = 0; i < array.Length; i++)
						{
							array[i].ResetTrigger(this.onlyResetTriggerCount);
						}
					}
				}
			}
		}

		// Token: 0x04006FD8 RID: 28632
		public List<GameObject> objectsToActivate = new List<GameObject>();

		// Token: 0x04006FD9 RID: 28633
		public List<GameObject> objectsToDeactivate = new List<GameObject>();

		// Token: 0x04006FDA RID: 28634
		public List<GameObject> triggersToReset = new List<GameObject>();

		// Token: 0x04006FDB RID: 28635
		public bool onlyResetTriggerCount;
	}
}
