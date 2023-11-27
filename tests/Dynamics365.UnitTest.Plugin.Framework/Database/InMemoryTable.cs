using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Database
{
    internal class InMemoryTable
    {
        //
        // Summary:
        //     The entity logical name for this table
        protected internal string _logicalName;

        //
        // Summary:
        //     Collection of entity records for this table
        protected internal Dictionary<Guid, Entity> _rows;

        //
        // Summary:
        //     The metadata definition for this table
        protected internal InMemoryTableMetadata _metadata;

        //
        // Summary:
        //     Returns an IEnumerable of all rows in the current table
        protected internal IEnumerable<Entity> Rows => _rows.Values;

        //
        // Summary:
        //     Default constructor with no metadata and an empty records table
        public InMemoryTable(string logicalName)
        {
            _logicalName = logicalName;
            _rows = new Dictionary<Guid, Entity>();
            _metadata = new InMemoryTableMetadata();
        }

        //
        // Summary:
        //     Creates a new table with specific entity metadata
        //
        // Parameters:
        //   logicalName:
        //
        //   entityMetadata:
        public InMemoryTable(string logicalName, EntityMetadata entityMetadata)
        {
            _logicalName = logicalName;
            _rows = new Dictionary<Guid, Entity>();
            _metadata = new InMemoryTableMetadata();
            SetMetadata(entityMetadata);
        }

        //
        // Summary:
        //     Return true if the current table already contains this record
        //
        // Parameters:
        //   e:
        //     The entity record to check
        protected internal bool Contains(Entity e)
        {
            return _rows.ContainsKey(e.Id);
        }

        //
        // Summary:
        //     Returns true if the current table contains a record with the specified id
        //
        // Parameters:
        //   key:
        //     The primary key of the entity record
        protected internal bool Contains(Guid key)
        {
            return _rows.ContainsKey(key);
        }

        //
        // Summary:
        //     Adds the entity record to the current table
        //
        // Parameters:
        //   e:
        //     The entity record to add
        protected internal void Add(Entity e)
        {
            _rows.Add(e.Id, e);
        }

        //
        // Summary:
        //     Replaces the current entity record with the given id
        //
        // Parameters:
        //   e:
        protected internal void Replace(Entity e)
        {
            _rows[e.Id] = e;
        }

        //
        // Summary:
        //     Remove the entity record by primary key
        //
        // Parameters:
        //   key:
        //     The primary key
        protected internal void Remove(Guid key)
        {
            _rows.Remove(key);
        }

        //
        // Summary:
        //     Returns a record by its primary key
        //
        // Parameters:
        //   key:
        //     The primary key
        protected internal Entity GetById(Guid key)
        {
            return _rows[key];
        }

        //
        // Summary:
        //     Sets the current metadata for this table
        //
        // Parameters:
        //   entityMetadata:
        protected internal void SetMetadata(EntityMetadata entityMetadata)
        {
            _metadata._entityMetadata = entityMetadata.Copy();
        }
    }
}
