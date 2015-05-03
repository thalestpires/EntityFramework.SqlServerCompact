﻿using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.DependencyInjection;

namespace ErikEJ.Data.Entity.SqlServerCompact
{
    public class SqlServerCompactOptionsExtension : RelationalOptionsExtension
    {
        public SqlServerCompactOptionsExtension()
        {
        }

        public SqlServerCompactOptionsExtension([NotNull] SqlServerCompactOptionsExtension copyFrom)
            : base(copyFrom)
        {
        }

        public override void ApplyServices(EntityFrameworkServicesBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
            //TODO - but requires everything to be implemented!
            //builder.AddSqlServerCompact();
        }
    }
}
