using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Interfaces
{
    public class XrmFakedRelationship
    {
        //
        // Summary:
        //     Relationship Type
        public enum FakeRelationshipType
        {
            //
            // Summary:
            //     Many to Many
            ManyToMany,
            //
            // Summary:
            //     One to Many
            OneToMany
        }

        private string entity1Attribute = string.Empty;

        private string entity2Attribute = string.Empty;

        //
        // Summary:
        //     Schema name of the many to many intersect entity
        public string IntersectEntity { get; set; }

        //
        // Summary:
        //     Entity name and attribute of the first entity participating in the relationship
        public string Entity1Attribute {
            get {
                if (entity1Attribute == entity2Attribute && Entity1LogicalName == Entity2LogicalName)
                {
                    return entity1Attribute + "one";
                }

                return entity1Attribute;
            }
            set {
                entity1Attribute = value;
            }
        }

        //
        // Summary:
        //     Entity1 Logical Name
        public string Entity1LogicalName { get; set; }

        //
        // Summary:
        //     Entity2 Logical Name
        public string Entity2LogicalName { get; set; }

        //
        // Summary:
        //     Entity name and attribute of the second entity participating in the relationship
        public string Entity2Attribute {
            get {
                if (entity1Attribute == entity2Attribute && Entity1LogicalName == Entity2LogicalName)
                {
                    return entity2Attribute + "two";
                }

                return entity2Attribute;
            }
            set {
                entity2Attribute = value;
            }
        }

        //
        // Summary:
        //     Relationship Type
        public FakeRelationshipType RelationshipType { get; set; }

        //
        // Summary:
        //     Fake Relationship
        public XrmFakedRelationship()
        {
            RelationshipType = FakeRelationshipType.ManyToMany;
        }

        //
        // Summary:
        //     Initializes a N:N relationship type
        //
        // Parameters:
        //   entityName:
        //
        //   entity1Attribute:
        //
        //   entity2Attribute:
        //
        //   entity1LogicalName:
        //
        //   entity2LogicalName:
        public XrmFakedRelationship(string entityName, string entity1Attribute, string entity2Attribute, string entity1LogicalName, string entity2LogicalName)
        {
            IntersectEntity = entityName;
            Entity1Attribute = entity1Attribute;
            Entity2Attribute = entity2Attribute;
            Entity1LogicalName = entity1LogicalName;
            Entity2LogicalName = entity2LogicalName;
            RelationshipType = FakeRelationshipType.ManyToMany;
        }

        //
        // Summary:
        //     Initializes a 1:N relationship type
        //
        // Parameters:
        //   entity1Attribute:
        //
        //   entity2Attribute:
        //
        //   entity1LogicalName:
        //
        //   entity2LogicalName:
        public XrmFakedRelationship(string entity1Attribute, string entity2Attribute, string entity1LogicalName, string entity2LogicalName)
        {
            Entity1Attribute = entity1Attribute;
            Entity2Attribute = entity2Attribute;
            Entity1LogicalName = entity1LogicalName;
            Entity2LogicalName = entity2LogicalName;
            RelationshipType = FakeRelationshipType.OneToMany;
        }
    }
}
