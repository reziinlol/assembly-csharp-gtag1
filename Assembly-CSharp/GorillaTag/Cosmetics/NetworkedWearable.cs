using System;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001246 RID: 4678
	public class NetworkedWearable : MonoBehaviour, ISpawnable, ITickSystemTick
	{
		// Token: 0x0600751D RID: 29981 RVA: 0x002662F8 File Offset: 0x002644F8
		private void Awake()
		{
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw)
			{
				this.isTwoHanded = false;
			}
			this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
			this.leftSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
			this.rightSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, false);
		}

		// Token: 0x0600751E RID: 29982 RVA: 0x0026634E File Offset: 0x0026454E
		private void OnEnable()
		{
			if (!this.IsSpawned)
			{
				return;
			}
			if (this.isLocal && !this.listenForChangesLocal)
			{
				this.SetWearableStateBool(this.startTrue);
				return;
			}
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x0600751F RID: 29983 RVA: 0x00266384 File Offset: 0x00264584
		public void ToggleWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleWearableStateBool on two handed object " + base.gameObject.name + ". please use ToggleLeftWearableStateBool or ToggleRightWearableStateBool instead", null);
				this.ToggleLeftWearableStateBool();
				return;
			}
			this.value = !this.value;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.wearableSlot, this.value);
			this.OnWearableStateChanged();
		}

		// Token: 0x06007520 RID: 29984 RVA: 0x0026645C File Offset: 0x0026465C
		public void SetWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetWearableStateBool on two handed object " + base.gameObject.name + ". please use SetLeftWearableStateBool or SetRightWearableStateBool instead", null);
				this.SetLeftWearableStateBool(newState);
				return;
			}
			if (this.value != newState)
			{
				this.value = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.wearableSlot, this.value);
				this.OnWearableStateChanged();
			}
		}

		// Token: 0x06007521 RID: 29985 RVA: 0x00266538 File Offset: 0x00264738
		public void ToggleLeftWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleLeftWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleLeftWearableStateBool on one handed object " + base.gameObject.name + ". Please use ToggleWearableStateBool instead", null);
				this.ToggleWearableStateBool();
				return;
			}
			this.leftHandValue = !this.leftHandValue;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.leftSlot, this.leftHandValue);
			this.OnLeftStateChanged();
		}

		// Token: 0x06007522 RID: 29986 RVA: 0x00266610 File Offset: 0x00264810
		public void ToggleRightWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleRightWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleRightWearableStateBool on one handed object " + base.gameObject.name + ". Please use ToggleWearableStateBool instead", null);
				this.ToggleWearableStateBool();
				return;
			}
			this.rightHandValue = !this.rightHandValue;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.rightSlot, this.rightHandValue);
			this.OnRightStateChanged();
		}

		// Token: 0x06007523 RID: 29987 RVA: 0x002666E8 File Offset: 0x002648E8
		public void SetLeftWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetLeftWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetLeftWearableStateBool on one handed object " + base.gameObject.name + ". Please use SetWearableStateBool instead", null);
				this.SetWearableStateBool(newState);
				return;
			}
			if (this.leftHandValue != newState)
			{
				this.leftHandValue = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.leftSlot, this.leftHandValue);
				this.OnLeftStateChanged();
			}
		}

		// Token: 0x06007524 RID: 29988 RVA: 0x002667C4 File Offset: 0x002649C4
		public void SetRightWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetRightWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetRightWearableStateBool on one handed object " + base.gameObject.name + ". Please use SetWearableStateBool instead", null);
				this.SetWearableStateBool(newState);
				return;
			}
			if (this.rightHandValue != newState)
			{
				this.rightHandValue = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.rightSlot, this.rightHandValue);
				this.OnRightStateChanged();
			}
		}

		// Token: 0x06007525 RID: 29989 RVA: 0x0026689E File Offset: 0x00264A9E
		public void OnDisable()
		{
			if (this.isLocal && !this.listenForChangesLocal)
			{
				this.SetWearableStateBool(false);
				return;
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x06007526 RID: 29990 RVA: 0x002668C6 File Offset: 0x00264AC6
		private void OnWearableStateChanged()
		{
			if (this.value)
			{
				UnityEvent onWearableStateTrue = this.OnWearableStateTrue;
				if (onWearableStateTrue == null)
				{
					return;
				}
				onWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onWearableStateFalse = this.OnWearableStateFalse;
				if (onWearableStateFalse == null)
				{
					return;
				}
				onWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x06007527 RID: 29991 RVA: 0x002668F1 File Offset: 0x00264AF1
		private void OnLeftStateChanged()
		{
			if (this.leftHandValue)
			{
				UnityEvent onLeftWearableStateTrue = this.OnLeftWearableStateTrue;
				if (onLeftWearableStateTrue == null)
				{
					return;
				}
				onLeftWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onLeftWearableStateFalse = this.OnLeftWearableStateFalse;
				if (onLeftWearableStateFalse == null)
				{
					return;
				}
				onLeftWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x06007528 RID: 29992 RVA: 0x0026691C File Offset: 0x00264B1C
		private void OnRightStateChanged()
		{
			if (this.rightHandValue)
			{
				UnityEvent onRightWearableStateTrue = this.OnRightWearableStateTrue;
				if (onRightWearableStateTrue == null)
				{
					return;
				}
				onRightWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onRightWearableStateFalse = this.OnRightWearableStateFalse;
				if (onRightWearableStateFalse == null)
				{
					return;
				}
				onRightWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x06007529 RID: 29993 RVA: 0x00266947 File Offset: 0x00264B47
		// (set) Token: 0x0600752A RID: 29994 RVA: 0x0026694F File Offset: 0x00264B4F
		public bool IsSpawned { get; set; }

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x0600752B RID: 29995 RVA: 0x00266958 File Offset: 0x00264B58
		// (set) Token: 0x0600752C RID: 29996 RVA: 0x00266960 File Offset: 0x00264B60
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600752D RID: 29997 RVA: 0x0026696C File Offset: 0x00264B6C
		public void OnSpawn(VRRig rig)
		{
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.CosmeticSelectedSide == ECosmeticSelectSide.Both)
			{
				GTDev.LogWarning<string>(string.Format("NetworkedWearable: Cosmetic {0} with category {1} has select side Both, assuming left side!", base.gameObject.name, this.assignedSlot), null);
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				GTDev.LogError<string>(string.Format("NetworkedWearable: Cosmetic {0} spawned with invalid category {1}!", base.gameObject.name, this.assignedSlot), null);
			}
			this.myRig = rig;
			this.isLocal = rig.isLocal;
			this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, this.CosmeticSelectedSide != ECosmeticSelectSide.Right);
		}

		// Token: 0x0600752E RID: 29998 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x0600752F RID: 29999 RVA: 0x00266A14 File Offset: 0x00264C14
		// (set) Token: 0x06007530 RID: 30000 RVA: 0x00266A1C File Offset: 0x00264C1C
		public bool TickRunning { get; set; }

		// Token: 0x06007531 RID: 30001 RVA: 0x00266A28 File Offset: 0x00264C28
		public void Tick()
		{
			if ((!this.isLocal || this.listenForChangesLocal) && this.IsSpawned)
			{
				if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
				{
					bool flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.leftSlot);
					if (this.leftHandValue != flag)
					{
						this.leftHandValue = flag;
						this.OnLeftStateChanged();
					}
					flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.rightSlot);
					if (this.rightHandValue != flag)
					{
						this.rightHandValue = flag;
						this.OnRightStateChanged();
						return;
					}
				}
				else
				{
					bool flag2 = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.wearableSlot);
					if (this.value != flag2)
					{
						this.value = flag2;
						this.OnWearableStateChanged();
					}
				}
			}
		}

		// Token: 0x06007532 RID: 30002 RVA: 0x00266AEC File Offset: 0x00264CEC
		public static bool IsCategoryValid(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
			case CosmeticsController.CosmeticCategory.Badge:
			case CosmeticsController.CosmeticCategory.Face:
			case CosmeticsController.CosmeticCategory.Paw:
			case CosmeticsController.CosmeticCategory.Fur:
			case CosmeticsController.CosmeticCategory.Shirt:
			case CosmeticsController.CosmeticCategory.Pants:
				return true;
			}
			return false;
		}

		// Token: 0x06007533 RID: 30003 RVA: 0x00266B24 File Offset: 0x00264D24
		private VRRig.WearablePackedStateSlots CosmeticCategoryToWearableSlot(CosmeticsController.CosmeticCategory category, bool isLeft)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return VRRig.WearablePackedStateSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return VRRig.WearablePackedStateSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return VRRig.WearablePackedStateSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!isLeft)
				{
					return VRRig.WearablePackedStateSlots.RightHand;
				}
				return VRRig.WearablePackedStateSlots.LeftHand;
			case CosmeticsController.CosmeticCategory.Fur:
				return VRRig.WearablePackedStateSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return VRRig.WearablePackedStateSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Pants:
				return VRRig.WearablePackedStateSlots.Pants1;
			}
			GTDev.LogWarning<string>(string.Format("NetworkedWearable: {0} item cannot set wearable state", category), null);
			return VRRig.WearablePackedStateSlots.Hat;
		}

		// Token: 0x040086BE RID: 34494
		[Tooltip("Whether the wearable state is toggled on by default.")]
		[SerializeField]
		private bool startTrue;

		// Token: 0x040086BF RID: 34495
		[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
		[SerializeField]
		private CosmeticsController.CosmeticCategory assignedSlot;

		// Token: 0x040086C0 RID: 34496
		[FormerlySerializedAs("IsTwoHanded")]
		[SerializeField]
		private bool isTwoHanded;

		// Token: 0x040086C1 RID: 34497
		private const string listenInfo = "listenForChangesLocal should be false in most cases";

		// Token: 0x040086C2 RID: 34498
		private const string listenDetails = "listenForChangesLocal should be false in most cases\nIf you have a first person part and a local rig part that both need to react to a state change\ncall the Toggle/Set functions to change the state from one prefab and check \nlistenForChangesLocal on the other prefab ";

		// Token: 0x040086C3 RID: 34499
		[SerializeField]
		private bool listenForChangesLocal;

		// Token: 0x040086C4 RID: 34500
		private VRRig.WearablePackedStateSlots wearableSlot;

		// Token: 0x040086C5 RID: 34501
		private VRRig.WearablePackedStateSlots leftSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x040086C6 RID: 34502
		private VRRig.WearablePackedStateSlots rightSlot = VRRig.WearablePackedStateSlots.RightHand;

		// Token: 0x040086C7 RID: 34503
		private VRRig myRig;

		// Token: 0x040086C8 RID: 34504
		private bool isLocal;

		// Token: 0x040086C9 RID: 34505
		private bool value;

		// Token: 0x040086CA RID: 34506
		private bool leftHandValue;

		// Token: 0x040086CB RID: 34507
		private bool rightHandValue;

		// Token: 0x040086CC RID: 34508
		[SerializeField]
		protected UnityEvent OnWearableStateTrue;

		// Token: 0x040086CD RID: 34509
		[SerializeField]
		protected UnityEvent OnWearableStateFalse;

		// Token: 0x040086CE RID: 34510
		[SerializeField]
		protected UnityEvent OnLeftWearableStateTrue;

		// Token: 0x040086CF RID: 34511
		[SerializeField]
		protected UnityEvent OnLeftWearableStateFalse;

		// Token: 0x040086D0 RID: 34512
		[SerializeField]
		protected UnityEvent OnRightWearableStateTrue;

		// Token: 0x040086D1 RID: 34513
		[SerializeField]
		protected UnityEvent OnRightWearableStateFalse;
	}
}
