using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialCache
{
    static Dictionary<ColorEnum, Material> ballMats;
    public static Material GetBallMat(ColorEnum color)
    {
        if (ballMats == null)
        {
            ballMats = new Dictionary<ColorEnum, Material>();
        }

        if (!ballMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Ball/{color}");
            ballMats[color] = mat;
        }

        return mat;
    }

    static Dictionary<ColorEnum, Material> boxActiveSelectableMats;
    public static Material GetBoxActiveSelectableMat(ColorEnum color)
    {
        if (boxActiveSelectableMats == null)
        {
            boxActiveSelectableMats = new Dictionary<ColorEnum, Material>();
        }

        if (!boxActiveSelectableMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Box/ActiveSelectable/{color}");
            boxActiveSelectableMats[color] = mat;
        }

        return mat;
    }
    
    static Dictionary<ColorEnum, Material> boxActiveMats;
    public static Material GetBoxActiveMat(ColorEnum color)
    {
        if (boxActiveMats == null)
        {
            boxActiveMats = new Dictionary<ColorEnum, Material>();
        }

        if (!boxActiveMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Box/Active/{color}");
            boxActiveMats[color] = mat;
        }

        return mat;
    }

    static Dictionary<ColorEnum, Material> boxInactiveMats;
    public static Material GetBoxInactiveMat(ColorEnum color)
    {
        if (boxInactiveMats == null)
        {
            boxInactiveMats = new Dictionary<ColorEnum, Material>();
        }

        if (!boxInactiveMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Box/Inactive/{color}");
            boxInactiveMats[color] = mat;
        }

        return mat;
    }

    static Dictionary<int, Material> lockMats;
    public static Material GetLockMat(int lockCode)
    {
        if (lockMats == null)
        {
            lockMats = new Dictionary<int, Material>();
        }

        if (!lockMats.TryGetValue(lockCode, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Locks/{lockCode}");
            lockMats[lockCode] = mat;
        }

        return mat;
    }

    static Dictionary<int, Material> keyMats;
    public static Material GetKeyMat(int lockCode)
    {
        if (keyMats == null)
        {
            keyMats = new Dictionary<int, Material>();
        }

        if (!keyMats.TryGetValue(lockCode, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Keys/{lockCode}");
            keyMats[lockCode] = mat;
        }

        return mat;
    }

    static Dictionary<ColorEnum, Material> holeMainMats;
    public static Material GetHoleMainMat(ColorEnum color)
    {
        if (holeMainMats == null)
        {
            holeMainMats = new Dictionary<ColorEnum, Material>();
        }

        if (!holeMainMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Hole/Main/{color}");
            holeMainMats[color] = mat;
        }

        return mat;
    }

    static Dictionary<ColorEnum, Material> holeInsideMats;
    public static Material GetHoleInsideMat(ColorEnum color)
    {
        if (holeInsideMats == null)
        {
            holeInsideMats = new Dictionary<ColorEnum, Material>();
        }

        if (!holeInsideMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Hole/Inside/{color}");
            holeInsideMats[color] = mat;
        }

        return mat;
    }
}
