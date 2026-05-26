using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200117D RID: 4477
	public class ListProcessor<T>
	{
		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06007155 RID: 29013 RVA: 0x0024FE42 File Offset: 0x0024E042
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06007156 RID: 29014 RVA: 0x0024FE4F File Offset: 0x0024E04F
		// (set) Token: 0x06007157 RID: 29015 RVA: 0x0024FE57 File Offset: 0x0024E057
		public InAction<T> ItemProcessor
		{
			get
			{
				return this.m_itemProcessorDelegate;
			}
			set
			{
				this.m_itemProcessorDelegate = value;
			}
		}

		// Token: 0x06007158 RID: 29016 RVA: 0x0024FE60 File Offset: 0x0024E060
		public ListProcessor() : this(10, null)
		{
		}

		// Token: 0x06007159 RID: 29017 RVA: 0x0024FE6B File Offset: 0x0024E06B
		public ListProcessor(int capacity, InAction<T> itemProcessorDelegate = null)
		{
			this.m_list = new List<T>(capacity);
			this.m_currentIndex = -1;
			this.m_listCount = -1;
			this.m_itemProcessorDelegate = itemProcessorDelegate;
		}

		// Token: 0x0600715A RID: 29018 RVA: 0x0024FE94 File Offset: 0x0024E094
		public virtual void Add(in T item)
		{
			this.m_listCount++;
			this.m_list.Add(item);
		}

		// Token: 0x0600715B RID: 29019 RVA: 0x0024FEB8 File Offset: 0x0024E0B8
		public virtual bool Remove(in T item)
		{
			int num = this.m_list.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			if (num < this.m_currentIndex)
			{
				this.m_currentIndex--;
			}
			this.m_listCount--;
			this.m_list.RemoveAt(num);
			return true;
		}

		// Token: 0x0600715C RID: 29020 RVA: 0x0024FF0F File Offset: 0x0024E10F
		public void Clear()
		{
			this.m_list.Clear();
			this.m_currentIndex = -1;
			this.m_listCount = -1;
		}

		// Token: 0x0600715D RID: 29021 RVA: 0x0024FF2A File Offset: 0x0024E12A
		public bool Contains(in T item)
		{
			return this.m_list.Contains(item);
		}

		// Token: 0x0600715E RID: 29022 RVA: 0x0024FF3D File Offset: 0x0024E13D
		public virtual void ProcessListSafe()
		{
			this.ProcessListSafe(this.m_itemProcessorDelegate);
		}

		// Token: 0x0600715F RID: 29023 RVA: 0x0024FF4C File Offset: 0x0024E14C
		public virtual void ProcessListSafe(InAction<T> customDelegate)
		{
			if (customDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				try
				{
					T t = this.m_list[this.m_currentIndex];
					customDelegate(t);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
				this.m_currentIndex++;
			}
		}

		// Token: 0x06007160 RID: 29024 RVA: 0x0024FFD8 File Offset: 0x0024E1D8
		public virtual void ProcessList()
		{
			this.ProcessList(this.m_itemProcessorDelegate);
		}

		// Token: 0x06007161 RID: 29025 RVA: 0x0024FFE8 File Offset: 0x0024E1E8
		public virtual void ProcessList(InAction<T> customDelegate)
		{
			if (customDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				T t = this.m_list[this.m_currentIndex];
				customDelegate(t);
				this.m_currentIndex++;
			}
		}

		// Token: 0x06007162 RID: 29026 RVA: 0x00250053 File Offset: 0x0024E253
		public IReadOnlyList<T> GetReadonlyList()
		{
			return this.m_list;
		}

		// Token: 0x0400813E RID: 33086
		protected readonly List<T> m_list;

		// Token: 0x0400813F RID: 33087
		protected int m_currentIndex;

		// Token: 0x04008140 RID: 33088
		protected int m_listCount;

		// Token: 0x04008141 RID: 33089
		protected InAction<T> m_itemProcessorDelegate;
	}
}
