using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApplication.Exceptions
{
    public class CommitteeNotFoundException : ApplicationException
    {
        public CommitteeNotFoundException(string name, Object key) : base($"Entity {name} - {key} is not found.")
        {

        }
    }
}
