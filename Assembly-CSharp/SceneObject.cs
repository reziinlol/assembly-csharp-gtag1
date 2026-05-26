using System;
using UnityEngine;

// Token: 0x02000DBA RID: 3514
[Serializable]
public class SceneObject : IEquatable<SceneObject>
{
	// Token: 0x06005636 RID: 22070 RVA: 0x001BFCE1 File Offset: 0x001BDEE1
	public Type GetObjectType()
	{
		if (string.IsNullOrWhiteSpace(this.typeString))
		{
			return null;
		}
		if (this.typeString.Contains("ProxyType"))
		{
			return ProxyType.Parse(this.typeString);
		}
		return Type.GetType(this.typeString);
	}

	// Token: 0x06005637 RID: 22071 RVA: 0x001BFD1B File Offset: 0x001BDF1B
	public SceneObject(int classID, ulong fileID)
	{
		this.classID = classID;
		this.fileID = fileID;
		this.typeString = UnityYaml.ClassIDToType[classID].AssemblyQualifiedName;
	}

	// Token: 0x06005638 RID: 22072 RVA: 0x001BFD47 File Offset: 0x001BDF47
	public bool Equals(SceneObject other)
	{
		return this.fileID == other.fileID && this.classID == other.classID;
	}

	// Token: 0x06005639 RID: 22073 RVA: 0x001BFD68 File Offset: 0x001BDF68
	public override bool Equals(object obj)
	{
		SceneObject sceneObject = obj as SceneObject;
		return sceneObject != null && this.Equals(sceneObject);
	}

	// Token: 0x0600563A RID: 22074 RVA: 0x001BFD88 File Offset: 0x001BDF88
	public override int GetHashCode()
	{
		int i = this.classID;
		int i2 = StaticHash.Compute((long)this.fileID);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x0600563B RID: 22075 RVA: 0x001BFDAD File Offset: 0x001BDFAD
	public static bool operator ==(SceneObject x, SceneObject y)
	{
		return x.Equals(y);
	}

	// Token: 0x0600563C RID: 22076 RVA: 0x001BFDB6 File Offset: 0x001BDFB6
	public static bool operator !=(SceneObject x, SceneObject y)
	{
		return !x.Equals(y);
	}

	// Token: 0x0400660D RID: 26125
	public int classID;

	// Token: 0x0400660E RID: 26126
	public ulong fileID;

	// Token: 0x0400660F RID: 26127
	[SerializeField]
	public string typeString;

	// Token: 0x04006610 RID: 26128
	public string json;
}
