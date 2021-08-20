using Unity.Entities;
using Latios;

public class AttackSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Moving>().WithNone<Movinginternal>()
           .ForEach((Entity e) =>
           {
               

           }).WithStructuralChanges().Run();
    }
}