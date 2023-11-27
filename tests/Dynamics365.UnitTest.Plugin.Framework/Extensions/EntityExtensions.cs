using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Extensions
{
    public static class EntityExtensions
    {
        //
        // Summary:
        //     Extension method to add an attribute and return the entity itself
        //
        // Parameters:
        //   e:
        //
        //   key:
        //
        //   value:
        public static Entity AddAttribute(this Entity e, string key, object value)
        {
            e.Attributes.Add(key, value);
            return e;
        }

        //
        // Summary:
        //     Projects the attributes of entity e so that only the attributes specified in
        //     the columnSet are returned
        //
        // Parameters:
        //   e:
        //
        //   columnSet:
        //
        //   context:
        internal static Entity ProjectAttributes(this Entity e, ColumnSet columnSet, IXrmFakedContext context)
        {
            return e.ProjectAttributes(new QueryExpression
            {
                ColumnSet = columnSet
            }, context);
        }

        //
        // Parameters:
        //   e:
        //
        //   context:
        public static void ApplyDateBehaviour(this Entity e, XrmFakedContext context)
        {
            EntityMetadata entityMetadataByName = context.GetEntityMetadataByName(e.LogicalName);
            if (e.LogicalName == null || entityMetadataByName == null || entityMetadataByName?.Attributes == null)
            {
                return;
            }

            List<DateTimeAttributeMetadata> list = (from a in entityMetadataByName?.Attributes
                                                    where a is DateTimeAttributeMetadata
                                                    select a as DateTimeAttributeMetadata).ToList();
            foreach (DateTimeAttributeMetadata item in list)
            {
                if (!e.Attributes.ContainsKey(item.LogicalName) || !(item.DateTimeBehavior == DateTimeBehavior.DateOnly))
                {
                    continue;
                }

                DateTime dateTime = (DateTime)e[item.LogicalName];
                e[item.LogicalName] = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
                break;
            }
        }

        //
        // Parameters:
        //   e:
        //
        //   projected:
        //
        //   le:
        //
        //   context:
        public static void ProjectAttributes(Entity e, Entity projected, LinkEntity le, IXrmFakedContext context)
        {
            string text = (string.IsNullOrWhiteSpace(le.EntityAlias) ? le.LinkToEntityName : le.EntityAlias);
            if (le.Columns.AllColumns)
            {
                foreach (string key in e.Attributes.Keys)
                {
                    if (key.StartsWith(text + "."))
                    {
                        projected[key] = e[key];
                    }
                }

                foreach (string key2 in e.FormattedValues.Keys)
                {
                    if (key2.StartsWith(text + "."))
                    {
                        projected.FormattedValues[key2] = e.FormattedValues[key2];
                    }
                }
            }
            else
            {
                foreach (string column in le.Columns.Columns)
                {
                    string text2 = text + "." + column;
                    if (e.Attributes.ContainsKey(text2))
                    {
                        projected[text2] = e[text2];
                    }

                    if (e.FormattedValues.ContainsKey(text2))
                    {
                        projected.FormattedValues[text2] = e.FormattedValues[text2];
                    }
                }
            }

            foreach (LinkEntity linkEntity in le.LinkEntities)
            {
                ProjectAttributes(e, projected, linkEntity, context);
            }
        }

        //
        // Parameters:
        //   e:
        //
        //   qe:
        //
        //   context:
        internal static Entity ProjectAttributes(this Entity e, QueryExpression qe, IXrmFakedContext context)
        {
            if (qe.ColumnSet == null || qe.ColumnSet.AllColumns)
            {
                return RemoveNullAttributes(e);
            }

            Entity entity = null;
            if (context.ProxyTypesAssemblies.Count() > 0)
            {
                Type type = context.FindReflectedType(e.LogicalName);
                if (type != null)
                {
                    object obj = Activator.CreateInstance(type);
                    entity = (Entity)obj;
                    entity.Id = e.Id;
                }
                else
                {
                    entity = new Entity(e.LogicalName)
                    {
                        Id = e.Id
                    };
                }
            }
            else
            {
                entity = new Entity(e.LogicalName)
                {
                    Id = e.Id
                };
            }

            foreach (string column in qe.ColumnSet.Columns)
            {
                if (!context.AttributeExistsInMetadata(e.LogicalName, column))
                {
                    throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {column} does not exist on this entity.");
                }

                if (e.Attributes.ContainsKey(column) && e.Attributes[column] != null)
                {
                    entity[column] = CloneAttribute(e[column], context);
                    string value = "";
                    if (e.FormattedValues.TryGetValue(column, out value))
                    {
                        entity.FormattedValues[column] = value;
                    }
                }
            }

            foreach (LinkEntity linkEntity in qe.LinkEntities)
            {
                ProjectAttributes(RemoveNullAttributes(e), entity, linkEntity, context);
            }

            return RemoveNullAttributes(entity);
        }

        //
        // Parameters:
        //   entity:
        public static Entity RemoveNullAttributes(Entity entity)
        {
            IList<string> list = (from attribute in entity.Attributes
                                  where attribute.Value == null || (attribute.Value is AliasedValue && (attribute.Value as AliasedValue).Value == null)
                                  select attribute.Key).ToList();
            foreach (string item in list)
            {
                entity.Attributes.Remove(item);
            }

            return entity;
        }

        //
        // Parameters:
        //   attributeValue:
        //
        //   context:
        //
        // Exceptions:
        //   T:System.Exception:
        internal static object CloneAttribute(object attributeValue, IXrmFakedContext context = null)
        {
            if (attributeValue == null)
            {
                return null;
            }

            Type type = attributeValue.GetType();
            if (type == typeof(string))
            {
                return new string((attributeValue as string).ToCharArray());
            }

            if (type == typeof(EntityReference))
            {
                EntityReference entityReference = attributeValue as EntityReference;
                EntityReference entityReference2 = new EntityReference(entityReference.LogicalName, entityReference.Id);
                EntityMetadata entityMetadata = null;
                bool flag = false;
                if (context != null)
                {
                    entityMetadata = context.GetEntityMetadataByName(entityReference.LogicalName);
                    flag = context.ContainsEntity(entityReference.LogicalName, entityReference.Id);
                }

                if (context != null && !string.IsNullOrEmpty(entityReference.LogicalName) && entityMetadata != null && !string.IsNullOrEmpty(entityMetadata.PrimaryNameAttribute) && flag)
                {
                    Entity entityById_Internal = (context as XrmFakedContext).GetEntityById_Internal(entityReference.LogicalName, entityReference.Id);
                    entityReference2.Name = entityById_Internal.GetAttributeValue<string>(entityMetadata.PrimaryNameAttribute);
                }
                else
                {
                    entityReference2.Name = CloneAttribute(entityReference.Name) as string;
                }

                if (entityReference.KeyAttributes != null)
                {
                    entityReference2.KeyAttributes = new KeyAttributeCollection();
                    entityReference2.KeyAttributes.AddRange(entityReference.KeyAttributes.Select((KeyValuePair<string, object> kvp) => new KeyValuePair<string, object>(CloneAttribute(kvp.Key) as string, kvp.Value)).ToArray());
                }

                return entityReference2;
            }

            if (type == typeof(BooleanManagedProperty))
            {
                BooleanManagedProperty booleanManagedProperty = attributeValue as BooleanManagedProperty;
                return new BooleanManagedProperty(booleanManagedProperty.Value);
            }

            if (type == typeof(OptionSetValue))
            {
                OptionSetValue optionSetValue = attributeValue as OptionSetValue;
                return new OptionSetValue(optionSetValue.Value);
            }

            if (type == typeof(AliasedValue))
            {
                AliasedValue aliasedValue = attributeValue as AliasedValue;
                return new AliasedValue(aliasedValue.EntityLogicalName, aliasedValue.AttributeLogicalName, CloneAttribute(aliasedValue.Value));
            }

            if (type == typeof(Money))
            {
                Money money = attributeValue as Money;
                return new Money(money.Value);
            }

            if (attributeValue.GetType() == typeof(EntityCollection))
            {
                EntityCollection entityCollection = attributeValue as EntityCollection;
                return new EntityCollection(entityCollection.Entities.Select((Entity e) => e.Clone(e.GetType())).ToList());
            }

            if (attributeValue is IEnumerable<Entity>)
            {
                IEnumerable<Entity> source = attributeValue as IEnumerable<Entity>;
                return source.Select((Entity e) => e.Clone(e.GetType())).ToArray();
            }

            if (type == typeof(byte[]))
            {
                byte[] array = attributeValue as byte[];
                byte[] array2 = new byte[array.Length];
                array.CopyTo(array2, 0);
                return array2;
            }

            if (attributeValue is OptionSetValueCollection)
            {
                OptionSetValueCollection optionSetValueCollection = attributeValue as OptionSetValueCollection;
                return new OptionSetValueCollection(optionSetValueCollection.ToArray());
            }

            if (type == typeof(int) || type == typeof(long))
            {
                return attributeValue;
            }

            if (type == typeof(decimal))
            {
                return attributeValue;
            }

            if (type == typeof(double))
            {
                return attributeValue;
            }

            if (type == typeof(float))
            {
                return attributeValue;
            }

            if (type == typeof(byte))
            {
                return attributeValue;
            }

            if (type == typeof(bool))
            {
                return attributeValue;
            }

            if (type == typeof(Guid))
            {
                return attributeValue;
            }

            if (type == typeof(DateTime))
            {
                return attributeValue;
            }

            if (attributeValue is Enum)
            {
                return attributeValue;
            }

            throw new Exception($"Attribute type not supported when trying to clone attribute '{type.ToString()}'");
        }

        //
        // Parameters:
        //   e:
        //
        //   context:
        public static Entity Clone(this Entity e, IXrmFakedContext context = null)
        {
            Entity entity = new Entity(e.LogicalName);
            entity.Id = e.Id;
            entity.LogicalName = e.LogicalName;
            if (e.FormattedValues != null)
            {
                FormattedValueCollection formattedValueCollection = new FormattedValueCollection();
                foreach (string key in e.FormattedValues.Keys)
                {
                    formattedValueCollection.Add(key, e.FormattedValues[key]);
                }

                entity.Inject("FormattedValues", formattedValueCollection);
            }

            foreach (string key2 in e.Attributes.Keys)
            {
                entity[key2] = ((e[key2] != null) ? CloneAttribute(e[key2], context) : null);
            }

            foreach (string key3 in e.KeyAttributes.Keys)
            {
                entity.KeyAttributes[key3] = ((e.KeyAttributes[key3] != null) ? CloneAttribute(e.KeyAttributes[key3]) : null);
            }

            return entity;
        }

        //
        // Parameters:
        //   e:
        //
        // Type parameters:
        //   T:
        public static T Clone<T>(this Entity e) where T : Entity
        {
            return (T)e.Clone(typeof(T));
        }

        //
        // Parameters:
        //   e:
        //
        //   t:
        //
        //   context:
        public static Entity Clone(this Entity e, Type t, IXrmFakedContext context = null)
        {
            if (t == null)
            {
                return e.Clone(context);
            }

            Entity entity = Activator.CreateInstance(t) as Entity;
            entity.Id = e.Id;
            entity.LogicalName = e.LogicalName;
            if (e.FormattedValues != null)
            {
                FormattedValueCollection formattedValueCollection = new FormattedValueCollection();
                foreach (string key in e.FormattedValues.Keys)
                {
                    formattedValueCollection.Add(key, e.FormattedValues[key]);
                }

                entity.Inject("FormattedValues", formattedValueCollection);
            }

            foreach (string key2 in e.Attributes.Keys)
            {
                entity[key2] = ((e[key2] != null) ? CloneAttribute(e[key2], context) : null);
            }

            foreach (string key3 in e.KeyAttributes.Keys)
            {
                entity.KeyAttributes[key3] = ((e.KeyAttributes[key3] != null) ? CloneAttribute(e.KeyAttributes[key3]) : null);
            }

            return entity;
        }

        //
        // Summary:
        //     Extension method to join the attributes of entity e and otherEntity
        //
        // Parameters:
        //   e:
        //
        //   otherEntity:
        //
        //   columnSet:
        //
        //   alias:
        //
        //   context:
        internal static Entity JoinAttributes(this Entity e, Entity otherEntity, ColumnSet columnSet, string alias, IXrmFakedContext context)
        {
            if (otherEntity == null)
            {
                return e;
            }

            otherEntity = otherEntity.Clone();
            if (columnSet.AllColumns)
            {
                foreach (string key in otherEntity.Attributes.Keys)
                {
                    e[alias + "." + key] = new AliasedValue(otherEntity.LogicalName, key, otherEntity[key]);
                }

                foreach (string key2 in otherEntity.FormattedValues.Keys)
                {
                    e.FormattedValues[alias + "." + key2] = otherEntity.FormattedValues[key2];
                }
            }
            else
            {
                foreach (string column in columnSet.Columns)
                {
                    if (!context.AttributeExistsInMetadata(otherEntity.LogicalName, column))
                    {
                        throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {column} does not exist on this entity.");
                    }

                    if (otherEntity.Attributes.ContainsKey(column))
                    {
                        e[alias + "." + column] = new AliasedValue(otherEntity.LogicalName, column, otherEntity[column]);
                    }
                    else
                    {
                        e[alias + "." + column] = new AliasedValue(otherEntity.LogicalName, column, null);
                    }

                    if (otherEntity.FormattedValues.ContainsKey(column))
                    {
                        e.FormattedValues[alias + "." + column] = otherEntity.FormattedValues[column];
                    }
                }
            }

            return e;
        }

        //
        // Parameters:
        //   e:
        //
        //   otherEntities:
        //
        //   columnSet:
        //
        //   alias:
        //
        //   context:
        internal static Entity JoinAttributes(this Entity e, IEnumerable<Entity> otherEntities, ColumnSet columnSet, string alias, IXrmFakedContext context)
        {
            foreach (Entity otherEntity in otherEntities)
            {
                Entity entity = otherEntity.Clone();
                if (columnSet.AllColumns)
                {
                    foreach (string key in entity.Attributes.Keys)
                    {
                        e[alias + "." + key] = new AliasedValue(otherEntity.LogicalName, key, entity[key]);
                    }

                    foreach (string key2 in otherEntity.FormattedValues.Keys)
                    {
                        e.FormattedValues[alias + "." + key2] = otherEntity.FormattedValues[key2];
                    }

                    continue;
                }

                foreach (string column in columnSet.Columns)
                {
                    if (!context.AttributeExistsInMetadata(otherEntity.LogicalName, column))
                    {
                        throw FakeOrganizationServiceFaultFactory.New(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {column} does not exist on this entity.");
                    }

                    if (entity.Attributes.ContainsKey(column))
                    {
                        e[alias + "." + column] = new AliasedValue(otherEntity.LogicalName, column, entity[column]);
                    }
                    else
                    {
                        e[alias + "." + column] = new AliasedValue(otherEntity.LogicalName, column, null);
                    }

                    if (otherEntity.FormattedValues.ContainsKey(column))
                    {
                        e.FormattedValues[alias + "." + column] = otherEntity.FormattedValues[column];
                    }
                }
            }

            return e;
        }

        //
        // Summary:
        //     Returns the key for the attribute name selected (could an entity reference or
        //     a primary key or a guid)
        //
        // Parameters:
        //   e:
        //
        //   sAttributeName:
        //
        //   context:
        internal static object KeySelector(this Entity e, string sAttributeName, IXrmFakedContext context)
        {
            if (sAttributeName.Contains("."))
            {
                string[] array = sAttributeName.Split('.');
                sAttributeName = $"{array[0]}.{array[1].ToLower()}";
            }
            else
            {
                sAttributeName = sAttributeName.ToLower();
            }

            if (!e.Attributes.ContainsKey(sAttributeName))
            {
                if (sAttributeName.Contains("id") && e.LogicalName.ToLower().Equals(sAttributeName.Substring(0, sAttributeName.Length - 2)))
                {
                    return e.Id;
                }

                return Guid.Empty;
            }

            object obj = null;
            AliasedValue aliasedValue;
            obj = (((aliasedValue = e[sAttributeName] as AliasedValue) == null) ? e[sAttributeName] : aliasedValue.Value);
            EntityReference entityReference = obj as EntityReference;
            if (entityReference != null)
            {
                return entityReference.Id;
            }

            OptionSetValue optionSetValue = obj as OptionSetValue;
            if (optionSetValue != null)
            {
                return optionSetValue.Value;
            }

            Money money = obj as Money;
            if (money != null)
            {
                return money.Value;
            }

            return obj;
        }

        //
        // Summary:
        //     Extension method to "hack" internal set properties on sealed classes via reflection
        //
        // Parameters:
        //   e:
        //
        //   property:
        //
        //   value:
        public static void Inject(this Entity e, string property, object value)
        {
            e.GetType().GetProperty(property).SetValue(e, value, null);
        }

        //
        // Parameters:
        //   e:
        //
        //   property:
        //
        //   value:
        public static void SetValueIfEmpty(this Entity e, string property, object value)
        {
            bool flag = e.Attributes.ContainsKey(property);
            if (!flag || (flag && e[property] == null))
            {
                e[property] = value;
            }
        }

        //
        // Summary:
        //     ToEntityReference implementation that converts an entity into an entity reference
        //     with key attribute info as well
        //
        // Parameters:
        //   e:
        //     Entity to convert to an Entity Reference
        public static EntityReference ToEntityReferenceWithKeyAttributes(this Entity e)
        {
            EntityReference entityReference = e.ToEntityReference();
            entityReference.KeyAttributes = e.KeyAttributes;
            return entityReference;
        }
    }
}
