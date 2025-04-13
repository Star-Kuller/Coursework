using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_13_1757)] 
public class M0009AddSolutionLikes : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("solution_likes")
            .WithColumn("solution_id").AsInt64().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable();
        
        Create.PrimaryKey("PK_solution_likes")
            .OnTable("solution_likes")
            .Columns(["solution_id", "user_id"]);
        
        Create.ForeignKey("FK_solution_likes_solutions")
            .FromTable("solution_likes").ForeignColumn("solution_id")
            .ToTable("solutions").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_solution_likes_users")
            .FromTable("solution_likes").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);
        
        Create.Index("IX_solution_likes_solution_id")
            .OnTable("solution_likes")
            .OnColumn("solution_id")
            .Ascending();
        
        Create.Index("IX_solution_likes_user_id")
            .OnTable("solution_likes")
            .OnColumn("user_id")
            .Ascending();
    }
}