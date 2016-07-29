using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PollObjects : MonoBehaviour
{
	public static PollObjects current;
	private List<GameObject> poll = new List<GameObject>();
    private Transform container;

	void Awake()
	{
		current = this;
	}

	public void FillPool(GameObject gameObj,int cnt = 1, Transform parent = null)
	{
        container = parent;


        for (int i = 0; i < cnt; i++)
		{
			_add(gameObj,parent);
		}
	}

	public GameObject Get(GameObject gameObj)
	{
        GameObject _result = null;
        if(gameObj != null)
        {
            for (int i = 0; i < poll.Count; i++)
            {
                if (poll[i] == null)
                    poll.RemoveAt(i);
                else if (!poll[i].activeSelf && gameObj.name == poll[i].name && poll[i].transform.parent == container)
                {
                    _result = poll[i];
                    break;
                }
            }


            if(_result == null)
                _result = _add(gameObj, container);
        }

        return _result;
	}

	private GameObject _add(GameObject gameObj, Transform parent = null)
	{
		GameObject obj = (GameObject) Instantiate(gameObj,new Vector3(0,0,0), Quaternion.identity);
		obj.SetActive(false);
		obj.name = gameObj.name;
		if(parent)
			obj.gameObject.transform.parent = parent;
		poll.Add(obj);
		return obj;
	}
}
