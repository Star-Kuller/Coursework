using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_02_1949)] 
public class M0001Init : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("programing_languages")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("description").AsString(500).NotNullable();
    }
}