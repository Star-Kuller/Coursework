using System.Data;
using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_10_1833)] 
public class M0007AddUser : Migration
{
    public override void Up()
    {
        Create.Table("roles")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable();
        
        Insert.IntoTable("roles").Row(new { name = "Администратор" });
        Insert.IntoTable("roles").Row(new { name = "Пользователь" });
        
        Create.Table("users")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("email").AsString(255).NotNullable().Unique()
            .WithColumn("password_hash").AsString(255).NotNullable()
            .WithColumn("about").AsCustom("TEXT").NotNullable().WithDefaultValue("")
            .WithColumn("score").AsInt64().NotNullable().WithDefaultValue(0)
            .WithColumn("role_id").AsInt64().NotNullable();
        
        Create.ForeignKey("FK_users_roles")
            .FromTable("users").ForeignColumn("role_id")
            .ToTable("roles").PrimaryColumn("id");
        
        Create.Index("IX_users_role_id")
            .OnTable("users")
            .OnColumn("role_id")
            .Ascending();
        
        Alter.Table("exercises")
            .AddColumn("author_id").AsInt64().NotNullable();
        
        Create.ForeignKey("FK_exercises_users")
            .FromTable("exercises").ForeignColumn("author_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.Index("IX_exercises_owner_id")
            .OnTable("exercises")
            .OnColumn("author_id")
            .Ascending();
        
        Delete.Column("by_exercise_author").FromTable("solutions");
        
        Alter.Table("solutions")
            .AddColumn("author_id").AsInt64().NotNullable();
        
        Create.ForeignKey("FK_solutions_users")
            .FromTable("solutions").ForeignColumn("author_id")
            .ToTable("users").PrimaryColumn("id");
        
        Create.Index("IX_solutions_owner_id")
            .OnTable("solutions")
            .OnColumn("author_id")
            .Ascending();
    }

    public override void Down()
    {
        Delete.Index("IX_solutions_author_id").OnTable("solutions");
        Delete.ForeignKey("FK_solutions_users").OnTable("solutions");
        
        Alter.Table("solutions")
            .AddColumn("by_exercise_author").AsBoolean().NotNullable().WithDefaultValue(false);
        
        Delete.Column("author_id").FromTable("solutions");
        
        Delete.Index("IX_exercises_author_id").OnTable("exercises");
        Delete.ForeignKey("FK_exercises_users").OnTable("exercises");
        
        Delete.Column("author_id").FromTable("exercises");
        
        Delete.Index("IX_users_email").OnTable("users");
        Delete.Index("IX_users_role_id").OnTable("users");
        
        Delete.ForeignKey("FK_users_roles").OnTable("users");
        
        Delete.Table("users");
        
        Delete.Table("roles");
    }
}