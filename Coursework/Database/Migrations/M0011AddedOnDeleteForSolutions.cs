using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_14_1238)] 
public class M0011AddedOnDeleteRules : Migration
{
    public override void Up()
    {
        Delete.ForeignKey("FK_solutions_users").OnTable("solutions");
        Delete.ForeignKey("FK_exercises_users").OnTable("exercises");
        Delete.ForeignKey("FK_exercises_programing_languages").OnTable("exercises");
        
        Create.ForeignKey("FK_solutions_users")
            .FromTable("solutions").ForeignColumn("author_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_exercises_users")
            .FromTable("exercises").ForeignColumn("author_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_exercises_programing_languages")
            .FromTable("exercises").ForeignColumn("language_id")
            .ToTable("programing_languages").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_solutions_users").OnTable("solutions");
        Delete.ForeignKey("FK_exercises_users").OnTable("exercises");
        Delete.ForeignKey("FK_exercises_programing_languages").OnTable("exercises");
        
        Create.ForeignKey("FK_solutions_users")
            .FromTable("solutions").ForeignColumn("author_id")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("FK_exercises_users")
            .FromTable("exercises").ForeignColumn("author_id")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("FK_exercises_programing_languages")
            .FromTable("exercises").ForeignColumn("language_id")
            .ToTable("programing_languages").PrimaryColumn("id");
    }
}