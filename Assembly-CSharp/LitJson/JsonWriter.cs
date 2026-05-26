using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000E85 RID: 3717
	public class JsonWriter
	{
		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x06005B30 RID: 23344 RVA: 0x001D0C20 File Offset: 0x001CEE20
		// (set) Token: 0x06005B31 RID: 23345 RVA: 0x001D0C28 File Offset: 0x001CEE28
		public int IndentValue
		{
			get
			{
				return this.indent_value;
			}
			set
			{
				this.indentation = this.indentation / this.indent_value * value;
				this.indent_value = value;
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x06005B32 RID: 23346 RVA: 0x001D0C46 File Offset: 0x001CEE46
		// (set) Token: 0x06005B33 RID: 23347 RVA: 0x001D0C4E File Offset: 0x001CEE4E
		public bool PrettyPrint
		{
			get
			{
				return this.pretty_print;
			}
			set
			{
				this.pretty_print = value;
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x06005B34 RID: 23348 RVA: 0x001D0C57 File Offset: 0x001CEE57
		public TextWriter TextWriter
		{
			get
			{
				return this.writer;
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x06005B35 RID: 23349 RVA: 0x001D0C5F File Offset: 0x001CEE5F
		// (set) Token: 0x06005B36 RID: 23350 RVA: 0x001D0C67 File Offset: 0x001CEE67
		public bool Validate
		{
			get
			{
				return this.validate;
			}
			set
			{
				this.validate = value;
			}
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x001D0C7C File Offset: 0x001CEE7C
		public JsonWriter()
		{
			this.inst_string_builder = new StringBuilder();
			this.writer = new StringWriter(this.inst_string_builder);
			this.Init();
		}

		// Token: 0x06005B39 RID: 23353 RVA: 0x001D0CA6 File Offset: 0x001CEEA6
		public JsonWriter(StringBuilder sb) : this(new StringWriter(sb))
		{
		}

		// Token: 0x06005B3A RID: 23354 RVA: 0x001D0CB4 File Offset: 0x001CEEB4
		public JsonWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.Init();
		}

		// Token: 0x06005B3B RID: 23355 RVA: 0x001D0CD8 File Offset: 0x001CEED8
		private void DoValidation(Condition cond)
		{
			if (!this.context.ExpectingValue)
			{
				this.context.Count++;
			}
			if (!this.validate)
			{
				return;
			}
			if (this.has_reached_end)
			{
				throw new JsonException("A complete JSON symbol has already been written");
			}
			switch (cond)
			{
			case Condition.InArray:
				if (!this.context.InArray)
				{
					throw new JsonException("Can't close an array here");
				}
				break;
			case Condition.InObject:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't close an object here");
				}
				break;
			case Condition.NotAProperty:
				if (this.context.InObject && !this.context.ExpectingValue)
				{
					throw new JsonException("Expected a property");
				}
				break;
			case Condition.Property:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't add a property here");
				}
				break;
			case Condition.Value:
				if (!this.context.InArray && (!this.context.InObject || !this.context.ExpectingValue))
				{
					throw new JsonException("Can't add a value here");
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005B3C RID: 23356 RVA: 0x001D0DFC File Offset: 0x001CEFFC
		private void Init()
		{
			this.has_reached_end = false;
			this.hex_seq = new char[4];
			this.indentation = 0;
			this.indent_value = 4;
			this.pretty_print = false;
			this.validate = true;
			this.ctx_stack = new Stack<WriterContext>();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
		}

		// Token: 0x06005B3D RID: 23357 RVA: 0x001D0E60 File Offset: 0x001CF060
		private static void IntToHex(int n, char[] hex)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = n % 16;
				if (num < 10)
				{
					hex[3 - i] = (char)(48 + num);
				}
				else
				{
					hex[3 - i] = (char)(65 + (num - 10));
				}
				n >>= 4;
			}
		}

		// Token: 0x06005B3E RID: 23358 RVA: 0x001D0EA1 File Offset: 0x001CF0A1
		private void Indent()
		{
			if (this.pretty_print)
			{
				this.indentation += this.indent_value;
			}
		}

		// Token: 0x06005B3F RID: 23359 RVA: 0x001D0EC0 File Offset: 0x001CF0C0
		private void Put(string str)
		{
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				for (int i = 0; i < this.indentation; i++)
				{
					this.writer.Write(' ');
				}
			}
			this.writer.Write(str);
		}

		// Token: 0x06005B40 RID: 23360 RVA: 0x001D0F0C File Offset: 0x001CF10C
		private void PutNewline()
		{
			this.PutNewline(true);
		}

		// Token: 0x06005B41 RID: 23361 RVA: 0x001D0F18 File Offset: 0x001CF118
		private void PutNewline(bool add_comma)
		{
			if (add_comma && !this.context.ExpectingValue && this.context.Count > 1)
			{
				this.writer.Write(',');
			}
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				this.writer.Write('\n');
			}
		}

		// Token: 0x06005B42 RID: 23362 RVA: 0x001D0F74 File Offset: 0x001CF174
		private void PutString(string str)
		{
			this.Put(string.Empty);
			this.writer.Write('"');
			int length = str.Length;
			int i = 0;
			while (i < length)
			{
				char c = str[i];
				switch (c)
				{
				case '\b':
					this.writer.Write("\\b");
					break;
				case '\t':
					this.writer.Write("\\t");
					break;
				case '\n':
					this.writer.Write("\\n");
					break;
				case '\v':
					goto IL_E4;
				case '\f':
					this.writer.Write("\\f");
					break;
				case '\r':
					this.writer.Write("\\r");
					break;
				default:
					if (c != '"' && c != '\\')
					{
						goto IL_E4;
					}
					this.writer.Write('\\');
					this.writer.Write(str[i]);
					break;
				}
				IL_141:
				i++;
				continue;
				IL_E4:
				if (str[i] >= ' ' && str[i] <= '~')
				{
					this.writer.Write(str[i]);
					goto IL_141;
				}
				JsonWriter.IntToHex((int)str[i], this.hex_seq);
				this.writer.Write("\\u");
				this.writer.Write(this.hex_seq);
				goto IL_141;
			}
			this.writer.Write('"');
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x001D10DA File Offset: 0x001CF2DA
		private void Unindent()
		{
			if (this.pretty_print)
			{
				this.indentation -= this.indent_value;
			}
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x001D10F7 File Offset: 0x001CF2F7
		public override string ToString()
		{
			if (this.inst_string_builder == null)
			{
				return string.Empty;
			}
			return this.inst_string_builder.ToString();
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001D1114 File Offset: 0x001CF314
		public void Reset()
		{
			this.has_reached_end = false;
			this.ctx_stack.Clear();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
			if (this.inst_string_builder != null)
			{
				this.inst_string_builder.Remove(0, this.inst_string_builder.Length);
			}
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x001D116F File Offset: 0x001CF36F
		public void Write(bool boolean)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(boolean ? "true" : "false");
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x001D119F File Offset: 0x001CF39F
		public void Write(decimal number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x001D11CC File Offset: 0x001CF3CC
		public void Write(double number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			string text = Convert.ToString(number, JsonWriter.number_format);
			this.Put(text);
			if (text.IndexOf('.') == -1 && text.IndexOf('E') == -1)
			{
				this.writer.Write(".0");
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B49 RID: 23369 RVA: 0x001D122B File Offset: 0x001CF42B
		public void Write(int number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x001D1257 File Offset: 0x001CF457
		public void Write(long number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001D1283 File Offset: 0x001CF483
		public void Write(string str)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			if (str == null)
			{
				this.Put("null");
			}
			else
			{
				this.PutString(str);
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001D12B5 File Offset: 0x001CF4B5
		public void Write(ulong number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005B4D RID: 23373 RVA: 0x001D12E4 File Offset: 0x001CF4E4
		public void WriteArrayEnd()
		{
			this.DoValidation(Condition.InArray);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("]");
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x001D1350 File Offset: 0x001CF550
		public void WriteArrayStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("[");
			this.context = new WriterContext();
			this.context.InArray = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x06005B4F RID: 23375 RVA: 0x001D13A4 File Offset: 0x001CF5A4
		public void WriteObjectEnd()
		{
			this.DoValidation(Condition.InObject);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("}");
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x001D1410 File Offset: 0x001CF610
		public void WriteObjectStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("{");
			this.context = new WriterContext();
			this.context.InObject = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x06005B51 RID: 23377 RVA: 0x001D1464 File Offset: 0x001CF664
		public void WritePropertyName(string property_name)
		{
			this.DoValidation(Condition.Property);
			this.PutNewline();
			this.PutString(property_name);
			if (this.pretty_print)
			{
				if (property_name.Length > this.context.Padding)
				{
					this.context.Padding = property_name.Length;
				}
				for (int i = this.context.Padding - property_name.Length; i >= 0; i--)
				{
					this.writer.Write(' ');
				}
				this.writer.Write(": ");
			}
			else
			{
				this.writer.Write(':');
			}
			this.context.ExpectingValue = true;
		}

		// Token: 0x04006A1B RID: 27163
		private static NumberFormatInfo number_format = NumberFormatInfo.InvariantInfo;

		// Token: 0x04006A1C RID: 27164
		private WriterContext context;

		// Token: 0x04006A1D RID: 27165
		private Stack<WriterContext> ctx_stack;

		// Token: 0x04006A1E RID: 27166
		private bool has_reached_end;

		// Token: 0x04006A1F RID: 27167
		private char[] hex_seq;

		// Token: 0x04006A20 RID: 27168
		private int indentation;

		// Token: 0x04006A21 RID: 27169
		private int indent_value;

		// Token: 0x04006A22 RID: 27170
		private StringBuilder inst_string_builder;

		// Token: 0x04006A23 RID: 27171
		private bool pretty_print;

		// Token: 0x04006A24 RID: 27172
		private bool validate;

		// Token: 0x04006A25 RID: 27173
		private TextWriter writer;
	}
}
