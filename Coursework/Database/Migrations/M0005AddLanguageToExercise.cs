using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_08_0054)] 
public class M0005AddLanguageToExercise : AutoReversingMigration
{
    public override void Up()
    {
        Alter.Table("exercises")
            .AddColumn("language_id").AsInt32().NotNullable();
        
        Create.Index("IX_exercises_language_id")
            .OnTable("exercises")
            .OnColumn("language_id")
            .Ascending();
        
        Create.ForeignKey("FK_exercises_programing_languages")
            .FromTable("exercises").ForeignColumn("language_id")
            .ToTable("programing_languages").PrimaryColumn("id");
    }
}