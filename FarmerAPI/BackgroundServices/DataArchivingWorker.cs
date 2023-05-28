using FarmerAPI.Models.SQLite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace VisionPicking.BackgroundServices
{
    public class DataArchivingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<DataArchivingWorker> logger;

        public DataArchivingWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<DataArchivingWorker> logger
        )
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    MigrateClimateData();
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"VideoRotation: {ex.Message}");
                }
            }
        }

        private void MigrateClimateData(int reservedDays = 90)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GreenHouseContext>();
            var cutoffDate = DateTime.Now.AddDays(-reservedDays);
            var years = db.Climate.Select(x => x.ObsTime).Where(x => x < cutoffDate).Select(x => x.Year).Distinct().ToList();

            foreach(var year in years)
			{
                var tableName = $"climate_{year}";
                if (!IsTableExists(db, tableName))
                {
                    db.Database.ExecuteSqlRaw($"CREATE TABLE \"{tableName}\"" +
                        "(\"ObsTime\" DATETIME," +
                        "\"Lux\" NUMERIC(7, 1)," +
                        "\"Rh\" NUMERIC(3, 1)," +
                        "\"Temperature\" NUMERIC(3, 1)," +
                        "PRIMARY KEY(\"ObsTime\" ASC))");
                    db.SaveChanges();
                }

                using var transcation = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                try
                {
                    db.Database.ExecuteSqlRaw($"INSERT INTO {tableName} (ObsTime, Lux, Rh, Temperature) " +
                        $"SELECT ObsTime, Lux, Rh, Temperature FROM climate WHERE strftime('%Y', ObsTime) = '{year}' and ObsTime <= '{cutoffDate:yyyy-MM-dd}'");
                    db.Database.ExecuteSqlRaw($"DELETE FROM climate WHERE strftime('%Y', ObsTime) = '{year}' and ObsTime <= '{cutoffDate:yyyy-MM-dd}'");
                    db.SaveChanges();

                    transcation.Commit();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    transcation.Rollback();
                }
            }
        }

        private bool IsTableExists(GreenHouseContext db,string tableName)
		{
            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            db.Database.OpenConnection();
            using var result = cmd.ExecuteReader();

			return result.HasRows;
		}
    }
}
