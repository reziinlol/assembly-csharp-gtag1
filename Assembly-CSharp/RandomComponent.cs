using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000CC3 RID: 3267
public abstract class RandomComponent<T> : MonoBehaviour
{
	// Token: 0x1700078E RID: 1934
	// (get) Token: 0x06005140 RID: 20800 RVA: 0x001ACE31 File Offset: 0x001AB031
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x1700078F RID: 1935
	// (get) Token: 0x06005141 RID: 20801 RVA: 0x001ACE39 File Offset: 0x001AB039
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x06005142 RID: 20802 RVA: 0x001ACE44 File Offset: 0x001AB044
	public void ResetRandom(int? seedValue = null)
	{
		if (!this.staticSeed)
		{
			this._seed = (seedValue ?? StaticHash.Compute(DateTime.UtcNow.Ticks));
		}
		else
		{
			this._seed = this.seed;
		}
		this._rnd = new SRand(this._seed);
	}

	// Token: 0x06005143 RID: 20803 RVA: 0x001ACEA4 File Offset: 0x001AB0A4
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x06005144 RID: 20804 RVA: 0x001ACED3 File Offset: 0x001AB0D3
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06005145 RID: 20805 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnNextItem(T item)
	{
	}

	// Token: 0x06005146 RID: 20806 RVA: 0x001ACEDB File Offset: 0x001AB0DB
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x06005147 RID: 20807 RVA: 0x001ACEEC File Offset: 0x001AB0EC
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		this.OnNextItem(t);
		UnityEvent<T> unityEvent = this.onNextItem;
		if (unityEvent != null)
		{
			unityEvent.Invoke(t);
		}
		return t;
	}

	// Token: 0x0400629F RID: 25247
	public T[] items = new T[0];

	// Token: 0x040062A0 RID: 25248
	public int seed;

	// Token: 0x040062A1 RID: 25249
	public bool staticSeed;

	// Token: 0x040062A2 RID: 25250
	public bool distinct = true;

	// Token: 0x040062A3 RID: 25251
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x040062A4 RID: 25252
	[NonSerialized]
	private T _lastItem;

	// Token: 0x040062A5 RID: 25253
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x040062A6 RID: 25254
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x040062A7 RID: 25255
	public UnityEvent<T> onNextItem;
}
