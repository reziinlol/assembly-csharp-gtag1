using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000A1A RID: 2586
[Serializable]
public sealed class ComponentFunctionReference<TResult>
{
	// Token: 0x17000626 RID: 1574
	// (get) Token: 0x06004230 RID: 16944 RVA: 0x00161C70 File Offset: 0x0015FE70
	public bool IsValid
	{
		get
		{
			return this._selection.component || !string.IsNullOrEmpty(this._selection.methodName);
		}
	}

	// Token: 0x06004231 RID: 16945 RVA: 0x00161C99 File Offset: 0x0015FE99
	private IEnumerable<ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>> GetMethodOptions()
	{
		if (this._target == null)
		{
			yield break;
		}
		yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>("NONE", default(ComponentFunctionReference<TResult>.MethodRef));
		Type type = typeof(GameObject);
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (MethodInfo methodInfo in type.GetMethods(flags))
		{
			if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(TResult))
			{
				string text = type.Name + "/" + methodInfo.Name;
				yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text, new ComponentFunctionReference<TResult>.MethodRef(this._target, methodInfo));
			}
		}
		MethodInfo[] array = null;
		foreach (Component comp in this._target.GetComponents<Component>())
		{
			type = comp.GetType();
			foreach (MethodInfo methodInfo2 in type.GetMethods(flags))
			{
				if (methodInfo2.GetParameters().Length == 0 && methodInfo2.ReturnType == typeof(TResult))
				{
					string text2 = type.Name + "/" + methodInfo2.Name;
					yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text2, new ComponentFunctionReference<TResult>.MethodRef(comp, methodInfo2));
				}
			}
			array = null;
			comp = null;
		}
		Component[] array2 = null;
		yield break;
	}

	// Token: 0x06004232 RID: 16946 RVA: 0x00161CAC File Offset: 0x0015FEAC
	public TResult Invoke()
	{
		if (this._cached == null)
		{
			this.Cache();
		}
		if (this._cached == null)
		{
			return default(TResult);
		}
		return this._cached();
	}

	// Token: 0x06004233 RID: 16947 RVA: 0x00161CE4 File Offset: 0x0015FEE4
	public void Cache()
	{
		this._cached = null;
		if (this._selection.component == null || string.IsNullOrEmpty(this._selection.methodName))
		{
			return;
		}
		MethodInfo method = this._selection.component.GetType().GetMethod(this._selection.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
		if (method != null)
		{
			this._cached = (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), this._selection.component, method);
		}
	}

	// Token: 0x0400540B RID: 21515
	[SerializeField]
	private GameObject _target;

	// Token: 0x0400540C RID: 21516
	[SerializeField]
	private ComponentFunctionReference<TResult>.MethodRef _selection;

	// Token: 0x0400540D RID: 21517
	private Func<TResult> _cached;

	// Token: 0x02000A1B RID: 2587
	[Serializable]
	private struct MethodRef
	{
		// Token: 0x06004235 RID: 16949 RVA: 0x00161D77 File Offset: 0x0015FF77
		public MethodRef(Object obj, MethodInfo m)
		{
			this.component = obj;
			this.methodName = m.Name;
		}

		// Token: 0x0400540E RID: 21518
		public Object component;

		// Token: 0x0400540F RID: 21519
		public string methodName;
	}
}
