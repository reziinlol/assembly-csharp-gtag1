using System;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class GTContactManager : MonoBehaviour
{
	// Token: 0x0600143A RID: 5178 RVA: 0x000028C5 File Offset: 0x00000AC5
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0006CAFC File Offset: 0x0006ACFC
	private static GTContactPoint[] InitContactPoints(int count)
	{
		GTContactPoint[] array = new GTContactPoint[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new GTContactPoint();
		}
		return array;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x0006CB28 File Offset: 0x0006AD28
	public static void RaiseContact(Vector3 point, Vector3 normal)
	{
		if (GTContactManager.gNextFree == -1)
		{
			return;
		}
		float time = GTShaderGlobals.Time;
		GTContactPoint gtcontactPoint = GTContactManager._gContactPoints[GTContactManager.gNextFree];
		gtcontactPoint.contactPoint = point;
		gtcontactPoint.radius = 0.04f;
		gtcontactPoint.counterVelocity = normal;
		gtcontactPoint.timestamp = time;
		gtcontactPoint.lifetime = 2f;
		gtcontactPoint.color = GTContactManager.gRND.NextColor();
		gtcontactPoint.free = 0U;
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0006CB90 File Offset: 0x0006AD90
	public static void ProcessContacts()
	{
		Matrix4x4[] shaderData = GTContactManager.ShaderData;
		GTContactPoint[] gContactPoints = GTContactManager._gContactPoints;
		int frame = GTShaderGlobals.Frame;
		for (int i = 0; i < 32; i++)
		{
			GTContactManager.Transfer(ref gContactPoints[i].data, ref shaderData[i]);
		}
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x0006CBD0 File Offset: 0x0006ADD0
	private static void Transfer(ref Matrix4x4 from, ref Matrix4x4 to)
	{
		to.m00 = from.m00;
		to.m01 = from.m01;
		to.m02 = from.m02;
		to.m03 = from.m03;
		to.m10 = from.m10;
		to.m11 = from.m11;
		to.m12 = from.m12;
		to.m13 = from.m13;
		to.m20 = from.m20;
		to.m21 = from.m21;
		to.m22 = from.m22;
		to.m23 = from.m23;
		to.m30 = from.m30;
		to.m31 = from.m31;
		to.m32 = from.m32;
		to.m33 = from.m33;
	}

	// Token: 0x0400190D RID: 6413
	public const int MAX_CONTACTS = 32;

	// Token: 0x0400190E RID: 6414
	public static Matrix4x4[] ShaderData = new Matrix4x4[32];

	// Token: 0x0400190F RID: 6415
	private static GTContactPoint[] _gContactPoints = GTContactManager.InitContactPoints(32);

	// Token: 0x04001910 RID: 6416
	private static int gNextFree = 0;

	// Token: 0x04001911 RID: 6417
	private static SRand gRND = new SRand(DateTime.UtcNow);
}
