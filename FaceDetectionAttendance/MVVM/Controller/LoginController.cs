using FaceDetectionAttendance.MVC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionAttendance.MVC.Controller
{
    public class LoginController
    {
        private readonly AccountModel accountModel;

        public LoginController(AccountModel accountModel)
        {
            this.accountModel = accountModel;
        }

        public bool Authenticate(string username, string password)
        {
            return username == accountModel.username && password == accountModel.password;
        }
    }
}
