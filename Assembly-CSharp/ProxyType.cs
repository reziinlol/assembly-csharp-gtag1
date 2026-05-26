using System;
using System.Globalization;
using System.Reflection;

// Token: 0x02000DB9 RID: 3513
public class ProxyType : Type
{
	// Token: 0x0600560D RID: 22029 RVA: 0x001BFA5C File Offset: 0x001BDC5C
	public ProxyType()
	{
	}

	// Token: 0x0600560E RID: 22030 RVA: 0x001BFA74 File Offset: 0x001BDC74
	public ProxyType(string typeName)
	{
		this._typeName = typeName;
	}

	// Token: 0x17000816 RID: 2070
	// (get) Token: 0x0600560F RID: 22031 RVA: 0x001BFA93 File Offset: 0x001BDC93
	public override string Name
	{
		get
		{
			return this._typeName;
		}
	}

	// Token: 0x17000817 RID: 2071
	// (get) Token: 0x06005610 RID: 22032 RVA: 0x001BFA9B File Offset: 0x001BDC9B
	public override string FullName
	{
		get
		{
			return ProxyType.kPrefix + this._typeName;
		}
	}

	// Token: 0x06005611 RID: 22033 RVA: 0x001BFAB0 File Offset: 0x001BDCB0
	public static ProxyType Parse(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentNullException("input");
		}
		input = input.Trim();
		if (!input.Contains(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (!input.StartsWith(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (input.Contains(','))
		{
			input = input.Split(',', StringSplitOptions.None)[0];
		}
		string text = input.Split('.', StringSplitOptions.None)[1].Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return ProxyType.kInvalidType;
		}
		return new ProxyType(text);
	}

	// Token: 0x06005612 RID: 22034 RVA: 0x001BFB3C File Offset: 0x001BDD3C
	public override string ToString()
	{
		return base.ToString() + "." + this._typeName;
	}

	// Token: 0x06005613 RID: 22035 RVA: 0x001BFB54 File Offset: 0x001BDD54
	public override object[] GetCustomAttributes(bool inherit)
	{
		return this._self.GetCustomAttributes(inherit);
	}

	// Token: 0x06005614 RID: 22036 RVA: 0x001BFB62 File Offset: 0x001BDD62
	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return this._self.GetCustomAttributes(attributeType, inherit);
	}

	// Token: 0x06005615 RID: 22037 RVA: 0x001BFB71 File Offset: 0x001BDD71
	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return this._self.IsDefined(attributeType, inherit);
	}

	// Token: 0x17000818 RID: 2072
	// (get) Token: 0x06005616 RID: 22038 RVA: 0x001BFB80 File Offset: 0x001BDD80
	public override Module Module
	{
		get
		{
			return this._self.Module;
		}
	}

	// Token: 0x17000819 RID: 2073
	// (get) Token: 0x06005617 RID: 22039 RVA: 0x001BFB8D File Offset: 0x001BDD8D
	public override string Namespace
	{
		get
		{
			return this._self.Namespace;
		}
	}

	// Token: 0x06005618 RID: 22040 RVA: 0x00002076 File Offset: 0x00000276
	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return TypeAttributes.NotPublic;
	}

	// Token: 0x06005619 RID: 22041 RVA: 0x00035D0D File Offset: 0x00033F0D
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x0600561A RID: 22042 RVA: 0x001BFB9A File Offset: 0x001BDD9A
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return this._self.GetConstructors(bindingAttr);
	}

	// Token: 0x0600561B RID: 22043 RVA: 0x001BFBA8 File Offset: 0x001BDDA8
	public override Type GetElementType()
	{
		return this._self.GetElementType();
	}

	// Token: 0x0600561C RID: 22044 RVA: 0x001BFBB5 File Offset: 0x001BDDB5
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		return this._self.GetEvent(name, bindingAttr);
	}

	// Token: 0x0600561D RID: 22045 RVA: 0x001BFBC4 File Offset: 0x001BDDC4
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return this._self.GetEvents(bindingAttr);
	}

	// Token: 0x0600561E RID: 22046 RVA: 0x001BFBD2 File Offset: 0x001BDDD2
	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		return this._self.GetField(name, bindingAttr);
	}

	// Token: 0x0600561F RID: 22047 RVA: 0x001BFBE1 File Offset: 0x001BDDE1
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return this._self.GetFields(bindingAttr);
	}

	// Token: 0x06005620 RID: 22048 RVA: 0x001BFBEF File Offset: 0x001BDDEF
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		return this._self.GetMembers(bindingAttr);
	}

	// Token: 0x06005621 RID: 22049 RVA: 0x00035D0D File Offset: 0x00033F0D
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06005622 RID: 22050 RVA: 0x001BFBFD File Offset: 0x001BDDFD
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return this._self.GetMethods(bindingAttr);
	}

	// Token: 0x06005623 RID: 22051 RVA: 0x001BFC0B File Offset: 0x001BDE0B
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return this._self.GetProperties(bindingAttr);
	}

	// Token: 0x06005624 RID: 22052 RVA: 0x001BFC1C File Offset: 0x001BDE1C
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		return this._self.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
	}

	// Token: 0x1700081A RID: 2074
	// (get) Token: 0x06005625 RID: 22053 RVA: 0x001BFC41 File Offset: 0x001BDE41
	public override Type UnderlyingSystemType
	{
		get
		{
			return this._self.UnderlyingSystemType;
		}
	}

	// Token: 0x06005626 RID: 22054 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsArrayImpl()
	{
		return false;
	}

	// Token: 0x06005627 RID: 22055 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsByRefImpl()
	{
		return false;
	}

	// Token: 0x06005628 RID: 22056 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsCOMObjectImpl()
	{
		return false;
	}

	// Token: 0x06005629 RID: 22057 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPointerImpl()
	{
		return false;
	}

	// Token: 0x0600562A RID: 22058 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	// Token: 0x1700081B RID: 2075
	// (get) Token: 0x0600562B RID: 22059 RVA: 0x001BFC4E File Offset: 0x001BDE4E
	public override Assembly Assembly
	{
		get
		{
			return this._self.Assembly;
		}
	}

	// Token: 0x1700081C RID: 2076
	// (get) Token: 0x0600562C RID: 22060 RVA: 0x001BFC5B File Offset: 0x001BDE5B
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName.Replace("ProxyType", this.FullName);
		}
	}

	// Token: 0x1700081D RID: 2077
	// (get) Token: 0x0600562D RID: 22061 RVA: 0x001BFC78 File Offset: 0x001BDE78
	public override Type BaseType
	{
		get
		{
			return this._self.BaseType;
		}
	}

	// Token: 0x1700081E RID: 2078
	// (get) Token: 0x0600562E RID: 22062 RVA: 0x001BFC85 File Offset: 0x001BDE85
	public override Guid GUID
	{
		get
		{
			return this._self.GUID;
		}
	}

	// Token: 0x0600562F RID: 22063 RVA: 0x00035D0D File Offset: 0x00033F0D
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06005630 RID: 22064 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool HasElementTypeImpl()
	{
		return false;
	}

	// Token: 0x06005631 RID: 22065 RVA: 0x001BFC92 File Offset: 0x001BDE92
	public override Type GetNestedType(string name, BindingFlags bindingAttr)
	{
		return this._self.GetNestedType(name, bindingAttr);
	}

	// Token: 0x06005632 RID: 22066 RVA: 0x001BFCA1 File Offset: 0x001BDEA1
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return this._self.GetNestedTypes(bindingAttr);
	}

	// Token: 0x06005633 RID: 22067 RVA: 0x001BFCAF File Offset: 0x001BDEAF
	public override Type GetInterface(string name, bool ignoreCase)
	{
		return this._self.GetInterface(name, ignoreCase);
	}

	// Token: 0x06005634 RID: 22068 RVA: 0x001BFCBE File Offset: 0x001BDEBE
	public override Type[] GetInterfaces()
	{
		return this._self.GetInterfaces();
	}

	// Token: 0x04006609 RID: 26121
	private Type _self = typeof(ProxyType);

	// Token: 0x0400660A RID: 26122
	private readonly string _typeName;

	// Token: 0x0400660B RID: 26123
	private static readonly string kPrefix = "ProxyType.";

	// Token: 0x0400660C RID: 26124
	private static InvalidType kInvalidType = new InvalidType();
}
