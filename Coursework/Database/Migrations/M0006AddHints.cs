using System.Data;
using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_10_0008)] 
public class M0006AddHints : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("hints")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("exercise_id").AsInt64().NotNullable()
            .WithColumn("cost").AsInt32().NotNullable()
            .WithColumn("text").AsString(1000).NotNullable();
        
        Create.ForeignKey("FK_hints_exercises")
            .FromTable("hints").ForeignColumn("exercise_id")
            .ToTable("exercises").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);
        
        Create.Index("IX_hints_exercise_id")
            .OnTable("hints")
            .OnColumn("exercise_id")
            .Ascending();
    }
}