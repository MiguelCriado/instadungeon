using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CursorController : MonoBehaviour {

    [SerializeField] Vector2 cursorPosition;
    [SerializeField] GameObject cursorPrefab;
    [SerializeField]
    GameObject dotPathPrefab;
    [SerializeField]
    GameObject xLinePath;
    [SerializeField] 
    GameObject yLinePath;
    [SerializeField] GameObject xSpot;

    public Vector2Int[] path;

    [SerializeField] Vector2 mLastTile;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        ResolveCursorPosition();
        if (TileChanged())
        {
            UpdateCursorPrefabState();
        }
        xSpot.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // ProcessClick();
        mLastTile = cursorPosition;
        DrawPath();
	}

    private void ResolveCursorPosition()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition = IDTools.IsoToCartesian(worldPoint.x, worldPoint.y);
        cursorPosition = IDTools.PointToTile(cursorPosition.x, cursorPosition.y);
    }

    private void UpdateCursorPrefabState()
    {
        if (GameManager.Instance.GetTile((int)cursorPosition.x, (int)cursorPosition.y) != null)
        {
            cursorPrefab.SetActive(true);
            cursorPrefab.transform.position = IDTools.CartesianToIso(cursorPosition.x, cursorPosition.y);
        }
        else
        {
            cursorPrefab.SetActive(false);
        } 
    }

    private void DrawPath() {
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(IDTools.CartesianToIso(path[i].x, path[i].y), IDTools.CartesianToIso(path[i + 1].x, path[i + 1].y));
        }
    }

    private bool TileChanged()
    {
        return !cursorPosition.Equals(mLastTile);
    }

    private void ProcessClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clicked!!");
            List<Vector2Int> pathFound = GameManager.Instance.FindPath(GameManager.Instance.characterPosition, cursorPosition);
            if (pathFound != null)
            {
                path = pathFound.ToArray();
                for (int i = 0; i < path.Length - 1; i++)
                {
                    Instantiate(dotPathPrefab, IDTools.CartesianToIso(path[i].x, path[i].y), Quaternion.identity);
                }
            }
        }
    }

    

    

}
