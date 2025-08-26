using UnityEngine;

public struct Pieces
{
    public PieceType _piece;
    public GameObject _model;

    public Pieces(PieceType piece, GameObject model)
    {
        _piece = piece;
        _model = model;
    }
}

public enum PieceType
{
    Horizontal_Straight,
    Vertical_Straight,
    Right_Up_Corner,
    Right_Down_Corner,
    Left_Up_Corner,
    Left_Down_Corner,
    T_Junction,
    TUpsideDown,
    TToLeft,
    TToRight,
    DeadEnd,
    DeadUpsideDown,
    DeadToright,
    DeadToLeft,
    Wall,
    Crossroad,
    Room
}