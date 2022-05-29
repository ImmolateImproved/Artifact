using Unity.Mathematics;

public static class MovementUtils
{
    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        float3 a = target - current;
        float magnitude = math.length(a);
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }
        return current + a / magnitude * maxDistanceDelta;
    }

    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta, out float distance)
    {
        var currentPosition = MoveTowards(current, target, maxDistanceDelta);

        distance = math.distance(current, target);

        return currentPosition;
    }
}