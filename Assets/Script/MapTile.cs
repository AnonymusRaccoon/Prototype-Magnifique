using UnityEngine;

[System.Serializable]
public class MapTile
{
    public string name;
    public Color color;
    public GameObject leftPrefab;
    public GameObject middlePrefab;
    public GameObject rightPrefab;
    public GameObject underPrefab;
    public GameObject leftUnder;
    public GameObject rightUnder;
    public GameObject bothUnder;
    public GameObject bothTop;

    public GameObject cornerLT;
    public GameObject cornerRT;

    public GameObject cornerLTR;
    public GameObject cornerRTL;

    public GameObject cornerLTRT;
}
