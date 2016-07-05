using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MonitoramentoWeb.Models
{


    public class Erro
    {
        public string NomeServidor { get; set; }
        public int Quantidade { get; set; }
    }

    public class Erros
    {
        List<Erro> erros = new List<Erro>();

        public Erros()
        {
            using (FileStream file = new FileStream((@"e:\home\agilus\Temp\erros.json"), FileMode.OpenOrCreate))
            {
                StreamReader sr = new StreamReader(file);
                string conteudo = sr.ReadToEnd();
                var js = new JavaScriptSerializer();
                erros = js.Deserialize<List<Erro>>(conteudo);
            }
        }

        private void GravaJson()
        {
            var js = new JavaScriptSerializer();
            var errosJson = js.Serialize(erros);
            using (FileStream file = new FileStream((@"e:\home\agilus\Temp\erros.json"), FileMode.Truncate))
            {
                StreamWriter sw = new StreamWriter(file);
                sw.Write(errosJson);
                sw.Flush();
            }
        }

        public void Add(string nomeServidor, int qtde)
        {
            var erro = new Erro() { NomeServidor = nomeServidor, Quantidade = qtde };
            erros.Add(erro);
            GravaJson();
        }

        public void Update(string nomeServidor, int qtde)
        {
            var erro = Busca(nomeServidor);
            if (erro == null)
            {
                Add(nomeServidor, qtde);
            }
            else
            {
                erros.Remove(erro);
                erro.Quantidade = qtde;
                erros.Add(erro);
            }
            GravaJson();
        }

        public void Remove(string nomeServidor)
        {
            var erro = Busca(nomeServidor);
            if (erro != null)
            {
                erros.Remove(erro);
                GravaJson();
            }
        }

        public bool ContainsKey(string nomeServidor)
        {
            var erro = erros.Find(e => e.NomeServidor == nomeServidor);
            return (erro != null);
        }

        public Erro Busca(string nomeServidor)
        {
            return erros.FirstOrDefault(e => e.NomeServidor == nomeServidor);
        }
    }

}