using HumanAPIClient.Model;
using HumanAPIClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoramento
{
    public static class Sms
    {
        private static string conta = "agilus.api";
        private static string senha = "Oe111OGqUg";

        private static SimpleSending cliente = new SimpleSending(conta, senha);

        private static SimpleMessage mensagem = new SimpleMessage();


        public static void Enviar(string texto, List<string> telefones)
        {

            foreach (var telefone in telefones)
            {
                mensagem.To = telefone;
                mensagem.Message = texto;
                cliente.send(mensagem);
            }
        }



    }
}
