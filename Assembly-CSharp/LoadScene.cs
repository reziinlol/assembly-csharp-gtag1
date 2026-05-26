using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200002E RID: 46
public class LoadScene : MonoBehaviour
{
	// Token: 0x060000A8 RID: 168 RVA: 0x00005396 File Offset: 0x00003596
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this._sceneName, LoadSceneMode.Single);
		while (asyncOperation.progress < 0.99f)
		{
			yield return null;
		}
		asyncOperation.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x040000C7 RID: 199
	[SerializeField]
	private float _delay;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	private string _sceneName;
}
