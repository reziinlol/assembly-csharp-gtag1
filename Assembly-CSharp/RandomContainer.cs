using System;
using UnityEngine;

// Token: 0x02000CC4 RID: 3268
public abstract class RandomContainer<T> : ScriptableObject
{
	// Token: 0x17000790 RID: 1936
	// (get) Token: 0x06005149 RID: 20809 RVA: 0x001ACF8C File Offset: 0x001AB18C
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x17000791 RID: 1937
	// (get) Token: 0x0600514A RID: 20810 RVA: 0x001ACF94 File Offset: 0x001AB194
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x0600514B RID: 20811 RVA: 0x001ACF9C File Offset: 0x001AB19C
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

	// Token: 0x0600514C RID: 20812 RVA: 0x001ACFFC File Offset: 0x001AB1FC
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x0600514D RID: 20813 RVA: 0x001AD02B File Offset: 0x001AB22B
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x0600514E RID: 20814 RVA: 0x001AD033 File Offset: 0x001AB233
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x0600514F RID: 20815 RVA: 0x001AD044 File Offset: 0x001AB244
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		return t;
	}

	// Token: 0x040062A8 RID: 25256
	public T[] items = new T[0];

	// Token: 0x040062A9 RID: 25257
	public int seed;

	// Token: 0x040062AA RID: 25258
	public bool staticSeed;

	// Token: 0x040062AB RID: 25259
	public bool distinct = true;

	// Token: 0x040062AC RID: 25260
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x040062AD RID: 25261
	[NonSerialized]
	private T _lastItem;

	// Token: 0x040062AE RID: 25262
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x040062AF RID: 25263
	[NonSerialized]
	private SRand _rnd;
}
