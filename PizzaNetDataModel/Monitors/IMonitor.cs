using PizzaNetDataModel.Model;
using System;
namespace PizzaNetDataModel.Monitors
{
    public interface IMonitor<T>
        where T : Entity
    {
        bool HasStateChanged(T entity);
        void StartMonitor(T entity);
        bool Update(T entity);
        bool IsMonitoring();
    }
}
