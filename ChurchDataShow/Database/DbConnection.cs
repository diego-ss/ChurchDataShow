using ChurchDataShow.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace ChurchDataShow.Database
{
    class DbConnection
    {
        private string _appDataFolder = $@"{ System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)}\ChurchDataShowFiles";
        private string _dbFile = @"\ChurchDataShow_Database.db";
        private string _fullDbName = "";

        private static DbConnection _instance;
        private SQLiteConnection sqliteConnection;

        private List<string> CreateTableCommands = new List<string>()
        {
            //tabela de músicas
            @"CREATE TABLE IF NOT EXISTS Musicas (
              `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
              `Cantor` VARCHAR(45) NOT NULL,
              `Nome` VARCHAR(45) NOT NULL,
              `Letra` LONGTEXT NOT NULL
            )",

            //tabela da bíblia
            @"CREATE TABLE IF NOT EXISTS Biblia (
              `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
              `idTraducao` INT NOT NULL,
              `IdLivro` INT NOT NULL,
              `Capitulo` INT NOT NULL,
              `Versiculo` INT NOT NULL,
              `Texto` LONGTEXT NOT NULL
            )",

            //tabela de livros da bíblia
            @"CREATE TABLE IF NOT EXISTS Livros (
              `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
              `Nome` VARCHAR(50) NOT NULL
            )",

            //tabela de traduções disponíveis
            @"CREATE TABLE IF NOT EXISTS Traducoes (
              `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
              `Nome` VARCHAR(25) NOT NULL
            )",
        };

        public DbConnection()
        {
        }

        public static DbConnection GetInstance()
        {
            if (_instance == null)
                _instance = new DbConnection();

            return _instance;
        }

        public async Task CreateDatabase()
        {
            _fullDbName = _appDataFolder + _dbFile;

            //if db does not exists, it will be created
            if (!Directory.Exists(_appDataFolder))
                Directory.CreateDirectory(_appDataFolder);

            if (!File.Exists(_fullDbName))
                SQLiteConnection.CreateFile(_fullDbName);

            Connect();
            await InitializeDatabase();
        }

        public SQLiteConnection GetDbConnection()
        {
            return _instance.sqliteConnection;
        }

        public DataTable GetDataTable(string table)
        {
            var connection = this.GetDbConnection();

            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();

            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM " + table;
                    da = new SQLiteDataAdapter(cmd.CommandText, connection);
                    da.Fill(dt);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetQueryTable(string query)
        {
            var connection = this.GetDbConnection();

            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();

            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    da = new SQLiteDataAdapter(cmd.CommandText, connection);
                    da.Fill(dt);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SQLiteConnection Connect()
        {
            try
            {
                sqliteConnection = new SQLiteConnection("Data Source=" + _fullDbName + "; Version=3;");
                sqliteConnection.Open();
                return sqliteConnection;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao se conectar ao banco de dados.\n" + ex.Message);
            }
        }

        private async Task<bool> InitializeDatabase()
        {
            //cria as tabelas caso não existam
            CreateTables();

            if ((GetDataTable("Biblia")).Rows.Count > 0)
                return true;

            //criar seeds para popular o bd dos textos, imagens e músicas inicialmente
            BibleSeedService bibleSeed = new BibleSeedService();
            await bibleSeed.SeedTraducoes();
            await bibleSeed.SeedLivros();
            await bibleSeed.SeedBiblia();

            return true;
        }

        private void CreateTables()
        {
            foreach (var command in CreateTableCommands)
            {
                try
                {
                    using (var cmd = GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = command;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
