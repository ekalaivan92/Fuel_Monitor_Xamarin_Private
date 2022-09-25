using FuelMonitor.BO.Models;
using SQLite;
using System;

namespace FuelMonitor.BO.Models
{
    [Table("fuelfills")]
    public class FuelFill
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public long? ODOValue { get; set; }
        public decimal? FuelFilled { get; set; }
        public decimal? FuelCost { get; set; }
        public byte[] PhotoCapute { get; set; }
    }
}