using System;
using System.IO;
using System.Text;

namespace GorillaTag.Scripts.Utilities
{
	// Token: 0x02001199 RID: 4505
	public class StringWriterWithEncoding : StringWriter
	{
		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x06007205 RID: 29189 RVA: 0x00251BA1 File Offset: 0x0024FDA1
		public override Encoding Encoding { get; }

		// Token: 0x06007206 RID: 29190 RVA: 0x00251BA9 File Offset: 0x0024FDA9
		public StringWriterWithEncoding(Encoding encoding)
		{
			this.Encoding = encoding;
		}
	}
}
