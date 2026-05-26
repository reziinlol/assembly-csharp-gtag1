using System;
using System.Collections.Generic;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000F39 RID: 3897
	public class CMSPlayAnimationTrigger : CMSTrigger
	{
		// Token: 0x06006126 RID: 24870 RVA: 0x001F4AF8 File Offset: 0x001F2CF8
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(PlayAnimationTriggerSettings))
			{
				PlayAnimationTriggerSettings playAnimationTriggerSettings = (PlayAnimationTriggerSettings)settings;
				this.animatedObjects = playAnimationTriggerSettings.animatedObjects;
				this.animationName = playAnimationTriggerSettings.animationName;
			}
			for (int i = this.animatedObjects.Count - 1; i >= 0; i--)
			{
				if (this.animatedObjects[i].IsNull())
				{
					this.animatedObjects.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x06006127 RID: 24871 RVA: 0x001F4B7C File Offset: 0x001F2D7C
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			foreach (GameObject gameObject in this.animatedObjects)
			{
				Animator component = gameObject.GetComponent<Animator>();
				if (component.IsNotNull())
				{
					component.Play(this.animationName);
				}
			}
		}

		// Token: 0x04006FDC RID: 28636
		public List<GameObject> animatedObjects = new List<GameObject>();

		// Token: 0x04006FDD RID: 28637
		public string animationName = "";
	}
}
