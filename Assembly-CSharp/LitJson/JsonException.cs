using System;

namespace LitJson
{
	// Token: 0x02000E74 RID: 3700
	public class JsonException : ApplicationException
	{
		// Token: 0x06005ABA RID: 23226 RVA: 0x001CEBE5 File Offset: 0x001CCDE5
		public JsonException()
		{
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x001CEBED File Offset: 0x001CCDED
		internal JsonException(ParserToken token) : base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x06005ABC RID: 23228 RVA: 0x001CEC05 File Offset: 0x001CCE05
		internal JsonException(ParserToken token, Exception inner_exception) : base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x06005ABD RID: 23229 RVA: 0x001CEC1E File Offset: 0x001CCE1E
		internal JsonException(int c) : base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x06005ABE RID: 23230 RVA: 0x001CEC37 File Offset: 0x001CCE37
		internal JsonException(int c, Exception inner_exception) : base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x06005ABF RID: 23231 RVA: 0x001CEC51 File Offset: 0x001CCE51
		public JsonException(string message) : base(message)
		{
		}

		// Token: 0x06005AC0 RID: 23232 RVA: 0x001CEC5A File Offset: 0x001CCE5A
		public JsonException(string message, Exception inner_exception) : base(message, inner_exception)
		{
		}
	}
}
