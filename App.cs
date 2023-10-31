using Sdk;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LprApp
{

    public class App
    {
        private static App instance;
        public string Ip { get; private set; }
        public ushort Porta { get; private set; }
        public int UserId { get; private set; }
        public string Login { get; private set; }
        public string Senha { get; private set; }
        public bool CapturaRegistrada { get; private set; }
        
        // endereço ip do disposito
        private byte[] m_serverAddressAVD;

        public static NET_SDK_DEVICEINFO NetSdkDeviceInfo = new NET_SDK_DEVICEINFO();
        public static SUBSCRIBE_CALLBACK SubscribeCallBack = null;

        private App() {
            Ip = ConfigurationManager.AppSettings["app_ip"];
            Porta = ushort.Parse(ConfigurationManager.AppSettings["app_port_data"]);
            Login = ConfigurationManager.AppSettings["app_user_login"];
            Senha = ConfigurationManager.AppSettings["app_user_password"];
        }

        public static void Init()
        {
            instance = new App();
        }

        public static App GetInstance()
        {
            return instance;
        }


        /// <summary>
        /// realiza o login do usuário no dispositivo 
        /// </summary>
        public void Logar()
        {
            bool bResult = SdkHelper.NET_SDK_Init();
            SdkHelper.NET_SDK_SetConnectTime();
            SdkHelper.NET_SDK_SetReconnect();
            if (bResult)
            {                
                UserId = SdkHelper.NET_SDK_Login(Ip, Porta, Login, Senha, ref NetSdkDeviceInfo);
            }
        }

        /// <summary>
        /// Habilita a captura de placas no dispositivo, após registrado a placas são recebidas no 
        /// método informado para CALLBACK.
        /// </summary>
        public void Registrar()
        {
            if(UserId > 0)
            {
                if (!CapturaRegistrada)
                {
                    bool ret = false;
                    NET_DVR_SUBSCRIBE_REPLY infoRegistro = new NET_DVR_SUBSCRIBE_REPLY();

                    ret = SdkHelper.NET_SDK_SmartSubscrib(UserId, (int)NET_DVR_SMART_TYPE.NET_IPC_SMART_VIHICLE, 0, ref infoRegistro);
                    if (ret)
                    {
                        Console.WriteLine("Captura registrada com sucesso");
                        m_serverAddressAVD = infoRegistro.serverAddress;
                        string serverAddress = Encoding.UTF8.GetString(m_serverAddressAVD);
                        Console.WriteLine($"EndPoint:  {serverAddress}");
                        SubscribeCallBack = RecebeCaptura;
                        bool sret = SdkHelper.NET_SDK_SetSubscribCallBack(SubscribeCallBack, IntPtr.Zero);
                        if (sret)
                        {
                            Console.WriteLine("Método de CALLBACK registrado com sucesso");
                            Console.WriteLine("Aguardando a captura: ");
                        }
                        CapturaRegistrada = true;

                    }
                    else
                    {
                        throw new Exception(SdkHelper.GetErrorMessage());
                    }
                }                           
            }
            else
            {
                throw new System.Exception("Realize o login do usuário no dispositivo.");
            }
        }

        /// <summary>
        /// recebe a captura de cada placa lida no dispositivo.
        /// </summary>
        /// <param name="lUserID"></param>
        /// <param name="dwCommand"></param>
        /// <param name="pBuf"></param>
        /// <param name="dwBufLen"></param>
        /// <param name="pUser"></param>
        private void RecebeCaptura(Int32 lUserID, Int32 dwCommand, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser)
        {
            switch (dwCommand)
            {
                case (Int32)NET_SDK_N9000_ALARM_TYPE.NET_SDK_N9000_ALARM_TYPE_VEHICE:
                    // informações do veículo 
                    NET_SDK_IVE_VEHICE_HEAD_INFO vehiceHeadInfo = new NET_SDK_IVE_VEHICE_HEAD_INFO();
                    int offset = Marshal.SizeOf(vehiceHeadInfo);
                    if (dwBufLen < Marshal.SizeOf(vehiceHeadInfo))
                    {
                        break;
                    }
                    vehiceHeadInfo = (NET_SDK_IVE_VEHICE_HEAD_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_VEHICE_HEAD_INFO));
                    pBuf += offset;
                    uint cntVheCle = vehiceHeadInfo.item_cnt;

                    if (cntVheCle > 0)
                    {
                        for (int i = 0; i < cntVheCle; i++)
                        {
                            NET_SDK_IVE_VEHICE_ITEM_INFO vehicleinfo = new NET_SDK_IVE_VEHICE_ITEM_INFO();
                            vehicleinfo = (NET_SDK_IVE_VEHICE_ITEM_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_SDK_IVE_VEHICE_ITEM_INFO));

                            StringBuilder strToShow = new StringBuilder();
                            strToShow.Append("Placa do veículo:" + SdkHelper.ByteToStr(vehicleinfo.plate) + "\r\n");
                            strToShow.Append("Quantidade de caracteres:" + vehicleinfo.plateCharCount + "\r\n");
                            Console.WriteLine(strToShow.ToString());
                            offset = Marshal.SizeOf(typeof(NET_SDK_IVE_VEHICE_ITEM_INFO));
                            pBuf += offset;

                            if (vehicleinfo.jpeg_len > 0)
                            {
                                if (i == 0)//第1张是源图片
                                {
                                    byte[] data = new byte[vehicleinfo.jpeg_len];
                                    Marshal.Copy(pBuf, data, 0, (int)vehicleinfo.jpeg_len);
                                    SavePicture(data);

                                    //string picName = vehicleinfo.plateId.ToString();
                                    //File.WriteAllBytes(Application.StartupPath + "\\SourceImg\\" + picName.ToString() + ".jpg", data);
                                    pBuf += (int)vehicleinfo.jpeg_len;//地址偏移
                                }
                                else
                                {
                                    byte[] data = new byte[vehicleinfo.jpeg_len];
                                    Marshal.Copy(pBuf, data, 0, (int)vehicleinfo.jpeg_len);
                                    SavePicture(data);

                                    //string picName = vehicleinfo.plateId.ToString();
                                    //File.WriteAllBytes(Application.StartupPath + "\\FaceImg\\" + picName.ToString() + ".jpg", data);
                                    pBuf += (int)vehicleinfo.jpeg_len;//地址偏移
                                }

                            }

                            if (vehicleinfo.jpeg_vir_len > vehicleinfo.jpeg_len)
                            {
                                pBuf += (int)(vehicleinfo.jpeg_vir_len - vehicleinfo.jpeg_len);
                            }
                        }
                    }
                    break;
            }
        }

        private void SavePicture(byte[] data)
        {
            if (data.Length < 1)
            {
                return;
            }
            MemoryStream stream = new MemoryStream(data);

            Image img = Image.FromStream(stream);
            string base_path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            string full_path = Path.Combine(base_path, "imgs");
            if (!Directory.Exists(full_path))
            {
                Directory.CreateDirectory(full_path);
            }
            string imgFile = $"{full_path}\\{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.jpg";
            Console.WriteLine($"Imagem salva em: {imgFile}");
            //img.Save(imgFile);
        }

        /// <summary>
        /// cancela o registro de captura
        /// </summary>
        public void Cancelar()
        {
            if (CapturaRegistrada)
            {
                int dwResult = 0;
                IntPtr temp = SdkHelper.PointArrayToIntPtr(m_serverAddressAVD);
                bool bret = SdkHelper.NET_SDK_UnSmartSubscrib(UserId, (int)NET_DVR_SMART_TYPE.NET_IPC_SMART_VIHICLE, 0, temp, ref dwResult);
                Marshal.FreeHGlobal(temp);
                if (bret)
                {
                    CapturaRegistrada = false;
                }
                else
                {
                    throw new Exception(SdkHelper.GetErrorMessage());
                }
            }
        }
    }
}
