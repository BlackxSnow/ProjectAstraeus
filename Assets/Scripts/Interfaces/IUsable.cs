using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUsable
{
    void Use(Actor user);
    bool Act(Entity user, Entity target, object iteratedOn);
    bool GetNextIteration(Entity target, out object iterateOn, out float time);

}
