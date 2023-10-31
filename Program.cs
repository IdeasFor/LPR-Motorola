
using LprApp;
using System;

namespace LprApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CriaLinhaTracejada();
            Console.WriteLine("PROGRAMA: LPR TESTS V1.0");
            CriaLinhaTracejada();
            Run();
            
            CriaLinhaTracejada();
            Console.WriteLine("\t\tFIM");

        }
    
        private static void Run()
        {           

            App.Init();
            var app = App.GetInstance();
            Console.WriteLine("\n");
            
            Console.WriteLine("Dados de acesso:");
            CriaLinhaTracejada();
            Console.WriteLine($"IP: {app.Ip}");
            Console.WriteLine($"Porta: {app.Porta}");
            Console.WriteLine($"Login: {app.Login}");
            Console.WriteLine($"Senha: {app.Senha}");
            Console.WriteLine("\n");

            #region "SDK"
            try
            {
                Console.WriteLine("Registrando o dispositivo:");
                CriaLinhaTracejada();
                Console.WriteLine($"Realizando o login no dispositivo ({app.Ip}:{app.Porta})");

                app.Logar();
                Console.WriteLine($"Login realizado, UserId({app.UserId}) obtido com sucesso\n");

                Console.WriteLine("Registrando a captura de placa (Subscrib Vehicle)");
                CriaLinhaTracejada();
                app.Registrar();
                Console.WriteLine("Registro efetuado com sucesso");

                CriaLinhaTracejada();
                Console.WriteLine("Precione qualquer tecla para finalizar");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion
        }

        private static void CriaLinhaTracejada(int sz = 80, int cr=1)
        {
            while (sz > 0)
            {
                Console.Write("-");
                sz--;
            }
            while (cr > 0)
            {
                Console.WriteLine();
                cr--;
            }
        }

        public static void MainOld(string[] args)
        {
            #region "WEB API"
            //HttpFactory httpFactory = new HttpFactory();
            //string urlBase = "http://192.168.1.6";
            //int shortPort = 80;
            //int longPort = 8080;

            //string login = "admin";
            //string senha = "Admin-1234";
            //Console.WriteLine($"url base: {urlBase}");
            //Console.WriteLine($"short port: {shortPort}");
            //Console.WriteLine($"long port: {longPort}");

            //Console.WriteLine($"Login: {login}");
            //Console.WriteLine($"Senha: {senha}");

            //LprMotorolaService.Init(httpFactory, urlBase, login, senha, shortPort, longPort);

            //Console.WriteLine("Iniando o serviço..");
            //var service = LprMotorolaService.Instance();

            //Console.WriteLine("Obtendo informações do dispositivo..\n\n");
            //var deviceInfo = service.GetDeviceInfo();
            //Console.WriteLine("DeviceInfo: ");
            //Console.WriteLine(deviceInfo);

            //Console.WriteLine("Se escrevendo para a captura de placa...\n\n");
            //var subscribe = service.SetSubscribe(30000);
            //Console.WriteLine("subscribe: ");
            //Console.WriteLine(subscribe);

            //Console.WriteLine("CDATA: ");
            //var uri = XmlParser.GetCDATAValue(subscribe);
            //Console.WriteLine(uri.ToString());

            //Console.WriteLine("\n\nAguardando uma captura: ");
            //do
            //{
            //    var eventData = service.GetEvent(uri.ToString());
            //    Console.WriteLine($"Response:\n {eventData}");
            //    Thread.Sleep(1000);
            //} while (true);
            #endregion
        }
    }    
}



