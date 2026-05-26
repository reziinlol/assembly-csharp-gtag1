using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

// Token: 0x02000CFF RID: 3327
[DefaultExecutionOrder(0)]
internal abstract class TickSystem<T> : MonoBehaviour
{
	// Token: 0x06005274 RID: 21108 RVA: 0x001B1AC7 File Offset: 0x001AFCC7
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06005275 RID: 21109 RVA: 0x001B1ADC File Offset: 0x001AFCDC
	private void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.TryRunCallbacks();
		TickSystem<T>.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06005276 RID: 21110 RVA: 0x001B1AFA File Offset: 0x001AFCFA
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06005277 RID: 21111 RVA: 0x001B1B10 File Offset: 0x001AFD10
	static TickSystem()
	{
		TickSystem<T>.preTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPre>(100);
		TickSystem<T>.tickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperTick>(100);
		TickSystem<T>.postTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPost>(100);
	}

	// Token: 0x06005278 RID: 21112 RVA: 0x001B1B83 File Offset: 0x001AFD83
	private static void OnEnterPlay()
	{
		TickSystem<T>.preTickCallbacks.Clear();
		TickSystem<T>.preTickWrapperTable.Clear();
		TickSystem<T>.tickCallbacks.Clear();
		TickSystem<T>.tickWrapperTable.Clear();
		TickSystem<T>.postTickCallbacks.Clear();
		TickSystem<T>.postTickWrapperTable.Clear();
	}

	// Token: 0x06005279 RID: 21113 RVA: 0x001B1BC4 File Offset: 0x001AFDC4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		if (callback.PreTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre = TickSystem<T>.preTickWrapperPool.Take();
		tickCallbackWrapperPre.target = callback;
		TickSystem<T>.preTickWrapperTable[callback] = tickCallbackWrapperPre;
		TickSystem<T>.preTickCallbacks.Add(tickCallbackWrapperPre);
		callback.PreTickRunning = true;
	}

	// Token: 0x0600527A RID: 21114 RVA: 0x001B1C0C File Offset: 0x001AFE0C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		if (callback.TickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick = TickSystem<T>.tickWrapperPool.Take();
		tickCallbackWrapperTick.target = callback;
		TickSystem<T>.tickWrapperTable[callback] = tickCallbackWrapperTick;
		TickSystem<T>.tickCallbacks.Add(tickCallbackWrapperTick);
		callback.TickRunning = true;
	}

	// Token: 0x0600527B RID: 21115 RVA: 0x001B1C54 File Offset: 0x001AFE54
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		if (callback.PostTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost = TickSystem<T>.postTickWrapperPool.Take();
		tickCallbackWrapperPost.target = callback;
		TickSystem<T>.postTickWrapperTable[callback] = tickCallbackWrapperPost;
		TickSystem<T>.postTickCallbacks.Add(tickCallbackWrapperPost);
		callback.PostTickRunning = true;
	}

	// Token: 0x0600527C RID: 21116 RVA: 0x001B1C9B File Offset: 0x001AFE9B
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem<T>.AddPreTickCallback(callback);
		TickSystem<T>.AddTickCallback(callback);
		TickSystem<T>.AddPostTickCallback(callback);
	}

	// Token: 0x0600527D RID: 21117 RVA: 0x001B1CB0 File Offset: 0x001AFEB0
	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.AddPostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x0600527E RID: 21118 RVA: 0x001B1D00 File Offset: 0x001AFF00
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem<T>.TickCallbackWrapperPre instance;
		if (!callback.PreTickRunning || !TickSystem<T>.preTickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.Remove(instance);
		callback.PreTickRunning = false;
		TickSystem<T>.preTickWrapperPool.Return(instance);
	}

	// Token: 0x0600527F RID: 21119 RVA: 0x001B1D44 File Offset: 0x001AFF44
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem<T>.TickCallbackWrapperTick instance;
		if (!callback.TickRunning || !TickSystem<T>.tickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.tickCallbacks.Remove(instance);
		callback.TickRunning = false;
		TickSystem<T>.tickWrapperPool.Return(instance);
	}

	// Token: 0x06005280 RID: 21120 RVA: 0x001B1D88 File Offset: 0x001AFF88
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem<T>.TickCallbackWrapperPost instance;
		if (!callback.PostTickRunning || !TickSystem<T>.postTickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.Remove(instance);
		callback.PostTickRunning = false;
		TickSystem<T>.postTickWrapperPool.Return(instance);
	}

	// Token: 0x06005281 RID: 21121 RVA: 0x001B1DCC File Offset: 0x001AFFCC
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem<T>.RemovePreTickCallback(callback);
		TickSystem<T>.RemoveTickCallback(callback);
		TickSystem<T>.RemovePostTickCallback(callback);
	}

	// Token: 0x06005282 RID: 21122 RVA: 0x001B1DE0 File Offset: 0x001AFFE0
	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.RemovePostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x040063AB RID: 25515
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPre> preTickWrapperPool;

	// Token: 0x040063AC RID: 25516
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPre> preTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPre>();

	// Token: 0x040063AD RID: 25517
	private static readonly Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre> preTickWrapperTable = new Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x040063AE RID: 25518
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperTick> tickWrapperPool;

	// Token: 0x040063AF RID: 25519
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperTick> tickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperTick>();

	// Token: 0x040063B0 RID: 25520
	private static readonly Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick> tickWrapperTable = new Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x040063B1 RID: 25521
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPost> postTickWrapperPool;

	// Token: 0x040063B2 RID: 25522
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPost> postTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPost>();

	// Token: 0x040063B3 RID: 25523
	private static readonly Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost> postTickWrapperTable = new Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x02000D00 RID: 3328
	private abstract class TickCallbackWrapper<U> : ObjectPoolEvents, ICallBack where U : class
	{
		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06005284 RID: 21124 RVA: 0x001B1E2E File Offset: 0x001B002E
		// (set) Token: 0x06005285 RID: 21125 RVA: 0x001B1E36 File Offset: 0x001B0036
		public U target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
			}
		}

		// Token: 0x06005286 RID: 21126
		public abstract void CallBack();

		// Token: 0x06005287 RID: 21127 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnTaken()
		{
		}

		// Token: 0x06005288 RID: 21128 RVA: 0x001B1E40 File Offset: 0x001B0040
		public void OnReturned()
		{
			this.target = default(U);
		}

		// Token: 0x040063B4 RID: 25524
		protected U m_target;
	}

	// Token: 0x02000D01 RID: 3329
	private class TickCallbackWrapperPre : TickSystem<T>.TickCallbackWrapper<ITickSystemPre>
	{
		// Token: 0x0600528A RID: 21130 RVA: 0x001B1E5C File Offset: 0x001B005C
		public override void CallBack()
		{
			this.m_target.PreTick();
		}
	}

	// Token: 0x02000D02 RID: 3330
	private class TickCallbackWrapperTick : TickSystem<T>.TickCallbackWrapper<ITickSystemTick>
	{
		// Token: 0x0600528C RID: 21132 RVA: 0x001B1E71 File Offset: 0x001B0071
		public override void CallBack()
		{
			this.m_target.Tick();
		}
	}

	// Token: 0x02000D03 RID: 3331
	private class TickCallbackWrapperPost : TickSystem<T>.TickCallbackWrapper<ITickSystemPost>
	{
		// Token: 0x0600528E RID: 21134 RVA: 0x001B1E86 File Offset: 0x001B0086
		public override void CallBack()
		{
			this.m_target.PostTick();
		}
	}
}
