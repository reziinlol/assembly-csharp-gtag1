using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003BF RID: 959
public static class XSceneRefGlobalHub
{
	// Token: 0x0600170A RID: 5898 RVA: 0x000856AC File Offset: 0x000838AC
	public static void Register(int ID, XSceneRefTarget obj)
	{
		if (ID > 0)
		{
			int sceneIndex = (int)obj.GetSceneIndex();
			if (sceneIndex >= 0)
			{
				XSceneRefGlobalHub.registry[sceneIndex][ID] = obj;
			}
		}
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x000856DC File Offset: 0x000838DC
	public static void Unregister(int ID, XSceneRefTarget obj)
	{
		int sceneIndex = (int)obj.GetSceneIndex();
		if (ID > 0 && sceneIndex >= 0)
		{
			if (sceneIndex < 0 || sceneIndex >= XSceneRefGlobalHub.registry.Count)
			{
				Debug.LogErrorFormat(obj, "Invalid scene index {0} cannot remove ID {1}", new object[]
				{
					sceneIndex,
					ID
				});
			}
			XSceneRefGlobalHub.registry[sceneIndex].Remove(ID);
		}
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x0008573E File Offset: 0x0008393E
	public static bool TryResolve(SceneIndex sceneIndex, int ID, out XSceneRefTarget result)
	{
		return XSceneRefGlobalHub.registry[(int)sceneIndex].TryGetValue(ID, out result);
	}

	// Token: 0x04002243 RID: 8771
	private static List<Dictionary<int, XSceneRefTarget>> registry = new List<Dictionary<int, XSceneRefTarget>>
	{
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		}
	};
}
