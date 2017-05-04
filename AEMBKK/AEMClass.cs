using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEMBKK
{
    class AEMClass
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string DeviceID { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
    }
}
