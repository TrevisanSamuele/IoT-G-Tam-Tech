﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Configuration;

namespace Autobus
{
    class DataSender
    {
        private RedisClient redis;
        private string url;

        public DataSender()
        {
            redis = new RedisClient("127.0.0.1");
            url = new AppSettingsReader().GetValue("urlApi", typeof(string)).ToString();
        }

        public void Read(string data)
        {
            redis.LPush("sensors_data", data);
        }
        public void Send()
        {
            try
            {
                if(redis.LLen("sensors_data")==0)
                {

                }
                else
                {
                    HttpWebRequest httpWebRequestData;
                    httpWebRequestData = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequestData.ContentType = "application/json";
                    httpWebRequestData.Method = "POST";
                    using (var streamWriter = new StreamWriter(httpWebRequestData.GetRequestStream()))
                    {

                        string dataJson = redis.BLPop(30, "sensors_data");
                        streamWriter.Write(dataJson);
                        streamWriter.Flush();
                        streamWriter.Close();

                    }
                    var httpResponseData = (HttpWebResponse)httpWebRequestData.GetResponse();
                    using (var streamReader = new StreamReader(httpResponseData.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
