using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Big,
    Medium,
    Small
}

public class Strength : MonoBehaviour
{
    public PieceType type;

    public int GetStrength()
    {
        switch (type)
        {
            case PieceType.Big:
                return 3;
            case PieceType.Medium:
                return 2;
            case PieceType.Small:
                return 1;
            default:
                return 0;
        }
    }
}