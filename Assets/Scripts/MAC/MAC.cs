using System;
using Random = System.Random;

[Serializable]
public struct SafeFloat
{
    private float _offset;
    private float _value;
    public float Value => _value - _offset;

    public SafeFloat(float value = 0)
    {
        _offset = MAC.Random.Range(MAC.Random.Range(-1000, -1), MAC.Random.Range(1, 1000));
        this._value = value + _offset;
    }

    public void Dispose()
    {
        _offset = 0;
        _value = 0;
    }

    public override string ToString() => Value.ToString();

    public static SafeFloat operator +(SafeFloat value1, SafeFloat value2) => new(value1.Value + value2.Value);
    public static SafeFloat operator +(SafeFloat value1, float value2) => new(value1.Value + value2);

    public static SafeFloat operator *(SafeFloat value1, SafeFloat value2) => new(value1.Value * value2.Value);
    public static SafeFloat operator *(SafeFloat value1, float value2) => new(value1.Value * value2);

    public static SafeFloat operator /(SafeFloat value1, SafeFloat value2) => new(value1.Value / value2.Value);
    public static SafeFloat operator /(SafeFloat value1, float value2) => new(value1.Value / value2);

    public static SafeFloat operator -(SafeFloat value1, SafeFloat value2) => new(value1.Value - value2.Value);
    public static SafeFloat operator -(SafeFloat value1, float value2) => new(value1.Value - value2);

    public static SafeFloat operator %(SafeFloat value1, SafeFloat value2) => new(value1.Value % value2.Value);
    public static SafeFloat operator %(SafeFloat value1, float value2) => new(value1.Value % value2);


    public static implicit operator SafeFloat(float x) => new(x);

    public static implicit operator float(SafeFloat x) => x.Value;
}

[Serializable]
public struct SafeInt
{
    private int _offset;
    private int _value;
    public int Value => _value - _offset;

    public SafeInt(int value = 0)
    {
        _offset = MAC.Random.Range(MAC.Random.Range(-1000, -1), MAC.Random.Range(1, 1000));
        this._value = value + _offset;
    }

    public void Dispose()
    {
        _offset = 0;
        _value = 0;
    }

    public override string ToString() => Value.ToString();

    public static SafeInt operator +(SafeInt value1, SafeInt value2) => new(value1.Value + value2.Value);
    public static SafeInt operator +(SafeInt value1, int value2) => new(value1.Value + value2);

    public static SafeInt operator *(SafeInt value1, SafeInt value2) => new(value1.Value * value2.Value);
    public static SafeInt operator *(SafeInt value1, int value2) => new(value1.Value * value2);

    public static SafeInt operator /(SafeInt value1, SafeInt value2) => new(value1.Value / value2.Value);
    public static SafeInt operator /(SafeInt value1, int value2) => new(value1.Value / value2);

    public static SafeInt operator -(SafeInt value1, SafeInt value2) => new(value1.Value - value2.Value);
    public static SafeInt operator -(SafeInt value1, int value2) => new(value1.Value - value2);

    public static SafeInt operator %(SafeInt value1, SafeInt value2) => new(value1.Value % value2.Value);
    public static SafeInt operator %(SafeInt value1, int value2) => new(value1.Value % value2);


    public static implicit operator SafeInt(int x) => new(x);

    public static implicit operator int(SafeInt x) => x.Value;
}

public static class MAC
{
    public static Random Random = new();
}
