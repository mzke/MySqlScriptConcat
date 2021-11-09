using System;
namespace MySqlScriptConcat;

public static class Create
{
	public static string CallDropFkScript()
	{
		return $"CALL tmp_drop_fk();";
	}
	public static string DropFkScript(string schemaName)
    {
        return @"

DELIMITER $$
DROP PROCEDURE IF EXISTS tmp_drop_fk; $$
CREATE PROCEDURE tmp_drop_fk()
BEGIN
	DECLARE done INT DEFAULT FALSE;
	DECLARE tablename VARCHAR(128);
    DECLARE constraintname VARCHAR(64);
	DECLARE curFK CURSOR FOR SELECT TABLE_NAME, CONSTRAINT_NAME
	                           FROM INFORMATION_SCHEMA.referential_constraints C
                              WHERE C.constraint_schema = '" + schemaName + @"';
	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

	OPEN curFK;
	read_loop: LOOP
		FETCH curFK INTO tablename, constraintname;
		IF done THEN
			LEAVE read_loop;
		END IF;
        
		SET @q = CONCAT('ALTER TABLE ', tablename, ' DROP FOREIGN KEY ', constraintname);
		
		PREPARE stmt1 FROM @q;
		EXECUTE stmt1;
		DEALLOCATE PREPARE stmt1;
	END LOOP;
  CLOSE curFK;
  END $$
  DELIMITER ;
";
    }

	public static string CallDropPkScript()
	{
		return $"CALL tmp_drop_pk();";
	}
	public static string DropPkScript(string schemaName)
    {
		return @"
DELIMITER $$
DROP PROCEDURE IF EXISTS tmp_drop_pk; $$
CREATE PROCEDURE tmp_drop_pk()
BEGIN
	DECLARE done INT DEFAULT FALSE;
	DECLARE tablename VARCHAR(128);
	DECLARE curPK CURSOR FOR SELECT TABLE_NAME
                               FROM INFORMATION_SCHEMA.columns C
                              WHERE C.table_schema = '" + schemaName + @"'
                                AND C.extra != 'auto_increment'
                                AND C.Column_key = 'PRI';
	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	OPEN curPK;
		read_loop: LOOP
			FETCH curPK INTO tablename;
			IF done THEN
				LEAVE read_loop;
			END IF;
			SET @q = CONCAT('ALTER TABLE ', tablename, ' DROP PRIMARY KEY');
			PREPARE stmt1 FROM @q;
			EXECUTE stmt1;
			DEALLOCATE PREPARE stmt1;
		END LOOP;
	  CLOSE curPK;

END $$
DELIMITER ;
";
    }

	public static string CallDropTriggersScript()
	{
		return $"CALL tmp_drop_triggers();";
	}
	public static string DropTriggersScript(string schemaName)
	{
		return @"
DELIMITER $$
DROP PROCEDURE IF EXISTS tmp_drop_triggers; $$
CREATE PROCEDURE tmp_drop_triggers()
BEGIN
	DECLARE done INT DEFAULT FALSE;
	DECLARE triggername VARCHAR(128);
	DECLARE curTRG CURSOR FOR SELECT  trigger_name
                             FROM INFORMATION_SCHEMA.triggers 
                             WHERE trigger_schema = '" + schemaName + @"';
	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	OPEN curTRG;
		read_loop: LOOP
			FETCH curTRG INTO triggername;
			IF done THEN
				LEAVE read_loop;
			END IF;
			SET @q = CONCAT('DROP TRIGGER IF EXISTS ', triggername, ';');
			PREPARE stmt1 FROM @q;
			EXECUTE stmt1;
			DEALLOCATE PREPARE stmt1;
		END LOOP;
	  CLOSE curTRG;
END $$
DELIMITER ;
";
	}

	public static string CallDropRoutinesScript(string schemaName, string routineType)
	{
		return $"CALL tmp_drop_routines('{schemaName}', '{routineType}');";
	}
	public static string DropRoutinesScript()
	{
		return @"
DELIMITER $$
DROP PROCEDURE IF EXISTS tmp_drop_routines; $$
CREATE PROCEDURE tmp_drop_routines(
	$schemaname VARCHAR(64),
	$routinetype VARCHAR(64)
)
BEGIN
	DECLARE done INT DEFAULT FALSE;
	DECLARE $routinename VARCHAR(128);
	DECLARE curRTN CURSOR FOR SELECT routine_name
                              FROM INFORMATION_SCHEMA.routines 
                              WHERE routine_schema = $schemaname
                              AND routine_type = $routinetype;
	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	OPEN curRTN;
		read_loop: LOOP
			FETCH curRTN INTO $routinename;
			IF done THEN
				LEAVE read_loop;
			END IF;
			SET @q = CONCAT('DROP ', $routinetype, ' ', $routinename, ';');
			PREPARE stmt1 FROM @q;
			EXECUTE stmt1;
			DEALLOCATE PREPARE stmt1;
		END LOOP;
	  CLOSE curRTN;
END $$
DELIMITER ;
";
	}

	public static string CallDropViewsScript(string schemaName)
	{
		return $"CALL tmp_drop_views('{schemaName}');";
	}
	public static string DropViewsScript()
	{
		return @"
DELIMITER $$
DROP PROCEDURE IF EXISTS tmp_drop_views; $$
CREATE PROCEDURE tmp_drop_views(
	$schemaname VARCHAR(64)
)
BEGIN
	DECLARE done INT DEFAULT FALSE;
	DECLARE $viewname VARCHAR(128);
	DECLARE curV CURSOR FOR SELECT table_name
                              FROM INFORMATION_SCHEMA.views 
                              WHERE table_schema = $schemaname;
   	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	OPEN curV;
		read_loop: LOOP
			FETCH curV INTO $viewname;
			IF done THEN
				LEAVE read_loop;
			END IF;
			SET @q = CONCAT('DROP VIEW ', $viewname, ';');
			PREPARE stmt1 FROM @q;
			EXECUTE stmt1;
			DEALLOCATE PREPARE stmt1;
		END LOOP;
	  CLOSE curV;
END $$
DELIMITER ;
";
	}

	public static string ViewIndexesScript(string schemaName)
	{
		return @"
DROP VIEW IF EXISTS tmp_indexes_view;
CREATE VIEW tmp_indexes_view AS
select distinct I.table_name,  I.index_name
from INFORMATION_SCHEMA.statistics I
join INFORMATION_SCHEMA.columns C on C.table_name = I.table_name and C.column_name = I.column_name
 WHERE I.table_schema = '" + schemaName + @"'
 AND C.extra =  '';
";
	}

	public static string DropIndexesScript()
	{
		return @"

DELIMITER $$
DROP PROCEDURE IF EXISTS tmp_drop_indexes; $$
CREATE PROCEDURE tmp_drop_indexes()
BEGIN
	DECLARE done INT DEFAULT FALSE;
    DECLARE tablename VARCHAR(128);
	DECLARE indexname VARCHAR(128);
	DECLARE curIX CURSOR FOR SELECT TABLE_NAME, INDEX_NAME
                               FROM tmp_indexes_view;						  
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	OPEN curIX;
	read_loop: LOOP
		FETCH curIX INTO tablename, indexname;
		IF done THEN
			LEAVE read_loop;
		END IF;
		SET @q = CONCAT('ALTER TABLE ', tablename, ' DROP INDEX ', indexname);
		PREPARE stmt1 FROM @q;
		EXECUTE stmt1;
		DEALLOCATE PREPARE stmt1;
	END LOOP;
  CLOSE curIX;
  END $$
  DELIMITER ;
";
	}

	public static string CallDropIndexes()
	{
		return $"CALL tmp_drop_indexes();";
	}
}