public class DungeonPathMarker {
    public MapLocation Location;
    public float G;
    public float H;
    public float F;

    public DungeonPathMarker Parent;
    public DungeonPathMarker(MapLocation l, float g, float h, float f, DungeonPathMarker parent)
    {
        Location = l;
        G = g;
        H = h;
        F = f;
        Parent = parent;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return Location.Equals(((DungeonPathMarker)obj).Location);
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}