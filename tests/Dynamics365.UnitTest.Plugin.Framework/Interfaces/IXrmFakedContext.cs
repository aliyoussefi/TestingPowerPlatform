using Dynamics365.UnitTest.Plugin.Framework.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework
{
    public interface IXrmFakedContext : IXrmBaseContext
    {
        //
        // Summary:
        //     Proxy Types Assemblies
        IEnumerable<Assembly> ProxyTypesAssemblies { get; }

        //
        // Summary:
        //     Relationships
        IEnumerable<XrmFakedRelationship> Relationships { get; }

        //
        // Summary:
        //     Creates a queryable for a strongly-typed entity
        //
        // Type parameters:
        //   T:
        IQueryable<T> CreateQuery<T>() where T : Entity;

        //
        // Summary:
        //     Creates a queryable for a late bound entity
        //
        // Parameters:
        //   logicalName:
        IQueryable<Entity> CreateQuery(string logicalName);

        //
        // Summary:
        //     Retrieves an entity by primary key as currently stored in the in-memory database.
        //     Useful if you want to bypass a retrieve message, and simpler than using CreateQuery.
        //
        // Parameters:
        //   id:
        //
        // Type parameters:
        //   T:
        T GetEntityById<T>(Guid id) where T : Entity;

        //
        // Summary:
        //     Same as GetEntityById<T> but for late bound entities
        //
        // Parameters:
        //   logicalName:
        //     Logical name of the entity record to retrieve
        //
        //   id:
        //     Primary key of the entity record to retrieve
        Entity GetEntityById(string logicalName, Guid id);

        //
        // Summary:
        //     Returns true if record of the logicalName and id exists in the in-memory database
        //
        // Parameters:
        //   logicalName:
        //
        //   id:
        bool ContainsEntity(string logicalName, Guid id);

        //
        // Summary:
        //     Receives a list of entities, that are used to initialize the context with those
        //
        // Parameters:
        //   entities:
        void Initialize(IEnumerable<Entity> entities);

        //
        // Summary:
        //     Initializes the context with a single entity when only one is needed
        //
        // Parameters:
        //   entity:
        //     The entity to initialize the context with
        void Initialize(Entity entity);

        //
        // Summary:
        //     Add Entity
        //
        // Parameters:
        //   e:
        void AddEntity(Entity e);

        //
        // Summary:
        //     Add Entity with defaults
        //
        // Parameters:
        //   e:
        //
        //   clone:
        void AddEntityWithDefaults(Entity e, bool clone = false);

        //
        // Summary:
        //     Create Entity
        //
        // Parameters:
        //   e:
        Guid CreateEntity(Entity e);

        //
        // Summary:
        //     Update Entity
        //
        // Parameters:
        //   e:
        void UpdateEntity(Entity e);

        //
        // Summary:
        //     Delete Entity
        //
        // Parameters:
        //   er:
        void DeleteEntity(EntityReference er);

        //
        // Summary:
        //     Find reflected Type
        //
        // Parameters:
        //   logicalName:
        Type FindReflectedType(string logicalName);

        //
        // Summary:
        //     Find reflected Attribute Type
        //
        // Parameters:
        //   earlyBoundType:
        //
        //   sEntityName:
        //
        //   attributeName:
        Type FindReflectedAttributeType(Type earlyBoundType, string sEntityName, string attributeName);

        //
        // Summary:
        //     Enable Proxy Types
        //
        // Parameters:
        //   assembly:
        void EnableProxyTypes(Assembly assembly);

        //
        // Summary:
        //     Initialize Metadata from enumeration
        //
        // Parameters:
        //   entityMetadataList:
        void InitializeMetadata(IEnumerable<EntityMetadata> entityMetadataList);

        //
        // Summary:
        //     Initialize Metadata from single entity
        //
        // Parameters:
        //   entityMetadata:
        void InitializeMetadata(EntityMetadata entityMetadata);

        //
        // Summary:
        //     Initialize Metadata from early-bound assembly
        //
        // Parameters:
        //   earlyBoundEntitiesAssembly:
        void InitializeMetadata(Assembly earlyBoundEntitiesAssembly);

        //
        // Summary:
        //     Create Metadata Query
        IQueryable<EntityMetadata> CreateMetadataQuery();

        //
        // Summary:
        //     Get Entity Metadata by name
        //
        // Parameters:
        //   sLogicalName:
        EntityMetadata GetEntityMetadataByName(string sLogicalName);

        //
        // Summary:
        //     Set Entity Metadata
        //
        // Parameters:
        //   em:
        void SetEntityMetadata(EntityMetadata em);

        //
        // Summary:
        //     Add Relationship
        //
        // Parameters:
        //   schemaname:
        //
        //   relationship:
        void AddRelationship(string schemaname, XrmFakedRelationship relationship);

        //
        // Summary:
        //     Remove Relationship
        //
        // Parameters:
        //   schemaname:
        void RemoveRelationship(string schemaname);

        //
        // Summary:
        //     Get Relationship
        //
        // Parameters:
        //   schemaName:
        XrmFakedRelationship GetRelationship(string schemaName);

        //
        // Summary:
        //     Get Record Unique Id (handles alternate keys)
        //
        // Parameters:
        //   record:
        //
        //   validate:
        Guid GetRecordUniqueId(EntityReference record, bool validate = true);
    }
    public interface IXrmContext
    {
        //
        // Summary:
        //     Returns an instance of an organization service
        IOrganizationService GetOrganizationService();
    }



    public interface IXrmFakedPluginExecutionContext
    {
    }
}
