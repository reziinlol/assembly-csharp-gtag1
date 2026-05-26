using System;

namespace GorillaNetworking
{
	// Token: 0x02001057 RID: 4183
	public class GorillaKeyboardButton : GorillaKeyButton<GorillaKeyboardBindings>
	{
		// Token: 0x0600691A RID: 26906 RVA: 0x002205BE File Offset: 0x0021E7BE
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
