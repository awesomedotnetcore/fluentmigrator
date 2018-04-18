#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;

using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Builders.Insert;
using FluentMigrator.Builders.Rename;
using FluentMigrator.Builders.Schema;
using FluentMigrator.Infrastructure;

namespace FluentMigrator
{
    /// <summary>
    /// The base migration class
    /// </summary>
    public abstract class MigrationBase : IMigration
    {
        /// <summary>
        /// Gets or sets the migration context
        /// </summary>
        internal IMigrationContext _context;
        private readonly object _mutex = new object();

        /// <inheritdoc />
        public object ApplicationContext { get; protected set; }

        /// <inheritdoc />
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public abstract void Up();

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public abstract void Down();

        /// <inheritdoc />
        public virtual void GetUpExpressions(IMigrationContext context)
        {
            lock (_mutex)
            {
                _context = context;
                ApplicationContext = context.ApplicationContext;
                ConnectionString = context.Connection;
                Up();
                _context = null;
            }
        }

        /// <inheritdoc />
        public virtual void GetDownExpressions(IMigrationContext context)
        {
            lock (_mutex)
            {
                _context = context;
                ApplicationContext = context.ApplicationContext;
                ConnectionString = context.Connection;
                Down();
                _context = null;
            }
        }

        /// <summary>
        /// Gets the starting point for alterations
        /// </summary>
        public IAlterExpressionRoot Alter
        {
            get { return new AlterExpressionRoot(_context); }
        }

        /// <summary>
        /// Gets the starting point for creating database objects
        /// </summary>
        public ICreateExpressionRoot Create
        {
            get { return new CreateExpressionRoot(_context); }
        }

        /// <summary>
        /// Gets the starting point for renaming database objects
        /// </summary>
        public IRenameExpressionRoot Rename
        {
            get { return new RenameExpressionRoot(_context); }
        }

        /// <summary>
        /// Gets the starting point for data insertion
        /// </summary>
        public IInsertExpressionRoot Insert
        {
            get { return new InsertExpressionRoot(_context); }
        }

        /// <summary>
        /// Gets the starting point for schema-rooted expressions
        /// </summary>
        public ISchemaExpressionRoot Schema
        {
            get { return new SchemaExpressionRoot(_context); }
        }

        /// <summary>
        /// Gets the starting point for database specific expressions
        /// </summary>
        /// <param name="databaseType">The supported database types</param>
        /// <returns>The database specific expression</returns>
        public IIfDatabaseExpressionRoot IfDatabase(params string[] databaseType)
        {
            return new IfDatabaseExpressionRoot(_context, databaseType);
        }

        /// <summary>
        /// Gets the starting point for database specific expressions
        /// </summary>
        /// <param name="databaseTypeFunc">The lambda that tests if the expression can be applied to the current database</param>
        /// <returns>The database specific expression</returns>
        public IIfDatabaseExpressionRoot IfDatabase(Predicate<string> databaseTypeFunc)
        {
            return new IfDatabaseExpressionRoot(_context, databaseTypeFunc);
        }
    }
}
