using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTest
{
    class EmailCount
    {
        public string email { get; set; }
        public long amount { get; set; }

        public EmailCount(string email, long amount)
        {
            this.email = email;
            this.amount = amount;
        }
    }
}
