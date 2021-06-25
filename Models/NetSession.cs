using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess.Models
{
    public class NetSession
    {
        public string Ip;
        public short Port =2103;
        public ChessGame Game;
        TcpClient client;
        TcpListener server;
        /// <summary>
        /// Инициализация как сервера
        /// </summary>
        public NetSession()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"),Port);
            server.Start();
            client = server.AcceptTcpClient();
        }
        /// <summary>
        /// Инициализация как клиента
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public NetSession(string ip,short port = 2103)
        {
            client = new TcpClient();
            client.Connect(ip, port);

        }

        public void TryClose()
        {
            if (client != null)
                client.Close();
            if (server != null)
            {
                server.Stop();

            }
        }
        /// <summary>
        /// Попытка сообщить клиенту противника о ходе. В случае не удаче возвраащает фолс
        /// </summary>
        /// <param name="MoveFrom">Откуда пытаемся переместить</param>
        /// <param name="MoveTo">Куда пытаемся переместить</param>
        /// <param name="additionalMasks">Доп. данные по типу на какую фигуру меняем пешку</param>
        /// <returns></returns>
        public bool TrySendAction(Point MoveFrom,Point MoveTo,int additionalMasks)
        {
            byte[] data = new byte[5];//1ые два MoveFrom, следующие MoveTo, последние доп, чтобы понять кого выбрал противник.
            data[0] = (byte)MoveFrom.X;
            data[1] = (byte)MoveFrom.Y;
            data[2] = (byte)MoveTo.X;
            data[3] = (byte)MoveTo.Y;
            data[4] = (byte)additionalMasks;
            
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);


            return true;
        }

        public event Action<Point, Point, FigureType> OnActionReady;

        public async Task AsyncGetAction()
        {
            var response =GetAction();
            OnActionReady.Invoke(response.Item1,response.Item2,response.Item3);
        }

        public (Point,Point,FigureType) GetAction()
        {
            var stream = client.GetStream();
            byte[] data = new byte[5];
            stream.Read(data, 0, data.Length);
            return (new Point(data[0], data[1]), new Point(data[2], data[3]), (FigureType)data[4]);
        }
    }
}
