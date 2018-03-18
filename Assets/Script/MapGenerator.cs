using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform mapContainer;

    [SerializeField] private MapTile[] tiles;
    [SerializeField] private Texture2D[] maps;

    private Texture2D map;

    private void Start()
    {
        //if (GameObject.Find("GameManager").GetComponent<NetworkManager>().selectedMap != -1)
        //    map = maps[GameObject.Find("GameManager").GetComponent<NetworkManager>().selectedMap];
        //else
            map = maps[Random.Range(0, maps.Length)];

        PlaceCamera();

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                CreateTile(x, y);
            }
        }
    }

    private void PlaceCamera()
    {
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Vector3 topLeft = new Vector3(0, map.height, 0);
        Vector3 bottomRight = new Vector3(map.width, 0, 0);

        cam.transform.position = new Vector3(map.width / 2, map.height / 2, -Vector3.Distance(topLeft, bottomRight) / 2.5f);
        NetworkManager netManager = GameObject.Find("GameManager").GetComponent<NetworkManager>();
        StartCoroutine(netManager.SetDeathZone(topLeft, bottomRight));
    }

    private void CreateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);
        MapTile tile = null;

        foreach(MapTile foo in tiles)
        {
            if(foo.color == pixelColor)
            {
                tile = foo;
            }
        }

        if (tile == null)
            return;

        bool topPosition = (y + 1 < map.height) ? map.GetPixel(x, y + 1) == pixelColor : false;
        //bool underPosition = (y - 1 > 0) ? map.GetPixel(x, y - 1) == pixelColor : false;
        bool leftPosition = (x - 1 > 0) ? map.GetPixel(x - 1, y) == pixelColor : false;
        bool rightPosition = (x + 1 < map.width) ? map.GetPixel(x + 1, y) == pixelColor : false;

        GameObject prefab = null;

        if (topPosition)
        {
            if (!leftPosition)
            {
                prefab = tile.leftUnder;

                if (!rightPosition)
                    prefab = tile.bothUnder;
            }
            else if (!rightPosition)
            {
                prefab = tile.rightUnder;
            }
            else
            {
                prefab = tile.underPrefab;
            }
        }
        else if (!leftPosition)
        {
            prefab = tile.leftPrefab;

            if (!rightPosition)
                prefab = tile.bothTop;
        }
        else if (!rightPosition)
        {
            prefab = tile.rightPrefab;
        }
        else
        {
            prefab = tile.middlePrefab;
        }

        Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, mapContainer);
    }
}
