using UnityEngine;

public static class LayerManager
{
    public static int Solid { get; private set; }
    public static int Tank { get; private set; }
    public static LayerMask TankMask { get; private set; }
    public static LayerMask DestructibleMask { get; private set; }
    public static LayerMask UIMask { get; private set; }

    static LayerManager()
    {
        TankMask = LayerMask.GetMask("Tank");
        DestructibleMask = LayerMask.GetMask("Destructible");
        Tank = LayerMask.NameToLayer(LayerMask.LayerToName(TankMask));
        Solid = LayerMask.NameToLayer("Solid");
        UIMask = LayerMask.GetMask("UI");
    }
}
