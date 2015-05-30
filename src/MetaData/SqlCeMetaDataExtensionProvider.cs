﻿using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace ErikEJ.Data.Entity.SqlServerCe.Metadata
{
    public class SqlCeMetadataExtensionProvider : IRelationalMetadataExtensionProvider
    {
        // TODO: Update with #875
        public virtual IRelationalEntityTypeExtensions Extensions(IEntityType entityType) => entityType.Relational();
        public virtual IRelationalForeignKeyExtensions Extensions(IForeignKey foreignKey) => foreignKey.Relational();
        public virtual IRelationalIndexExtensions Extensions(IIndex index) => index.Relational();
        public virtual IRelationalKeyExtensions Extensions(IKey key) => key.Relational();
        public virtual IRelationalModelExtensions Extensions(IModel model) => model.SqlCe();
        //TODO ErikEJ Expand (maybe)
        public virtual IRelationalPropertyExtensions Extensions(IProperty property) => property.SqlCe();
    }
}
