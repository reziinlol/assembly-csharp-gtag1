using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200118D RID: 4493
	public class MonkeGravityManager : MonoBehaviour
	{
		// Token: 0x060071CB RID: 29131 RVA: 0x002510AC File Offset: 0x0024F2AC
		private void Awake()
		{
			Vector3 vector = -Physics.gravity.normalized;
			MonkeGravityManager.k_defaultGravityInfo.gravityUpDirection = vector;
			MonkeGravityManager.k_defaultGravityInfo.rotationDirection = vector;
			MonkeGravityManager.k_defaultGravityInfo.rotationSpeed = this.defaultRotationSpeed;
			MonkeGravityManager.k_defaultGravityInfo.gravityStrength = this.defaultGravityStrength;
		}

		// Token: 0x060071CC RID: 29132 RVA: 0x00251102 File Offset: 0x0024F302
		private void FixedUpdate()
		{
			MonkeGravityManager.k_zones.RunCallbacks();
			MonkeGravityManager.k_controllers.RunCallbacks();
		}

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x060071CD RID: 29133 RVA: 0x00251118 File Offset: 0x0024F318
		public static GravityInfo DefaultGravityInfo
		{
			get
			{
				return MonkeGravityManager.k_defaultGravityInfo;
			}
		}

		// Token: 0x060071CE RID: 29134 RVA: 0x00251120 File Offset: 0x0024F320
		public static void AddMonkeGravityController(MonkeGravityController gravity)
		{
			Collider activatorCollider = gravity.ActivatorCollider;
			MonkeGravityManager.k_allowedColliders.TryAdd(activatorCollider, gravity);
			MonkeGravityManager.k_controllers.Add(gravity);
		}

		// Token: 0x060071CF RID: 29135 RVA: 0x0025114D File Offset: 0x0024F34D
		public static void RemoveMonkeGravityController(MonkeGravityController gravity)
		{
			MonkeGravityManager.k_allowedColliders.Remove(gravity.ActivatorCollider);
			MonkeGravityManager.k_controllers.Remove(gravity);
		}

		// Token: 0x060071D0 RID: 29136 RVA: 0x00251170 File Offset: 0x0024F370
		[return: TupleElementNames(new string[]
		{
			"found",
			"target"
		})]
		public static ValueTuple<bool, MonkeGravityController> GetMonkeGravityController(Collider collider)
		{
			MonkeGravityController item;
			return new ValueTuple<bool, MonkeGravityController>(MonkeGravityManager.k_allowedColliders.TryGetValue(collider, out item), item);
		}

		// Token: 0x060071D1 RID: 29137 RVA: 0x00251190 File Offset: 0x0024F390
		public static void AddGravityCallback(BasicGravityZone zone)
		{
			MonkeGravityManager.k_zones.Add(zone);
		}

		// Token: 0x060071D2 RID: 29138 RVA: 0x0025119E File Offset: 0x0024F39E
		public static void RemoveGravityCallback(BasicGravityZone zone)
		{
			MonkeGravityManager.k_zones.Remove(zone);
		}

		// Token: 0x04008188 RID: 33160
		[SerializeField]
		private float defaultRotationSpeed = 10f;

		// Token: 0x04008189 RID: 33161
		[SerializeField]
		private float defaultGravityStrength = -9.3f;

		// Token: 0x0400818A RID: 33162
		private static readonly CallbackContainerUnique<BasicGravityZone> k_zones = new CallbackContainerUnique<BasicGravityZone>(5);

		// Token: 0x0400818B RID: 33163
		private static readonly Dictionary<Collider, MonkeGravityController> k_allowedColliders = new Dictionary<Collider, MonkeGravityController>(10);

		// Token: 0x0400818C RID: 33164
		private static readonly CallbackContainerUnique<MonkeGravityController> k_controllers = new CallbackContainerUnique<MonkeGravityController>(10);

		// Token: 0x0400818D RID: 33165
		private static GravityInfo k_defaultGravityInfo = new GravityInfo
		{
			gravityUpDirection = Vector3.up,
			rotationDirection = Vector3.up,
			rotationSpeed = 10f,
			gravityStrength = -9.3f,
			rotate = false
		};
	}
}
