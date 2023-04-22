using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FaceDetectionAttendance.Model
{
    public class Dataconnecttion
    {
        private SqlConnection dc = new SqlConnection("Data Source = (localdb)\\MSSqlLocalDB; Initial catalog = CCPTPM; Integrated Security=true; TrustServerCertificate=True");
        public SqlConnection GetConnection()
        {
            return this.dc;
        }
    }
}
