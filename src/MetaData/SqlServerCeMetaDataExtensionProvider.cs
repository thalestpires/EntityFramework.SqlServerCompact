﻿using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace Microsoft.Data.Entity.Sqlite.Metadata
{
    public class SqlServerCeMetadataExtensionProvider : IRelationalMetadataExtensionProvider
    {
        // TODO: Update with #875
        public virtual IRelationalEntityTypeExtensions Extensions(IEntityType entityType) => entityType.Relational();
        public virtual IRelationalForeignKeyExtensions Extensions(IForeignKey foreignKey) => foreignKey.Relational();
        public virtual IRelationalIndexExtensions Extensions(IIndex index) => index.Relational();
        public virtual IRelationalKeyExtensions Extensions(IKey key) => key.Relational();
        public virtual IRelationalModelExtensions Extensions(IModel model) => model.Relational();
        public virtual IRelationalPropertyExtensions Extensions(IProperty property) => property.Relational();
    }
}
