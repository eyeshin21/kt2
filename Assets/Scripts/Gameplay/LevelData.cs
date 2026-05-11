using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int gridWidth = 0;
    public int gridHeight = 0;
    public List<GridSlotData> gridSlotsData = new List<GridSlotData>();

    public List<BoxData> boxesData = new List<BoxData>();
    public List<TunnelData> tunnelsData = new List<TunnelData>();
    public List<PinData> pinsData = new List<PinData>();
    public List<ClothData> clothsData = new List<ClothData>();
    public List<LockChainData> lockChainsData = new List<LockChainData>();

    public List<HoleDataDefault> holesDataDefault = new List<HoleDataDefault>();
    public List<HoleData> queueHoles = new List<HoleData>();
}

[System.Serializable]
public class HoleData
{
    public ColorEnum color = ColorEnum.Blue;
    public bool isHidden = false;
}

[System.Serializable]
public class HoleDataDefault
{
    public HoleData firstLayerHole = new HoleData();
    public HoleData secondLayerHole = new HoleData();
    public HoleData thirdLayerHole = new HoleData();
}

[System.Serializable]
public class GridSlotData
{
    public int coordinateX = 0;
    public int coordinateY = 0;
    public GridSlotType gridSlotType = GridSlotType.Road;
}

[System.Serializable]
public enum GridSlotType
{
    None = 0,
    Road = 1
}

[System.Serializable]
public class BoxData
{
    public int coordinateX = 0;
    public int coordinateY = 0;

    public ColorEnum color = ColorEnum.Blue;

    public bool isHidden = false;

    public bool hasIce = false;
    public int iceCount = 0;

    public bool isLink = false;
    public Vector2IntS linkedPos;

    public bool hasCrate = false;
    public int crateCount = 0;

    public bool isLock = false;
    public bool isKey = false;
    public int lockCode = 0;

    public bool hasShutter = false;
    public bool isShutterOpen = false;
}

[System.Serializable]
public class TunnelData
{
    public int coordinateX = 0;
    public int coordinateY = 0;
    public Direction direction = Direction.None;
    public List<BoxData> boxesData = new List<BoxData>();
}

[System.Serializable]
public class PinData
{
    public int coordinateX = 0;
    public int coordinateY = 0;
    public Direction direction = Direction.None;
}

[System.Serializable]
public class ClothData
{
    public int coordinateX = 0;
    public int coordinateY = 0;
    public ClothType clothType = ClothType.None;
    public int clothCount = 0;
}

[System.Serializable]
public class LockChainData
{
    public int coordinateX = 0;
    public int coordinateY = 0;
    public Axis lockChainAxis = Axis.None;
    public int lockChainLength = 0;
    public int lockChainCode = 0;
}

[System.Serializable]
public enum Direction
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4
}

[System.Serializable]
public enum Axis
{
    None = 0,
    Vertical = 1,
    Horizontal = 2,
}

[System.Serializable]
public enum ClothType
{
    None = 0,
    TwoxTwo = 1,
    TwoxThree = 2,
    ThreexTwo = 3
}


[System.Serializable]
public class Vector2IntS
{
    public int x;
    public int y;

    public Vector2IntS()
    {

    }

    public Vector2IntS(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2IntS(Vector2Int position)
    {
        x = position.x;
        y = position.y;
    }
}

[System.Serializable]
public class Vector3S
{
    public float x;
    public float y;
    public float z;

    public Vector3S()
    {
    }

    public Vector3S(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3S(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }
}

public static class VectorExtensions
{
    public static Vector2Int ToVector2Int(this Vector2IntS vector2IntS)
    {
        return new Vector2Int(vector2IntS.x, vector2IntS.y);
    }

    public static Vector2IntS ToVector2IntS(this Vector2Int vector2Int)
    {
        return new Vector2IntS(vector2Int);
    }

    public static Vector3 ToVector3(this Vector3S vector3S)
    {
        return new Vector3(vector3S.x, vector3S.y, vector3S.z);
    }

    public static Vector3S ToVector3S(this Vector3 vector3)
    {
        return new Vector3S(vector3);
    }
}