using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Services
{
    public class MyService : IMyService
    {
        public string Hello(string name)
        {
            return $"Welcome, {name} \n\n{DateTime.Now}";
        }
    }
}