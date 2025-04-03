using System.Data;
using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_03_1220)] 
public class M0003AddExercises : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("difficulty_levels")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable();
        
        Insert.IntoTable("difficulty_levels").Row(new { name = "Очень легко" });
        Insert.IntoTable("difficulty_levels").Row(new { name = "Легко" });
        Insert.IntoTable("difficulty_levels").Row(new { name = "Средне" });
        Insert.IntoTable("difficulty_levels").Row(new { name = "Сложно" });
        Insert.IntoTable("difficulty_levels").Row(new { name = "Очень сложно" });
        
        Create.Table("exercises")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("difficulty_id").AsInt64().NotNullable()
            .WithColumn("score").AsInt64().NotNullable()
            .WithColumn("short_description").AsString(250).Nullable()
            .WithColumn("full_description").AsCustom("TEXT").NotNullable()
            .WithColumn("is_published").AsBoolean().NotNullable()
            .WithColumn("s3_key_source").AsString(1024).Nullable()
            .WithColumn("s3_key_tests").AsString(1024).NotNullable();
        
        Create.ForeignKey("FK_exercises_difficulty_levels")
            .FromTable("exercises").ForeignColumn("difficulty_id")
            .ToTable("difficulty_levels").PrimaryColumn("id");
        
        Create.Index("IX_exercises_difficulty_id")
            .OnTable("exercises")
            .OnColumn("difficulty_id")
            .Ascending();
        
        Create.Table("frameworks_exercises")
            .WithColumn("framework_id").AsInt64().NotNullable()
            .WithColumn("exercise_id").AsInt64().NotNullable();
        
        Create.PrimaryKey("PK_frameworks_exercises")
            .OnTable("frameworks_exercises")
            .Columns(["framework_id", "exercise_id"]);
        
        Create.ForeignKey("FK_frameworks_exercises_frameworks")
            .FromTable("frameworks_exercises").ForeignColumn("framework_id")
            .ToTable("frameworks").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);;

        Create.ForeignKey("FK_frameworks_exercises_exercises")
            .FromTable("frameworks_exercises").ForeignColumn("exercise_id")
            .ToTable("exercises").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);;
        
        Create.Index("IX_frameworks_exercises_framework_id")
            .OnTable("frameworks_exercises")
            .OnColumn("framework_id")
            .Ascending();
        
        Create.Index("IX_frameworks_exercises_exercise_id")
            .OnTable("frameworks_exercises")
            .OnColumn("exercise_id")
            .Ascending();
    }
}