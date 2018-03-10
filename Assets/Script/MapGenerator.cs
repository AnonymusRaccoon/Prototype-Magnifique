using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D[] maps;
    private Texture2D map;

    private void Start()
    {
        if (GameObject.Find("GameManager").GetComponent<NetworkManager>().selectedMap != -1)
            map = maps[GameObject.Find("GameManager").GetComponent<NetworkManager>().selectedMap];
        else
            map = maps[Random.Range(0, maps.Length)];

        for(int x = 0; x < map.width; x++)
        {
            for(int y = 0; y < map.height; y++)
            {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y)
    {
        //Color pixelColor = map.GetPixel(x, y);
    }
}
