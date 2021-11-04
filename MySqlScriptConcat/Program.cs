using System.IO;
using MySqlScriptConcat;

public class Program
{
    public static void Main(string[] args)
    {
        var schemaName = args[0];

        var outputFile = "./script.sql";

        //using StreamWriter script = File.AppendText("./script.sql");
        Script script = new(outputFile);
        script.WriteLine($"use {schemaName};");

        script.Concat("_pre.sql");

        script.WriteLine(Create.DropPkScript(schemaName));
        script.WriteLine(Create.DropFkScript(schemaName));
        script.WriteLine(Create.DropIndexesScript());
        script.WriteLine(Create.ViewIndexesScript(schemaName));
        script.WriteLine(Create.DropTriggersScript(schemaName));
        script.WriteLine(Create.DropRoutinesScript());

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

        /*
        var arquivos = Directory.GetFiles(".", "*_table.sql");
        foreach (var table in arquivos)
        {
            string? line = "";
            using StreamReader tableFile = new(table);
            {
                while ((line = tableFile.ReadLine()) != null)
                {
                    script.WriteLine(line);
                }
            }
        }*/
        script.Concat("*_table.sql");
        script.WriteLine(@"\! echo 'Tables created.'");
        /*
        var pks = Directory.GetFiles(".", "*_pk.sql");
        foreach (var pk in pks)
        {
            string? line = "";
            using StreamReader sr = new(pk);
            {
                while ((line = sr.ReadLine()) != null)
                {
                    script.WriteLine(line);
                }
            }
        }*/
        script.Concat("*_pk.sql");
        script.WriteLine(@"\! echo 'Primary Keys created.'");
        /*
        var fks = Directory.GetFiles(".", "*_fk.sql");
        foreach (var fk in fks)
        {
            string? line = "";
            using StreamReader sr = new(fk);
            {
                while ((line = sr.ReadLine()) != null)
                {
                    script.WriteLine(line);
                }
            }
        }*/
        script.Concat("*_fk.sql");
        script.WriteLine(@"\! echo 'Foreign Keys created.'");

        /*
        var idxs = Directory.GetFiles(".", "*_idx.sql");
        foreach (var idx in idxs)
        {
            string? line = "";
            using StreamReader sr = new(idx);
            {
                while ((line = sr.ReadLine()) != null)
                {
                    script.WriteLine(line);
                }
            }
        }*/
        script.Concat("*_idx.sql");
        script.WriteLine(@"\! echo 'Indexes created.'");
        /*
        var triggersFiles = Directory.GetFiles(".", "*_trg.sql");
        foreach (var trgFile in triggersFiles)
        {
            string? line = "";
            using StreamReader sr = new(trgFile);
            {
                while ((line = sr.ReadLine()) != null)
                {
                    script.WriteLine(line);
                }
            }
        }*/
        script.Concat("*_trg.sql");
        script.WriteLine(@"\! echo 'Triggers created.'");
        /*
        var viewFiles = Directory.GetFiles(".", "*_view.sql");
        foreach (var vFile in viewFiles)
        {
            string? line = "";
            using StreamReader sr = new(vFile);
            {
                while ((line = sr.ReadLine()) != null)
                {
                    script.WriteLine(line);
                }
            }
        }*/
        script.Concat("*_view.sql");
        script.WriteLine(@"\! echo 'Views created.'");

        script.Concat("*_fx.sql");
        script.WriteLine(@"\! echo 'Functions created.'");

        script.Concat("*_sp.sql");
        script.WriteLine(@"\! echo 'Store Procedures created.'");


        script.WriteLine("DROP PROCEDURE tmp_drop_pk;");
        script.WriteLine("DROP PROCEDURE tmp_drop_fk;");
        script.WriteLine("DROP VIEW tmp_indexes_view;");
        script.WriteLine("SHOW WARNINGS;");
        script.WriteLine("SHOW ERRORS;");
        script.Close();
    }
}