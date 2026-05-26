using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001247 RID: 4679
	public class BackpackGrabbableCosmetic : HoldableObject
	{
		// Token: 0x06007535 RID: 30005 RVA: 0x00266BA3 File Offset: 0x00264DA3
		private void Awake()
		{
			this.currentItemsCount = this.startItemsCount;
			this.canGrab = true;
		}

		// Token: 0x06007536 RID: 30006 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x06007537 RID: 30007 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void DropItemCleanup()
		{
		}

		// Token: 0x06007538 RID: 30008 RVA: 0x00266BB8 File Offset: 0x00264DB8
		public void Update()
		{
			if (!this.canGrab && Time.time - this.lastGrabTime >= this.coolDownTimer)
			{
				this.canGrab = true;
			}
		}

		// Token: 0x06007539 RID: 30009 RVA: 0x00266BE0 File Offset: 0x00264DE0
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (this.IsEmpty())
			{
				Debug.LogWarning("Can't remove item, Backpack is empty, need to refill.");
				return;
			}
			if (!this.canGrab)
			{
				return;
			}
			this.lastGrabTime = Time.time;
			this.canGrab = false;
			SnowballThrowable snowballThrowable;
			((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
			this.RemoveItem();
		}

		// Token: 0x0600753A RID: 30010 RVA: 0x00266C4F File Offset: 0x00264E4F
		public void AddItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.maxCapacity <= this.currentItemsCount)
			{
				Debug.LogWarning("Can't add item, backpack is at full capacity.");
				return;
			}
			this.currentItemsCount++;
			this.UpdateState();
		}

		// Token: 0x0600753B RID: 30011 RVA: 0x00266C87 File Offset: 0x00264E87
		public void RemoveItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount < 0)
			{
				Debug.LogWarning("Can't remove item, Backpack is empty.");
				return;
			}
			this.currentItemsCount--;
			this.UpdateState();
		}

		// Token: 0x0600753C RID: 30012 RVA: 0x00266CBA File Offset: 0x00264EBA
		public void RefillBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.startItemsCount)
			{
				return;
			}
			this.currentItemsCount = this.startItemsCount;
			this.UpdateState();
		}

		// Token: 0x0600753D RID: 30013 RVA: 0x00266CE6 File Offset: 0x00264EE6
		public void EmptyBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == 0)
			{
				return;
			}
			this.currentItemsCount = 0;
			this.UpdateState();
		}

		// Token: 0x0600753E RID: 30014 RVA: 0x00266D07 File Offset: 0x00264F07
		public bool IsFull()
		{
			return !this.useCapacity || this.maxCapacity == this.currentItemsCount;
		}

		// Token: 0x0600753F RID: 30015 RVA: 0x00266D22 File Offset: 0x00264F22
		public bool IsEmpty()
		{
			return this.useCapacity && this.currentItemsCount == 0;
		}

		// Token: 0x06007540 RID: 30016 RVA: 0x00266D38 File Offset: 0x00264F38
		private void UpdateState()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.maxCapacity)
			{
				UnityEvent onReachedMaxCapacity = this.OnReachedMaxCapacity;
				if (onReachedMaxCapacity == null)
				{
					return;
				}
				onReachedMaxCapacity.Invoke();
				return;
			}
			else
			{
				if (this.currentItemsCount != 0)
				{
					if (this.currentItemsCount == this.startItemsCount)
					{
						UnityEvent onRefilled = this.OnRefilled;
						if (onRefilled == null)
						{
							return;
						}
						onRefilled.Invoke();
					}
					return;
				}
				UnityEvent onFullyEmptied = this.OnFullyEmptied;
				if (onFullyEmptied == null)
				{
					return;
				}
				onFullyEmptied.Invoke();
				return;
			}
		}

		// Token: 0x040086D5 RID: 34517
		[GorillaSoundLookup]
		public int materialIndex;

		// Token: 0x040086D6 RID: 34518
		[SerializeField]
		private bool useCapacity = true;

		// Token: 0x040086D7 RID: 34519
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x040086D8 RID: 34520
		[SerializeField]
		private int maxCapacity;

		// Token: 0x040086D9 RID: 34521
		[SerializeField]
		private int startItemsCount;

		// Token: 0x040086DA RID: 34522
		[Space]
		public UnityEvent OnReachedMaxCapacity;

		// Token: 0x040086DB RID: 34523
		public UnityEvent OnFullyEmptied;

		// Token: 0x040086DC RID: 34524
		public UnityEvent OnRefilled;

		// Token: 0x040086DD RID: 34525
		private int currentItemsCount;

		// Token: 0x040086DE RID: 34526
		private bool canGrab;

		// Token: 0x040086DF RID: 34527
		private float lastGrabTime;
	}
}
