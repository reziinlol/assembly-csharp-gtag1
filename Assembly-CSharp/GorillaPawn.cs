using System;
using UnityEngine;

// Token: 0x02000888 RID: 2184
[Obsolete]
public class GorillaPawn : MonoBehaviour
{
	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x060038FD RID: 14589 RVA: 0x001375AE File Offset: 0x001357AE
	public VRRig rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x060038FE RID: 14590 RVA: 0x001375B6 File Offset: 0x001357B6
	public ZoneEntityBSP zoneEntity
	{
		get
		{
			return this._zoneEntity;
		}
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x060038FF RID: 14591 RVA: 0x001375BE File Offset: 0x001357BE
	public new Transform transform
	{
		get
		{
			return this._transform;
		}
	}

	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x06003900 RID: 14592 RVA: 0x001375C6 File Offset: 0x001357C6
	public XformNode handLeft
	{
		get
		{
			return this._handLeftXform;
		}
	}

	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x06003901 RID: 14593 RVA: 0x001375CE File Offset: 0x001357CE
	public XformNode handRight
	{
		get
		{
			return this._handRightXform;
		}
	}

	// Token: 0x1700050C RID: 1292
	// (get) Token: 0x06003902 RID: 14594 RVA: 0x001375D6 File Offset: 0x001357D6
	public XformNode body
	{
		get
		{
			return this._bodyXform;
		}
	}

	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x06003903 RID: 14595 RVA: 0x001375DE File Offset: 0x001357DE
	public XformNode head
	{
		get
		{
			return this._headXform;
		}
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x001375E6 File Offset: 0x001357E6
	private void Awake()
	{
		this.Setup(false);
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x001375F0 File Offset: 0x001357F0
	private void Setup(bool force)
	{
		this._transform = base.transform;
		this._rig = base.GetComponentInChildren<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._zoneEntity = this._rig.zoneEntity;
		bool flag = force || this._handLeft.AsNull<Transform>() == null;
		bool flag2 = force || this._handRight.AsNull<Transform>() == null;
		bool flag3 = force || this._head.AsNull<Transform>() == null;
		if (!flag && !flag2 && !flag3)
		{
			return;
		}
		foreach (Transform transform in this._rig.mainSkin.bones)
		{
			string name = transform.name;
			if (flag3 && name.StartsWith("head", StringComparison.OrdinalIgnoreCase))
			{
				this._head = transform;
				this._headXform = new XformNode();
				this._headXform.localPosition = new Vector3(0f, 0.13f, 0.015f);
				this._headXform.radius = 0.12f;
				this._headXform.parent = transform;
			}
			else if (flag && name.StartsWith("hand.L", StringComparison.OrdinalIgnoreCase))
			{
				this._handLeft = transform;
				this._handLeftXform = new XformNode();
				this._handLeftXform.localPosition = new Vector3(-0.014f, 0.034f, 0f);
				this._handLeftXform.radius = 0.044f;
				this._handLeftXform.parent = transform;
			}
			else if (flag2 && name.StartsWith("hand.R", StringComparison.OrdinalIgnoreCase))
			{
				this._handRight = transform;
				this._handRightXform = new XformNode();
				this._handRightXform.localPosition = new Vector3(0.014f, 0.034f, 0f);
				this._handRightXform.radius = 0.044f;
				this._handRightXform.parent = transform;
			}
		}
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x001377F7 File Offset: 0x001359F7
	private bool CanRun()
	{
		if (GorillaPawn._gPawnActiveCount > 10)
		{
			Debug.LogError(string.Format("Cannot register more than {0} pawns.", 10));
			return false;
		}
		return true;
	}

	// Token: 0x06003907 RID: 14599 RVA: 0x0013781C File Offset: 0x00135A1C
	private void OnEnable()
	{
		if (!this.CanRun())
		{
			return;
		}
		this._id = -1;
		if (this._rig && this._rig.OwningNetPlayer != null)
		{
			this._id = this._rig.OwningNetPlayer.ActorNumber;
		}
		this._index = GorillaPawn._gPawnActiveCount++;
		GorillaPawn._gPawns[this._index] = this;
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x0013788C File Offset: 0x00135A8C
	private void OnDisable()
	{
		this._id = -1;
		if (!this.CanRun())
		{
			return;
		}
		if (this._index < 0 || this._index >= GorillaPawn._gPawnActiveCount - 1)
		{
			return;
		}
		int num = --GorillaPawn._gPawnActiveCount;
		GorillaPawn._gPawns.Swap(this._index, num);
		this._index = num;
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x001378E8 File Offset: 0x00135AE8
	private void OnDestroy()
	{
		int num = GorillaPawn._gPawns.IndexOfRef(this);
		GorillaPawn._gPawns[num] = null;
		Array.Sort<GorillaPawn>(GorillaPawn._gPawns, new Comparison<GorillaPawn>(GorillaPawn.ComparePawns));
		int num2 = 0;
		while (num2 < GorillaPawn._gPawns.Length && GorillaPawn._gPawns[num2])
		{
			num2++;
		}
		GorillaPawn._gPawnActiveCount = num2;
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x00137948 File Offset: 0x00135B48
	private static int ComparePawns(GorillaPawn x, GorillaPawn y)
	{
		bool flag = x.AsNull<GorillaPawn>() == null;
		bool flag2 = y.AsNull<GorillaPawn>() == null;
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			return 1;
		}
		if (flag2)
		{
			return -1;
		}
		return x._index.CompareTo(y._index);
	}

	// Token: 0x1700050E RID: 1294
	// (get) Token: 0x0600390B RID: 14603 RVA: 0x00137991 File Offset: 0x00135B91
	public static GorillaPawn[] AllPawns
	{
		get
		{
			return GorillaPawn._gPawns;
		}
	}

	// Token: 0x1700050F RID: 1295
	// (get) Token: 0x0600390C RID: 14604 RVA: 0x00137998 File Offset: 0x00135B98
	public static int ActiveCount
	{
		get
		{
			return GorillaPawn._gPawnActiveCount;
		}
	}

	// Token: 0x17000510 RID: 1296
	// (get) Token: 0x0600390D RID: 14605 RVA: 0x0013799F File Offset: 0x00135B9F
	public static Matrix4x4[] ShaderData
	{
		get
		{
			return GorillaPawn._gShaderData;
		}
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x001379A8 File Offset: 0x00135BA8
	public static void SyncPawnData()
	{
		Matrix4x4[] gShaderData = GorillaPawn._gShaderData;
		m4x4 m4x = default(m4x4);
		for (int i = 0; i < GorillaPawn._gPawnActiveCount; i++)
		{
			GorillaPawn gorillaPawn = GorillaPawn._gPawns[i];
			Vector4 worldPosition = gorillaPawn._headXform.worldPosition;
			Vector4 worldPosition2 = gorillaPawn._bodyXform.worldPosition;
			Vector4 worldPosition3 = gorillaPawn._handLeftXform.worldPosition;
			Vector4 worldPosition4 = gorillaPawn._handRightXform.worldPosition;
			m4x.SetRow0(ref worldPosition);
			m4x.SetRow1(ref worldPosition2);
			m4x.SetRow2(ref worldPosition3);
			m4x.SetRow3(ref worldPosition4);
			m4x.Push(ref gShaderData[i]);
		}
		for (int j = GorillaPawn._gPawnActiveCount; j < 10; j++)
		{
			MatrixUtils.Clear(ref gShaderData[j]);
		}
	}

	// Token: 0x04004901 RID: 18689
	[SerializeField]
	private Transform _transform;

	// Token: 0x04004902 RID: 18690
	[SerializeField]
	private Transform _handLeft;

	// Token: 0x04004903 RID: 18691
	[SerializeField]
	private Transform _handRight;

	// Token: 0x04004904 RID: 18692
	[SerializeField]
	private Transform _head;

	// Token: 0x04004905 RID: 18693
	[Space]
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04004906 RID: 18694
	[SerializeField]
	private ZoneEntityBSP _zoneEntity;

	// Token: 0x04004907 RID: 18695
	[Space]
	[SerializeField]
	private XformNode _handLeftXform;

	// Token: 0x04004908 RID: 18696
	[SerializeField]
	private XformNode _handRightXform;

	// Token: 0x04004909 RID: 18697
	[SerializeField]
	private XformNode _bodyXform;

	// Token: 0x0400490A RID: 18698
	[SerializeField]
	private XformNode _headXform;

	// Token: 0x0400490B RID: 18699
	[Space]
	private int _id;

	// Token: 0x0400490C RID: 18700
	private int _index;

	// Token: 0x0400490D RID: 18701
	private bool _invalid;

	// Token: 0x0400490E RID: 18702
	public const int MAX_PAWNS = 10;

	// Token: 0x0400490F RID: 18703
	private static GorillaPawn[] _gPawns = new GorillaPawn[10];

	// Token: 0x04004910 RID: 18704
	private static int _gPawnActiveCount = 0;

	// Token: 0x04004911 RID: 18705
	private static Matrix4x4[] _gShaderData = new Matrix4x4[10];
}
