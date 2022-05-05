using Unity.Collections;
using Unity.Mathematics;

public struct NativeArray2D<T> where T : struct
{
    private NativeArray<T> array;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public int Size => Width * Height;

    public T this[int i]
    {
        get => array[i];
    }

    public T this[int x, int y]
    {
        get => array[x + y * Width];
        set => array[x + y * Width] = value;
    }

    public T this[int2 index]
    {
        get => this[index.x, index.y];
        set => this[index.x, index.y] = value;
    }

    public NativeArray2D(int width, int height, NativeArray<T> array)
    {
        this.array = array;
        Width = width;
        Height = height;
    }

    public bool IndexInRange(int2 index)
    {
        return index.x >= 0 && index.x < Width && index.y >= 0 && index.y < Height;
    }

    public void Dispose()
    {
        if (array.IsCreated)
            array.Dispose();
    }
}