using System;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000F36 RID: 3894
	public class CMSLuau : CMSTrigger
	{
		// Token: 0x0600611E RID: 24862 RVA: 0x001F4762 File Offset: 0x001F2962
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				LuauVm.touchEventsQueue.Enqueue(base.gameObject);
			}
		}
	}
}
