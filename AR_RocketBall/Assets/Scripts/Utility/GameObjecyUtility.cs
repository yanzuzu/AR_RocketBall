using UnityEngine;
using System.Collections.Generic;

public static class GameObjectUtils
{
	public static T GetChildComponent<T>(this GameObject root, string name) where T : Component
	{
		GameObject go = FindChild(root, name);

		if(go != null)
		{
			return go.GetComponent<T>();
		}

		return null;
	}


	public static T GetChildComponentByPath<T>(this GameObject root, string path) where T : Component
	{
		GameObject go = FindChildByPath(root, path);

		if(go != null)
		{
			return go.GetComponent<T>();
		}

		return null;
	}

	public static List<GameObject> FindChildByTag(this GameObject root, string tag)
	{
		List<GameObject> result = new List<GameObject> ();
		GameObject go;
		int cnt = 0;

		// Find the children in base level first.
		for(cnt = 0; cnt < root.transform.childCount; cnt++)
		{
			go = root.transform.GetChild(cnt).gameObject;

			if(go.tag == tag)
			{
				result.Add (go);
			}
		}

		// Find children's children
		for(cnt = 0; cnt < root.transform.childCount; cnt++)
		{
			go = root.transform.GetChild(cnt).gameObject;

			if(go.transform.childCount > 0)
			{
				List<GameObject> gos = FindChildByTag(go, tag);
				result.AddRange(gos);
			}
		}

		return result;
	}

	public static GameObject FindChild(this GameObject root, string name)
	{
		GameObject go = null;
		int cnt = 0;

		if(root == null)
		{
			return null;
		}

		// Find the children in base level first.
		for(cnt = 0; cnt < root.transform.childCount; cnt++)
		{
			go = root.transform.GetChild(cnt).gameObject;

			if(go.name == name)
			{
				return go;
			}
		}

		// Find children's children
		for(cnt = 0; cnt < root.transform.childCount; cnt++)
		{
			go = root.transform.GetChild(cnt).gameObject;

			if(go.transform.childCount > 0)
			{
				go = FindChild(go, name);

				if(go != null)
				{
					return go;
				}
			}
		}

		return null;
	}


	public static GameObject FindChildByPath(this GameObject root, string path)
	{	
		if(root == null)
		{
			return null;
		}

		string[] argus = path.Split('/');
		GameObject result = null;
		GameObject curGameObj = root;
		int idx = -1;

		for (int i = 0; i < argus.Length; i++)
		{
			idx++;
			curGameObj = curGameObj.FindChild(argus[i]);
			if (curGameObj == null)
			{
				break;
			}

			if (idx == argus.Length - 1)
			{
				result = curGameObj;
			}
		}

		return result;
	}


	public static void AddChild(this GameObject parent, GameObject child)
	{
		if (null != child && null != parent)
		{
			child.transform.SetParent(parent.transform, false);
		}
	}


	public static void FitScreen(this GameObject parent)
	{
		float ratio = (float)Screen.width / Screen.height;

		parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1080 * ratio, 1080);
	}

	public static void ChangeLayer(this GameObject parent, string layerName )
	{
		if (null == parent)
		{
			return;
		}
		parent.layer = LayerMask.NameToLayer (layerName);
		foreach( Transform t in parent.transform)
		{
			ChangeLayer(t.gameObject,layerName);
		}
	}

	public static void DeleteAllChilds(this GameObject root, string[] exceptions = null)
	{
		if (root.transform.childCount == 0)
		{
			return;
		}

		bool delete = true;

		for(int i = 0; i < root.transform.childCount;)
		{
			delete = true;

			if (null != exceptions)
			{
				for (int j = 0; j < exceptions.Length; j++)
				{
					if (root.transform.GetChild(i).name == exceptions[j])
					{
						delete = false;
						break;
					}
				}
			}

			if (delete)
			{
				GameObject.DestroyImmediate(root.transform.GetChild(i).gameObject, true);
			}
			else
			{
				i++;
			}
		}
	}
}
