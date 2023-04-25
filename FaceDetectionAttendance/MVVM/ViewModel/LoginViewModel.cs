using FaceDetectionAttendance.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionAttendance.MVVM.Controller
{
    public class LoginViewModel
    {
        private readonly AccountModel accountModel;

        public LoginViewModel(AccountModel accountModel)
        {
            this.accountModel = accountModel;
        }

        public bool Authenticate(string username, string password)
        {
            return username == accountModel.username && password == accountModel.password;
        }
    }
}
