﻿using ErikEJ.Data.Entity.SqlServerCe.MetaData.ModelConventions;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Metadata.ModelConventions;

namespace ErikEJ.Data.Entity.SqlServerCe.Metadata
{
    public class SqlServerCeModelBuilderFactory : ModelBuilderFactory
    {
        protected override ConventionSet CreateConventionSet()
        {
            var conventions = base.CreateConventionSet();

            conventions.ModelConventions.Add(new SqlServerCeValueGenerationStrategyConvention());

            return conventions;
        }
    }
}
