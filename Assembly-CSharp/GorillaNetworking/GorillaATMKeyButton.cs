using System;

namespace GorillaNetworking
{
	// Token: 0x02001048 RID: 4168
	public class GorillaATMKeyButton : GorillaKeyButton<GorillaATMKeyBindings>
	{
		// Token: 0x06006837 RID: 26679 RVA: 0x0021A10D File Offset: 0x0021830D
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaATMKeyButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
