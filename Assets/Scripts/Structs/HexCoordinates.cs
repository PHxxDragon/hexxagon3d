using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HexCoordinates
{
    public int q;
    public int r;

    private static Dictionary<HexCoordinates, HexCoordinates> clockWise = new Dictionary<HexCoordinates, HexCoordinates>();
    static HexCoordinates() {
        clockWise.Add(DOWNLEFT(), RIGHT());
        clockWise.Add(DOWNRIGHT(), UPRIGHT());
        clockWise.Add(RIGHT(), UPLEFT());
        clockWise.Add(UPRIGHT(), LEFT());
        clockWise.Add(UPLEFT(), DOWNLEFT());
        clockWise.Add(LEFT(), DOWNRIGHT());
    }
    public HexCoordinates(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    

    public static HexCoordinates FromQR(int q, int r)
    {
        return new HexCoordinates(q, r);
    }

    public static HexCoordinates FromRS(int r, int s)
    {
        return new HexCoordinates(-s -r, r);
    }

    public static HexCoordinates FromQS(int q, int s)
    {
        return new HexCoordinates(q, -s - q);
    }

    public static HexCoordinates operator* (HexCoordinates a, int b)
    {
        return new HexCoordinates(a.q * b, a.r * b);
    }

    public static HexCoordinates operator+ (HexCoordinates a, HexCoordinates b)
    {
        return new HexCoordinates(a.q + b.q, a.r + b.r);
    }

    public static HexCoordinates operator- (HexCoordinates a, HexCoordinates b)
    {
        return new HexCoordinates(a.q - b.q, a.r - b.r);
    }

    public static int Distance(HexCoordinates a, HexCoordinates b)
    {
        return (Math.Abs(a.q - b.q) + Math.Abs(a.r - b.r) + Math.Abs(a.s - b.s)) / 2;
    }

    public HexCoordinates Copy()
    {
        return new HexCoordinates(q, r);
    }

    public HexCoordinates Left(int inc = 1)
    {
        return new HexCoordinates(q - inc, r);
    }


    public HexCoordinates Right(int inc = 1)
    {
        return new HexCoordinates(q + inc, r);
    }

    public HexCoordinates Upleft(int inc = 1)
    {
        return new HexCoordinates(q, r - inc);
    }

    public HexCoordinates Upright(int inc = 1)
    {
        return new HexCoordinates(q + inc, r - inc);
    }

    public HexCoordinates Downleft(int inc = 1)
    {
        return new HexCoordinates(q - inc, r + inc);
    }

    public HexCoordinates Downright(int inc = 1)
    {
        return new HexCoordinates(q, r + inc);
    }

    public static HexCoordinates LEFT(int inc = 1)
    {
        return new HexCoordinates(- inc, 0);
    }


    public static HexCoordinates RIGHT(int inc = 1)
    {
        return new HexCoordinates(inc, 0);
    }

    public static HexCoordinates UPLEFT(int inc = 1)
    {
        return new HexCoordinates(0, - inc);
    }

    public static HexCoordinates UPRIGHT(int inc = 1)
    {
        return new HexCoordinates(inc, - inc);
    }

    public static HexCoordinates DOWNLEFT(int inc = 1)
    {
        return new HexCoordinates(-inc, inc);
    }

    public static HexCoordinates DOWNRIGHT(int inc = 1)
    {
        return new HexCoordinates(0, inc);
    }

    public static HexCoordinates ZERO()
    {
        return new HexCoordinates(0, 0);
    }

    public static IEnumerable<HexCoordinates> ListAllCoordsAtRange(HexCoordinates center, int range)
    {
        if (range == 0)
        {
            yield return center;
            yield break;
        }
        if (range == 1)
        {
            yield return center.Downleft(1);
            yield return center.Left(1);
            yield return center.Upleft(1);
            yield return center.Upright(1);
            yield return center.Right(1);
            yield return center.Downright(1);
            yield break;
        }
        else
        {
            foreach (var coords in ListAllCoordsAtRange(ZERO(), 1))
            {
                for (int i = 0; i < range; i++)
                {
                    yield return center + coords * range + clockWise[coords] * i;
                }
            }
        }
    }

    public override bool Equals(object obj)
    {
        var item = obj as HexCoordinates;
        if (item == null)
        {
            return false;
        }

        return q == item.q && r == item.r;
    }

    public override int GetHashCode()
    {
        return (q, r).GetHashCode();
    }

    public int s { 
        get
        {
            return -q -r;
        }
    }
}
