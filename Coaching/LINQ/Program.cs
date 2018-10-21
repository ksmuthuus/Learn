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
            var manufacturers = m.ProcessManufacturers("manufacturers.csv");

            var query = from car in cars
                        join manufacturer in manufacturers
                        on car.Manufacturer equals manufacturer.Name
                        //where car.Manufacturer == "BMW" && car.YearModel == 2016
                        orderby car.Combined descending, car.Name ascending
                        select new
                        {
                            manufacturer.HeadQuarters,
                            car.Name,
                            car.Combined
                        };

            var query1 = cars.Join(manufacturers,
                                    car => car.Manufacturer,
                                    manufacturer => manufacturer.Name,
                                    (car, manufacturer) => new
                                    {
                                        manufacturer.HeadQuarters,
                                        car.Name,
                                        car.Combined
                                    })
                                    .OrderByDescending(car => car.Combined)
                                    .ThenBy(car => car.Name);

            foreach(var result in query1.Take(10))
            {
                Console.WriteLine($"{result.HeadQuarters} {result.Name} {result.Combined}");
            }
        }
    }
}
