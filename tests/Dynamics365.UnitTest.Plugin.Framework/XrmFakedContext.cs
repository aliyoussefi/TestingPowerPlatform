using Dynamics365.UnitTest.Plugin.Framework.Interfaces.Plugins;
using Dynamics365.UnitTest.Plugin.Framework.Interfaces;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Dynamics365.UnitTest.Plugin.Framework.Middleware;
using Dynamics365.UnitTest.Plugin.Framework.Database;
using Dynamics365.UnitTest.Plugin.Framework.Services;
using Dynamics365.UnitTest.Plugin.Framework.Extensions;
using Dynamics365.UnitTest.Plugin.Framework.Integrity;

namespace Dynamics365.UnitTest.Plugin.Framework
{
    public class XrmFakedContext : IXrmFakedContext, IXrmBaseContext
    {
        //
        // Parameters:
        //   req:
        public delegate OrganizationResponse ServiceRequestExecution(OrganizationRequest req);

        //
        // Summary:
        //     Entity Active StateCode
        protected const int EntityActiveStateCode = 0;

        //
        // Summary:
        //     Entity Inactive StateCode
        protected const int EntityInactiveStateCode = 1;

        //
        // Summary:
        //     Internal middleware setup
        internal IMiddlewareBuilder _builder;

        protected internal IOrganizationService _service;

        private readonly Dictionary<string, object> _properties;

        private readonly IXrmFakedTracingService _fakeTracingService;

        //
        // Summary:
        //     Stores the current license context (the current selected license of the 3 available
        //     licenses)
        //public FakeXrmEasyLicense? LicenseContext { get; set; }

        public IXrmFakedPluginContextProperties PluginContextProperties { get; set; }

        //
        // Summary:
        //     All proxy type assemblies available on mocked database.
        private List<Assembly> _proxyTypesAssemblies { get; set; }

        public IEnumerable<Assembly> ProxyTypesAssemblies => _proxyTypesAssemblies;

        protected internal bool Initialised { get; set; }

        //
        // Summary:
        //     Internal In-Memory Database
        internal InMemoryDb Db { get; set; }

        [Obsolete("Please use ProxyTypesAssemblies to retrieve assemblies and EnableProxyTypes to add new ones")]
        public Assembly ProxyTypesAssembly {
            get {
                return _proxyTypesAssemblies.FirstOrDefault();
            }
            set {
                _proxyTypesAssemblies = new List<Assembly>();
                if (value != null)
                {
                    _proxyTypesAssemblies.Add(value);
                }
            }
        }

        //
        // Summary:
        //     Sets the user to assign the CreatedBy and ModifiedBy properties when entities
        //     are added to the context. All requests will be executed on behalf of this user
        [Obsolete("Please use CallerProperties instead")]
        public EntityReference CallerId { get; set; }

        [Obsolete("Please use CallerProperties instead")]
        public EntityReference BusinessUnitId { get; set; }

        private Dictionary<string, XrmFakedRelationship> _relationships { get; set; }

        //
        // Summary:
        //     Relationships
        public IEnumerable<XrmFakedRelationship> Relationships => _relationships.Values;

        public IEntityInitializerService EntityInitializerService { get; set; }

        //
        // Summary:
        //     Default max count value when retrieving data, defaults to 5000
        public int MaxRetrieveCount { get; set; }

        public EntityInitializationLevel InitializationLevel { get; set; }

        public ICallerProperties CallerProperties { get; set; }

        //
        // Summary:
        //     System TimeZone
        public TimeZoneInfo SystemTimeZone { get; set; }

        //
        // Summary:
        //     Stores some minimal metadata info if dynamic entities are used and no injected
        //     metadata was used
        protected internal Dictionary<string, Dictionary<string, string>> AttributeMetadataNames { get; set; }

        //
        // Parameters:
        //   record:
        //
        //   validate:
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        public Guid GetRecordUniqueId(EntityReference record, bool validate = true)
        {
            if (string.IsNullOrWhiteSpace(record.LogicalName))
            {
                throw new InvalidOperationException("The entity logical name must not be null or empty.");
            }

            if (!Db.ContainsTable(record.LogicalName) && !Db.ContainsTableMetadata(record.LogicalName))
            {
                if (ProxyTypesAssemblies == null || !ProxyTypesAssemblies.Any())
                {
                    throw new InvalidOperationException("The entity logical name " + record.LogicalName + " is not valid.");
                }

                if (!ProxyTypesAssemblies.SelectMany((Assembly p) => p.GetTypes()).Any((Type type) => FindReflectedType(record.LogicalName) != null))
                {
                    throw new InvalidOperationException("The entity logical name " + record.LogicalName + " is not valid.");
                }
            }

            if (record.Id == Guid.Empty && record.HasKeyAttributes())
            {
                if (Db.ContainsTableMetadata(record.LogicalName))
                {
                    EntityMetadata tableMetadata = Db.GetTableMetadata(record.LogicalName);
                    EntityKeyMetadata[] keys = tableMetadata.Keys;
                    foreach (EntityKeyMetadata entityKeyMetadata in keys)
                    {
                        if (record.KeyAttributes.Keys.Count != entityKeyMetadata.KeyAttributes.Length || !entityKeyMetadata.KeyAttributes.All((string x) => record.KeyAttributes.Keys.Contains(x)))
                        {
                            continue;
                        }

                        if (Db.ContainsTable(record.LogicalName))
                        {
                            InMemoryTable table = Db.GetTable(record.LogicalName);
                            Entity entity = table.Rows.SingleOrDefault((Entity x) => record.KeyAttributes.All((KeyValuePair<string, object> k) => x.Attributes.ContainsKey(k.Key) && x.Attributes[k.Key] != null && x.Attributes[k.Key].Equals(k.Value)));
                            if (entity != null)
                            {
                                return entity.Id;
                            }
                        }

                        if (validate)
                        {
                            throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
                            {
                                Message = record.LogicalName + " with the specified Alternate Keys Does Not Exist"
                            });
                        }
                    }
                }

                if (validate)
                {
                    throw new InvalidOperationException("The requested key attributes do not exist for the entity " + record.LogicalName);
                }
            }

            return record.Id;
        }

        //
        // Summary:
        //     Updates an entity in the context directly (i.e. skips any middleware setup)
        //
        // Parameters:
        //   e:
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        public void UpdateEntity(Entity e)
        {
            if (e == null)
            {
                throw new InvalidOperationException("The entity must not be null");
            }

            e = e.Clone(e.GetType());
            EntityReference record = e.ToEntityReferenceWithKeyAttributes();
            e.Id = GetRecordUniqueId(record);
            if (ContainsEntity(e.LogicalName, e.Id))
            {
                IIntegrityOptions property = GetProperty<IIntegrityOptions>();
                InMemoryTable table = Db.GetTable(e.LogicalName);
                Entity byId = table.GetById(e.Id);
                foreach (string item in e.Attributes.Keys.ToList())
                {
                    object obj = e[item];
                    if (obj == null)
                    {
                        byId.Attributes.Remove(item);
                        continue;
                    }

                    if (obj is DateTime)
                    {
                        byId[item] = ConvertToUtc((DateTime)e[item]);
                        continue;
                    }

                    if (obj is EntityReference && property.ValidateEntityReferences)
                    {
                        EntityReference er = (EntityReference)e[item];
                        obj = ResolveEntityReference(er);
                    }

                    byId[item] = obj;
                }

                byId["modifiedon"] = DateTime.UtcNow;
                byId["modifiedby"] = CallerProperties.CallerId;
                return;
            }

            throw FakeOrganizationServiceFaultFactory.New($"{e.LogicalName} with Id {e.Id} Does Not Exist");
        }

        //
        // Summary:
        //     Returns an entity record by logical name and primary key
        //
        // Parameters:
        //   logicalName:
        //
        //   id:
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        public Entity GetEntityById(string logicalName, Guid id)
        {
            Entity entityById_Internal = GetEntityById_Internal(logicalName, id);
            return entityById_Internal.Clone(null, this);
        }

        internal Entity GetEntityById_Internal(string sLogicalName, Guid id, Type t = null)
        {
            if (!Db.ContainsTable(sLogicalName))
            {
                throw new InvalidOperationException("The entity logical name '" + sLogicalName + "' is not valid.");
            }

            InMemoryTable table = Db.GetTable(sLogicalName);
            if (!table.Contains(id))
            {
                throw new InvalidOperationException("The id parameter '" + id.ToString() + "' for entity logical name '" + sLogicalName + "' is not valid.");
            }

            return table.GetById(id);
        }

        //
        // Summary:
        //     Returns true if the entity record with the specified logical name and id exists
        //     in the InMemory database
        //
        // Parameters:
        //   sLogicalName:
        //
        //   id:
        public bool ContainsEntity(string sLogicalName, Guid id)
        {
            if (!Db.ContainsTable(sLogicalName))
            {
                return false;
            }

            InMemoryTable table = Db.GetTable(sLogicalName);
            if (!table.Contains(id))
            {
                return false;
            }

            return true;
        }

        //
        // Summary:
        //     Returns a strongly-typed entity record by Id and its class name
        //
        // Parameters:
        //   id:
        //
        // Type parameters:
        //   T:
        public T GetEntityById<T>(Guid id) where T : Entity
        {
            Type typeFromHandle = typeof(T);
            string sLogicalName = "";
            if (typeFromHandle.GetCustomAttributes(typeof(EntityLogicalNameAttribute), inherit: true).Length != 0)
            {
                sLogicalName = (typeFromHandle.GetCustomAttributes(typeof(EntityLogicalNameAttribute), inherit: true)[0] as EntityLogicalNameAttribute).LogicalName;
            }

            Entity entityById_Internal = GetEntityById_Internal(sLogicalName, id, typeFromHandle);
            return entityById_Internal.Clone(typeFromHandle, this) as T;
        }

        //
        // Parameters:
        //   er:
        protected EntityReference ResolveEntityReference(EntityReference er)
        {
            if (!Db.ContainsTable(er.LogicalName) || !ContainsEntity(er.LogicalName, er.Id))
            {
                if (er.Id == Guid.Empty && er.HasKeyAttributes())
                {
                    return ResolveEntityReferenceByAlternateKeys(er);
                }

                throw FakeOrganizationServiceFaultFactory.New($"{er.LogicalName} With Id = {er.Id:D} Does Not Exist");
            }

            return er;
        }

        //
        // Parameters:
        //   er:
        protected EntityReference ResolveEntityReferenceByAlternateKeys(EntityReference er)
        {
            Guid recordUniqueId = GetRecordUniqueId(er);
            return new EntityReference
            {
                LogicalName = er.LogicalName,
                Id = recordUniqueId
            };
        }

        //
        // Summary:
        //     Fakes the delete method. Very similar to the Retrieve one
        //
        // Parameters:
        //   er:
        public void DeleteEntity(EntityReference er)
        {
            if (!Db.ContainsTable(er.LogicalName))
            {
                if (ProxyTypesAssemblies.Count() == 0)
                {
                    throw new InvalidOperationException("The entity logical name " + er.LogicalName + " is not valid.");
                }

                if (FindReflectedType(er.LogicalName) == null)
                {
                    throw new InvalidOperationException("The entity logical name " + er.LogicalName + " is not valid.");
                }
            }

            if (Db.ContainsTable(er.LogicalName) && ContainsEntity(er.LogicalName, er.Id))
            {
                InMemoryTable table = Db.GetTable(er.LogicalName);
                table.Remove(er.Id);
                return;
            }

            throw FakeOrganizationServiceFaultFactory.New($"{er.LogicalName} with Id {er.Id} Does Not Exist");
        }

        //
        // Parameters:
        //   e:
        public void AddEntityDefaultAttributes(Entity e)
        {
            IIntegrityOptions property = GetProperty<IIntegrityOptions>();
            if (property.ValidateEntityReferences)
            {
                Entity e2 = new Entity("systemuser")
                {
                    Id = CallerProperties.CallerId.Id
                };
                AddEntityRecordInternal(e2);
            }

            bool isManyToManyRelationshipEntity = e.LogicalName != null && _relationships.ContainsKey(e.LogicalName);
            EntityInitializerService.Initialize(e, CallerProperties.CallerId.Id, this, isManyToManyRelationshipEntity);
        }

        //
        // Parameters:
        //   e:
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        protected internal void ValidateEntity(Entity e)
        {
            if (e == null)
            {
                throw new InvalidOperationException("The entity must not be null");
            }

            if (string.IsNullOrWhiteSpace(e.LogicalName))
            {
                throw new InvalidOperationException("The LogicalName property must not be empty");
            }

            if (e.Id == Guid.Empty)
            {
                throw new InvalidOperationException("The Id property must not be empty");
            }
        }

        //
        // Parameters:
        //   e:
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        public Guid CreateEntity(Entity e)
        {
            if (e == null)
            {
                throw new InvalidOperationException("The entity must not be null");
            }

            Entity entity = e.Clone(e.GetType());
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            string text = e.LogicalName + "id";
            if (!entity.Attributes.ContainsKey(text))
            {
                entity[text] = entity.Id;
            }

            ValidateEntity(entity);
            if (entity.Id != Guid.Empty && ContainsEntity(entity.LogicalName, entity.Id))
            {
                throw new InvalidOperationException($"There is already a record of entity {entity.LogicalName} with id {entity.Id}, can't create with this Id.");
            }

            if (entity.Attributes.ContainsKey("statecode"))
            {
                entity["statecode"] = new OptionSetValue(0);
            }

            AddEntityWithDefaults(entity);
            if (e.RelatedEntities.Count > 0)
            {
                foreach (KeyValuePair<Relationship, EntityCollection> relatedEntity in e.RelatedEntities)
                {
                    Relationship key = relatedEntity.Key;
                    EntityReferenceCollection entityReferenceCollection = new EntityReferenceCollection();
                    foreach (Entity entity2 in relatedEntity.Value.Entities)
                    {
                        Guid id = CreateEntity(entity2);
                        entityReferenceCollection.Add(new EntityReference(entity2.LogicalName, id));
                    }

                    AssociateRequest request = new AssociateRequest
                    {
                        Target = entity.ToEntityReference(),
                        Relationship = key,
                        RelatedEntities = entityReferenceCollection
                    };
                    _service.Execute(request);
                }
            }

            return entity.Id;
        }

        //
        // Summary:
        //     Adds an entity record to the in-memory database with some default values for
        //     out of the box attributes
        //
        // Parameters:
        //   e:
        //     Entity record to add
        //
        //   clone:
        //     True if it should clone the entity record before adding it
        public void AddEntityWithDefaults(Entity e, bool clone = false)
        {
            AddEntityDefaultAttributes(e);
            AddEntity(clone ? e.Clone(e.GetType()) : e);
        }

        internal void AddEntityRecordInternal(Entity e)
        {
            Db.AddOrReplaceEntityRecord(e);
        }

        //
        // Parameters:
        //   e:
        //
        // Exceptions:
        //   T:System.Exception:
        public void AddEntity(Entity e)
        {
            if (ProxyTypesAssemblies.Count() == 0 && e.GetType().IsSubclassOf(typeof(Entity)))
            {
                EnableProxyTypes(Assembly.GetAssembly(e.GetType()));
            }

            ValidateEntity(e);
            IIntegrityOptions property = GetProperty<IIntegrityOptions>();
            foreach (string item in e.Attributes.Keys.ToList())
            {
                object obj = e[item];
                if (obj is DateTime)
                {
                    e[item] = ConvertToUtc((DateTime)e[item]);
                }

                if (obj is EntityReference && property.ValidateEntityReferences)
                {
                    EntityReference er = (EntityReference)e[item];
                    e[item] = ResolveEntityReference(er);
                }
            }

            AddEntityRecordInternal(e);
            if (!AttributeMetadataNames.ContainsKey(e.LogicalName))
            {
                AttributeMetadataNames.Add(e.LogicalName, new Dictionary<string, string>());
            }

            if (ProxyTypesAssemblies.Count() > 0)
            {
                Type type = FindReflectedType(e.LogicalName);
                if (type != null)
                {
                    PropertyInfo[] properties = type.GetProperties();
                    PropertyInfo[] array = properties;
                    foreach (PropertyInfo propertyInfo in array)
                    {
                        if (!AttributeMetadataNames[e.LogicalName].ContainsKey(propertyInfo.Name))
                        {
                            AttributeMetadataNames[e.LogicalName].Add(propertyInfo.Name, propertyInfo.Name);
                        }
                    }

                    return;
                }

                throw new Exception($"Couldnt find reflected type for {e.LogicalName}");
            }

            foreach (string key in e.Attributes.Keys)
            {
                if (!AttributeMetadataNames[e.LogicalName].ContainsKey(key))
                {
                    AttributeMetadataNames[e.LogicalName].Add(key, key);
                }
            }
        }

        //
        // Parameters:
        //   attribute:
        protected internal DateTime ConvertToUtc(DateTime attribute)
        {
            return DateTime.SpecifyKind(attribute, DateTimeKind.Utc);
        }

        //
        // Parameters:
        //   license:
        [Obsolete("The default constructor is deprecated. Please use MiddlewareBuilder to build a custom XrmFakedContext")]
        public XrmFakedContext()
        {
            //LicenseContext = license;
            _fakeTracingService = new XrmFakedTracingService();
            _properties = new Dictionary<string, object>();
            _service = A.Fake<IOrganizationService>();
            _builder = MiddlewareBuilder.New(this).AddFakeMessageExecutors().UseMessages();
            //if (LicenseContext.HasValue)
            //{
            //    _builder = _builder.SetLicense(LicenseContext.Value);
            //}

            _builder.Build();
            Init();
        }

        internal XrmFakedContext(IMiddlewareBuilder middlewareBuilder)
        {
            _builder = middlewareBuilder;
            _fakeTracingService = new XrmFakedTracingService();
            _properties = new Dictionary<string, object>();
            _service = A.Fake<IOrganizationService>();
            Init();
        }

        private void Init()
        {
            CallerProperties = new CallerProperties();
            MaxRetrieveCount = 5000;
            AttributeMetadataNames = new Dictionary<string, Dictionary<string, string>>();
            Db = new InMemoryDb();
            _relationships = new Dictionary<string, XrmFakedRelationship>();
            EntityInitializerService = new DefaultEntityInitializerService();
            SetProperty((IAccessRightsRepository)new AccessRightsRepository());
            SetProperty((IOptionSetMetadataRepository)new OptionSetMetadataRepository());
            SetProperty((IStatusAttributeMetadataRepository)new StatusAttributeMetadataRepository());
            SystemTimeZone = TimeZoneInfo.Local;
            InitializationLevel = EntityInitializationLevel.Default;
            _proxyTypesAssemblies = new List<Assembly>();
            GetOrganizationService();
        }

        //
        // Summary:
        //     Checks if this XrmFakedContext has a property of the given type
        //
        // Type parameters:
        //   T:
        //     The property type
        public bool HasProperty<T>()
        {
            return _properties.ContainsKey(typeof(T).FullName);
        }

        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   T:System.TypeAccessException:
        public T GetProperty<T>()
        {
            if (!_properties.ContainsKey(typeof(T).FullName))
            {
                throw new TypeAccessException("Property of type '" + typeof(T).FullName + "' doesn't exists");
            }

            return (T)_properties[typeof(T).FullName];
        }

        //
        // Parameters:
        //   property:
        //
        // Type parameters:
        //   T:
        public void SetProperty<T>(T property)
        {
            if (!_properties.ContainsKey(typeof(T).FullName))
            {
                _properties.Add(typeof(T).FullName, property);
            }
            else
            {
                _properties[typeof(T).FullName] = property;
            }
        }

        //
        // Summary:
        //     Returns an interface to an organization service that will execute requests according
        //     to the middleware setup
        public IOrganizationService GetOrganizationService()
        {
            return GetFakedOrganizationService(this);
        }

        public IOrganizationServiceFactory GetOrganizationServiceFactory()
        {
            IOrganizationServiceFactory fakedServiceFactory = A.Fake<IOrganizationServiceFactory>();
            A.CallTo(() => fakedServiceFactory.CreateOrganizationService(A<Guid?>._)).ReturnsLazily((Guid? g) => GetOrganizationService());
            return fakedServiceFactory;
        }

        public IXrmFakedTracingService GetTracingService()
        {
            return _fakeTracingService;
        }

        //
        // Summary:
        //     Initializes the context with the provided entities
        //
        // Parameters:
        //   entities:
        public virtual void Initialize(IEnumerable<Entity> entities)
        {
            if (Initialised)
            {
                throw new Exception("Initialize should be called only once per unit test execution and XrmFakedContext instance.");
            }

            if (entities == null)
            {
                throw new InvalidOperationException("The entities parameter must be not null");
            }

            foreach (Entity entity in entities)
            {
                AddEntityWithDefaults(entity, clone: true);
            }

            Initialised = true;
        }

        //
        // Parameters:
        //   e:
        public void Initialize(Entity e)
        {
            Initialize(new List<Entity> { e });
        }

        //
        // Summary:
        //     Enables support for the early-cound types exposed in a specified assembly.
        //
        // Parameters:
        //   assembly:
        //     An assembly containing early-bound entity types.
        //
        // Remarks:
        //     See issue #334 on GitHub. This has quite similar idea as is on SDK method https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.client.organizationserviceproxy.enableproxytypes.
        public void EnableProxyTypes(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (_proxyTypesAssemblies.Contains(assembly))
            {
                throw new InvalidOperationException("Proxy types assembly " + assembly.GetName().Name + " is already enabled.");
            }

            _proxyTypesAssemblies.Add(assembly);
        }

        //
        // Parameters:
        //   executor:
        //
        // Type parameters:
        //   T:
        //[Obsolete("Please use MiddlewareBuilder's functionality to set custom message executors")]
        //public void AddFakeMessageExecutor<T>(IFakeMessageExecutor executor) where T : OrganizationRequest
        //{
        //    _builder.AddFakeMessageExecutor<T>(executor);
        //}

        ////
        //// Type parameters:
        ////   T:
        //[Obsolete("Please use MiddlewareBuilder's functionality to set custom message executors. If you want to remove one, simply remove it from the middleware setup.")]
        //public void RemoveFakeMessageExecutor<T>() where T : OrganizationRequest
        //{
        //    _builder.RemoveFakeMessageExecutor<T>();
        //}

        //
        // Parameters:
        //   message:
        //
        //   executor:
        //[Obsolete("Please use MiddlewareBuilder's functionality to set custom message executors")]
        //public void AddGenericFakeMessageExecutor(string message, IFakeMessageExecutor executor)
        //{
        //    _builder.AddGenericFakeMessageExecutor(message, executor);
        //}

        ////
        //// Parameters:
        ////   message:
        //[Obsolete("Please use MiddlewareBuilder's functionality to set custom message executors. If you want to remove one, simply remove it from the middleware setup.")]
        //public void RemoveGenericFakeMessageExecutor(string message)
        //{
        //    if (HasProperty<GenericMessageExecutors>())
        //    {
        //        GenericMessageExecutors property = GetProperty<GenericMessageExecutors>();
        //        if (property.ContainsKey(message))
        //        {
        //            property.Remove(message);
        //        }
        //    }
        //}

        //
        // Parameters:
        //   schemaname:
        //
        //   relationship:
        public void AddRelationship(string schemaname, XrmFakedRelationship relationship)
        {
            _relationships.Add(schemaname, relationship);
        }

        //
        // Parameters:
        //   schemaname:
        public void RemoveRelationship(string schemaname)
        {
            _relationships.Remove(schemaname);
        }

        //
        // Parameters:
        //   schemaName:
        public XrmFakedRelationship GetRelationship(string schemaName)
        {
            if (_relationships.ContainsKey(schemaName))
            {
                return _relationships[schemaName];
            }

            return null;
        }

        //
        // Parameters:
        //   sourceEntityName:
        //
        //   sourceAttributeName:
        //
        //   targetEntityName:
        //
        //   targetAttributeName:
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        public void AddAttributeMapping(string sourceEntityName, string sourceAttributeName, string targetEntityName, string targetAttributeName)
        {
            if (string.IsNullOrWhiteSpace(sourceEntityName))
            {
                throw new ArgumentNullException("sourceEntityName");
            }

            if (string.IsNullOrWhiteSpace(sourceAttributeName))
            {
                throw new ArgumentNullException("sourceAttributeName");
            }

            if (string.IsNullOrWhiteSpace(targetEntityName))
            {
                throw new ArgumentNullException("targetEntityName");
            }

            if (string.IsNullOrWhiteSpace(targetAttributeName))
            {
                throw new ArgumentNullException("targetAttributeName");
            }

            Entity entity = new Entity
            {
                LogicalName = "entitymap",
                Id = Guid.NewGuid(),
                ["targetentityname"] = targetEntityName,
                ["sourceentityname"] = sourceEntityName
            };
            Entity e = new Entity
            {
                LogicalName = "attributemap",
                Id = Guid.NewGuid(),
                ["entitymapid"] = new EntityReference("entitymap", entity.Id),
                ["targetattributename"] = targetAttributeName,
                ["sourceattributename"] = sourceAttributeName
            };
            AddEntityWithDefaults(entity);
            AddEntityWithDefaults(e);
        }

        //
        // Summary:
        //     Deprecated. Use GetOrganizationService instead
        [Obsolete("Use GetOrganizationService instead")]
        public IOrganizationService GetFakedOrganizationService()
        {
            return GetFakedOrganizationService(this);
        }

        //
        // Parameters:
        //   context:
        protected IOrganizationService GetFakedOrganizationService(XrmFakedContext context)
        {
            return context._service;
        }

        //
        // Parameters:
        //   entityMetadataList:
        //
        // Exceptions:
        //   T:System.Exception:
        public void InitializeMetadata(IEnumerable<EntityMetadata> entityMetadataList)
        {
            if (entityMetadataList == null)
            {
                throw new Exception("Entity metadata parameter can not be null");
            }

            foreach (EntityMetadata entityMetadata in entityMetadataList)
            {
                if (string.IsNullOrWhiteSpace(entityMetadata.LogicalName))
                {
                    throw new Exception("An entity metadata record must have a LogicalName property.");
                }

                if (Db.ContainsTableMetadata(entityMetadata.LogicalName))
                {
                    throw new Exception("An entity metadata record with the same logical name was previously added. ");
                }

                Db.AddOrUpdateMetadata(entityMetadata.LogicalName, entityMetadata);
            }
        }

        //
        // Parameters:
        //   entityMetadata:
        public void InitializeMetadata(EntityMetadata entityMetadata)
        {
            InitializeMetadata(new List<EntityMetadata> { entityMetadata });
        }

        //
        // Parameters:
        //   earlyBoundEntitiesAssembly:
        public void InitializeMetadata(Assembly earlyBoundEntitiesAssembly)
        {
            IEnumerable<EntityMetadata> enumerable = MetadataGenerator.FromEarlyBoundEntities(earlyBoundEntitiesAssembly);
            if (enumerable.Any())
            {
                InitializeMetadata(enumerable);
            }
        }

        public IQueryable<EntityMetadata> CreateMetadataQuery()
        {
            return Db.AllMetadata.AsQueryable();
        }

        //
        // Parameters:
        //   sLogicalName:
        public EntityMetadata GetEntityMetadataByName(string sLogicalName)
        {
            if (Db.ContainsTableMetadata(sLogicalName))
            {
                return Db.GetTableMetadata(sLogicalName);
            }

            return null;
        }

        //
        // Parameters:
        //   em:
        public void SetEntityMetadata(EntityMetadata em)
        {
            Db.AddOrUpdateMetadata(em.LogicalName, em);
        }

        //
        // Parameters:
        //   sEntityName:
        //
        //   sAttributeName:
        //
        //   attributeType:
        public AttributeMetadata GetAttributeMetadataFor(string sEntityName, string sAttributeName, Type attributeType)
        {
            if (Db.ContainsTableMetadata(sEntityName))
            {
                EntityMetadata entityMetadataByName = GetEntityMetadataByName(sEntityName);
                AttributeMetadata attributeMetadata = entityMetadataByName.Attributes.Where((AttributeMetadata a) => a.LogicalName.Equals(sAttributeName)).FirstOrDefault();
                if (attributeMetadata != null)
                {
                    return attributeMetadata;
                }
            }

            if (attributeType == typeof(string))
            {
                return new StringAttributeMetadata(sAttributeName);
            }

            return new StringAttributeMetadata(sAttributeName);
        }

        //
        // Parameters:
        //   logicalName:
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        public Type FindReflectedType(string logicalName)
        {
            IEnumerable<Type> enumerable = from a in ProxyTypesAssemblies
                                           select FindReflectedType(logicalName, a) into t
                                           where t != null
                                           select t;
            if (enumerable.Count() > 1)
            {
                string text = "Type " + logicalName + " is defined in multiple assemblies: ";
                foreach (Type item in enumerable)
                {
                    text = text + item.Assembly.GetName().Name + "; ";
                }

                int length = text.LastIndexOf("; ");
                text = text.Substring(0, length) + ".";
                throw new InvalidOperationException(text);
            }

            return enumerable.SingleOrDefault();
        }

        //
        // Summary:
        //     Finds reflected type for given entity from given assembly.
        //
        // Parameters:
        //   logicalName:
        //     Entity logical name which type is searched from given assembly.
        //
        //   assembly:
        //     Assembly where early-bound type is searched for given logicalName.
        //
        // Returns:
        //     Early-bound type of logicalName if it's found from assembly. Otherwise null is
        //     returned.
        private static Type FindReflectedType(string logicalName, Assembly assembly)
        {
            try
            {
                if (assembly == null)
                {
                    throw new ArgumentNullException("assembly");
                }

                return (from t in assembly.GetTypes()
                        where typeof(Entity).IsAssignableFrom(t)
                        where t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), inherit: true).Length != 0
                        where ((EntityLogicalNameAttribute)t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), inherit: true)[0]).LogicalName.Equals(logicalName.ToLower())
                        select t).FirstOrDefault();
            }
            catch (ReflectionTypeLoadException ex)
            {
                string text = "";
                Exception[] loaderExceptions = ex.LoaderExceptions;
                foreach (Exception ex2 in loaderExceptions)
                {
                    text = text + ex2.Message + " ";
                }

                throw new Exception("XrmFakedContext.FindReflectedType: " + text);
            }
        }

        //
        // Parameters:
        //   earlyBoundType:
        //
        //   sEntityName:
        //
        //   attributeName:
        //
        // Exceptions:
        //   T:System.Exception:
        public Type FindReflectedAttributeType(Type earlyBoundType, string sEntityName, string attributeName)
        {
            PropertyInfo propertyInfo = earlyBoundType.GetEarlyBoundTypeAttribute(attributeName);
            if (propertyInfo == null && attributeName.EndsWith("name"))
            {
                attributeName = attributeName.Substring(0, attributeName.Length - 4);
                propertyInfo = earlyBoundType.GetEarlyBoundTypeAttribute(attributeName);
                if (propertyInfo.PropertyType != typeof(EntityReference))
                {
                    propertyInfo = null;
                }
            }

            if (propertyInfo == null || propertyInfo.PropertyType.FullName == null)
            {
                Type type = this.FindAttributeTypeInInjectedMetadata(sEntityName, attributeName);
                if (type == null)
                {
                    throw new Exception($"XrmFakedContext.FindReflectedAttributeType: Attribute {attributeName} not found for type {earlyBoundType}");
                }

                return type;
            }

            if (propertyInfo.PropertyType.FullName.EndsWith("Enum") || propertyInfo.PropertyType.BaseType.FullName.EndsWith("Enum"))
            {
                return typeof(int);
            }

            if (!propertyInfo.PropertyType.FullName.StartsWith("System."))
            {
                try
                {
                    object obj = Activator.CreateInstance(propertyInfo.PropertyType);
                    if (obj is Entity)
                    {
                        return typeof(EntityReference);
                    }
                }
                catch
                {
                }
            }
            else if (propertyInfo.PropertyType.FullName.StartsWith("System.Nullable"))
            {
                return propertyInfo.PropertyType.GenericTypeArguments[0];
            }

            return propertyInfo.PropertyType;
        }

        //
        // Parameters:
        //   entityLogicalName:
        public IQueryable<Entity> CreateQuery(string entityLogicalName)
        {
            return CreateQuery<Entity>(entityLogicalName);
        }

        //
        // Type parameters:
        //   T:
        public IQueryable<T> CreateQuery<T>() where T : Entity
        {
            Type typeFromHandle = typeof(T);
            if (ProxyTypesAssemblies.Count() == 0)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(T));
                if (assembly != null)
                {
                    EnableProxyTypes(assembly);
                }
            }

            string entityLogicalName = "";
            if (typeFromHandle.GetCustomAttributes(typeof(EntityLogicalNameAttribute), inherit: true).Length != 0)
            {
                entityLogicalName = (typeFromHandle.GetCustomAttributes(typeof(EntityLogicalNameAttribute), inherit: true)[0] as EntityLogicalNameAttribute).LogicalName;
            }

            return CreateQuery<T>(entityLogicalName);
        }

        //
        // Parameters:
        //   entityLogicalName:
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   T:System.Exception:
        protected IQueryable<T> CreateQuery<T>(string entityLogicalName) where T : Entity
        {
            Type type = FindReflectedType(entityLogicalName);
            if ((type == null && !(typeof(T) == typeof(Entity))) || (typeof(T) == typeof(Entity) && string.IsNullOrWhiteSpace(entityLogicalName)))
            {
                throw new Exception("The type " + entityLogicalName + " was not found");
            }

            List<T> list = new List<T>();
            if (!Db.ContainsTable(entityLogicalName))
            {
                return list.AsQueryable();
            }

            InMemoryTable table = Db.GetTable(entityLogicalName);
            foreach (Entity row in table.Rows)
            {
                if (type != null)
                {
                    Entity entity = row.Clone(type);
                    list.Add((T)entity);
                }
                else
                {
                    list.Add((T)row.Clone());
                }
            }

            return list.AsQueryable();
        }

        //
        // Parameters:
        //   entityName:
        public IQueryable<Entity> CreateQueryFromEntityName(string entityName)
        {
            return Db.GetTable(entityName).Rows.AsQueryable();
        }
    }
}
