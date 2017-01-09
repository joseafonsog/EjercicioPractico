using EjercicioPractico.Libs;
using EjercicioPractico.Libs.Models;
using EjercicioPractico.Libs.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EjercicioPractico.Tests
{
    [TestClass]
    public class JobLoggerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error Code 001: Path to the configuration is empty.")]
        public void NeedAPathToConfigFile()
        {
            var log = new JobLogger("");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException), "Error Code 002: Couldn't found a configuration file.")]
        public void NeedAValidPathToConfigFile()
        {
            var log = new JobLogger("a");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Error Code 003: Please configure types logs(info,warning or error)")]
        public void MustHaveATypeConfiguration()
        {
            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger1.cfg.json");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Error Code 004: Types only can be info, warning or error")]
        public void MustHaveAValidType()
        {
            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger2.cfg.json");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Error Code 005: Please configure connection and table fields")]
        public void MustHaveAConectionStringToTheDatabase()
        {
            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger3.cfg.json");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Error Code 006: Please configure route and name fields")]
        public void MustHaveARouteAndNameFields()
        {
            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger3.cfg.json");
        }

        [TestMethod]
        public void Must_Log_To_Console()
        {
            // Arrange

            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger4.cfg.json");



            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var expected =
                    string.Format(
                        DateTime.Now.ToShortDateString() + " - [INFO]: This must be INFO{0}" +
                        DateTime.Now.ToShortDateString() + " - [WARNING]: This must be WARNING{0}" +
                        DateTime.Now.ToShortDateString() + " - [ERROR]: This must be ERROR{0}", Environment.NewLine);

                // Act

                log.Info("This must be INFO");
                log.Warning("This must be WARNING");
                log.Error("This must be ERROR");

                //Assert

                Assert.AreEqual(sw.ToString(), expected);

            }
        }

        [TestMethod]
        public void Must_Log_To_Database()
        {
            // Arrange

            ILogRepository logRepository = new FakeLogRepository();
            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger5.cfg.json", logRepository);
            IList<Log> expected = new List<Log>();

            expected.Add(new Log
            {
                Message = "This must be INFO Database log",
                Type = "Info",
                Date = DateTime.Now
            });

            expected.Add(new Log
            {
                Message = "This must be WARNING Database log",
                Type = "Warning",
                Date = DateTime.Now
            });

            expected.Add(new Log
            {
                Message = "This must be ERROR Database log",
                Type = "Error",
                Date = DateTime.Now
            });

            // Act

            log.Info("This must be INFO Database log");
            log.Warning("This must be WARNING Database log");
            log.Error("This must be ERROR Database log");

            var logs = logRepository.GetAll();

            // Assert

            Assert.AreEqual(logs.ToList().Count(), 3);
            Assert.AreEqual(logs.ToList()[0].ToString(), expected[0].ToString());
            Assert.AreEqual(logs.ToList()[1].ToString(), expected[1].ToString());
            Assert.AreEqual(logs.ToList()[2].ToString(), expected[2].ToString());
        }

        [TestMethod]
        public void Must_Log_To_File()
        {
            // Arrange

            ILogRepository logRepository = new FakeLogRepository();
            var log = new JobLogger(AppDomain.CurrentDomain.BaseDirectory + "/logger6.cfg.json", logRepository);

            var expected =
                string.Format(
                    DateTime.Now.ToShortDateString() + " - [INFO]: This must be INFO File log{0}" +
                    DateTime.Now.ToShortDateString() + " - [WARNING]: This must be WARNING File log{0}" +
                    DateTime.Now.ToShortDateString() + " - [ERROR]: This must be ERROR File log{0}", Environment.NewLine);

            // Act

            log.Info("This must be INFO File log");
            log.Warning("This must be WARNING File log");
            log.Error("This must be ERROR File log");

            var result = File.ReadAllText("C:/Temp/Logs/log" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt");

            // Assert

            Assert.IsTrue(File.Exists("C:/Temp/Logs/log" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt"));
            Assert.AreEqual(result, expected);
            File.Delete("C:/Temp/Logs/log" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt");
        }
    }
}
