using FuelMonitor.BO.Models;
using FuelMonitor.BO.Views;
using FuelMonitor.Utils;
using System.Collections.Generic;

namespace FuelMonitor.BO.DAO
{
    public class FuelFillDAO
    {
        public static FuelFill Create(FuelFill entity)
        {
            AppUtil.Connection.Insert(entity);

            return AppUtil.Connection.Get<FuelFill>(entity.ID);
        }

        public static FuelFill Update(FuelFill entity)
        {
            AppUtil.Connection.Update(entity);

            return AppUtil.Connection.Get<FuelFill>(entity.ID);
        }

        public static FuelFill Get(long id)
        {
            return AppUtil.Connection.Get<FuelFill>(id);
        }

        public static List<FuelFillView> GetAllView()
        {
            var Query = @"
                select 
                    *,
                    odovalue - lastodovalue as distancetraveled,
                    round((odovalue - lastodovalue) / fuelfilled, 2) as avgkmpl
                from 
                    (select 
                        *, 
                        (lag(ODOValue, 1) over (order by date, _id)) as lastodovalue 
                    from fuelfills 
                    order by date desc, _id desc)x";

            return AppUtil.Connection.Query<FuelFillView>(Query);
        }
    }
}