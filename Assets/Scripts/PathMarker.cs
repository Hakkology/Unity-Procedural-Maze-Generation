using UnityEngine;

public class PathMarker 
{
    public MapLocation Location;
    public float G;
    public float H;
    public float F;

    public GameObject Marker;
    public PathMarker Parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject marker, PathMarker parent)
    {
        Location = l;
        G = g;
        H = h;
        F = f;
        Marker = marker;
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
            return Location.Equals(((PathMarker)obj).Location);
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}