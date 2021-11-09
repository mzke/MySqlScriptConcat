using System.IO;
using MySqlScriptConcat;

public class Program
{
    public static void Main(string[] args)
    {
        var schemaName = args[0];
        var outputFile = "./script.sql";
        Script script = new(outputFile);
        script.WriteLine($"use {schemaName};");

        script.Concat("_pre.sql");

        script.WriteLine(Create.DropPkScript(schemaName));
        script.WriteLine(Create.DropFkScript(schemaName));
        script.WriteLine(Create.DropIndexesScript());
        script.WriteLine(Create.ViewIndexesScript(schemaName));
        script.WriteLine(Create.DropTriggersScript(schemaName));
        script.WriteLine(Create.DropRoutinesScript());
        script.WriteLine(Create.DropViewsScript());

        

        // ERROR 1295 (HY000): This command is not supported in the prepared statement protocol yet
        // script.WriteLine(Create.CallDropRoutinesScript(schemaName, "PROCEDURE"));
        // script.WriteLine(@"\! echo 'Store Procedures dropped.'");
        // script.WriteLine(Create.CallDropRoutinesScript(schemaName, "FUNCTION"));
        // script.WriteLine(@"\! echo 'Functions dropped.'");

        script.WriteLine(Create.CallDropTriggersScript());
        script.WriteLine(@"\! echo 'Triggers dropped.'");

        script.WriteLine(Create.CallDropFkScript());
        script.WriteLine(@"\! echo 'Foreign Keys dropped.'");
       
        script.WriteLine(Create.CallDropPkScript());
        script.WriteLine(@"\! echo 'Primary Keys dropped.'");

        script.WriteLine(Create.CallDropIndexes());
        script.WriteLine(@"\! echo 'Indexes dropped.'");

        script.WriteLine(Create.CallDropViewsScript(schemaName));

        script.Concat("*_table.sql");
        script.WriteLine(@"\! echo 'Tables created.'");
        
        script.Concat("*_pk.sql");
        script.WriteLine(@"\! echo 'Primary Keys created.'");
        
        script.Concat("*_fk.sql");
        script.WriteLine(@"\! echo 'Foreign Keys created.'");

        script.Concat("*_idx.sql");
        script.WriteLine(@"\! echo 'Indexes created.'");
        
        script.Concat("*_trg.sql");
        script.WriteLine(@"\! echo 'Triggers created.'");
        
        script.Concat("*_view.sql");
        script.WriteLine(@"\! echo 'Views created.'");

        script.Concat("*_fx.sql");
        script.WriteLine(@"\! echo 'Functions created.'");

        script.Concat("*_sp.sql");
        script.WriteLine(@"\! echo 'Store Procedures created.'");

        script.WriteLine("DROP PROCEDURE tmp_drop_pk;");
        script.WriteLine("DROP PROCEDURE tmp_drop_fk;");
       
        
        script.Concat("_post.sql");
        script.Close();
    }
}