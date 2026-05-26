using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200029F RID: 671
public class ApplyMaterialProperty : MonoBehaviour
{
	// Token: 0x060011A1 RID: 4513 RVA: 0x0005E744 File Offset: 0x0005C944
	private void Start()
	{
		this.UpdateShaderPropertyIds();
		if (this.applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0005E75C File Offset: 0x0005C95C
	public void Apply()
	{
		if (!this._renderer)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		ApplyMaterialProperty.ApplyMode applyMode = this.mode;
		if (applyMode == ApplyMaterialProperty.ApplyMode.MaterialInstance)
		{
			this.ApplyMaterialInstance();
			return;
		}
		if (applyMode != ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock)
		{
			return;
		}
		this.ApplyMaterialPropertyBlock();
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0005E79E File Offset: 0x0005C99E
	public void SetColor(string propertyName, Color color)
	{
		this.SetColor(Shader.PropertyToID(propertyName), color);
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x0005E7AD File Offset: 0x0005C9AD
	public void SetColor(int propertyId, Color color)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Color;
		orCreateData.color = color;
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0005E7C4 File Offset: 0x0005C9C4
	public void SetFloat(string propertyName, float value)
	{
		this.SetFloat(Shader.PropertyToID(propertyName), value);
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x0005E7D3 File Offset: 0x0005C9D3
	public void SetFloat(int propertyId, float value)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyId, null);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Float;
		orCreateData.@float = value;
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0005E7EC File Offset: 0x0005C9EC
	private ApplyMaterialProperty.CustomMaterialData GetOrCreateData(int id, string propertyName)
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i].id == id)
			{
				return this.customData[i];
			}
		}
		ApplyMaterialProperty.CustomMaterialData customMaterialData = new ApplyMaterialProperty.CustomMaterialData(id, propertyName);
		this.customData.Add(customMaterialData);
		return customMaterialData;
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x0005E848 File Offset: 0x0005CA48
	private void ApplyMaterialInstance()
	{
		if (!this._instance)
		{
			this._instance = base.GetComponent<MaterialInstance>();
			if (this._instance == null)
			{
				this._instance = base.gameObject.AddComponent<MaterialInstance>();
			}
		}
		Material material = this.targetMaterial = this._instance.Material;
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				material.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				material.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				material.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				material.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				material.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				material.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x0005EA14 File Offset: 0x0005CC14
	private void ApplyMaterialPropertyBlock()
	{
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		for (int i = 0; i < this.customData.Count; i++)
		{
			switch (this.customData[i].dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				this._block.SetColor(this.customData[i].id, this.customData[i].color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				this._block.SetFloat(this.customData[i].id, this.customData[i].@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				this._block.SetVector(this.customData[i].id, this.customData[i].vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				this._block.SetTexture(this.customData[i].id, this.customData[i].texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0005EBD4 File Offset: 0x0005CDD4
	private void UpdateShaderPropertyIds()
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			if (this.customData[i] != null && !string.IsNullOrEmpty(this.customData[i].name))
			{
				this.customData[i].id = Shader.PropertyToID(this.customData[i].name);
			}
		}
	}

	// Token: 0x0400151F RID: 5407
	public ApplyMaterialProperty.ApplyMode mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;

	// Token: 0x04001520 RID: 5408
	[FormerlySerializedAs("materialToApplyBlock")]
	public Material targetMaterial;

	// Token: 0x04001521 RID: 5409
	[SerializeField]
	private MaterialInstance _instance;

	// Token: 0x04001522 RID: 5410
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04001523 RID: 5411
	public List<ApplyMaterialProperty.CustomMaterialData> customData;

	// Token: 0x04001524 RID: 5412
	[SerializeField]
	private bool applyOnStart;

	// Token: 0x04001525 RID: 5413
	[NonSerialized]
	private MaterialPropertyBlock _block;

	// Token: 0x020002A0 RID: 672
	public enum ApplyMode
	{
		// Token: 0x04001527 RID: 5415
		MaterialInstance,
		// Token: 0x04001528 RID: 5416
		MaterialPropertyBlock
	}

	// Token: 0x020002A1 RID: 673
	public enum SuportedTypes
	{
		// Token: 0x0400152A RID: 5418
		Color,
		// Token: 0x0400152B RID: 5419
		Float,
		// Token: 0x0400152C RID: 5420
		Vector2,
		// Token: 0x0400152D RID: 5421
		Vector3,
		// Token: 0x0400152E RID: 5422
		Vector4,
		// Token: 0x0400152F RID: 5423
		Texture2D
	}

	// Token: 0x020002A2 RID: 674
	[Serializable]
	public class CustomMaterialData
	{
		// Token: 0x060011AC RID: 4524 RVA: 0x0005EC54 File Offset: 0x0005CE54
		public CustomMaterialData(string propertyName)
		{
			this.name = propertyName;
			this.id = Shader.PropertyToID(propertyName);
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x0005ECC4 File Offset: 0x0005CEC4
		public CustomMaterialData(int propertyId, string propertyName)
		{
			this.name = propertyName;
			this.id = propertyId;
			this.dataType = ApplyMaterialProperty.SuportedTypes.Color;
			this.color = default(Color);
			this.@float = 0f;
			this.vector2 = default(Vector2);
			this.vector3 = default(Vector3);
			this.vector4 = default(Vector4);
			this.texture2D = null;
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x0005ED30 File Offset: 0x0005CF30
		public override int GetHashCode()
		{
			return new ValueTuple<int, ApplyMaterialProperty.SuportedTypes, Color, float, Vector2, Vector3, Vector4, ValueTuple<Texture2D>>(this.id, this.dataType, this.color, this.@float, this.vector2, this.vector3, this.vector4, new ValueTuple<Texture2D>(this.texture2D)).GetHashCode();
		}

		// Token: 0x04001530 RID: 5424
		public string name;

		// Token: 0x04001531 RID: 5425
		public int id;

		// Token: 0x04001532 RID: 5426
		public ApplyMaterialProperty.SuportedTypes dataType;

		// Token: 0x04001533 RID: 5427
		public Color color;

		// Token: 0x04001534 RID: 5428
		public float @float;

		// Token: 0x04001535 RID: 5429
		public Vector2 vector2;

		// Token: 0x04001536 RID: 5430
		public Vector3 vector3;

		// Token: 0x04001537 RID: 5431
		public Vector4 vector4;

		// Token: 0x04001538 RID: 5432
		public Texture2D texture2D;
	}
}
