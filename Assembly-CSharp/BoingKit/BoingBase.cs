using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001350 RID: 4944
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x06007C89 RID: 31881 RVA: 0x0028DDE2 File Offset: 0x0028BFE2
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x06007C8A RID: 31882 RVA: 0x0028DDEA File Offset: 0x0028BFEA
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x06007C8B RID: 31883 RVA: 0x0028DDF2 File Offset: 0x0028BFF2
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x06007C8C RID: 31884 RVA: 0x0028DDFA File Offset: 0x0028BFFA
		protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
		{
			this.m_previousVersion = this.m_currentVersion;
			if (this.m_currentVersion.Revision < 33)
			{
				this.m_initialVersion = Version.Invalid;
				this.m_previousVersion = Version.Invalid;
			}
			this.m_currentVersion = newVersion;
		}

		// Token: 0x04008DB3 RID: 36275
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x04008DB4 RID: 36276
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x04008DB5 RID: 36277
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
