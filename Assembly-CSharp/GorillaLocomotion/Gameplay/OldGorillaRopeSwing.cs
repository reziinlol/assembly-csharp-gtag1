using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001103 RID: 4355
	public class OldGorillaRopeSwing : MonoBehaviourPun
	{
		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x06006DAD RID: 28077 RVA: 0x0023D6B8 File Offset: 0x0023B8B8
		// (set) Token: 0x06006DAE RID: 28078 RVA: 0x0023D6C0 File Offset: 0x0023B8C0
		public bool isIdle { get; private set; }

		// Token: 0x06006DAF RID: 28079 RVA: 0x0023D6C9 File Offset: 0x0023B8C9
		private void Awake()
		{
			this.SetIsIdle(true);
		}

		// Token: 0x06006DB0 RID: 28080 RVA: 0x0023D6D2 File Offset: 0x0023B8D2
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true);
			}
		}

		// Token: 0x06006DB1 RID: 28081 RVA: 0x0023D6E4 File Offset: 0x0023B8E4
		private void Update()
		{
			if (this.localPlayerOn && this.localGrabbedRigid)
			{
				float magnitude = this.localGrabbedRigid.linearVelocity.magnitude;
				if (magnitude > 2.5f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				float num = MathUtils.Linear(magnitude, 0f, 10f, -0.07f, 0.5f);
				if (num > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num, Time.deltaTime);
				}
			}
			if (!this.isIdle)
			{
				if (!this.localPlayerOn && this.remotePlayers.Count == 0)
				{
					foreach (Rigidbody rigidbody in this.bones)
					{
						float magnitude2 = rigidbody.linearVelocity.magnitude;
						float num2 = Time.deltaTime * this.settings.frictionWhenNotHeld;
						if (num2 < magnitude2 - 0.1f)
						{
							rigidbody.linearVelocity = Vector3.MoveTowards(rigidbody.linearVelocity, Vector3.zero, num2);
						}
					}
				}
				bool flag = false;
				for (int j = 0; j < this.bones.Length; j++)
				{
					if (this.bones[j].linearVelocity.magnitude > 0.1f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true);
					this.potentialIdleTimer = 0f;
				}
			}
		}

		// Token: 0x06006DB2 RID: 28082 RVA: 0x0023D890 File Offset: 0x0023BA90
		private void SetIsIdle(bool idle)
		{
			this.isIdle = idle;
			this.ToggleIsKinematic(idle);
			if (idle)
			{
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].linearVelocity = Vector3.zero;
					this.bones[i].angularVelocity = Vector3.zero;
					this.bones[i].transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06006DB3 RID: 28083 RVA: 0x0023D8FC File Offset: 0x0023BAFC
		private void ToggleIsKinematic(bool kinematic)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].isKinematic = kinematic;
				if (kinematic)
				{
					this.bones[i].interpolation = RigidbodyInterpolation.None;
				}
				else
				{
					this.bones[i].interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
		}

		// Token: 0x06006DB4 RID: 28084 RVA: 0x0023D94B File Offset: 0x0023BB4B
		public Rigidbody GetBone(int index)
		{
			if (index >= this.bones.Length)
			{
				return this.bones.Last<Rigidbody>();
			}
			return this.bones[index];
		}

		// Token: 0x06006DB5 RID: 28085 RVA: 0x0023D96C File Offset: 0x0023BB6C
		public int GetBoneIndex(Rigidbody r)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i] == r)
				{
					return i;
				}
			}
			return this.bones.Length - 1;
		}

		// Token: 0x06006DB6 RID: 28086 RVA: 0x0023D9A8 File Offset: 0x0023BBA8
		public void AttachLocalPlayer(XRNode xrNode, Rigidbody rigid, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(rigid);
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Rigidbody rigidbody in this.bones)
				{
					list.Add(rigidbody.transform.localEulerAngles);
					list2.Add(rigidbody.linearVelocity);
				}
			}
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2f))
			{
				this.SetVelocity_RPC(boneIndex, velocity, true, list.ToArray(), list2.ToArray());
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 2)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.localGrabbedRigid = rigid;
		}

		// Token: 0x06006DB7 RID: 28087 RVA: 0x0023DB2B File Offset: 0x0023BD2B
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localGrabbedRigid = null;
		}

		// Token: 0x06006DB8 RID: 28088 RVA: 0x0023DB64 File Offset: 0x0023BD64
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			return true;
		}

		// Token: 0x06006DB9 RID: 28089 RVA: 0x0023DBCB File Offset: 0x0023BDCB
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
		}

		// Token: 0x06006DBA RID: 28090 RVA: 0x0023DBDC File Offset: 0x0023BDDC
		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
				{
					boneIndex,
					velocity,
					wholeRope,
					ropeRotations,
					ropeVelocities
				});
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities);
		}

		// Token: 0x06006DBB RID: 28091 RVA: 0x0023DC40 File Offset: 0x0023BE40
		[PunRPC]
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			this.SetIsIdle(false);
			if (ropeRotations != null && ropeVelocities != null && ropeRotations.Length != 0)
			{
				this.ToggleIsKinematic(true);
				for (int i = 0; i < ropeRotations.Length; i++)
				{
					if (i != 0)
					{
						this.bones[i].transform.localRotation = Quaternion.Euler(ropeRotations[i]);
						this.bones[i].linearVelocity = ropeVelocities[i];
					}
				}
				this.ToggleIsKinematic(false);
			}
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone)
			{
				if (wholeRope)
				{
					int num = 0;
					float maxLength = Mathf.Min(velocity.magnitude, 15f);
					foreach (Rigidbody rigidbody in this.bones)
					{
						Vector3 vector = velocity / (float)boneIndex * (float)num;
						vector = Vector3.ClampMagnitude(vector, maxLength);
						rigidbody.linearVelocity = vector;
						num++;
					}
					return;
				}
				bone.linearVelocity = velocity;
			}
		}

		// Token: 0x04007EB8 RID: 32440
		public const float kPlayerMass = 0.8f;

		// Token: 0x04007EB9 RID: 32441
		public const float ropeBitGenOffset = 1f;

		// Token: 0x04007EBA RID: 32442
		public const float MAX_ROPE_SPEED = 15f;

		// Token: 0x04007EBB RID: 32443
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x04007EBC RID: 32444
		public Rigidbody[] bones = Array.Empty<Rigidbody>();

		// Token: 0x04007EBD RID: 32445
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x04007EBE RID: 32446
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x04007EBF RID: 32447
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x04007EC0 RID: 32448
		private bool localPlayerOn;

		// Token: 0x04007EC1 RID: 32449
		private XRNode localPlayerXRNode;

		// Token: 0x04007EC2 RID: 32450
		private Rigidbody localGrabbedRigid;

		// Token: 0x04007EC3 RID: 32451
		private const float MAX_VELOCITY_FOR_IDLE = 0.1f;

		// Token: 0x04007EC4 RID: 32452
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x04007EC6 RID: 32454
		private float potentialIdleTimer;

		// Token: 0x04007EC7 RID: 32455
		[Header("Config")]
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x04007EC8 RID: 32456
		[SerializeField]
		private GorillaRopeSwingSettings settings;
	}
}
