﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class SqlCeScaffoldingModelFactory : RelationalScaffoldingModelFactory
    {
        public SqlCeScaffoldingModelFactory(
            [NotNull] IDiagnosticsLogger<LoggerCategory.Scaffolding> loggerFactory,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] IDatabaseModelFactory databaseModelFactory,
            [NotNull] CandidateNamingService candidateNamingService,
            [NotNull] IPluralizer pluralizer)
            : base(loggerFactory, typeMapper, databaseModelFactory, candidateNamingService, pluralizer)
        {
        }

        public override IModel Create(string connectionString, TableSelectionSet tableSelectionSet)
        {
            if ((tableSelectionSet != null)
                && tableSelectionSet.Schemas.Any())
            {
                //TODO ErikEJ fix logging (see sqlite provider)
                //Logger.LogWarning("You have specified some schema selections. The SQL Server Compact provider does not support these and they will be ignored. Note: it does support table selections.");

                tableSelectionSet.Schemas.ToList().ForEach(s => s.IsMatched = true);
            }

            var model = base.Create(connectionString, tableSelectionSet);
            model.Scaffolding().UseProviderMethodName = nameof(SqlCeDbContextOptionsExtensions.UseSqlCe);
            return model;
        }

        protected override PropertyBuilder VisitColumn(EntityTypeBuilder builder, ColumnModel column)
        {
            var propertyBuilder = base.VisitColumn(builder, column);

            if (propertyBuilder == null)
            {
                return null;
            }

            VisitTypeMapping(propertyBuilder, column);

            VisitDefaultValue(column, propertyBuilder);

            return propertyBuilder;
        }

        protected override KeyBuilder VisitPrimaryKey(EntityTypeBuilder builder, TableModel table)
        {
            var keyBuilder = base.VisitPrimaryKey(builder, table);

            if (keyBuilder == null)
            {
                return null;
            }

            // If this property is the single integer primary key on the EntityType then
            // KeyConvention assumes ValueGeneratedOnAdd(). If the underlying column does
            // not have Identity set then we need to set to ValueGeneratedNever() to
            // override this behavior.

            var pkColumns = table.Columns.Where(c => c.PrimaryKeyOrdinal.HasValue).ToList();
            if ((pkColumns.Count != 1)
                || pkColumns[0].SqlCe().IsIdentity
                || (pkColumns[0].ValueGenerated != null)
                || (pkColumns[0].DefaultValue != null))
            {
                return keyBuilder;
            }

            var property = builder.Metadata.FindProperty(GetPropertyName(pkColumns[0]));
            var propertyType = property?.ClrType?.UnwrapNullableType();

            if ((propertyType?.IsIntegerForIdentity() == true)
                || (propertyType == typeof(Guid)))
            {
                property.ValueGenerated = ValueGenerated.Never;
            }

            return keyBuilder;
        }

        private void VisitTypeMapping(PropertyBuilder propertyBuilder, ColumnModel column)
        {
            var sqlCeColumn = column;
            if (sqlCeColumn == null)
            {
                return;
            }

            if (sqlCeColumn.SqlCe().IsIdentity)
            {
                if (typeof(byte) == propertyBuilder.Metadata.ClrType)
                {
                    //TODO ErikEJ fix logging (see sqlite provider)
                    //Logger.LogWarning($"For column {column.DisplayName}. This column is set up as an Identity column, but the SQL Server data type is {column.DataType}. This will be mapped to CLR type byte which does not allow the SqlServerValueGenerationStrategy.IdentityColumn setting. Generating a matching Property but ignoring the Identity setting.");
                }
                else
                {
                    propertyBuilder
                        .ValueGeneratedOnAdd();
                }
            }

            // undo quirk in reverse type mapping to litters code with unnecessary nvarchar annotations
            if ((typeof(string) == propertyBuilder.Metadata.ClrType)
                && (propertyBuilder.Metadata.Relational().ColumnType == "nvarchar"))
            {
                propertyBuilder.Metadata.Relational().ColumnType = null;
            }
        }

        private void VisitDefaultValue(ColumnModel column, PropertyBuilder propertyBuilder)
        {
            if (column.DefaultValue != null)
            {
                ((Property)propertyBuilder.Metadata).SetValueGenerated(null, ConfigurationSource.Explicit);
                propertyBuilder.Metadata.Relational().DefaultValueSql = null;

                var defaultExpression = ConvertSqlCeDefaultValue(column.DefaultValue);
                if (defaultExpression != null)
                {
                    if (!((defaultExpression == "NULL")
                            && propertyBuilder.Metadata.ClrType.IsNullableType()))
                    {
                        propertyBuilder.HasDefaultValueSql(defaultExpression);
                    }
                }
                else
                {
                    //TODO ErikEJ fix logging (see sqlite provider)
                    //Logger.LogWarning(
                    //    $"For column {column.DisplayName} unable to interpret default value {column.DefaultValue}. Will not generate code setting a default value for the property {propertyBuilder.Metadata.Name} on entity type {propertyBuilder.Metadata.DeclaringEntityType.Name}.");
                }
            }
        }

        private string ConvertSqlCeDefaultValue(string sqlCeDefaultValue)
        {
            if (sqlCeDefaultValue.Length < 2)
            {
                return null;
            }

            while ((sqlCeDefaultValue[0] == '(')
                   && (sqlCeDefaultValue[sqlCeDefaultValue.Length - 1] == ')'))
            {
                sqlCeDefaultValue = sqlCeDefaultValue.Substring(1, sqlCeDefaultValue.Length - 2);
            }

            return sqlCeDefaultValue;
        }
    }
}
