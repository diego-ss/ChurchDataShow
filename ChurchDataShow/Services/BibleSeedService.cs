using ChurchDataShow.Database;
using ChurchDataShow.Models;
using ChurchDataShow.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchDataShow.Services
{
    class BibleSeedService
    {
        private DbConnection _dbConnection;
        private List<Livro> _lstLivros;
        private List<Traducao> _lstTraducoes;
        private BibliaRepositorio _repositorio;

        public BibleSeedService()
        {
            _dbConnection = DbConnection.GetInstance();

            //carregando listas de referência
            _repositorio = new BibliaRepositorio();
        }

        public async Task<bool> SeedTraducoes()
        {
            List<string> lstTraducoes = new List<string>()
            {
                "ACF", "JFA", "RA", "RC"
            };

            try
            {
                BibliaRepositorio repositorio = new BibliaRepositorio();

                foreach (var trad in lstTraducoes)
                    await repositorio.AdicionarTraducao(trad);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SeedLivros()
        {
            var livros = (from livro in (ChurchDataShow.Properties.Resources.Livros.Split(new string[] { "\n" }, StringSplitOptions.None))
                          select livro.Trim()).ToList();

            try
            {
                BibliaRepositorio repositorio = new BibliaRepositorio();

                foreach (var livro in livros)
                    await repositorio.AdicionarLivro(livro);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SeedBiblia()
        {
            _lstTraducoes = await _repositorio.ListarTraducoes();
            _lstLivros = await _repositorio.ListarLivros();


            //Seeding IB Bible
            var biblia_ib = ChurchDataShow.Properties.Resources.Velho_ib + ChurchDataShow.Properties.Resources.Novo_ib;
            var lstBiblia_ib = (from linha in biblia_ib.Split(new string[] { "\n" }, StringSplitOptions.None)
                                    .Where(s => s.Trim() != "" && s.Trim() != ">")
                                select linha).ToList();

            await AdicionarVersiculo(lstBiblia_ib, "ACF");


            //Seeding JFA Bible
            var biblia_jfa = ChurchDataShow.Properties.Resources.Velho_JFA + ChurchDataShow.Properties.Resources.Novo_JFA;
            var lstBiblia_jfa = (from linha in biblia_jfa.Split(new string[] { "\n" }, StringSplitOptions.None)
                                    .Where(s => s.Trim() != "" && s.Trim() != ">")
                                select linha).ToList();

            await AdicionarVersiculo(lstBiblia_jfa, "JFA");


            //Seeding RA Bible
            var biblia_ra = ChurchDataShow.Properties.Resources.Velho_RA + ChurchDataShow.Properties.Resources.Novo_RA;
            var lstBiblia_ra = (from linha in biblia_jfa.Split(new string[] { "\n" }, StringSplitOptions.None)
                                    .Where(s => s.Trim() != "" && s.Trim() != ">")
                                 select linha).ToList();

            await AdicionarVersiculo(lstBiblia_ra, "RA");


            //Seeding RC Bible
            var biblia_rc = ChurchDataShow.Properties.Resources.Velho_RC + ChurchDataShow.Properties.Resources.Novo_RC;
            var lstBiblia_rc = (from linha in biblia_jfa.Split(new string[] { "\n" }, StringSplitOptions.None)
                                    .Where(s => s.Trim() != "" && s.Trim() != ">")
                                select linha).ToList();

            await AdicionarVersiculo(lstBiblia_rc, "RC");


            return true;
        }

        public async Task<bool> AdicionarVersiculo(List<string> listaVersiculos, string Traducao)
        {
            int capituloAtual = 0;

            int idTraducao = _lstTraducoes.Where(t => t.Nome == Traducao).FirstOrDefault().Id;
            int idLivro = 0;

            foreach (var versiculo in listaVersiculos)
            {
                //capítulo
                if (versiculo.Trim()[0] == '>')
                {
                    string livroAtual = versiculo.Split('>')[1].Split('[')[0].Trim();
                    capituloAtual = int.Parse(versiculo.Split('[')[1].Split(']')[0]);
                    idLivro = _lstLivros.Where(l => l.Nome == livroAtual).FirstOrDefault().Id;
                }
                //versículo
                else
                {
                    try
                    {
                        int numVersiculo = int.Parse(versiculo.Split(' ')[0].Replace(".", ""));
                        string texto = versiculo.Substring(versiculo.IndexOf(" "), versiculo.Length - numVersiculo.ToString().Length).Trim();

                        TextoBiblia textoBiblia = new TextoBiblia();
                        textoBiblia.IdLivro = idLivro;
                        textoBiblia.IdTraducao = idTraducao;
                        textoBiblia.Capitulo = capituloAtual;
                        textoBiblia.Texto = texto;
                        textoBiblia.Versiculo = numVersiculo;

                        await _repositorio.AdicionarReferenciaBiblia(textoBiblia);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
