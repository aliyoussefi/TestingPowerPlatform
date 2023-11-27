using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Database
{
    internal class InMemoryDb
    {
        //
        // Summary:
        //     A collection of tables indexed by its logical name
        protected internal Dictionary<string, InMemoryTable> _tables;

        protected internal IEnumerable<EntityMetadata> AllMetadata => (from t in _tables
                                                                       where t.Value._metadata._entityMetadata != null
                                                                       select t.Value._metadata._entityMetadata.Copy()).AsEnumerable();

        //
        // Summary:
        //     Default InMemoryDb constructor with an empty list of tables
        public InMemoryDb()
        {
            _tables = new Dictionary<string, InMemoryTable>();
        }

        //
        // Summary:
        //     Returns true if the InMemoryDb contains a table object with the specified name
        //
        // Parameters:
        //   logicalName:
        protected internal bool ContainsTable(string logicalName)
        {
            return _tables.ContainsKey(logicalName);
        }

        //
        // Summary:
        //     Returns true if the InMemoryDb contains a table object with a non-empty entity
        //     metadata
        //
        // Parameters:
        //   logicalName:
        protected internal bool ContainsTableMetadata(string logicalName)
        {
            return _tables.ContainsKey(logicalName) && _tables[logicalName]._metadata._entityMetadata != null;
        }

        //
        // Summary:
        //     Returns the table with the specified logical name
        //
        // Parameters:
        //   logicalName:
        protected internal InMemoryTable GetTable(string logicalName)
        {
            return _tables[logicalName];
        }

        //
        // Summary:
        //     Returns the EntityMetadata for the specified logical name
        //
        // Parameters:
        //   logicalName:
        protected internal EntityMetadata GetTableMetadata(string logicalName)
        {
            return _tables[logicalName]._metadata._entityMetadata?.Copy();
        }

        //
        // Summary:
        //     Adds and returns the table that was added. If a table with the same logicalName
        //     was added, this will raise an exception
        //
        // Parameters:
        //   logicalName:
        //
        //   table:
        protected internal void AddTable(string logicalName, out InMemoryTable table)
        {
            if (_tables.ContainsKey(logicalName))
            {
                throw new TableAlreadyExistsException(logicalName);
            }

            table = new InMemoryTable(logicalName);
            _tables.Add(logicalName, table);
        }

        //
        // Summary:
        //     Adds the specified entity metadata to this InMemory database
        //
        // Parameters:
        //   logicalName:
        //
        //   entityMetadata:
        protected internal void AddOrUpdateMetadata(string logicalName, EntityMetadata entityMetadata)
        {
            InMemoryTable inMemoryTable = null;
            if (!_tables.ContainsKey(logicalName))
            {
                inMemoryTable = new InMemoryTable(logicalName, entityMetadata);
                _tables.Add(logicalName, inMemoryTable);
            }
            else
            {
                _tables[logicalName].SetMetadata(entityMetadata);
            }
        }

        protected internal void AddEntityRecord(Entity e)
        {
            if (!ContainsTable(e.LogicalName))
            {
                AddTable(e.LogicalName, out var _);
            }
        }

        protected internal void AddOrReplaceEntityRecord(Entity e)
        {
            InMemoryTable table = null;
            if (!ContainsTable(e.LogicalName))
            {
                AddTable(e.LogicalName, out table);
            }

            table = _tables[e.LogicalName];
            if (table.Contains(e))
            {
                table.Replace(e);
            }
            else
            {
                table.Add(e);
            }
        }
    }
}
