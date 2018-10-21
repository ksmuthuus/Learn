using Coaching.LINQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coaching
{
    class Program
    {
        static void Main(string[] args)
        {
            Car c = new Car();
            var cars = c.ProcessCars("fuel.csv");

            Manufacturer m = new Manufacturer();
            var manu = m.ProcessManufacturers("manufacturers.csv");

            var query = from car in cars
                        where car.Manufacturer == "BMW"
                        select car;

            var query1 = cars.Where(car => car.Manufacturer == "BMW");

            foreach(var result in query1)
            {
                Console.WriteLine(result.Name);
            }
        }
    }
}
