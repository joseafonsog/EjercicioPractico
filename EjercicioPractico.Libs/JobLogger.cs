using EjercicioPractico.Libs.Models;
using EjercicioPractico.Libs.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EjercicioPractico.Libs
{
    public class JobLogger
    {
        private readonly string[] _types;
        private readonly List<Sources> _sources;
        private readonly string _name;
        private readonly string _route;
        private readonly ILogRepository _context;

        private enum Types
        {
            Info,
            Warning,
            Error
        }

        private enum Sources
        {
            File,
            Console,
            Database
        }

        public JobLogger(string pathConfig = null, ILogRepository customeContext = null)
        {
            try
            {
                if (pathConfig == null)
                    pathConfig = AppDomain.CurrentDomain.BaseDirectory + "/logger.cfg.json";

                var logConf = JObject.Parse(File.ReadAllText(pathConfig));
                var types = (string)logConf["types"];
                var sourceConsole = logConf["sources"]["console"];
                var sourceDatabase = logConf["sources"]["database"];
                var sourceFile = logConf["sources"]["file"];

                if (string.IsNullOrEmpty(types))
                    throw new Exception("Error Code 003: It's necessary configure types logs(info,warning or error)");
                _types = types.Split(',');
                for (var i = 0; i < _types.Count(); i++)
                {
                    if (_types[i] == "info" || _types[i] == "warning" || _types[i] == "error")
                        continue;
                    throw new Exception("Error Code 004: Types only can be info, warning or error");
                }

                _sources = new List<Sources>();
                if ((bool)sourceConsole["active"])
                    _sources.Add(Sources.Console);

                if ((bool)sourceDatabase["active"])
                {
                    var connectString = (string)sourceDatabase["connection"];
                    var table = (string)sourceDatabase["table"];
                    if (string.IsNullOrEmpty(connectString) || string.IsNullOrEmpty(table))
                    {
                        throw new Exception("Error Code 005: It's necessary configure connection and table fields");
                    }

                    _context = customeContext ?? new LogRepository(connectString, table);

                    _sources.Add(Sources.Database);
                }

                if ((bool)sourceFile["active"])
                {
                    var name = (string)sourceFile["name"];
                    var route = (string)sourceFile["route"];
                    if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(name))
                    {
                        throw new Exception("Error Code 006: It's necessary configure route and name fields");
                    }
                    _name = name;
                    _route = route;
                    _sources.Add(Sources.File);
                }
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Error Code 001: It's necesary a path to the configuration file.");
            }

            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Error Code 002: Couldn't found a configuration file.");
            }
        }

        public void Info(string message)
        {
            if (!_types.Contains(Types.Info.ToString().ToLower())) return;
            SetLog(message, Types.Info);
        }

        public void Warning(string message)
        {
            if (!_types.Contains(Types.Warning.ToString().ToLower())) return;
            SetLog(message, Types.Warning);
        }

        public void Error(string message)
        {
            if (!_types.Contains(Types.Error.ToString().ToLower())) return;
            SetLog(message, Types.Error);
        }

        private void SetLog(string message, Types type)
        {
            if (_sources.Contains(Sources.Console))
            {
                SetColor(type);
                Console.WriteLine(Log(message, type));
            }
            if (_sources.Contains(Sources.Database))
            {
                SetLogDataBase(message, type);
            }
            if (_sources.Contains(Sources.File))
            {
                SetLogFile(_name, _route, message, type);
            }
        }

        private static string Log(string message, Types type)
        {
            return Date() + " - [" + type.ToString().ToUpper() + "]: " + message;
        }

        private void SetLogDataBase(string message, Types type)
        {
            var log = new Log
            {
                Message = message,
                Type = type.ToString(),
                Date = DateTime.Now
            };


            _context.Save(log);

        }

        private static string Date()
        {
            return DateTime.Now.ToShortDateString();
        }

        private static void SetLogFile(string name, string route, string message, Types type)
        {
            var fullRoute = route + name + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";
            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }
            File.AppendAllText(fullRoute, Log(message + Environment.NewLine, type));
        }

        private static void SetColor(Types type)
        {
            switch (type)
            {

                case Types.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Types.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Types.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
        }
    }
}
