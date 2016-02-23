using System.Linq;
using UnityEngine;

public static class GameObjectExtensions {
	
	// 全ての子オブジェクトを返します
	// 非アクティブなオブジェクトも取得する場合にincludeInactiveにtrueを渡す
	public static GameObject[] GetChildren(this GameObject self, bool includeInactive = false) {
		return self.GetComponentsInChildren<Transform>(includeInactive)
			.Where(c => c != self.transform)
			.Select(c => c.gameObject)
			.ToArray();
	}
}