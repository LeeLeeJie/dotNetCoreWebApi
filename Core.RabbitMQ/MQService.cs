
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.RabbitMQ
{
   public class MQService
    {
        private string _queueName = "queue_X";
        private string _message = "hello MQ";
        private readonly Logger Logger;
        public MQService(string queueName, string message = "")
        {
            _queueName = queueName;
            _message = message;
            Logger = LogManager.GetLogger("MQLogger");

        }
        IConnectionFactory conFactory = new ConnectionFactory//创建连接工厂对象
        {
            HostName = "192.168.10.28",//IP地址
            Port = 5672,//端口号
            UserName = "LeeLeeJie",//用户账号
            Password = "123698745"//用户密码
        };

        /// <summary>
        /// 发送消息到队列
        /// </summary>
        public void ExeSend()
        {
            using (IConnection con = conFactory.CreateConnection())//创建连接对象
            {
                using (IModel channel = con.CreateModel())
                {
                    channel.QueueDeclare
                    (
                       queue: _queueName,//消息队列名称
                       durable: false,//是否缓存
                       exclusive: false,
                       autoDelete: false,
                       arguments: null
                    );
                    var body = Encoding.UTF8.GetBytes(_message);
                    Logger.Info("开始发送消息,routingKey:" + _queueName + ",message:" + _message);
                    //发送消息
                    channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                    Logger.Info("发送消息结束~~~~~~~~~~~~~~~~");
                }

            }
        }

        public void ExeReceive()
        {
            using (IConnection conn = conFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //声明一个队列
                    channel.QueueDeclare
                    (
                       queue: _queueName,//消息队列名称
                       durable: false,//是否缓存
                       exclusive: false,
                       autoDelete: false,
                       arguments: null
                    );
                    //创建消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        byte[] message = ea.Body;//接收到的消息
                        Logger.Info("接收到消息为：" + Encoding.UTF8.GetString(ea.Body));
                    };
                    //消费者开启监听
                    channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
                }
            }
        }
    }
}
