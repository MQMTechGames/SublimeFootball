using UnityEngine;

public class TestZoneManager : MonoBehaviour
{
    public bool doTest;
    public FieldSide TestSide;
    //public Vector3 TestPosition;
    public Transform TestTransform;

    public GameZone foundGameZone;

    private GameZonesManager _zonesManager;

    void Awake()
    {
        _zonesManager = FindObjectOfType<GameZonesManager>();
        DebugUtils.Assert(_zonesManager!=null, "_zonesManager!=null");
    }

    void Update()
    {
        if(doTest)
        {
            //doTest = false;
            Test();
        }
    }

    void Test()
    {
        foundGameZone = _zonesManager.GetGameZone(TestTransform.position, TestSide);
        Debug.Log("Game Zone Type is: " + foundGameZone.ZoneType);
    }
}
