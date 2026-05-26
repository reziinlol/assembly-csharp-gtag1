using System;

namespace BoingKit
{
	// Token: 0x0200135C RID: 4956
	public struct Version : IEquatable<Version>
	{
		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x06007CCE RID: 31950 RVA: 0x0028FE60 File Offset: 0x0028E060
		public readonly int MajorVersion { get; }

		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06007CCF RID: 31951 RVA: 0x0028FE68 File Offset: 0x0028E068
		public readonly int MinorVersion { get; }

		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x06007CD0 RID: 31952 RVA: 0x0028FE70 File Offset: 0x0028E070
		public readonly int Revision { get; }

		// Token: 0x06007CD1 RID: 31953 RVA: 0x0028FE78 File Offset: 0x0028E078
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.MajorVersion.ToString(),
				".",
				this.MinorVersion.ToString(),
				".",
				this.Revision.ToString()
			});
		}

		// Token: 0x06007CD2 RID: 31954 RVA: 0x0028FED3 File Offset: 0x0028E0D3
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x06007CD3 RID: 31955 RVA: 0x0028FEF5 File Offset: 0x0028E0F5
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x06007CD4 RID: 31956 RVA: 0x0028FF0C File Offset: 0x0028E10C
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x06007CD5 RID: 31957 RVA: 0x0028FF61 File Offset: 0x0028E161
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x06007CD6 RID: 31958 RVA: 0x0028FF6D File Offset: 0x0028E16D
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x06007CD7 RID: 31959 RVA: 0x0028FF85 File Offset: 0x0028E185
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x06007CD8 RID: 31960 RVA: 0x0028FFB8 File Offset: 0x0028E1B8
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x04008E4F RID: 36431
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x04008E50 RID: 36432
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x04008E51 RID: 36433
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
