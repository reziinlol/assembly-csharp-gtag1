using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000904 RID: 2308
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x06003C3F RID: 15423 RVA: 0x00148F62 File Offset: 0x00147162
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06003C40 RID: 15424 RVA: 0x00148F6A File Offset: 0x0014716A
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06003C41 RID: 15425 RVA: 0x00148F74 File Offset: 0x00147174
	protected virtual void Tick()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this._instances.Count; i++)
		{
			T t = this._instances[i];
			if (t)
			{
				this.OnTick(deltaTime, t);
			}
		}
	}

	// Token: 0x06003C42 RID: 15426 RVA: 0x00148FBF File Offset: 0x001471BF
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06003C43 RID: 15427 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06003C44 RID: 15428 RVA: 0x00148FC8 File Offset: 0x001471C8
	private bool RegisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Register] Instance is null.", null);
			return false;
		}
		if (this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnRegister(instance);
		return true;
	}

	// Token: 0x06003C45 RID: 15429 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06003C46 RID: 15430 RVA: 0x0014902C File Offset: 0x0014722C
	private bool UnregisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Unregister] Instance is null.", null);
			return false;
		}
		if (!this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Remove(instance);
		this.OnUnregister(instance);
		return true;
	}

	// Token: 0x06003C47 RID: 15431 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x06003C48 RID: 15432 RVA: 0x0014908E File Offset: 0x0014728E
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x06003C49 RID: 15433 RVA: 0x0014908E File Offset: 0x0014728E
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x06003C4A RID: 15434 RVA: 0x0014909B File Offset: 0x0014729B
	int IReadOnlyCollection<!0>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x1700056F RID: 1391
	T IReadOnlyList<!0>.this[int index]
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x06003C4C RID: 15436 RVA: 0x001490B6 File Offset: 0x001472B6
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x001490C4 File Offset: 0x001472C4
	protected static void SetSingleton(GTSystem<T> system)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (GTSystem<T>.gSingleton != null && GTSystem<T>.gSingleton != system)
		{
			Object.Destroy(system);
			GTDev.LogWarning<string>("Singleton of type " + GTSystem<T>.gSingleton.GetType().Name + " already exists.", null);
			return;
		}
		GTSystem<T>.gSingleton = system;
		if (!GTSystem<T>.gInitializing)
		{
			return;
		}
		GTSystem<T>.gSingleton._instances.Clear();
		T[] collection = (from x in GTSystem<T>.gQueueRegister
		where x != null
		select x).ToArray<T>();
		GTSystem<T>.gSingleton._instances.AddRange(collection);
		GTSystem<T>.gQueueRegister.Clear();
		PhotonView component = GTSystem<T>.gSingleton.GetComponent<PhotonView>();
		if (component != null)
		{
			GTSystem<T>.gSingleton._photonView = component;
			GTSystem<T>.gSingleton._networked = true;
		}
		GTSystem<T>.gInitializing = false;
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x001491B4 File Offset: 0x001473B4
	public static void Register(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		GTSystem<T>.gSingleton.RegisterInstance(instance);
	}

	// Token: 0x06003C4F RID: 15439 RVA: 0x00149220 File Offset: 0x00147420
	public static void Unregister(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		GTSystem<T>.gSingleton.UnregisterInstance(instance);
	}

	// Token: 0x04004CDD RID: 19677
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04004CDE RID: 19678
	[SerializeField]
	private bool _networked;

	// Token: 0x04004CDF RID: 19679
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04004CE0 RID: 19680
	private static GTSystem<T> gSingleton;

	// Token: 0x04004CE1 RID: 19681
	private static bool gInitializing = false;

	// Token: 0x04004CE2 RID: 19682
	private static bool gAppQuitting = false;

	// Token: 0x04004CE3 RID: 19683
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
