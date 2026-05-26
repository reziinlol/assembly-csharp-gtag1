using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F07 RID: 3847
	public class GameObjectManagerWithId : MonoBehaviour
	{
		// Token: 0x06005FEB RID: 24555 RVA: 0x001EF140 File Offset: 0x001ED340
		private void Awake()
		{
			Transform[] componentsInChildren = this.objectsContainer.GetComponentsInChildren<Transform>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				GameObjectManagerWithId.gameObjectData gameObjectData = new GameObjectManagerWithId.gameObjectData();
				gameObjectData.transform = componentsInChildren[i];
				gameObjectData.id = this.zone.ToString() + i.ToString();
				this.objectData.Add(gameObjectData);
			}
		}

		// Token: 0x06005FEC RID: 24556 RVA: 0x001EF1A6 File Offset: 0x001ED3A6
		private void OnDestroy()
		{
			this.objectData.Clear();
		}

		// Token: 0x06005FED RID: 24557 RVA: 0x001EF1B4 File Offset: 0x001ED3B4
		public void ReceiveEvent(string id, Transform _transform)
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.id == id)
				{
					gameObjectData.isMatched = true;
					gameObjectData.followTransform = _transform;
				}
			}
		}

		// Token: 0x06005FEE RID: 24558 RVA: 0x001EF21C File Offset: 0x001ED41C
		private void Update()
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.isMatched)
				{
					gameObjectData.transform.transform.position = gameObjectData.followTransform.position;
					gameObjectData.transform.transform.rotation = gameObjectData.followTransform.rotation;
				}
			}
		}

		// Token: 0x04006E84 RID: 28292
		public GameObject objectsContainer;

		// Token: 0x04006E85 RID: 28293
		public GTZone zone;

		// Token: 0x04006E86 RID: 28294
		private readonly List<GameObjectManagerWithId.gameObjectData> objectData = new List<GameObjectManagerWithId.gameObjectData>();

		// Token: 0x02000F08 RID: 3848
		private class gameObjectData
		{
			// Token: 0x04006E87 RID: 28295
			public Transform transform;

			// Token: 0x04006E88 RID: 28296
			public Transform followTransform;

			// Token: 0x04006E89 RID: 28297
			public string id;

			// Token: 0x04006E8A RID: 28298
			public bool isMatched;
		}
	}
}
