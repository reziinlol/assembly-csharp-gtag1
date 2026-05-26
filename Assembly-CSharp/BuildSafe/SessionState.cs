using System;

namespace BuildSafe
{
	// Token: 0x02001014 RID: 4116
	public class SessionState
	{
		// Token: 0x170009A8 RID: 2472
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x04007609 RID: 30217
		public static readonly SessionState Shared = new SessionState();
	}
}
