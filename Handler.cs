using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace project.lambda
{
    public class ConnectionInfo
    {
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public string DbName { get; set; }
        public string DbHost { get; set; }
        public int DbPort { get; set; }
    }

    public class LambdaInput
    {
        public ConnectionInfo Connection { get; set; }

        public string QueryText { get; set; }
    }

    public class LambdaOutput
    {
        public string Name { get; set; }
        public bool Old { get; set; }
    }

    public class Function
    {
        private static readonly string cxnStringFormat = "server={0};uid={1};pwd={2};database={3};Connection Timeout=60";

        private string GetCxnString(ConnectionInfo cxn)
        {
            return string.Format(cxnStringFormat, cxn.DbHost, cxn.DbUser, cxn.DbPassword, cxn.DbName);
        }

        private static MySqlCommand GetCommand(MySqlConnection conn, string query)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        public List<Dictionary<string, object>> RunQueryHandler(LambdaInput input, ILambdaContext context)
        {
            var cxnString = GetCxnString(input.Connection);
            var query = input.QueryText;

            var result = new List<Dictionary<string, object>>();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(cxnString))
            {
                var cmd = GetCommand(conn, query);
                var reader = cmd.ExecuteReader();

                var columns = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                }

                while (reader.Read())
                {
                    var record = new Dictionary<string, object>();
                    foreach (var column in columns)
                    {
                        record.Add(column, reader[column]);
                    }
                    result.Add(record);
                }
            }
            return result;
        }
    }
}















// using Amazon.Lambda.Core;
// using System;

// [assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

// namespace AwsDotnetCsharp
// {
//     public class Handler
//     {
//        public Response Hello(Request request)
//        {
//            return new Response("Go Serverless v1.0! Your function executed successfully!", request);
//        }
//     }

//     public class Response
//     {
//       public string Message {get; set;}
//       public Request Request {get; set;}

//       public Response(string message, Request request){
//         Message = message;
//         Request = request;
//       }
//     }

//     public class Request
//     {
//       public string Key1 {get; set;}
//       public string Key2 {get; set;}
//       public string Key3 {get; set;}

//       public Request(string key1, string key2, string key3){
//         Key1 = key1;
//         Key2 = key2;
//         Key3 = key3;
//       }
//     }
// }
