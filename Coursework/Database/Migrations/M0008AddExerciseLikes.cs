using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_13_1448)] 
public class M0008AddExerciseLikes : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("exercise_likes")
            .WithColumn("exercise_id").AsInt64().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable();
        
        Create.PrimaryKey("PK_exercise_likes")
            .OnTable("exercise_likes")
            .Columns(["exercise_id", "user_id"]);
        
        Create.ForeignKey("FK_exercise_likes_exercises")
            .FromTable("exercise_likes").ForeignColumn("exercise_id")
            .ToTable("exercises").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_exercise_likes_users")
            .FromTable("exercise_likes").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);
        
        Create.Index("IX_exercise_likes_exercise_id")
            .OnTable("exercise_likes")
            .OnColumn("exercise_id")
            .Ascending();
        
        Create.Index("IX_exercise_likes_user_id")
            .OnTable("exercise_likes")
            .OnColumn("user_id")
            .Ascending();
    }
}