using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCrowd.DataAccess.Interface.Tests
{
    interface ITestStatus :IEntity
    {
        string Status { get; set; }
    }
}
