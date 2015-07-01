using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public List<GameObject> Entities = new List<GameObject>();

    public void AddEntity(GameObject entity) 
    {
        entity.transform.SetParent(this.transform);
        Entities.Add(entity);
    }

    public GameObject RemoveEntity(GameObject entity)
    {
        GameObject result = null;
        if (Entities.Contains(entity))
        {
            Entities.Remove(entity);
            entity.transform.SetParent(null);
        }
        return result;
    }

    public int Cost()
    {
        int result = 0;
        for (int i = 0; i < Entities.Count; i++)
        {
            Walkable walkable = Entities[i].GetComponent<Walkable>();
            if (walkable)
            {
                result += walkable.Cost;
            }
        }
        return result;
    }
}
