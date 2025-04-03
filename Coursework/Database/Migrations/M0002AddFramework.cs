using System.Data;
using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_03_1136)] 
public class M0002AddFramework : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("frameworks")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("description").AsString(500).NotNullable()
            .WithColumn("language_id").AsInt64().NotNullable();

        Create.ForeignKey("FK_frameworks_programing_languages")
            .FromTable("frameworks").ForeignColumn("language_id")
            .ToTable("programing_languages").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);
        
        Create.Index("IX_frameworks_language_id")
            .OnTable("frameworks")
            .OnColumn("language_id")
            .Ascending();
    }
}