using UnityEngine;

public class GameZonesManager : MonoBehaviour
{
    private const float kHitRadius = 1f;

    [SerializeField]
    LayerMask _leftSideZoneLayer;

    [SerializeField]
    LayerMask _rightSideZoneLayer;

    [SerializeField]
    GameZone _defaultGameZone;

    public GameZone GetGameZone(Vector3 position, FieldSide side)
    {
        if(side == FieldSide.Left)
        {
            return GetGameZone(position, _leftSideZoneLayer);
        }
        else
        {
            return GetGameZone(position, _rightSideZoneLayer);
        }
    }

    private GameZone GetGameZone(Vector3 position, LayerMask mask)
    {
        Collider[] hitCOlliders = Physics.OverlapSphere(position, kHitRadius, mask);
        foreach (Collider collider in hitCOlliders)
        {
            GameZone zone = collider.gameObject.GetComponent<GameZone>();
            if(zone != null)
            {
                return zone;
            }
        }

        return _defaultGameZone;
    }
}
