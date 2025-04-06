using System.Data;
using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_06_0007)] 
public class M0004AddSolutions  : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("solutions")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("s3_key").AsString(1024).NotNullable()
            .WithColumn("by_exercise_author").AsBoolean().NotNullable()
            .WithColumn("exercise_id").AsInt64().NotNullable();
        
        Create.ForeignKey("FK_solutions_exercises")
            .FromTable("solutions").ForeignColumn("exercise_id")
            .ToTable("exercises").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);;
        
        Create.Index("IX_solutions_exercise_id")
            .OnTable("solutions")
            .OnColumn("exercise_id")
            .Ascending();
        
        Alter.Column("short_description")
            .OnTable("exercises")
            .AsAnsiString(150)
            .NotNullable();
    }
}