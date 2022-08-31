using FuelMonitor.BO.Models;
using SQLite;
using System;
using System.IO;

namespace FuelMonitor.Utils
{
    public static class AppUtil
    {
        private static SQLiteConnection _connectionCache;
        public static SQLiteConnection Connection { get => GetSqliteConnection(); }

        private static SQLiteConnection GetSqliteConnection()
        {
            if (_connectionCache != null)
            {
                return _connectionCache;
            }

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "fuelMonitor.db3");

            _connectionCache = new SQLiteConnection(dbPath);

            _connectionCache.CreateTable<FuelFill>();

            return _connectionCache;
        }
    }
}