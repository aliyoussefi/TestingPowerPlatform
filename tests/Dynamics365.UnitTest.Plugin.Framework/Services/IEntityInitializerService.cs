using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Services
{
    public interface IEntityInitializerService
    {
        //
        // Parameters:
        //   e:
        //
        //   ctx:
        //
        //   isManyToManyRelationshipEntity:
        Entity Initialize(Entity e, XrmFakedContext ctx, bool isManyToManyRelationshipEntity = false);

        //
        // Parameters:
        //   e:
        //
        //   gCallerId:
        //
        //   ctx:
        //
        //   isManyToManyRelationshipEntity:
        Entity Initialize(Entity e, Guid gCallerId, XrmFakedContext ctx, bool isManyToManyRelationshipEntity = false);
    }
}
