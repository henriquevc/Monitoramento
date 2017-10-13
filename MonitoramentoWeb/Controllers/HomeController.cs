using MonitoramentoWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonitoramentoWeb.Controllers
{
    public class HomeController : Controller
    {
        Erros erros = new Erros();
        List<string> telefones = new List<string>() { "5511991967697", "5511938003983" };

        public void Index()
        {
            //ler arquivo com as conexões que deverão ser testadas e o arquivo de log
            StreamReader sr = new StreamReader(@"e:\home\agilus\Temp\config.txt", true);
            StreamWriter sw = new StreamWriter(@"e:\home\agilus\Temp\logServico.txt", true);

            string linha;

            //looping para ler linha por linha do arquivo de conexão
            while ((linha = sr.ReadLine()) != null)
            {

                SqlConnection conexao = new SqlConnection(linha);
                string nomeServidor = linha.Substring(12, linha.IndexOf(';') - 12);

                //tentativa de conexão
                try
                {
                    conexao.Open();
                    conexao.Close();

                    //se a conexão voltar
                    if (erros.ContainsKey(nomeServidor))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + " - Conexão com o servidor " + nomeServidor + " voltou!");
                        erros.Remove(nomeServidor);
                        Sms.Enviar("O servidor " + nomeServidor + " voltou", telefones);
                    }

                }
                //se a conexão deu erro
                catch
                {
                    int qtde = 0;
                    sw.WriteLine(DateTime.Now.ToString() + " - Conexão com o servidor " + nomeServidor + " está fora do ar!");

                    if (erros.ContainsKey(nomeServidor))
                    {
                        var item = erros.Busca(nomeServidor);
                        qtde = item.Quantidade;
                        qtde++;
                        erros.Update(nomeServidor, qtde);
                    }
                    else
                    {
                        erros.Add(nomeServidor, 1);
                        qtde = 1;
                    }
                    //envio do sms
                    if (qtde <= 3 && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                    {
                        sw.WriteLine(DateTime.Now.ToString() + " - Conexão com o servidor " + nomeServidor + " está fora do ar!");
                        Sms.Enviar("O servidor " + nomeServidor + " esta fora do ar", telefones);
                    }
                }
                finally
                {
                    sw.Flush();
                }
            }
            sw.WriteLine(" ");
            sw.Close();
            sr.Close();
        }

    }
}
