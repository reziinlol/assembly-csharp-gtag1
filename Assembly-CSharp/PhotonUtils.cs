using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

// Token: 0x02000CBC RID: 3260
public static class PhotonUtils
{
	// Token: 0x06005103 RID: 20739 RVA: 0x001AB8BC File Offset: 0x001A9ABC
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11, out T12 arg12)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
		arg11 = (T11)((object)args[startIndex + 10]);
		arg12 = (T12)((object)args[startIndex + 11]);
	}

	// Token: 0x06005104 RID: 20740 RVA: 0x001AB994 File Offset: 0x001A9B94
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
		arg11 = (T11)((object)args[startIndex + 10]);
	}

	// Token: 0x06005105 RID: 20741 RVA: 0x001ABA5C File Offset: 0x001A9C5C
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
	}

	// Token: 0x06005106 RID: 20742 RVA: 0x001ABB10 File Offset: 0x001A9D10
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
	}

	// Token: 0x06005107 RID: 20743 RVA: 0x001ABBB4 File Offset: 0x001A9DB4
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
	}

	// Token: 0x06005108 RID: 20744 RVA: 0x001ABC48 File Offset: 0x001A9E48
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
	}

	// Token: 0x06005109 RID: 20745 RVA: 0x001ABCC8 File Offset: 0x001A9EC8
	public static void ParseArgs<T1, T2, T3, T4, T5, T6>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
	}

	// Token: 0x0600510A RID: 20746 RVA: 0x001ABD38 File Offset: 0x001A9F38
	public static void ParseArgs<T1, T2, T3, T4, T5>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
	}

	// Token: 0x0600510B RID: 20747 RVA: 0x001ABD98 File Offset: 0x001A9F98
	public static void ParseArgs<T1, T2, T3, T4>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
	}

	// Token: 0x0600510C RID: 20748 RVA: 0x001ABDE5 File Offset: 0x001A9FE5
	public static void ParseArgs<T1, T2, T3>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
	}

	// Token: 0x0600510D RID: 20749 RVA: 0x001ABE16 File Offset: 0x001AA016
	public static void ParseArgs<T1, T2>(this object[] args, int startIndex, out T1 arg1, out T2 arg2)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
	}

	// Token: 0x0600510E RID: 20750 RVA: 0x001ABE36 File Offset: 0x001AA036
	public static void ParseArgs<T1>(this object[] args, int startIndex, out T1 arg1)
	{
		arg1 = (T1)((object)args[startIndex]);
	}

	// Token: 0x0600510F RID: 20751 RVA: 0x001ABE48 File Offset: 0x001AA048
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11, out T12 arg12)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		arg11 = default(T11);
		arg12 = default(T12);
		if (args == null || args.Length < startIndex + 12)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10, out arg11, out arg12);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005110 RID: 20752 RVA: 0x001ABEFC File Offset: 0x001AA0FC
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		arg11 = default(T11);
		if (args == null || args.Length < startIndex + 11)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10, out arg11);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005111 RID: 20753 RVA: 0x001ABFA4 File Offset: 0x001AA1A4
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		if (args == null || args.Length < startIndex + 10)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005112 RID: 20754 RVA: 0x001AC044 File Offset: 0x001AA244
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		if (args == null || args.Length < startIndex + 9)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005113 RID: 20755 RVA: 0x001AC0D8 File Offset: 0x001AA2D8
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		if (args == null || args.Length < startIndex + 8)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005114 RID: 20756 RVA: 0x001AC160 File Offset: 0x001AA360
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		if (args == null || args.Length < startIndex + 7)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005115 RID: 20757 RVA: 0x001AC1E0 File Offset: 0x001AA3E0
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		if (args == null || args.Length < startIndex + 6)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005116 RID: 20758 RVA: 0x001AC254 File Offset: 0x001AA454
	public static bool TryParseArgs<T1, T2, T3, T4, T5>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		if (args == null || args.Length < startIndex + 5)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005117 RID: 20759 RVA: 0x001AC2C0 File Offset: 0x001AA4C0
	public static bool TryParseArgs<T1, T2, T3, T4>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		if (args == null || args.Length < startIndex + 4)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005118 RID: 20760 RVA: 0x001AC320 File Offset: 0x001AA520
	public static bool TryParseArgs<T1, T2, T3>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		if (args == null || args.Length < startIndex + 3)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06005119 RID: 20761 RVA: 0x001AC378 File Offset: 0x001AA578
	public static bool TryParseArgs<T1, T2>(this object[] args, int startIndex, out T1 arg1, out T2 arg2)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		if (args == null || args.Length < startIndex + 2)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x0600511A RID: 20762 RVA: 0x001AC3C4 File Offset: 0x001AA5C4
	public static bool TryParseArgs<T1>(this object[] args, int startIndex, out T1 arg1)
	{
		arg1 = default(T1);
		if (args == null || args.Length < startIndex + 1)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x0600511B RID: 20763 RVA: 0x001AC408 File Offset: 0x001AA608
	public static ref readonly T[] FetchDelegatesNonAlloc<T>(T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return PhotonUtils.EmptyArray<T>.Ref();
		}
		return @delegate.GetInvocationListUnsafe<T>();
	}

	// Token: 0x0600511C RID: 20764 RVA: 0x001AC424 File Offset: 0x001AA624
	public static object[] FetchScratchArray(int size)
	{
		if (size < 0)
		{
			throw new Exception("Size cannot be less than 0.");
		}
		object[] array;
		if (!PhotonUtils.gLengthToArgsArray.TryGetValue(size, out array))
		{
			array = new object[size];
			PhotonUtils.gLengthToArgsArray.Add(size, array);
		}
		return array;
	}

	// Token: 0x0600511D RID: 20765 RVA: 0x001AC464 File Offset: 0x001AA664
	public static NetPlayer GetNetPlayer(int actorNumber)
	{
		NetworkSystem networkSystem;
		if (!PhotonUtils.TryGetNetSystem(out networkSystem))
		{
			return null;
		}
		return networkSystem.GetPlayer(actorNumber);
	}

	// Token: 0x1700078C RID: 1932
	// (get) Token: 0x0600511E RID: 20766 RVA: 0x001AC484 File Offset: 0x001AA684
	public static int LocalActorNumber
	{
		get
		{
			NetPlayer localNetPlayer = PhotonUtils.LocalNetPlayer;
			if (localNetPlayer == null)
			{
				return -1;
			}
			return localNetPlayer.ActorNumber;
		}
	}

	// Token: 0x1700078D RID: 1933
	// (get) Token: 0x0600511F RID: 20767 RVA: 0x001AC4A4 File Offset: 0x001AA6A4
	public static NetPlayer LocalNetPlayer
	{
		get
		{
			if (PhotonUtils.gLocalNetPlayer != null)
			{
				return PhotonUtils.gLocalNetPlayer;
			}
			NetworkSystem networkSystem;
			if (PhotonUtils.TryGetNetSystem(out networkSystem))
			{
				PhotonUtils.gLocalNetPlayer = networkSystem.GetLocalPlayer();
			}
			return PhotonUtils.gLocalNetPlayer;
		}
	}

	// Token: 0x06005120 RID: 20768 RVA: 0x001AC4D7 File Offset: 0x001AA6D7
	private static bool TryGetNetSystem(out NetworkSystem ns)
	{
		if (!PhotonUtils.gNetSystem)
		{
			PhotonUtils.gNetSystem = NetworkSystem.Instance;
		}
		if (!PhotonUtils.gNetSystem)
		{
			ns = null;
			return false;
		}
		ns = PhotonUtils.gNetSystem;
		return true;
	}

	// Token: 0x06005121 RID: 20769 RVA: 0x001AC508 File Offset: 0x001AA708
	static PhotonUtils()
	{
		for (int i = 0; i <= 16; i++)
		{
			PhotonUtils.gLengthToArgsArray.Add(i, new object[i]);
		}
	}

	// Token: 0x0400627B RID: 25211
	private static NetworkSystem gNetSystem;

	// Token: 0x0400627C RID: 25212
	private static NetPlayer gLocalNetPlayer;

	// Token: 0x0400627D RID: 25213
	private static readonly Dictionary<int, object[]> gLengthToArgsArray = new Dictionary<int, object[]>(16);

	// Token: 0x0400627E RID: 25214
	private const int ARG_ARRAYS = 16;

	// Token: 0x02000CBD RID: 3261
	private static class EmptyArray<T>
	{
		// Token: 0x06005122 RID: 20770 RVA: 0x001AC53F File Offset: 0x001AA73F
		public static ref readonly T[] Ref()
		{
			return ref PhotonUtils.EmptyArray<T>.gEmpty;
		}

		// Token: 0x0400627F RID: 25215
		private static readonly T[] gEmpty = Array.Empty<T>();
	}

	// Token: 0x02000CBE RID: 3262
	public static class CustomTypes
	{
		// Token: 0x06005124 RID: 20772 RVA: 0x001AC554 File Offset: 0x001AA754
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitOnLoad()
		{
			PhotonPeer.RegisterType(typeof(Color32), 67, new SerializeMethod(PhotonUtils.CustomTypes.SerializeColor32), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeColor32));
			PhotonPeer.RegisterType(typeof(UnityEngine.BoundsInt), 73, new SerializeMethod(PhotonUtils.CustomTypes.SerializeBoundsInt), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeBoundsInt));
			PhotonPeer.RegisterType(typeof(int3), 74, new SerializeMethod(PhotonUtils.CustomTypes.SerializeInt3), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeInt3));
			PhotonPeer.RegisterType(typeof(Voxel), 88, new SerializeStreamMethod(PhotonUtils.CustomTypes.SerializeVoxel), new DeserializeStreamMethod(PhotonUtils.CustomTypes.DeserializeVoxel));
			PhotonPeer.RegisterType(typeof(VoxelAction), 89, new SerializeMethod(PhotonUtils.CustomTypes.SerializeVoxelAction), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeVoxelAction));
		}

		// Token: 0x06005125 RID: 20773 RVA: 0x001AC633 File Offset: 0x001AA833
		public static byte[] SerializeColor32(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<Color32>((Color32)value);
		}

		// Token: 0x06005126 RID: 20774 RVA: 0x001AC640 File Offset: 0x001AA840
		public static object DeserializeColor32(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<Color32>(data);
		}

		// Token: 0x06005127 RID: 20775 RVA: 0x001AC64D File Offset: 0x001AA84D
		public static byte[] SerializeBoundsInt(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<UnityEngine.BoundsInt>((UnityEngine.BoundsInt)value);
		}

		// Token: 0x06005128 RID: 20776 RVA: 0x001AC65A File Offset: 0x001AA85A
		public static object DeserializeBoundsInt(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<UnityEngine.BoundsInt>(data);
		}

		// Token: 0x06005129 RID: 20777 RVA: 0x001AC667 File Offset: 0x001AA867
		public static byte[] SerializeInt3(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<int3>((int3)value);
		}

		// Token: 0x0600512A RID: 20778 RVA: 0x001AC674 File Offset: 0x001AA874
		public static object DeserializeInt3(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<int3>(data);
		}

		// Token: 0x0600512B RID: 20779 RVA: 0x001AC681 File Offset: 0x001AA881
		public static byte[] SerializeVoxelAction(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<VoxelAction>((VoxelAction)value);
		}

		// Token: 0x0600512C RID: 20780 RVA: 0x001AC68E File Offset: 0x001AA88E
		public static object DeserializeVoxelAction(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<VoxelAction>(data);
		}

		// Token: 0x0600512D RID: 20781 RVA: 0x001AC69C File Offset: 0x001AA89C
		private static short SerializeVoxel(StreamBuffer stream, object value)
		{
			Voxel voxel = (Voxel)value;
			byte[] obj = PhotonUtils.CustomTypes.memVox;
			lock (obj)
			{
				byte[] array = PhotonUtils.CustomTypes.memVox;
				array[0] = voxel.Material;
				array[1] = voxel.Density;
				stream.Write(array, 0, 2);
			}
			return 2;
		}

		// Token: 0x0600512E RID: 20782 RVA: 0x001AC700 File Offset: 0x001AA900
		private static object DeserializeVoxel(StreamBuffer stream, short length)
		{
			Voxel voxel = default(Voxel);
			if (length == 2)
			{
				return voxel;
			}
			byte[] obj = PhotonUtils.CustomTypes.memVox;
			lock (obj)
			{
				stream.Read(PhotonUtils.CustomTypes.memVox, 0, 2);
				voxel.Material = PhotonUtils.CustomTypes.memVox[0];
				voxel.Density = PhotonUtils.CustomTypes.memVox[1];
			}
			return voxel;
		}

		// Token: 0x0600512F RID: 20783 RVA: 0x001AC77C File Offset: 0x001AA97C
		private static T CastToStruct<T>(byte[] bytes) where T : struct
		{
			return MemoryMarshal.Read<T>(bytes);
		}

		// Token: 0x06005130 RID: 20784 RVA: 0x001AC789 File Offset: 0x001AA989
		private static byte[] CastToBytes<T>(T data) where T : struct
		{
			byte[] staticArray = PhotonUtils.CustomTypes._arrayBag.GetStaticArray(Marshal.SizeOf<T>());
			MemoryMarshal.Write<T>(staticArray, ref data);
			return staticArray;
		}

		// Token: 0x04006280 RID: 25216
		private static StaticArrayBag<byte> _arrayBag = new StaticArrayBag<byte>();

		// Token: 0x04006281 RID: 25217
		private const short LEN_C32 = 4;

		// Token: 0x04006282 RID: 25218
		private const int SizeVox = 2;

		// Token: 0x04006283 RID: 25219
		private static readonly byte[] memVox = new byte[2];
	}
}
