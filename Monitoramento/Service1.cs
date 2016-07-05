using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace Monitoramento
{
    public partial class Service1 : ServiceBase
    {
        //atributos 
        Timer timer1;
        Dictionary<string, int> erros = new Dictionary<string, int>();
        List<string> telefones = new List<string>() { "5511991967697", "5511947223903" };

        //construtor para inicializar o serviço
        public Service1()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "logServico.txt"), true);

            sw.WriteLine(DateTime.Now.ToString() + " - Servico iniciado!");
            sw.Flush();
            sw.Close();
            //(método chamado, objeto, tempo para iniciar, tempo para rodar novamente)
            timer1 = new Timer(new TimerCallback(timer1_Tick), null, 1, 300000);
        }

        protected override void OnStop()
        {

            StreamWriter sw = new StreamWriter(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "logServico.txt"), true);

            sw.WriteLine(DateTime.Now.ToString() + " - Servico Parado!");
            timer1.Dispose();
            sw.Flush();
            sw.Close();
        }

        private void timer1_Tick(object sender)
        {
            //ler arquivo com as conexões que deverão ser testadas e o arquivo de log
            StreamReader sr = new StreamReader(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "config.txt"), true);
            StreamWriter sw = new StreamWriter(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "logServico.txt"), true);

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
                catch (Exception ex)
                {
                    int qtde = 0;
                    sw.WriteLine(DateTime.Now.ToString() + " - Conexão com o servidor " + nomeServidor + " está fora do ar!");

                    if (erros.ContainsKey(nomeServidor))
                    {
                        var item = erros.FirstOrDefault(c => c.Key == nomeServidor);
                        qtde = item.Value;
                        qtde++;
                        erros.Remove(nomeServidor);
                        erros.Add(nomeServidor, qtde);
                    }
                    else 
                    {
                        erros.Add(nomeServidor, 1);
                        qtde = 1;
                    }
                    //envio do sms
                    if (qtde <= 3 && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                    {
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
