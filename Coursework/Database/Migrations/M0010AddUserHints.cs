using FluentMigrator;

namespace Coursework.Database.Migrations;

[Migration(2025_04_13_1853)] 
public class M0010AddUserHints : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("user_hints")
            .WithColumn("hint_id").AsInt64().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable();
        
        Create.PrimaryKey("PK_user_hints")
            .OnTable("user_hints")
            .Columns(["hint_id", "user_id"]);
        
        Create.ForeignKey("FK_user_hints_hints")
            .FromTable("user_hints").ForeignColumn("hint_id")
            .ToTable("hints").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_user_hints_users")
            .FromTable("user_hints").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);
        
        Create.Index("IX_user_hints_hint_id")
            .OnTable("user_hints")
            .OnColumn("hint_id")
            .Ascending();
        
        Create.Index("IX_user_hints_user_id")
            .OnTable("user_hints")
            .OnColumn("user_id")
            .Ascending();
    }
}