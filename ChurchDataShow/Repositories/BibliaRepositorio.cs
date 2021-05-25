using ChurchDataShow.Database;
using ChurchDataShow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchDataShow.Repositories
{
    class BibliaRepositorio
    {
        private DbConnection _dbConnection;

        public BibliaRepositorio()
        {
            _dbConnection = DbConnection.GetInstance();
        }

        public async Task<bool> AdicionarTraducao(string traducao)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var cmd = _dbConnection.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Traducoes(Nome) values (@nome)";
                        cmd.Parameters.AddWithValue("@nome", traducao);
                        cmd.ExecuteNonQuery();
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AdicionarLivro(string livro)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var cmd = _dbConnection.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Livros(Nome) values (@nome)";
                        cmd.Parameters.AddWithValue("@nome", livro);
                        cmd.ExecuteNonQuery();
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AdicionarReferenciaBiblia(TextoBiblia referencia)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var cmd = _dbConnection.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Biblia(IdTraducao, IdLivro, Capitulo, Versiculo, Texto) " +
                        "values (@idtraducao, @idlivro, @capitulo, @versiculo, @texto)";
                        cmd.Parameters.AddWithValue("@idtraducao", referencia.IdTraducao);
                        cmd.Parameters.AddWithValue("@idlivro", referencia.IdLivro);
                        cmd.Parameters.AddWithValue("@capitulo", referencia.Capitulo);
                        cmd.Parameters.AddWithValue("@versiculo", referencia.Versiculo);
                        cmd.Parameters.AddWithValue("@texto", referencia.Texto);
                        cmd.ExecuteNonQuery();
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Traducao>> ListarTraducoes()
        {
            try
            {
                List<Traducao> lstTraducoes = new List<Traducao>();
                DataTable dt = new DataTable();

                await Task.Run(() =>
                {
                    dt = _dbConnection.GetDataTable("Traducoes");
                });

                lstTraducoes = (from DataRow dr in dt.Rows
                                select new Traducao
                                {
                                    Id = int.Parse(dr["Id"].ToString()),
                                    Nome = dr["Nome"].ToString()
                                }).ToList();

                return lstTraducoes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Livro>> ListarLivros()
        {
            try
            {
                List<Livro> lstLivros = new List<Livro>();
                DataTable dt = new DataTable();

                await Task.Run(() =>
                {
                    dt = _dbConnection.GetDataTable("Livros");
                });

                lstLivros = (from DataRow dr in dt.Rows
                                select new Livro
                                {
                                    Id = int.Parse(dr["Id"].ToString()),
                                    Nome = dr["Nome"].ToString()
                                }).ToList();

                return lstLivros;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<int>> ListarCapitulos(int livro)
        {
            try
            {
                List<int> lstCapitulos = new List<int>();
                DataTable dt = new DataTable();

                await Task.Run(() =>
                {
                    dt = _dbConnection.GetQueryTable("SELECT Capitulo FROM Biblia WHERE IdLivro = " + livro);
                });

                lstCapitulos = (from DataRow dr in dt.Rows
                             select int.Parse(dr["Capitulo"].ToString())).Distinct().ToList();

                return lstCapitulos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<int>> ListarVersiculos(int livro, int capitulo)
        {
            try
            {
                List<int> lstCapitulos = new List<int>();
                DataTable dt = new DataTable();

                await Task.Run(() =>
                {
                    dt = _dbConnection.GetQueryTable("SELECT Versiculo FROM Biblia WHERE IdLivro = " + livro + " AND Capitulo = " + capitulo);
                });

                lstCapitulos = (from DataRow dr in dt.Rows
                                select int.Parse(dr["Versiculo"].ToString())).Distinct().ToList();

                return lstCapitulos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TextoBiblia> BuscarTexto(int livro, int capitulo, int versiculo, int traducao)
        {
            try
            {
                DataTable dt = new DataTable();

                await Task.Run(() =>
                {
                    dt = _dbConnection.GetQueryTable("SELECT * FROM Biblia WHERE IdLivro = " + livro + 
                            " AND Capitulo = " + capitulo + " AND Versiculo = " + versiculo + " AND IdTraducao = " + traducao);
                });

                return (from DataRow dr in dt.Rows
                                select new TextoBiblia { 
                                    Id = int.Parse(dr["Id"].ToString()),
                                    Capitulo = capitulo,
                                    Versiculo = versiculo,
                                    IdTraducao = traducao,
                                    Texto = dr["Texto"].ToString(),
                                    IdLivro = livro
                                }).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
