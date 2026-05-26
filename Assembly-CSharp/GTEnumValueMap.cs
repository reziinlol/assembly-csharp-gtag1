using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029C RID: 668
[Serializable]
public class GTEnumValueMap<T> : ISerializationCallbackReceiver
{
	// Token: 0x06001197 RID: 4503 RVA: 0x0005E621 File Offset: 0x0005C821
	public bool TryGet(long i, out T o)
	{
		return this._enumValue_to_unityObject.TryGetValue(i, out o);
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06001198 RID: 4504 RVA: 0x0005E630 File Offset: 0x0005C830
	public IEnumerable<T> Values
	{
		get
		{
			return this._enumValue_to_unityObject.Values;
		}
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0005E63D File Offset: 0x0005C83D
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		this.Init();
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0005E648 File Offset: 0x0005C848
	public void Init()
	{
		if (this.m_enumValueAndUnityObjectPairs == null)
		{
			return;
		}
		if (this._enumValue_to_unityObject == null)
		{
			this._enumValue_to_unityObject = new Dictionary<long, T>();
		}
		this._enumValue_to_unityObject.Clear();
		foreach (GTEnumValueMap<T>.EnumValueToUnityObject enumValueToUnityObject in this.m_enumValueAndUnityObjectPairs)
		{
			if (enumValueToUnityObject.enabled && enumValueToUnityObject.value != null)
			{
				this._enumValue_to_unityObject[enumValueToUnityObject.enumKey] = enumValueToUnityObject.value;
			}
		}
		if (!Application.isEditor)
		{
			this.m_enumScriptGuid = null;
			this.m_enumValueAndUnityObjectPairs = null;
		}
	}

	// Token: 0x04001517 RID: 5399
	private Dictionary<long, T> _enumValue_to_unityObject = new Dictionary<long, T>();

	// Token: 0x04001518 RID: 5400
	[Tooltip("The GUID to the Enum script asset which is what is serialized in editor (not used at runtime). This is exposed and editable as a precaution but shouldn't be necessary to have to use.")]
	[SerializeField]
	private string m_enumScriptGuid;

	// Token: 0x04001519 RID: 5401
	[SerializeField]
	private List<GTEnumValueMap<T>.EnumValueToUnityObject> m_enumValueAndUnityObjectPairs = new List<GTEnumValueMap<T>.EnumValueToUnityObject>();

	// Token: 0x0200029D RID: 669
	[Serializable]
	private struct EnumValueToUnityObject
	{
		// Token: 0x0400151A RID: 5402
		public bool enabled;

		// Token: 0x0400151B RID: 5403
		public long enumKey;

		// Token: 0x0400151C RID: 5404
		public string enumName;

		// Token: 0x0400151D RID: 5405
		public T value;
	}
}
